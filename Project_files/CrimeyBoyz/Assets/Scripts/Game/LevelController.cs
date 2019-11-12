/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MLAPI;
using CrimeyBoyz.GameState;

//Controls Level specific tasks (Spawning objects & players)
public class LevelController : MonoBehaviour {

    public bool isTestScene = false; //if true, will listen for controllers to press the "start" button. Upon hitting start, spawns the player inside the start elevator, even if they are the tablet player
    public GameObject playerPrefab;

    private Dictionary<int, player> players;
    private Elevator spawnElevator;

    void Start() {
        players = new Dictionary<int, player>();

        if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer) {
            if (SessionManager.Singleton.tabletPlayer == -1)
                SessionManager.Singleton.tabletPlayer = Random.Range(1, players.Count);

            if (SessionManager.Singleton.IsNetworkedGameStateReady()) {
                //New floor has an empty end elevator
                SessionManager.Singleton.networkedGameState.inElevator.Clear();
                //Add the tablet player to the end elevator list
                SessionManager.Singleton.networkedGameState.inElevator.Add(SessionManager.Singleton.tabletPlayer);
            } else {
                Debug.LogWarning("Networked game state not ready, current tablet player is likely going to be wrong");
            }
        }
        Elevator[] elevators = Resources.FindObjectsOfTypeAll<Elevator>();
        foreach (Elevator elevator in elevators) {
            if(elevator.isSpawnElevator) {
                spawnElevator = elevator;
                break;
            }
        }

        

        if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer) {
            //If listening for controllers on start, expecting there to be no players and will spawn a player on start
            if (!isTestScene) {
                SpawnPlayers();
            }
        }

        StartCoroutine(DelayedSpawn(0.5f));

    }

    private void OnEnable() {
        if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer) {
            if (isTestScene) {
                SessionManager.Singleton.StartListeningForControllers(true);
                SessionManager.Singleton.PlayerStateChangedCallback += SpawnPlayers;
            }
        }
    }

    void Update() {

        //Return to Main menu upon hitting "ESC" key
        //KeyCode.Escape works for Android & Windows Builds
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SessionManager.Singleton.ReturnToMainMenu();
        }

        //Return to main menu if the connection with the tablet is lost
        if (ConnectionManager.Singleton.GetState() == ConnectionState.DISCONNECTED) {
            Debug.Log("Stopping game due to disconnection");
            SessionManager.Singleton.ReturnToMainMenu();
        }

    }

    private void OnDisable() {
        if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer) {
            if (isTestScene) {
                //Debug.Log("Removing");
                SessionManager.Singleton.PlayerStateChangedCallback -= SpawnPlayers;
            }
        }
    }

    IEnumerator DelayedSpawn(float delay) {
        yield return new WaitForSeconds(delay);
        InitEnvironmentObjects();
    }

    /// <summary>
    /// Initialises the game environment in the network
    /// </summary>
    private void InitEnvironmentObjects() {
        NetworkedObject[] networkedObjects = Resources.FindObjectsOfTypeAll<NetworkedObject>();
        foreach (NetworkedObject obj in networkedObjects) {
            if (!obj.gameObject.activeSelf) {
                if(obj.GetComponent<NetworkedSpawner>() != null) {
                    //Debug.Log("Networked spawner found");
                }
                //Debug.Log("Found inactive networkedObject, activating and spawning");
                if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer) {
                    obj.gameObject.SetActive(true);
                    obj.Spawn(destroyWithScene: true);

                } else {
                    Destroy(obj.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Spawn all players into the scene
    /// </summary>
    private void SpawnPlayers() {
        if (!NetworkingManager.Singleton.IsHost && !NetworkingManager.Singleton.IsServer) {
            Debug.LogError("Tried to spawn a player when not the server");
            return;
        }

        if(!SessionManager.Singleton.IsNetworkedGameStateReady()) {
            Debug.LogError("Tried to spawn players when networked game state not ready");
            return;
        }

        //Debug.Log("Spawning Players in Scene");

        Dictionary<int, PlayerInfo> connectedPlayers = SessionManager.Singleton.networkedGameState.GetAllPlayers();

        //Debug.Log("Connected players: " + connectedPlayers.Count);

        foreach (KeyValuePair<int, PlayerInfo> gamer in connectedPlayers)
        {
            if (gamer.Key != SessionManager.Singleton.tabletPlayer || isTestScene)
            {
                if (!players.ContainsKey(gamer.Key))
                {
                    //Spawn a new player in the scene
                    GameObject newPlayer = Instantiate(playerPrefab);

                    //Add to saved list of players
                    players.Add(gamer.Key, newPlayer.GetComponent<player>());
                    players[gamer.Key].SetPlayerNumber(gamer.Key);

                    //Record the initial spawn of this player
                    SessionManager.Singleton.dataRecorder.RecordInteraction("PlayerSpawn", gamer.Key, newPlayer.transform.position.x, newPlayer.transform.position.y, null);

                    spawnElevator.MovePlayerIntoElevator(players[gamer.Key]);

                    //Spawn over the network
                    newPlayer.GetComponent<NetworkedObject>().Spawn(destroyWithScene: true); //by default spawns with the server as the owner
                    Debug.Log("P" + gamer.Key.ToString() + " Spawned into the game");
                }

                players[gamer.Key].setName(gamer.Value.characterName);
                if (gamer.Value.assignedController > 0)
                {
                    players[gamer.Key].setController(gamer.Value.assignedController);
                }
                //Debug.Log(players[gamer.Key].getName() + " settings updated");
            }
        }
    }
}
