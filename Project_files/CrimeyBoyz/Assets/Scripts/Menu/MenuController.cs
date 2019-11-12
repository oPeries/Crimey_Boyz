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
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Main manager for the menu scene
//Manages switching between menu items (Game lobby, settings menu, main menu...)
//Manages closing / restarting the game
public class MenuController : MonoBehaviour {

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //Singleton instance
    public static MenuController Singleton { get; private set; }

    //Menu containers, enabled / disabled when changing menus
    public GameObject MainMenuObject;
    public GameObject SettingsMenuObject;
    public GameObject GameLobbyObject;

    public GameObject missionControlLogo;
    public SettingsMenuController settingMenuController;

    private EventSystem eventSystem; //Current event system, used to set the currently selected object

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Behavioural ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    private void Awake() {
        //Print build info
        string buildSettings = "Build Settings:";

#if UNITY_EDITOR_WIN
        buildSettings += " Windows Editor,";
#endif

#if UNITY_STANDALONE_WIN
        buildSettings += " Windows Standalone,";
#endif

#if UNITY_ANDROID
        buildSettings += " Android,";
#endif

#if DECO3801_GAME_BUILD
        buildSettings += " Game Build,";
        missionControlLogo.SetActive(false);
#endif

#if DECO3801_MISSIONCONTROL_BUILD
        buildSettings += " Mission Control Build,";
        missionControlLogo.SetActive(true);
#endif

        Debug.Log(buildSettings.Substring(0, buildSettings.Length - 1)); //Remove the "," from the end of the string

#if UNITY_ANDROID
        Application.targetFrameRate = 60;
#endif

        SettingsMenuObject.SetActive(true);
    }

    private void Start() {
        //Initialise to main menu
        eventSystem = EventSystem.current;
        OpenMainMenu();
        StartCoroutine(DelayedLoadSettings());
    }

    IEnumerator DelayedLoadSettings() {
        yield return new WaitForSeconds(1f);
        Debug.Log("Delayed Settings menu load");


        settingMenuController.LoadSettings();
    }

    /// <summary>
    /// Set the singleton
    /// </summary>
    private void OnEnable() {

        if (Singleton != null && this != Singleton) {
            Destroy(this.gameObject);
        } else {
            Singleton = this;
        }
    }

    private void Update() {

        //KeyCode.Escape works for Android & Windows Builds
        if (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.Escape)) {
            if (!MainMenuObject.activeInHierarchy) {
                OpenMainMenu();
            } else {
                ExitGame();
            }
        }

#if UNITY_STANDALONE_WIN
        if(eventSystem.currentSelectedGameObject == null) {
            for (int i = 1; i < 6; i++) {
                if (Mathf.Abs(Input.GetAxis("Vertical" + i)) >= 0.7) {
                    SetSelectedButton();
                    break;
                }
            }
        }
#endif
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /// <summary>
    /// Change to the main menu
    /// </summary>
    public void OpenMainMenu() {
        //Set main menu to active menu
        CloseMenus();
        MainMenuObject.SetActive(true);

        //Stop server/client (if running)
        ConnectionManager.Singleton.CloseConnection();
        SessionManager.Singleton.ResetUnassignedControllers();

        //Set the selected button so WASD / Controller inputs can navigate the menu
#if UNITY_STANDALONE_WIN
        SetSelectedButton();
#endif

    }

    /// <summary>
    /// Change to the Game Lobby
    /// </summary>
    public void OpenGameLobby() {
        //Set game lobby to active menu
        CloseMenus();
        GameLobbyObject.SetActive(true);

        //Set the selected button so WASD / Controller inputs can navigate the menu
#if UNITY_STANDALONE_WIN
        SetSelectedButton();
#endif
    }

    /// <summary>
    /// Change to the settings menu
    /// </summary>
    public void OpenSettingsMenu() {
        //Set settings menu to active menu
        CloseMenus();
        SettingsMenuObject.SetActive(true);

        //Set the selected button so WASD / Controller inputs can navigate the menu
#if UNITY_STANDALONE_WIN
        SetSelectedButton();
#endif
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void ExitGame() {
#if !UNITY_EDITOR
        CloseMenus();
        Application.Quit();
#endif
    }

    /// <summary>
    /// Restart the game
    /// </summary>
    public void RestartGame() {

#if UNITY_EDITOR
        //Not applicable in the editor
        OpenMainMenu();
        Debug.Log("MenuController.Restart() Called");
        return;

#elif UNITY_STANDALONE_WIN
        CloseMenus();
        //Restart in windows by running the .exe again and closing this one
        System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe")); //new program
        ExitGame(); //kill current process

#else
        Debug.LogError("Attempted to restart on a non-compatible build");
        
#endif

    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /// <summary>
    /// Close all menus and stop the server/client
    /// </summary>
    private void CloseMenus() {
        //ConnectionManager.Singleton.CloseConnection();
        //SessionManager.Singleton.StopListeningForControllers();
        //SessionManager.Singleton.ResetPlayers();

        MainMenuObject.SetActive(false);
        SettingsMenuObject.SetActive(false);
        GameLobbyObject.SetActive(false);
    }

    private void SetSelectedButton() {
        Button firstButton = null;

        if (MainMenuObject.activeInHierarchy) {
            firstButton = MainMenuObject.GetComponentsInChildren<Button>()[0];

        } else if(SettingsMenuObject.activeInHierarchy) {
            firstButton = SettingsMenuObject.GetComponentsInChildren<Button>()[0];

        } else if(GameLobbyObject.activeInHierarchy) {
            firstButton = GameLobbyObject.GetComponentsInChildren<Button>()[0];
        }

        if (firstButton != null) {
            eventSystem.SetSelectedGameObject(firstButton.transform.gameObject);
        }
    }

}
