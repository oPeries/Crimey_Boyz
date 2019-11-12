/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using System.Collections;
using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;
using System;

//Used to control and manage the network connection between the tablet & game builds
//Use this manager to start & stop the networking communication between the game and controller
//Currently only detects disconnection and logs and error but may add the ability to re-connect upon lost connection
public class ConnectionManager : MonoBehaviour {

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Defines ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public static ConnectionManager Singleton { get; private set; } //Singleton instance

    public bool startOnLoad = false; //set to start the server/client immediately upon opening the scene
    public bool isServer = true; //set to load as a client
    public bool retryUponFailure = true; //Server - will start listening for clients again after the client disconnects. Client - will try 
    public float deniedRetryDelay = 5f; //How long before attempting to reconnect after client denied by server (if retry enabled) (client only)
    public float disonnectedRetryDelay = 2f; //How long before attempting to reconnect after disconnect (if retry enabled)

    private ConnectionState currentState; //State of the connection between this instance and the server/client
    private ulong missionControlID; //network id for the mission control (is the tablet when connected, else is the ID for the server)

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Behavioural ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void Awake() {
        missionControlID = 0; //Initially set this to the server's id
        currentState = ConnectionState.STOPPED;
    }

    //Assign callbacks (MALPI - https://github.com/MidLevel/MLAPI/blob/master/MLAPI/Core/NetworkingManager.cs)
    private void Start() {
        NetworkingManager.Singleton.OnClientConnectedCallback = ClientConnected;
        NetworkingManager.Singleton.OnClientDisconnectCallback = ClientDisconnected;
    }

    private void Update() {

        //If this is a client instance searching for a server, check to see if a server has been discovered. If a server has been discovered, try connect to it
        if (!isServer && currentState == ConnectionState.SEARCHING) {
            if (LANdiscovery.Singleton.GetResponseIP() != null) {
                if (!NetworkingManager.Singleton.IsClient) {
                    //Debug.Log(LANdiscovery.Singleton.GetResponseIP().ToString());
                    ((UnetTransport)NetworkingManager.Singleton.NetworkConfig.NetworkTransport).ConnectAddress = LANdiscovery.Singleton.GetResponseIP().ToString();
                    LANdiscovery.Singleton.Stop();
                    NetworkingManager.Singleton.StartClient();
                    currentState = ConnectionState.CONNECTING;
                } else {
                    Debug.LogWarning("Client already running while listening, this shouldn't happen");
                }
            }
        }
    }


    /// <summary>
    /// Sets the singleton for this class. Ensures only one copy of this script is instantiated.
    /// Starts a new connection if set to startOnLoad
    /// </summary>
    private void OnEnable() {

        if (Singleton != null && this != Singleton) {
            Destroy(this);
        } else {
            Singleton = this;
        }

        if (startOnLoad) {
            InitiateConnection();
        }
    }


    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /// <summary>
    /// Gets the current state of the client/server connection
    /// </summary>
    /// <returns>
    /// Returns the current state
    /// </returns>
    public ConnectionState GetState() {
        return currentState;
    }

    /// <summary>
    /// Gets the ID of the tablet
    /// </summary>
    /// <returns>
    /// Returns a ulong containing the tablet ID
    /// </returns>
    [Obsolete("Use GetMissionControlID instead")]
    public ulong GetTabletID() {
        return GetMissionControlID();
    }

    /// <summary>
    /// Get the ID of mission control (client)
    /// Note: should only be called on the server side
    /// </summary>
    /// <returns>
    /// Returns a ulong containing the network ID for MissionControl
    /// </returns>
    public ulong GetMissionControlID() {
        //if(!isServer) {
        //    Debug.LogWarning("GetMissionControlID called from the client....");
        //}
        return missionControlID;
    }

    /// <summary>
    /// If set to client, start searching for a server and connect to it once found
    /// If set to server, start the server and listen for the client (tablet)
    /// </summary>
    [Obsolete("Use InitiateConnection() instead")]
    public void OpenConnection() {
        InitiateConnection();
    }

    /// <summary>
    /// If set to client, start searching for a server and connect to it once found
    /// If set to server, start the server and listen for the client (tablet)
    /// </summary>
    public void InitiateConnection() {
        if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsClient || NetworkingManager.Singleton.IsServer) {
            Debug.LogError("Tried to start game connection when NetworkingManager already running");
            return;
        }

        if (isServer) {
            NetworkingManager.Singleton.ConnectionApprovalCallback = ApproveConnection;
            NetworkingManager.Singleton.StartHost();
            LANdiscovery.Singleton.StartListening();
            missionControlID = NetworkingManager.Singleton.ServerClientId;
            currentState = ConnectionState.LISTENING;

        } else {
            LANdiscovery.Singleton.StartTransmitting();
            currentState = ConnectionState.SEARCHING;
        }

