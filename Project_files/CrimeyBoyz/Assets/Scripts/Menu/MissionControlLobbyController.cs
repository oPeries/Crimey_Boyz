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
using UnityEngine.UI;
using CrimeyBoyz.GameState;

namespace CrimeyBoyz.Menu.MissionControl {
    //Manages the controller lobby (connecting to an available game over LAN)
    //Designed to be attached to the ContollerLobby prefab
    //Designed to start searching for a potential server when enabled
    public class MissionControlLobbyController : MonoBehaviour {

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Defines ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public enum LobbyState {
            DISABLED,
            LIMBO,
            SIGNIN,
            LOGIN,
            SIGNUP,
            EDITSAVED,
            NOCONNECTION,
            CONTROLLERASSIGNMENT,
            ALLSET
        }

        private const float missionControlConnectedDelay = 1f;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //Sub menus for this lobby
        public MissionControlStatusController missionControlController;
        public SignInMenuController signInController;
        public LoginMenuController loginController;
        public SignupMenuController signupController;
        public EditUserMenuController editController;
        public GameObject controllerAssignment;
        public GameObject noConnectionScreen;
        public GameObject allSetScreen;
        public Button addAnotherUserButton;


        private LobbyState currentState;
        private bool waitingForDbResponse;
        private ConnectionState lastConnectionState;
        private string currentUser; //current user waiting for a db response / editing

        private Dictionary<int, string> signedInUsers; //all users currently logged in

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Behavioural ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private void Start() {
            signedInUsers = new Dictionary<int, string>();
            waitingForDbResponse = false;
            lastConnectionState = ConnectionState.STOPPED;
            currentState = LobbyState.DISABLED;
            currentUser = "";

            SessionManager.Singleton.PlayerStateChangedCallback += CheckForControllers;
        }

        /// <summary>
        /// When enabled, start looking for a server
        /// </summary>
        private void OnEnable() {
            if (ConnectionManager.Singleton != null) {

                ConnectionManager.Singleton.isServer = false;
                ConnectionManager.Singleton.retryUponFailure = true;
                ConnectionManager.Singleton.InitiateConnection();
                lastConnectionState = ConnectionState.STOPPED;
            }
            SetNextState(LobbyState.DISABLED);
        }

        private void Update() {
            if(currentState == LobbyState.DISABLED) {
                ConnectionState current = ConnectionManager.Singleton.GetState();
                if (current != lastConnectionState) {
                    missionControlController.UpdateMissionControlStatus();
                }
                if(current == ConnectionState.CONNECTED && SessionManager.Singleton.IsNetworkedGameStateReady()) {
                    SetNextState(LobbyState.LIMBO);
                    StartCoroutine(DelayedMenuSwitch(MatchSignedInUsers(), missionControlConnectedDelay));
                }
                lastConnectionState = current;
            }

            if(lastConnectionState == ConnectionState.CONNECTED && ConnectionManager.Singleton.GetState() != ConnectionState.CONNECTED) {
                SetNextState(LobbyState.DISABLED);
            }

            DbResponse response = DbConnection.Singleton.GetResponse();
            if (waitingForDbResponse && response != null) {
                //Got a reply from the db controller

                waitingForDbResponse = false;
                if(response.state == DbResponseState.NOCONNECTION) {
                    SetNextState(LobbyState.NOCONNECTION);

                } else if(currentState == LobbyState.SIGNIN) {
                    if(response.state == DbResponseState.SUCCESS) {
                        SetUserToSignedIn(currentUser);
                        SetNextState(LobbyState.CONTROLLERASSIGNMENT, currentUser);

                    } else {
                        SetNextState(LobbyState.EDITSAVED, currentUser);
                        editController.UpdateEditMenu(currentUser, response);

                    }
                } else if(currentState == LobbyState.SIGNUP) {
                    signupController.UpdateSignUpPage(response);
                    if(response.state == DbResponseState.SUCCESS) {
                        SetUserToSignedIn(currentUser);
                        SetNextState(LobbyState.CONTROLLERASSIGNMENT, currentUser);
                    }

                } else if (currentState == LobbyState.LOGIN) {
                    loginController.UpdateLoginPage(response);
                    if (response.state == DbResponseState.SUCCESS) {
                        SetUserToSignedIn(currentUser);
                        //StartCoroutine(DelayedMenuSwitch(LobbyState.CONTROLLERASSIGNMENT, 1f));
                        SetNextState(LobbyState.CONTROLLERASSIGNMENT, currentUser);
                    }

                } else if (currentState == LobbyState.EDITSAVED) {
                    editController.UpdateEditMenu(currentUser, response);
                    if(response.state == DbResponseState.SUCCESS) {
                        SetNextState(LobbyState.SIGNIN, currentUser);
                    }

                } else {
                    Debug.LogWarning("Db response received when not in a state expecting a response");
                }
            }   
        }

