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
using TMPro;
//using MLAPI.SceneManagement;

//Controls Level specific tasks (Spawning objects & players)
public class ElevatorLevelController : MonoBehaviour
{

    public float tabletTimer = 0;
    public bool timerOn = false;
    //SINGLETON
    public static ElevatorLevelController Singleton { get; private set; }
    
    
    public player richPlayer; //Current player object in charge of distributing money
    public List<player> playersInElevator = new List<player>();

    public bool listenForControllersOnStart; //if true, will listen for controllers to press the "start" button. Upon hitting start, spawns the player inside the start elevator
    public GameObject playerPrefab;

    public bool distribute = false;
    public bool dontdist = false;

    public TextMeshPro playerText;
    public TextMeshPro timerText;


    private List<int> potentialTablets = new List<int>();

    private Dictionary<int, player> players;
    void Start()
    {
        players = new Dictionary<int, player>();
        //Add the tablet player to the end elevator list
        SessionManager.Singleton.tabletPlayer = -1;

        if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer)
        {
            //If listening for controllers on start, expecting there to be no players and will spawn a player on start
            if (!listenForControllersOnStart)
            {
                SpawnPlayers();
            }
        }

        StartCoroutine(DelayedSpawn(0.5f));

    }

    private void OnEnable()
    {
        if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer)
        {
            if (listenForControllersOnStart)
            {
                SessionManager.Singleton.StartListeningForControllers(true);
                SessionManager.Singleton.PlayerStateChangedCallback += SpawnPlayers;

            }
        }

        //Set Singleton
        if (Singleton != null && this != Singleton)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
        }
    }

    void Update()
    {

        //Return to Main menu upon hitting "ESC" key
        //KeyCode.Escape works for Android & Windows Builds
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SessionManager.Singleton.ReturnToMainMenu();
        }

        //Return to main menu if the connection with the tablet is lost
        if (ConnectionManager.Singleton.GetState() == ConnectionState.DISCONNECTED)
        {
            Debug.Log("Stopping game due to disconnection");
            SessionManager.Singleton.ReturnToMainMenu();
        }

        if (timerOn)
        {
            tabletTimer += Time.deltaTime;
            //This is an example of the text that will be shown, Must change from Debug.Log to an actual in game text
            timerText.SetText("Starting new level in " + (Mathf.Round((5-tabletTimer) * 10.0f) / 10.0f).ToString());

            if (tabletTimer >= 5)
            {
                tabletTimer = 0;
                SessionManager.Singleton.LoadNextFloor();
                timerOn = false;
            }
        }

    }

    private void OnDisable()
    {
        if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer)
        {
            if (listenForControllersOnStart)
            {
                //Debug.Log("Removing");
                SessionManager.Singleton.PlayerStateChangedCallback -= SpawnPlayers;
            }
        }
    }

    IEnumerator DelayedSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        InitEnvironmentObjects();
    }

    /// <summary>
    /// Initialises the game environment in the network
    /// </summary>
    private void InitEnvironmentObjects()
    {
        NetworkedObject[] networkedObjects = Resources.FindObjectsOfTypeAll<NetworkedObject>();
        foreach (NetworkedObject obj in networkedObjects)
        {
            if (!obj.gameObject.activeSelf)
            {
                if (obj.GetComponent<NetworkedSpawner>() != null)
                {
                    Debug.Log("Networked spawner found");
                }
                //Debug.Log("Found inactive networkedObject, activating and spawning");
                if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer)
                {
                    obj.gameObject.SetActive(true);
                    obj.Spawn(destroyWithScene: true);

                }
                else
                {
                    //Destroy(obj.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Spawn all players into the scene
    /// </summary>
    private void SpawnPlayers()
    {
        if (!NetworkingManager.Singleton.IsHost && !NetworkingManager.Singleton.IsServer)
        {
            Debug.LogError("Tried to spawn a player when not the server");
            return;
        }

        if (!SessionManager.Singleton.IsNetworkedGameStateReady())
        {
            Debug.LogError("Tried to spawn players when networked game state not ready");
            return;
        }

        //Debug.Log("Spawning Players in Scene");

        //Dictionary<int, PlayerInfo> connectedPlayers = SessionManager.Singleton.networkedGameState.GetAllPlayers();

        //Debug.Log("Connected players: " + connectedPlayers.Count);
        Debug.Log(SessionManager.Singleton.networkedGameState.inElevator.Count + " players in the elevator at the end of that level");

        foreach (int playerNumber in SessionManager.Singleton.networkedGameState.inElevator)
        {
            if (!players.ContainsKey(playerNumber))
            {
                //Spawn a new player in the scene
                GameObject newPlayer = Instantiate(playerPrefab);

                //Add to saved list of players
                players.Add(playerNumber, newPlayer.GetComponent<player>());
                newPlayer.GetComponent<Transform>().position = new Vector3(0, -4, 0);
                players[playerNumber].SetPlayerNumber(playerNumber);

                //Spawn over the network
                newPlayer.GetComponent<NetworkedObject>().Spawn(destroyWithScene: true); //by default spawns with the server as the owner
                PlayerInfo gamer = SessionManager.Singleton.networkedGameState.GetPlayer(playerNumber);
                Debug.Log("P" + playerNumber.ToString() + " Spawned into the game");

                players[playerNumber].setName(gamer.characterName);
                if (gamer.assignedController > 0)
                {
                    players[playerNumber].setController(gamer.assignedController);
                }

                if (playerNumber == SessionManager.Singleton.networkedGameState.richPlayer.Value)
                {
                    richPlayer = newPlayer.GetComponent<player>();
                }
                else
                {
                    playersInElevator.Add(newPlayer.GetComponent<player>());
                }
            }
        }
        if(richPlayer != null)
        {
            distribute = true;
        }
        else
        {
            dontdist = true;
            spawnRest();
        }
    }

    public void spawnRest()
    {
        foreach (KeyValuePair<int, PlayerInfo> newestPlayer in SessionManager.Singleton.networkedGameState.GetAllPlayers())
        {
            if (!SessionManager.Singleton.networkedGameState.inElevator.Contains(newestPlayer.Key))
            {
                int playerNumber = newestPlayer.Key;
                //Spawn a new player in the scene
                GameObject newPlayer = Instantiate(playerPrefab);

                //Add to saved list of players
                players.Add(playerNumber, newPlayer.GetComponent<player>());
                newPlayer.GetComponent<Transform>().position = new Vector3(0, -4, 0);
                players[playerNumber].SetPlayerNumber(playerNumber);

                //Spawn over the network
                newPlayer.GetComponent<NetworkedObject>().Spawn(destroyWithScene: true); //by default spawns with the server as the owner
                PlayerInfo gamer = SessionManager.Singleton.networkedGameState.GetPlayer(playerNumber);
                Debug.Log("P" + playerNumber.ToString() + " Spawned into the game");

                players[playerNumber].setName(gamer.characterName);
                potentialTablets.Add(playerNumber);

                if (gamer.assignedController > 0)
                {
                    players[playerNumber].setController(gamer.assignedController);
                }
            }
        }
        checkWin();
        nextTablet();
    }

    private void nextTablet()
    {
        if(potentialTablets.Count > 0)
        {
            //If some players don't make it, pick one at random to be the next tablet player
           SessionManager.Singleton.tabletPlayer = potentialTablets[Random.Range(0, potentialTablets.Count)];
        }
        else
        {
            //If all players make it, last player in the elevator is tablet player
            SessionManager.Singleton.tabletPlayer = SessionManager.Singleton.networkedGameState.inElevator[SessionManager.Singleton.networkedGameState.inElevator.Count-1];
        }

        playerText.SetText(SessionManager.Singleton.networkedGameState.GetPlayer(SessionManager.Singleton.tabletPlayer).username + " is new Mission Control");
        //TODO Stuff to tell the players who the new tablet player is and that
        //Once it's all done, we just need to do
        timerOn = true;
        
    }

    private void checkWin()
    {
        if (NetworkingManager.Singleton.IsServer || NetworkingManager.Singleton.IsHost) {
            bool groupWin = true;
            bool soloWin = false;
            foreach (KeyValuePair<int, PlayerInfo> newestPlayer in SessionManager.Singleton.networkedGameState.GetAllPlayers()) {
                if (newestPlayer.Value.score > SessionManager.Singleton.soloWin) {
                    soloWin = true;
                }
                if (newestPlayer.Value.score < SessionManager.Singleton.groupWin) {
                    groupWin = false;
                }
            }

            if (groupWin) {
                SessionManager.Singleton.LoadWinScene(true);
            } else if (soloWin) {
                SessionManager.Singleton.LoadWinScene(false);
            }
        }
    }
}
