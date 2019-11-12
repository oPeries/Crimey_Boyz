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
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.NetworkedVar.Collections;
using CrimeyBoyz.GameState;

//Manages game state info across the network that must be kept between scene transitions
//Pretty much a networked data structure
public class NetworkedGameState : NetworkedBehaviour {

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Defines ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public float maxUpdateFrequency = 5f; //Max number of times per second each networked var can be updated

    private NetworkedDictionary<int, string> syncedUsernames = new NetworkedDictionary<int, string>(); //Key: player number, Value: corresponding username (username is "" if using a guest account, and null if user is in the process of loggin in)
    private NetworkedDictionary<int, string> syncedCharacterNames = new NetworkedDictionary<int, string>(); //Key: player number, Value: corresponding character name
    private NetworkedDictionary<int, int> syncedControllerAssignments = new NetworkedDictionary<int, int>(); //Key: player number, Value: corresponding controller (can set to -1 for unassigned)
    private NetworkedDictionary<int, int> syncedScores = new NetworkedDictionary<int, int>(); //Key: player number, Value: corresponding score

    private NetworkedVar<int> syncedCurrentFloor = new NetworkedVar<int>(); //The current floor of the game in progress (-1 if in the menu)
    private NetworkedDictionary<int, int> syncedMissionControlHistory = new NetworkedDictionary<int, int>(); //Key: a floor number, Value: the player number of who was missionControl for that floor

    public NetworkedList<int> inElevator;
    public NetworkedVar<int> richPlayer;

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Behavioural ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void Awake() {

        if(SessionManager.Singleton != null) {
            SessionManager.Singleton.SetNetworkedGameState(this);

        } else {
            Debug.LogWarning("Could not find session manager when starting networked game state");
        }

    }

    void Start() {

        //Init networked vars
        syncedUsernames.Settings.WritePermission = NetworkedVarPermission.Everyone;
        syncedUsernames.Settings.SendTickrate = maxUpdateFrequency;
        syncedUsernames.OnDictionaryChanged += OnUsernameChanged;

        syncedCharacterNames.Settings.WritePermission = NetworkedVarPermission.ServerOnly;
        syncedCharacterNames.Settings.SendTickrate = maxUpdateFrequency;
        syncedCharacterNames.OnDictionaryChanged += OnCharacterNameChanged;

        syncedControllerAssignments.Settings.WritePermission = NetworkedVarPermission.ServerOnly;
        syncedControllerAssignments.Settings.SendTickrate = maxUpdateFrequency;
        syncedControllerAssignments.OnDictionaryChanged += OnControllerAssignmentChanged;

        syncedScores.Settings.WritePermission = NetworkedVarPermission.ServerOnly;
        syncedScores.Settings.SendTickrate = maxUpdateFrequency;
        syncedScores.OnDictionaryChanged += OnScoreChanged;

        syncedCurrentFloor.Settings.WritePermission = NetworkedVarPermission.ServerOnly;
        syncedCurrentFloor.Settings.SendTickrate = maxUpdateFrequency;

        syncedMissionControlHistory.Settings.WritePermission = NetworkedVarPermission.ServerOnly;
        syncedMissionControlHistory.Settings.SendTickrate = maxUpdateFrequency;

        richPlayer.Value = -1;

        inElevator.OnListChanged += OnInElevatorChanged;

        if (IsServer) {
            ResetAll();
        }
    }


    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //How many users/players are currently logged in?
    public int PlayerCount() {
        return syncedUsernames.Count;
    }

    //Does the player identified by the given player number exists? (Note: player numbers are expected to start at 1)
    public bool DoesPlayerExist(int playerNum) {
        return syncedUsernames.ContainsKey(playerNum);
    }

    //Check if there is a player not logged in (has a null username)
    public bool IsPlayerNotLoggedIn() {
        foreach(KeyValuePair<int, string> user in syncedUsernames) {
            if(user.Value == null) {
                return true;
            }
        }
        return false;
    }

