/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.SceneManagement;
using System.Collections;

//Overall manager for any information that needs to be kept between scenes
//Including
//  - Each player's name, character & controller assignment 
//  - Game score (When Implemented)
//  - Player Login Info (When Implemented)
//  - Game statistics (When Implemented)
//
//Also manages top level scene switching
public class SessionManager : MonoBehaviour {

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Defines ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //Information for each player's state
    [Obsolete("Use Public PlayerInfo class instead")]
    public class PlayerInfo {
        public int controllerNum;
        public string name;
        public int score;

        public PlayerInfo(int controllerNumber, string playerName) {
            controllerNum = controllerNumber;
            name = playerName;
            score = 0;
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //[Obsolete("Use SessionManager.PlayerStateChangedCallback instead")]
    //public Action PlayerConnectedCallback; //Called when a player connects (presses start button on their controller && not already connected && SessionManager is listening for more connections)


    public static SessionManager Singleton { get; private set; }

    public DataRecorder dataRecorder; //The instance of local data info

    //Networked game information that remains between scenes (will be null until setup. Call IsNetworkedGameStateReady() before using)
    public NetworkedGameState networkedGameState { get; private set; }
    public GameObject networkedGameStatePrefab; //Prefab to spawn as the networked game state

    public Action PlayerStateChangedCallback; //Called when any player's username / character name / controller assignment changes (called on both instances of the game when connected over the network)
    public Action PlayerScoreChangedCallback; //Called when any player's score changes (called on both instances of the game when connected over the network)

    public int floorsUntilGameOver = 5; //after going though this many floors, the game will send the players to the gameover scene


    private List<int> unassignedControllers; //List of controller numbers not yet assigned to a player
    private bool isListeningForControllers = false;
    private bool createNewUserOnControllerAdded = false; //If true & listening for controllers, will create a new user when an unassigned controller hits the start button

    public int tabletPlayer = -1; //Current tablet player. If -1, no player is set as tablet player

    public int groupWin;
    public int soloWin;

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Behavioural ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    void Start() {
        ResetUnassignedControllers();
        NetworkSceneManager.OnSceneSwitched += OnSceneChanged;
    }

    //Sets the singleton for this class. Ensures only one copy of this script is instantiated.
    private void OnEnable() {

        if (Singleton != null && this != Singleton) {
            Destroy(this.gameObject);
        } else {
            Singleton = this;
        }
    }

    void Update() {

        //Listen for new players hitting "start" on their controllers
        CheckForControllerAssignment();

        //Spawn the networked game state if not already spawned
        if(networkedGameState == null && (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer)) { //&& ConnectionManager.Singleton.GetState() == ConnectionState.CONNECTED
            //Server running but networkedGameState not set, create a new one
            GameObject newSpawn = Instantiate(networkedGameStatePrefab);

            networkedGameState = newSpawn.GetComponent<NetworkedGameState>();
            if (networkedGameState == null) {
                Debug.LogError("NetworkedGameStatePrafab does not have a NetworkedGameState component!");
            } else {
                //Spawn over the network
                newSpawn.GetComponent<NetworkedObject>().Spawn(destroyWithScene: false); //by default spawns with the server as the owner
            }
        }
    }


    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //Set the networked game state
    //Expected to be called by the NetworkedGameState once added to the scene
    public void SetNetworkedGameState(NetworkedGameState gameState) {
        //if (networkedGameState != null) {
        //    Debug.LogWarning("Overriding networked game state. Is there two in the scene?");
        //}
        networkedGameState = gameState;
    }

    //Checks if the networked game state is ready to be used by the game
    //Returns true if the networked game state is visible on the server and all clients, false otherwise
    public bool IsNetworkedGameStateReady() {
        if (networkedGameState == null) {
            return false;
        }

        return networkedGameState.NetworkedObject.IsSpawned;
    }

    //When called, will start listening for unassigned controllers pressing the start button
    //This will initiate adding a new user / assigned a controller to the next user without on
    [Obsolete("Use SessionManager.singleton.StartListeningForControllers(true) for the same functionality")]
    public void StartListeningForControllers() {
        StartListeningForControllers(true);
    }

    //When called, will start listening for unassigned controllers pressing the start button
    //When an unassigned controller presses the start button, the next player without a controller assigned will be assigned the controller that presses start
    //If createNewUsers is true, pressing start on an unassigned controller will create a new user if there is no existing user in need of controller assignment
    public void StartListeningForControllers(bool createNewUsers) {
        Debug.Log("Started listening for controllers" + (createNewUsers? "" : " (Will add new users if needed)"));
        isListeningForControllers = true;
        createNewUserOnControllerAdded = createNewUsers;
    }

    /// <summary>
    ///  Will stop listening for unassigned controllers (an also stop assigning them to users)
    /// </summary>
    public void StopListeningForControllers() {
        isListeningForControllers = false;
        createNewUserOnControllerAdded = false;
    }

    /// <summary>
    /// A function that can be called to change the score of any player.
    /// Scores cannot become negative
    /// </summary> 
    /// <param name="cNum"> The controller number of the player that is losing score </param>
    /// <param name="change"> The value which must be added to the player's score </param>
    [Obsolete("Please use SessionManager.Singleton.networkedGameState.UpdateScore instead. Note the use of player num and not controller num")]
    public void updatePlayerScore(int cNum, int change) {

        if (!IsNetworkedGameStateReady()) {
            Debug.LogError("Tried to set player's score when game state not ready");
            return;
        }

        Dictionary<int, CrimeyBoyz.GameState.PlayerInfo> players = networkedGameState.GetAllPlayers();

        foreach (KeyValuePair<int, CrimeyBoyz.GameState.PlayerInfo> player in players) {
            networkedGameState.UpdateScore(player.Key, change);
        }
    }

    /// <summary>
    /// A function to get the players currently in the game
    /// </summary>
    /// <returns>
    /// Returns a dictionary of all players in the game, identified by their 'player number'
    /// </returns>
    [Obsolete("User SessionManager.Singleton.networkedGameState.GetAllPlayers instead")]
    public Dictionary<int, PlayerInfo> GetPlayers() {

        Dictionary<int, PlayerInfo> result = new Dictionary<int, PlayerInfo>();

        if (!IsNetworkedGameStateReady()) {
            Debug.LogError("Tried to set player's score when game state not ready");
            return result;
        }

        Dictionary<int, CrimeyBoyz.GameState.PlayerInfo> temp = networkedGameState.GetAllPlayers();

        foreach(KeyValuePair<int, CrimeyBoyz.GameState.PlayerInfo> player in temp) {
            result[player.Key] = new PlayerInfo(player.Value.assignedController, player.Value.characterName);
            result[player.Key].score = player.Value.score;
        }

        return result;
        
    }

    /// <summary>
    /// A function to get the scores of players currently in the game
    /// </summary>
    /// <returns>
    /// Returns a dictionary of players and their scores, identified by their 'player numbers'
    /// </returns>
    [Obsolete("Please use SessionManager.Singleton.networkedGameState.GetAllScores() instead")]
    public Dictionary<int, int> GetPlayerScores() {

        if (!IsNetworkedGameStateReady()) {
            Debug.LogError("Tried to get player's score when game state not ready");
            return new Dictionary<int, int>();
        }

        return networkedGameState.GetAllScores();
    }

    /// <summary>
    /// A function that gets the information on a specific player based on their player number
    /// </summary>
    /// <param name="playerNum"> The player number of the selected player </param>
    /// <returns>The player associated with the player number 'playerNum' /returns>
    [Obsolete("Please use SessionManager.Singleton.networkedGameState.GetPlayer instead")]
    public PlayerInfo GetPlayer(int playerNum) {

        if (!IsNetworkedGameStateReady()) {
            Debug.LogError("Tried to get player when game state not ready");
            return null;
        }

        CrimeyBoyz.GameState.PlayerInfo tmp = networkedGameState.GetPlayer(playerNum);

        PlayerInfo player = new PlayerInfo(tmp.assignedController, tmp.characterName);
        player.score = tmp.score;

        return player;
    }


    /// <summary>
    /// A function to reset the players in the game
    /// </summary>
    [Obsolete("Should not need to use this. Players are reset when calling SessionManager.Singleton.ReturnToMainMenu")]
    public void ResetPlayers() {
        if (!IsNetworkedGameStateReady()) {
            Debug.LogError("Tried to reset players when game state not ready");
            return;
        }

        networkedGameState.ResetAll();

        ResetUnassignedControllers();
    }

    /// <summary>
    /// A function to assign a player with a name
    /// </summary>
    /// <param name="playerNum"> The number of the player being accessed </param>
    /// <param name="name"> The new name of the player </param>
    [Obsolete("Please use SessionManager.Singleton.networkedGameState.SetCharacterName instead")]
    public void AssignPlayerName(int playerNum, string name) {

        if (!IsNetworkedGameStateReady()) {
            Debug.LogError("Tried to assign player name when game state not ready");
            return;
        }

        networkedGameState.SetCharacterName(playerNum, name);
    }

    /// <summary>
    /// Stops the current game and return to main menu (only if main menu added to build settings)
    /// If menu not in build settings, reloads the current scene
    /// Note: will reset game state (scores, user settings, logged in users....)
    /// </summary>
    public void ReturnToMainMenu() {

        //PlayerConnectedCallback = null;
        PlayerStateChangedCallback = null;
        PlayerScoreChangedCallback = null;
        ResetUnassignedControllers();
        StopListeningForControllers();
        if (IsNetworkedGameStateReady()) {
            networkedGameState.ResetAll();
        }

        ConnectionManager.Singleton.retryUponFailure = false;
        ConnectionManager.Singleton.CloseConnection();

        if (IsSceneInBuild("Menu")) {
            SceneManager.LoadScene("Menu");

        } else {
            Debug.LogWarning("Tried to return to main menu but it does not exist in the build settings, restarting current scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// Load the next game level (Currently just loads the same level over and over)
    /// </summary>
    [Obsolete("Call LoadNextFloor instead")]
    public void LoadNextLevel() {
        LoadNextFloor();
    }

    public void LoadWinScene(bool groupWin) {
        if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer) {
            StopListeningForControllers();

            string sceneToLoad = groupWin ? "WinSceneGroup" : "WinSceneSolo";

            if (IsSceneInBuild(sceneToLoad)) {
                NetworkSceneManager.SwitchScene(sceneToLoad);
            } else {
                Debug.LogError("The Scene \"" + sceneToLoad + "\" does not exist in the build, returning to main menu");
                ReturnToMainMenu();
            }

        } else {
            Debug.LogWarning("Tried to load next floor when not the server");
        }
    }

        /// <summary>
        /// Load the next game floor
        /// Currently loads the same level until the floorsUntiGameOver limit is reached
        /// Once the limit is reached, proceeds to gameOver scene
        /// </summary>
        public void LoadNextFloor() {
        if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer) {

            if(!IsNetworkedGameStateReady()) {
                Debug.LogError("Tried to load the next floor when the networked game state is not ready");
                return;
            }

            if(SceneManager.GetActiveScene().name == "Menu") {
                dataRecorder.ResetData();
                dataRecorder.StartSession(networkedGameState);
            }

            StopListeningForControllers();

            int nextFloor = networkedGameState.GetCurrentFloor() + 1;
            string sceneToLoad;

            //Set the next scene based on nextFloor value
            if (SceneManager.GetActiveScene().name != "ElevatorInterlude" && SceneManager.GetActiveScene().name != "Menu") {
                sceneToLoad = "ElevatorInterlude";

            } else if (nextFloor > floorsUntilGameOver) {
                sceneToLoad = "LoseScene";

            } else {

                sceneToLoad = "Floor " + nextFloor.ToString();
                networkedGameState.SetCurrentFloor(nextFloor);
            }


            if (IsSceneInBuild(sceneToLoad)) {
                NetworkSceneManager.SwitchScene(sceneToLoad);
            } else {
                Debug.LogError("The Scene \"" + sceneToLoad + "\" does not exist in the build, returning to main menu");
                ReturnToMainMenu();
            }
        } else {
            Debug.LogWarning("Tried to load next floor when not the server");
        }
    }

    /// <summary>
    /// Resets the ownership of objects in the environment
    /// </summary>
    public static void ResetEnvironmentObjectOwnership() {
        if (NetworkingManager.Singleton.IsServer || NetworkingManager.Singleton.IsHost) {
            Debug.Log("Resetting all environment object ownership");

            NetworkedEnvironmentObject[] objectsToReset = Resources.FindObjectsOfTypeAll<NetworkedEnvironmentObject>();

            foreach (NetworkedEnvironmentObject objToReset in objectsToReset) {
                objToReset.ResetOwnership();
            }
        }
    }

    //Reset unassigned controller to their default values
    public void ResetUnassignedControllers() {
        unassignedControllers = new List<int> { 1, 2, 3, 4, 5 };
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //Callback for when the scene changed
    private void OnSceneChanged() {
        //Debug.Log("Scene changed, disabling retry on disconnect");
        //ConnectionManager.Singleton.retryUponFailure = false;

        ResetEnvironmentObjectOwnership();
        if (IsNetworkedGameStateReady()) {

            if (SceneManager.GetActiveScene().name == "ElevatorInterlude") {
                dataRecorder.DisableInteractionsUntilNextRound();

            } else {

                dataRecorder.StartNextRound(networkedGameState);
            }
        }
    }


    //If isListeningForControllers is false, does nothing
    //Check if a user has hit the "Start" button on an unassigned controller
    //If this is the case, will assign the controller that pressed start to the next player without a controller assigned
    //If ListenForControllers(true) was called then will create a new player if non without a controller exist
    private void CheckForControllerAssignment() {
        //listen for a player to join (Press Start button)
        if (!isListeningForControllers) {
            return;
        }

        foreach (int controllerNum in unassignedControllers) {
            if (Input.GetButton("Start" + controllerNum)) {

                //Unassigned controller hit start button, assign them to a player
                Dictionary<int, CrimeyBoyz.GameState.PlayerInfo> players = networkedGameState.GetAllPlayers();

                bool foundPlayer = false;
                bool assignedController = false;

                foreach (KeyValuePair<int, CrimeyBoyz.GameState.PlayerInfo> player in players) {
                    if (player.Value.assignedController == -1) {
                        //Found a player without a controller

                        if (!IsNetworkedGameStateReady()) {
                            Debug.LogError("Tried to set player's controller when game state not ready");
                            return;
                        }

                        networkedGameState.SetControllerAssignment(player.Key, controllerNum);
                        networkedGameState.SetCharacterName(player.Key, AllCharacters.Singleton.getDefaultName());
                        Debug.Log("Username: '" + player.Value.username + "' is using controller number: " + controllerNum);
                        foundPlayer = true;
                        assignedController = true;
                    }
                }

                if (!foundPlayer && createNewUserOnControllerAdded) {
                    //Create a new guest account
                    if (!IsNetworkedGameStateReady()) {
                        Debug.LogError("Tried to create a new player when game state not ready");
                        return;
                    }
                    int newPlayer = players.Count + 1;
                    networkedGameState.SetUsername(newPlayer, "");
                    networkedGameState.SetCharacterName(newPlayer, AllCharacters.Singleton.getDefaultName());
                    networkedGameState.SetControllerAssignment(newPlayer, controllerNum);
                    assignedController = true;
                }

                if (assignedController) {
                    unassignedControllers.Remove(controllerNum);
                    PlayerStateChangedCallback?.Invoke();
                    //PlayerConnectedCallback?.Invoke();
                }
                break;
            }
        }
    }

    //Check if the given scene name is in the build settings
    private bool IsSceneInBuild(string sceneName) {
        List<string> scenesInBuild = new List<string>();
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            int lastSlash = path.LastIndexOf("/");
            scenesInBuild.Add(path.Substring(lastSlash + 1, path.LastIndexOf(".") - lastSlash - 1));
        }

        return scenesInBuild.Contains(sceneName);
    }

    public void setWinCon()
    {
        switch (networkedGameState.GetAllPlayers().Count)
        {
            case 4:
                groupWin = 1000;
                soloWin = 2500;
                break;
            case 5:
                groupWin = 800;
                soloWin = 2000;
                break;
            default:
                //If less than 4, use the values for 3 players
                groupWin = 1200;
                soloWin = 3000;
                break;
        }
    }
}