        private void OnDestroy() {
            SessionManager.Singleton.PlayerStateChangedCallback -= CheckForControllers;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public Dictionary<int, string> GetSignedInUsers() {
            return new Dictionary<int, string>(signedInUsers);
        }

        public void LoginUser(string username, string password) {
            DbConnection.Singleton.CheckLogin(username, password);
            waitingForDbResponse = true;
            currentUser = username;
        }

        public void SignUpUser(string username, string name, string email, string password1, string password2) {
            DbConnection.Singleton.CreateUser(username, name, email, password1, password2);
            waitingForDbResponse = true;
            currentUser = username;
        }

        public void SetNextState(LobbyState desiredState) {
            SetNextState(desiredState, "");
        }

        //username is the name of the user requesting the state change (edit / login / controller assignment)
        //if username is "" then assumed guest or don't care
        //if username is null then assumed they are in the process of logging in
        public void SetNextState(LobbyState desiredState, string username) {
            if (!SessionManager.Singleton.IsNetworkedGameStateReady() || ConnectionManager.Singleton.GetState() != ConnectionState.CONNECTED || desiredState == LobbyState.DISABLED) {
                currentState = LobbyState.DISABLED;
                SetVisibleObjects();
                missionControlController.UpdateMissionControlStatus();

            } else if (desiredState == LobbyState.LIMBO) {
                currentState = LobbyState.LIMBO;

            } else if (desiredState == LobbyState.SIGNIN) {
                if (signedInUsers.Count < 3 || username == null || SessionManager.Singleton.networkedGameState.IsPlayerNotLoggedIn()) {
                    currentState = LobbyState.SIGNIN;
                    SetVisibleObjects();
                    signInController.UpdateSignInPage();

                } else {
                    SetNextState(LobbyState.ALLSET);
                }

            } else if (desiredState == LobbyState.LOGIN) {
                currentState = LobbyState.LOGIN;
                SetVisibleObjects();
                loginController.UpdateLoginPage(null);

            } else if (desiredState == LobbyState.SIGNUP) {
                currentState = LobbyState.SIGNUP;
                SetVisibleObjects();
                signupController.UpdateSignUpPage(null);

            } else if (desiredState == LobbyState.EDITSAVED) {
                currentState = LobbyState.EDITSAVED;
                SetVisibleObjects();
                editController.UpdateEditMenu(username, null);

            } else if (desiredState == LobbyState.NOCONNECTION) {
                currentState = LobbyState.NOCONNECTION;
                SetVisibleObjects();

            } else if (desiredState == LobbyState.CONTROLLERASSIGNMENT) {
                currentState = LobbyState.CONTROLLERASSIGNMENT;

                controllerAssignment.GetComponentInChildren<Text>().text = username + " please press the start button on your controller";
                currentUser = username;

                SetVisibleObjects();

            } else if (desiredState == LobbyState.ALLSET) {
                currentState = LobbyState.ALLSET;
                if (signedInUsers.Count >= 5) {
                    addAnotherUserButton.gameObject.SetActive(false);
                } else {
                    addAnotherUserButton.gameObject.SetActive(true);
                }
                SetVisibleObjects();
            }
        }

        public void BackToSignIn() {
            SetNextState(LobbyState.SIGNIN);
            if (waitingForDbResponse) {
                waitingForDbResponse = false;
                DbConnection.Singleton.DisregardNextResponse();
            }
        }

        //Exit the lobby screen and return to main menu
        public void ExitLobby() {
            if (waitingForDbResponse) {
                waitingForDbResponse = false;
                DbConnection.Singleton.DisregardNextResponse();
            }
            ConnectionManager.Singleton.CloseConnection();
            signedInUsers.Clear();
            MenuController.Singleton.OpenMainMenu();
        }

        public void AddGuestUser() {
            SetUserToSignedIn("");
            SetNextState(LobbyState.CONTROLLERASSIGNMENT, "");
        }

        public void AddExtraPlayer() {
            AddNewUser(null);
            SetNextState(LobbyState.SIGNIN, null);
        }


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void SetVisibleObjects() {
            missionControlController.gameObject.SetActive(currentState == LobbyState.DISABLED);
            signInController.gameObject.SetActive(currentState == LobbyState.SIGNIN);
            loginController.gameObject.SetActive(currentState == LobbyState.LOGIN);
            signupController.gameObject.SetActive(currentState == LobbyState.SIGNUP);
            editController.gameObject.SetActive(currentState == LobbyState.EDITSAVED);
            noConnectionScreen.SetActive(currentState == LobbyState.NOCONNECTION);
            controllerAssignment.SetActive(currentState == LobbyState.CONTROLLERASSIGNMENT);
            allSetScreen.SetActive(currentState == LobbyState.ALLSET);
        }

        IEnumerator DelayedMenuSwitch(LobbyState desiredState, float delay) {
            
            yield return new WaitForSeconds(delay);

            //Debug.Log("Delayed Menu switch");

            if (desiredState == LobbyState.CONTROLLERASSIGNMENT) {
                //Debug.Log("Delayed Menu switch - controller assignmet");

                //Find the next user to assign a controller to
                //If there is no users to assign controllers to, go to signin screen
                Dictionary<int, PlayerInfo> players = SessionManager.Singleton.networkedGameState.GetAllPlayers();
                desiredState = LobbyState.SIGNIN;
                foreach (KeyValuePair<int, PlayerInfo> player in players) {
                    if (player.Value.assignedController < 0 && player.Value.username != null) {
                        Debug.Log("Still have another user without a controller: " + player.Value.username);
                        desiredState = LobbyState.CONTROLLERASSIGNMENT;
                        currentUser = player.Value.username;
                        break;
                    }
                }
            }

            SetNextState(desiredState, currentUser);
        }

        private void AddNewUser(string username) {
            if (!SessionManager.Singleton.IsNetworkedGameStateReady()) {
                Debug.LogError("Tried to add a new player when networked game state not ready");
                return;
            }

            int userCount = SessionManager.Singleton.networkedGameState.PlayerCount();
            SessionManager.Singleton.networkedGameState.SetUsername(userCount + 1, username);
        }

        private LobbyState MatchSignedInUsers() {
            if (!SessionManager.Singleton.IsNetworkedGameStateReady()) {
                Debug.LogError("Tried to match signed in users when networked game state not ready");
                return LobbyState.DISABLED;
            }

            bool modificationsMade = false;

            foreach (int pNum in signedInUsers.Keys) {
                PlayerInfo player = SessionManager.Singleton.networkedGameState.GetPlayer(pNum);

                if (player == null || player.username != signedInUsers[pNum]) {

                    modificationsMade = true;
                    SessionManager.Singleton.networkedGameState.SetUsername(pNum, signedInUsers[pNum]);
                    if (player != null && player.username != signedInUsers[pNum]) {
                        Debug.LogWarning("Username miss-match when re-syncing players, overriding");
                    }
                }
            }

            Dictionary<int, PlayerInfo> players = SessionManager.Singleton.networkedGameState.GetAllPlayers();

            foreach (int pNum in players.Keys) {
                if (!signedInUsers.ContainsKey(pNum)) {
                    signedInUsers[pNum] = players[pNum].username;
                    modificationsMade = true;
                }
            }

            return modificationsMade ? LobbyState.CONTROLLERASSIGNMENT : LobbyState.SIGNIN;
        }


        private void SetUserToSignedIn(string username) {
            if (!SessionManager.Singleton.IsNetworkedGameStateReady()) {
                Debug.LogError("Tried to login a player when networked game state not ready");
                return;
            }

            Dictionary<int, PlayerInfo> players = SessionManager.Singleton.networkedGameState.GetAllPlayers();

            foreach (int pNum in players.Keys) {
                if(players[pNum].username == null) {
                    SessionManager.Singleton.networkedGameState.SetUsername(pNum, username);
                    signedInUsers[pNum] = username;
                    return;
                }
            }

            AddNewUser(username);
            signedInUsers[players.Count + 1] = username;
        }


        private void CheckForControllers() {
            //Debug.Log("Check for controllers called");
            if (currentState == LobbyState.CONTROLLERASSIGNMENT) {
                if (!SessionManager.Singleton.IsNetworkedGameStateReady()) {
                    SetNextState(LobbyState.DISABLED);

                } else {
                    Dictionary<int, PlayerInfo> players = SessionManager.Singleton.networkedGameState.GetAllPlayers();
                    bool stopWaiting = false;
                    bool foundUsername = false;
                    foreach (KeyValuePair<int, PlayerInfo> player in players) {

                        if (player.Value.username == currentUser) {
                            foundUsername = true;

                            if (player.Value.assignedController >= 0) {
                                stopWaiting = true;
                                break;
                            }
                        }
                    }

                    if (!foundUsername) {
                        Debug.LogWarning("Could not find user when waiting for controller assignment");
                        SetNextState(LobbyState.SIGNIN);
                    }
                    if (stopWaiting) {
                        Debug.Log("Controller assigned, moving to next player");
                        StartCoroutine(DelayedMenuSwitch(LobbyState.CONTROLLERASSIGNMENT, 0.5f));
                    }
                }
            }
        }
    }
}