    //Build the playerInfo for the given player number (returns null if player does not exits)
    //NOTE: returns a copy of the player info and changing the returned result will not change the actual values. Use the methods in this class instead
    public PlayerInfo GetPlayer(int playerNum) {

        if(!DoesPlayerExist(playerNum)) {
            return null;
        }

        PlayerInfo result = new PlayerInfo();
        result.username = syncedUsernames[playerNum];

        if(syncedCharacterNames.ContainsKey(playerNum)) {
            result.characterName = syncedCharacterNames[playerNum];
        }

        if (syncedControllerAssignments.ContainsKey(playerNum)) {
            result.assignedController = syncedControllerAssignments[playerNum];
        }

        if (syncedScores.ContainsKey(playerNum)) {
            result.score = syncedScores[playerNum];
        }

        return result;
    }

    //Build the playerInfo's for all players
    //Int in returned value is the player's payerNum
    //Returns empty dictionary if no players exist
    //NOTE: returns a copy of the player info and changing the returned result will not change the actual values. Use the methods in this class instead
    public Dictionary<int, PlayerInfo> GetAllPlayers() {

        Dictionary<int, PlayerInfo> result = new Dictionary<int, PlayerInfo>();

        foreach (int playerNum in syncedUsernames.Keys) {
            result[playerNum] = GetPlayer(playerNum);
        }

        return result;
    }

    //Set the username for the specified player ("" is a guest player and null is for a player in the process of signing in)
    //Setting a username for a player number that does not exist will be considered as "creating" that player
    //Note: playerNum is expected to be the next playerNum in the sequence (e.g. if there are n players their numbers should be 1,2,3.. x)
    //Note: setting username to null is expected when that player is in the process of logging in
    //Note: setting the username to "" is expected when the player is using a guest account
    public void SetUsername(int playerNum, string username) {
        Debug.Log("Setting player " + playerNum + "'s username to \"" + username + "\"");
        if (playerNum != syncedUsernames.Count + 1) {
            Debug.LogWarning("Warning: username added out of order, may cause issues");
        }
        syncedUsernames[playerNum] = username;
    }

    //Set the character name for the given player number (should only be called on the server)
    public void SetCharacterName(int playerNum, string characterName) {
        Debug.Log("Setting player " + playerNum + " to character name: " + characterName);
        if (!IsServer) {
            Debug.LogWarning("Tried to set character name when not the server, leaving name as is");
            return;
        }

        if (!DoesPlayerExist(playerNum)) {
            Debug.LogWarning("Setting character name for a player that does not exist. Player: " + playerNum.ToString());
        }

        syncedCharacterNames[playerNum] = characterName;
    }

    //Set the assigned controller for the given player number (should only be called on the server)
    //Can set controller to -1 to be considered unsassigned
    public void SetControllerAssignment(int playerNum, int controllerNum) {
        //Debug.Log("Setting player " + playerNum + " to controller: " + controllerNum);
        if (!IsServer) {
            Debug.LogWarning("Tried to set assigned controller when not the server, leaving as is");
            return;
        }

        if (!DoesPlayerExist(playerNum)) {
            Debug.LogWarning("Setting controller for a player that does not exist. Player: " + playerNum.ToString());
        }

        syncedControllerAssignments[playerNum] = controllerNum;
    }




    //Get the score for the given player number
    //Returns -1 if the given player does not exit or has not been synced yet
    public int GetPlayerScore(int playerNum) {
        if(!DoesPlayerExist(playerNum) || !syncedScores.ContainsKey(playerNum)) {
            return -1;
        }

        return syncedScores[playerNum];        
    }

    //Get the scores for all players
    //Returns an empty dict if no players exist
    //NOTE: returns a copy of the player socore and changing the returned result will not change the actual values. Use the methods in this class instead
    public Dictionary<int, int> GetAllScores() {
        Dictionary<int, int> result = new Dictionary<int, int>();

        foreach (int playerNum in syncedUsernames.Keys) {
            result[playerNum] = GetPlayerScore(playerNum);
        }

        return result;
    }

    //Set the score for the given player number (should only be called on the server)
    public void SetScore(int playerNum, int score) {
        Debug.Log("Setting player " + playerNum + "'s score to: " + score);
        if (!IsServer) {
            Debug.LogWarning("Tried to set character score when not the server, leaving as is");
            return;
        }

        if (!DoesPlayerExist(playerNum)) {
            Debug.LogWarning("Setting score for a player that does not exist. Player: " + playerNum.ToString());
        }

        syncedScores[playerNum] = score;
    }

