/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using CrimeyBoyz.GameState;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Attach to a Character Selection tile to manage character selection and customisation
public class CharacterSelectionController : MonoBehaviour {

    public Text playerText;
    public Image playerSprite;
    public Text characterText;
    public Text usernameText;

    private int playerNum; //The number of this player. E.g. Player 1. Player 2...
    private float axisCutoff = 0.7f; //Value in the joystick axis to register as a left/right/up/down make sure between 0 and 1
    private int assignedController;
    private bool recentlyChanged; //has the player recently changed their name and have to wait for joystick to return to the middle

    void Start() {
        //Set a callback when a new controller is connected
        SessionManager.Singleton.PlayerStateChangedCallback += UpdatePlayerStatus;

        assignedController = -1; //No controller assigned yet
    }

    private void OnEnable() {
        setPlayerNumber();
        UpdatePlayerStatus();
    }

    private void OnDestroy() {
        //Remove callback
        SessionManager.Singleton.PlayerStateChangedCallback -= UpdatePlayerStatus;
    }

    //switches chosen character when user moves left/right on their joystick
    void Update() {
        if(assignedController != -1) {
            float axis = Input.GetAxis("Horizontal" + assignedController);
            if (axis >= axisCutoff && !recentlyChanged) {

                string newName = AllCharacters.Singleton.getNextName(characterText.text);
                SetCharacterName(newName);
                SetPlayerSprite(newName);
                recentlyChanged = true;

            } else if (axis <= -1 * axisCutoff && !recentlyChanged) {

                string newName = AllCharacters.Singleton.getPreviousName(characterText.text);
                SetCharacterName(newName);
                SetPlayerSprite(newName);
                recentlyChanged = true;

            } else if (Mathf.Abs(axis) < axisCutoff) {
                recentlyChanged = false;
            }
        }
    }

    public void SetPlayerNumber(int playerNum) {
        this.playerNum = playerNum;
        UpdatePlayerStatus();
    }

    public int GetPlayerNumber() {
        return playerNum;
    }

    /// <summary>
    /// Callback for when a new controller hits "start"
    /// Check if the player associated with playerNum now has a joined controller
    /// </summary>
    private void UpdatePlayerStatus() {

        if(!SessionManager.Singleton.IsNetworkedGameStateReady()) {
            return;
        }

        PlayerInfo player = SessionManager.Singleton.networkedGameState.GetPlayer(playerNum);

        if(player == null || player.username == null) {
            assignedController = -1;
            SetUsername(null);
            SetCharacterName("Please Sign in from mission control");
            return;
        }

        SetUsername(player.username);
        assignedController = player.assignedController;
        //Debug.Log("Controller is: " + assignedController);
        if(assignedController >= 0) {
            string characterName = player.characterName;
            if (characterName == null || !characterName.Equals(characterText.text)) {
                characterName = AllCharacters.Singleton.getDefaultName();
                Debug.Log("Name set to " + characterName);
                SetCharacterName(characterName);
            }
            SetPlayerSprite(characterName);

        } else {
            Dictionary<int, PlayerInfo> players = SessionManager.Singleton.networkedGameState.GetAllPlayers();
            for(int i = 0; i < players.Count; i++ ) {
                if(players[i + 1].assignedController == -1) {

                    if (i + 1 == playerNum) {
                        SetCharacterName("");
                    } else {
                        SetCharacterName("Waiting for player " + i.ToString());
                    }
                    return;
                }
            }
            
        }
    }

    /// <summary>
    /// Set the "Player X" string at the top of the tile to match the assigned player number
    /// </summary>
    private void setPlayerNumber() {
        playerText.text = "Player " + playerNum.ToString();
    }

    private void SetCharacterName(string name) {
        if(name.Equals("")) {
            characterText.text = "Please Press Start";
        } else {
            characterText.text = name;
            if (AllCharacters.Singleton.characterNames.Contains(name))
                SessionManager.Singleton.networkedGameState.SetCharacterName(playerNum, name);
        }
    }

    private void SetUsername(string username) {
        if(username == null) {
            usernameText.text = "";
        } else if(username.Equals("")) {
            usernameText.text = "Guest";
        } else {
            usernameText.text = username;
        }
    }

    /// <summary>
    /// Get the sprite associated with the given character name. Set to be the sprite of this tile.
    /// </summary>
    /// <param name="name"> The name of the current chosen character </param>
    private void SetPlayerSprite(string name) {
        playerSprite.enabled = true;
        playerSprite.sprite = AllCharacters.Singleton.getSprite(name);
    }
}
