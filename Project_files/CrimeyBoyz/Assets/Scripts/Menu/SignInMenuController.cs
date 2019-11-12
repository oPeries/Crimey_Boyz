/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using CrimeyBoyz.GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CrimeyBoyz.Menu.MissionControl {

    //Main controller for the "PlayerSignIn" section of the mission control lobby
    //Also manages the signIn tiles
    public class SignInMenuController : MonoBehaviour {


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Defines ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private const string loginTileName = "Login with an Existing Account";
        private const string signUpTileName = "Create A New Account";

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public Text signInText; //"Player X Sign In" text
        public GameObject tileContainer; //Object with all its children as the PlayerSignInTile prefab
        public GameObject SignInTilePrefab; //prefab to spawn more sign in tiles
        public MissionControlLobbyController myController; //Main lobby controller to call when it's time to switch menu

        private List<SignInTileController> signInTiles; //Current signin tiles spawned in the menu

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Behavioural ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        private void Awake() {
            signInTiles = new List<SignInTileController>();
            SignInTileController[] tiles = GetComponentsInChildren<SignInTileController>();
            foreach(SignInTileController tile in tiles) {
                signInTiles.Add(tile);
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //Should be called when a tile's "Edit" button is pressed
        //Will let the main controller know that the next menu displayed should be the edit user one
        public void EditUser(string username) {
            myController.SetNextState(MissionControlLobbyController.LobbyState.EDITSAVED, username);
        }

        public void ForgetUser(string username) {
            SavedAccounts.ForgetUser(username);
            myController.SetNextState(MissionControlLobbyController.LobbyState.SIGNIN);
        }

        public void TilePressed(SignInTileController tile) {
            if (tile.IsUserTile()) {
                string cred = SavedAccounts.GetUserCred(tile.GetTileName());
                if (cred != null) {
                    myController.LoginUser(tile.GetTileName(), cred);
                    tile.SetLoading(true);
                } else {
                    Debug.LogError("Cookie is null for user: " + tile.GetTileName());
                }
            } else if(tile.GetTileName().Equals(loginTileName)) {
                myController.SetNextState(MissionControlLobbyController.LobbyState.LOGIN);

            } else if(tile.GetTileName().Equals(signUpTileName)) {
                myController.SetNextState(MissionControlLobbyController.LobbyState.SIGNUP);

            } else {
                Debug.LogError("Unknown tile pressed: " + tile.GetTileName());
            }
        }

        public void UpdateSignInPage() {

            //Set signin tile
            string playerNum = "X";
            if(SessionManager.Singleton.IsNetworkedGameStateReady()) {
                Dictionary<int, PlayerInfo> players = SessionManager.Singleton.networkedGameState.GetAllPlayers();
                bool foundPlayer = false;
                foreach(KeyValuePair<int, PlayerInfo> player in players) {
                    if(player.Value.username == null) {
                        playerNum = player.Key.ToString();
                        foundPlayer = true;
                        break;
                    }
                }
                if(!foundPlayer) {
                    playerNum = (players.Count + 1).ToString();
                }
            }
            signInText.text = "Player " + playerNum + " sign in";

            //Ensure tile at index 0 is the login tile
            if (signInTiles.Count == 0) {
                GameObject tile = Instantiate(SignInTilePrefab, tileContainer.transform);
                signInTiles.Add(tile.GetComponent<SignInTileController>());
                signInTiles[0].SetTileText(loginTileName, false, this);

            } else if (!signInTiles[0].GetTileName().Equals(loginTileName)) {
                signInTiles[0].SetTileText(loginTileName, false, this);
            }
            if (signInTiles[0].transform.GetSiblingIndex() != 0) {
                signInTiles[0].transform.SetSiblingIndex(0);
            }


            //Ensure tile at index 0 is the login tile
            if (signInTiles.Count == 1) {
                GameObject tile = Instantiate(SignInTilePrefab, tileContainer.transform);
                signInTiles.Insert(1, tile.GetComponent<SignInTileController>());
                signInTiles[1].SetTileText(signUpTileName, false, this);

            } else if (!signInTiles[0].GetTileName().Equals(signUpTileName)) {
                signInTiles[1].SetTileText(signUpTileName, false, this);
            }
            if (signInTiles[1].transform.GetSiblingIndex() != 1) {
                signInTiles[1].transform.SetSiblingIndex(1);
            }

            //Now check that all user accounts have a tile
            List<string> users = SavedAccounts.GetAllSavedUsers();
            Dictionary<int, string> loggedInUsers = myController.GetSignedInUsers();
            for (int i = 2; i < users.Count + 2; i++) {
                //Check if there is a tile for this user already
                if(signInTiles.Count < (i + 1)) {
                    //Here if another tile needs to be added
                    GameObject tile = Instantiate(SignInTilePrefab, tileContainer.transform);
                    tile.transform.SetSiblingIndex(i);
                    signInTiles.Insert(i, tile.GetComponent<SignInTileController>());
                    signInTiles[i].SetTileText(users[i - 2], transform, this);

                } else if (!signInTiles[i].GetTileName().Equals(users[i - 2])) {
                    signInTiles[i].transform.SetSiblingIndex(i);
                    signInTiles[i].SetTileText(users[i - 2], transform, this);
                }

                //Set tile to logged in or not (based on currently logged in users)
                if(loggedInUsers.ContainsValue(users[i - 2])) {
                    signInTiles[i].SetLoggedIn(true);
                } else {
                    signInTiles[i].SetLoggedIn(false);
                }
            }

            //cleanup
            while(signInTiles.Count > users.Count + 2) {
                GameObject tile = signInTiles[users.Count + 2].gameObject;
                signInTiles.RemoveAt(users.Count + 2);
                Destroy(tile);
            }
        }

        



        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    }
}