    //Changes the score for player playerNum by the amount specified
    //If the updated player score will be below 50, does nothing
    public void UpdateScore(int playerNum, int change) {
        Debug.Log("Changing player " + playerNum + "'s score to: " + change.ToString());
        if (!IsServer) {
            Debug.LogWarning("Tried to update score when not the server, leaving as is");
            return;
        }

        if (!DoesPlayerExist(playerNum)) {
            Debug.LogWarning("Updating score for a player that does not exist. Player: " + playerNum.ToString());
        }

        if ((change < 0 && GetPlayerScore(playerNum) >= 50) || change > 0) {
            if (change == -50)
                Debug.Log("Should be losing score");
            SetScore(playerNum, GetPlayerScore(playerNum) + change);
        }
    }



    //Get the current floor for the game (-1 if in menus / not started)
    public int GetCurrentFloor() {
        return syncedCurrentFloor.Value;
    }

    //Set the current floor (should only be called on the server)
    public void SetCurrentFloor(int currentFloor) {
        //Debug.Log("Setting current floor to " + currentFloor);
        if (!IsServer) {
            Debug.LogWarning("Tried to set current floor when not the server, leaving as is");
            return;
        }

        syncedCurrentFloor.Value = currentFloor;
    }



    //History of who was mission control
    //NOTE: returns a copy of the mission control history and changing the returned result will not change the actual values. Use the methods in this class instead
    public Dictionary<int, int> GetMissionControlHistory() {
        Dictionary<int, int> result = new Dictionary<int, int>();

        foreach (int floorNum in syncedMissionControlHistory.Keys) {
            result[floorNum] = syncedMissionControlHistory[floorNum];
        }

        return result;
    }

    //Set the mission control history (should only be called on the server)
    //playerNum is the player who was mission control for the specified floor
    public void SetMissionControlHistory(int floor, int playerNum) {
        Debug.Log("Setting player " + playerNum + " to being the tablet player for floor " + floor);
        if (!IsServer) {
            Debug.LogWarning("Tried to set mission control history when not the server, leaving as is");
            return;
        }

        if (!DoesPlayerExist(playerNum)) {
            Debug.LogWarning("Setting Mission Control history to a player that does not exists, this should not happen");
        }

        syncedMissionControlHistory[floor] = playerNum;
    }


    //Clears all data in the game state (resets to default)
    public void ResetAll() {
        Debug.Log("Clearing networked game state of all data");

        syncedUsernames.Clear();
        syncedCharacterNames.Clear();
        syncedControllerAssignments.Clear();
        syncedScores.Clear();

        syncedCurrentFloor.Value = 0;
        syncedMissionControlHistory.Clear();
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //Called whenever the syncedUsername dictionary changes
    private void OnUsernameChanged(NetworkedDictionaryEvent<int, string> changeEvent) {

        //Debug.Log(string.Format("Username Changed Event Type:{0} Key:{1} Value{2}", changeEvent.GetType().ToString(), changeEvent.key, changeEvent.value));

        if (DoesPlayerExist(changeEvent.key)) {
            if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer) {
                SetScore(changeEvent.key, 0);
                Debug.Log("Setting score to 0 after new user added, playernum: " + changeEvent.key.ToString());
            }
        }

        SessionManager.Singleton?.PlayerStateChangedCallback?.Invoke();
    }

    //Called whenever the syncedCharacterNames dictionary changes
    private void OnCharacterNameChanged(NetworkedDictionaryEvent<int, string> changeEvent) {
        SessionManager.Singleton?.PlayerStateChangedCallback?.Invoke();
    }

    //Called whenever the snycedControllerAssignments dictionary changes
    private void OnControllerAssignmentChanged(NetworkedDictionaryEvent<int, int> changeEvent) {
        //Debug.Log("Character controller Assignment changed: " + changeEvent.key + " : " + changeEvent.value);
        SessionManager.Singleton?.PlayerStateChangedCallback?.Invoke();
    }

    //Called whenever the syncedScores dictionary changes
    private void OnScoreChanged(NetworkedDictionaryEvent<int, int> changeEvent) {
        SessionManager.Singleton?.PlayerScoreChangedCallback?.Invoke();
    }

    //Called whenever the syncedScores dictionary changes
    private void OnInElevatorChanged(NetworkedListEvent<int> changeEvent) {
        /*string result = "InElevator change. Current list: [";

        foreach(int i in inElevator) {
            result += i.ToString() + ", ";
        }

        Debug.Log(result + "]");*/
    }
}