        Debug.Log((isServer ? "Server" : "Client") + " initiated");
    }

    /// <summary>
    /// Stop server / client / host / discovery
    /// </summary>
    public void CloseConnection() {
        LANdiscovery.Singleton.Stop();

        if (NetworkingManager.Singleton.IsHost) {
            NetworkingManager.Singleton.StopHost();

        } else if (NetworkingManager.Singleton.IsClient) {
            NetworkingManager.Singleton.StopClient();

        } else if (NetworkingManager.Singleton.IsServer) {
            NetworkingManager.Singleton.StopServer();

        }

        currentState = ConnectionState.STOPPED;
        Debug.Log((isServer ? "Server" : "Client") + " stopped");
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


    /// <summary>
    /// Callback for when a new client connects (Called on the server and all clients, including the recently connected client)
    /// Pretty sure this is called before the client is approved or not
    /// </summary>
    /// <param name="clientID"> The ID of the client recently connected </param>
    private void ClientConnected(ulong clientID) {
        Debug.Log("Client Connected");

        if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer) {
            if (NetworkingManager.Singleton.ConnectedClientsList.Count > 2) {
                Debug.LogWarning("Warning: More than one client connected");
            }

        } else if (NetworkingManager.Singleton.IsClient) {

            if (NetworkingManager.Singleton.IsConnectedClient && NetworkingManager.Singleton.LocalClientId == clientID) {
                Debug.Log("Connection approved by server");
                currentState = ConnectionState.CONNECTED;
                missionControlID = NetworkingManager.Singleton.LocalClientId;
            } else {
                Debug.LogWarning("Warning: possible second client connected");
            }

        } else {
            Debug.LogError("Client Connected called when networking manager not running");
        }

    }

    /// <summary>
    /// Callback for when a client disconnects (Called on the sever and all clients, even if the client was not yet approved by the server)
    /// </summary>
    /// <param name="clientID"> The ID of the client recently disconnected</param>
    private void ClientDisconnected(ulong clientID) {
        
        //If this is the server and only 1 client currently connected then the tablet has disconnected (the single client IS the server / host)
        if(NetworkingManager.Singleton.IsServer || NetworkingManager.Singleton.IsHost) {

            if (NetworkingManager.Singleton.ConnectedClientsList.Count < 2 && currentState == ConnectionState.CONNECTED) {
                currentState = ConnectionState.DISCONNECTED;
                Debug.LogWarning("Client disconnected");

                //Start listening for a client again (if specified)
                if (retryUponFailure) {
                    StartCoroutine(DelayedListen(disonnectedRetryDelay));
                }
            }

        //Check if this instance is the one that was just disconnected (will be called even if a client tries to connect but was not approved)
        } else if (!NetworkingManager.Singleton.IsConnectedClient) {

            //Check if this instance was previously connected
            if (currentState == ConnectionState.CONNECTED) {

                currentState = ConnectionState.DISCONNECTED;
                Debug.LogWarning("Disconnected from the server!");
                //CloseConnection();

                //Try connecting again (if set to retry)
                if (retryUponFailure) {
                    StartCoroutine(DelayedStart(disonnectedRetryDelay));
                }

            } else if (currentState == ConnectionState.CONNECTING){

                //If still in connecting state, was rejected by the server
                currentState = ConnectionState.DENIED;
                Debug.Log("Failed to connect to the server. Timed out? Connection Denied?");
                CloseConnection();
                if (retryUponFailure) {
                    StartCoroutine(DelayedStart(deniedRetryDelay));
                }
            }
        }
    }

    /// <summary>
    /// Sever-side function to approve or deny a client's connection request
    /// Will only accept one client, denies any future requests
    /// </summary>
    /// <param name="connectionBuffer"> Array of bytes containing the connection buffer </param>
    /// <param name="clientID"> ID of the client requesting approval </param>
    /// <param name="approval"> Delegate to notify of connection approval / rejection </param>
    private void ApproveConnection(byte[] connectionBuffer, ulong clientID, NetworkingManager.ConnectionApprovedDelegate approval) {

        Debug.Log("Request for client approval invoked");
        bool approved;
        //2 clients allowed as the game counts as 1 client
        if (NetworkingManager.Singleton.ConnectedClientsList.Count >= 2) {
            Debug.Log("Client request denied");
            approved = false;
        } else {
            Debug.Log("Client request approved");
            missionControlID = clientID;
            approved = true;
        }

        if(approved) {
            currentState = ConnectionState.CONNECTED;
            LANdiscovery.Singleton.Stop();
        }

        approval(NetworkingManager.Singleton.NetworkConfig.CreatePlayerPrefab, null, approved, null, null);
    }

    //Initiate the client / server after the given delay (in seconds)
    private IEnumerator DelayedStart(float delay) {

        yield return new WaitForSeconds(delay);
        if (retryUponFailure) {
            InitiateConnection();
        }
    }

    //Start listening for client broadcast requests after the given delay (in seconds)
    private IEnumerator DelayedListen(float delay) {

        yield return new WaitForSeconds(delay);
        if (currentState != ConnectionState.STOPPED) {
            LANdiscovery.Singleton.StartListening();
            currentState = ConnectionState.LISTENING;
        }
    }
}
