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
using UnityEngine.UI;

//Manages the top level setting menu
//Loads the settings menu prefabs based on build
//
public class SettingsMenuController : MonoBehaviour, SettingsMenuItem {

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Defines ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private const int BUTTON_OFF = 0;
    private const int BUTTON_CANCEL = 1;
    private const int BUTTON_BACK = 2;
    private const int BUTTON_SAVE = 3;
    private const int BUTTON_WILL_RESTART = 4;
    private const int BUTTON_NEEDS_RESTART = 5;

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private SettingsMenuItem[] settingsMenuItems;

    public GameObject backButton;
    public GameObject saveButton;

    public GameObject windowsSettingsPrefab;
    public GameObject androidSettingsPrefab;
    public GameObject gameSettingsPrefab;
    public GameObject missionControlSettingsPrefab;


    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Behavioural ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //Instantiate settings menu prefabs
    private void Awake() {

        AddBuildPrefabs();

        settingsMenuItems = GetComponentsInChildren<SettingsMenuItem>();
    }

    private void Start() {

        foreach (Dropdown element in GetComponentsInChildren<Dropdown>()) {
            element.onValueChanged.AddListener(delegate { SettingsChanged(); });
        }

        foreach (InputField element in GetComponentsInChildren<InputField>()) {
            element.onValueChanged.AddListener(delegate { SettingsChanged(); });
        }

        LoadSettings();
    }

    private void OnEnable() {
        LoadSettings();
    }

    private void OnDisable() {
        SetSaveButton(BUTTON_OFF);
        SetBackButton(BUTTON_BACK);
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /// <summary>
    /// Save the current settings values
    /// </summary>
    public void SaveSettings() {
        if (settingsMenuItems == null) {
            return;
        }

        foreach (SettingsMenuItem script in settingsMenuItems) {
            if ((Object) script != this) {
                script.SaveSettings();
            }
        }

        SetBackButton(BUTTON_BACK);

#if UNITY_STANDALONE_WIN
        if(IsRestartRequired()) {
            MenuController.Singleton.RestartGame();
        }
#endif
        LoadSettings();

        MenuController.Singleton.OpenMainMenu();
    }

    /// <summary>
    /// Load each item in the settings menu (Match what is currently saved)
    /// </summary>
    public void LoadSettings() {
        if (settingsMenuItems == null) {
            return;
        }
        foreach (SettingsMenuItem script in settingsMenuItems) {
            if ((Object)script != this) {
                script.LoadSettings();
            }
        }

        SetSaveButton(BUTTON_OFF);
        SetBackButton(BUTTON_BACK);
    }

    /// <summary>
    /// Check if a restart is required for any setting to apply
    /// </summary>
    /// <returns> True if a restart is required, false if not </returns>
    public bool IsRestartRequired() {
        if (settingsMenuItems == null) {
            return false;
        }
        foreach (SettingsMenuItem script in settingsMenuItems) {
            if ((Object)script != this) {
                if (script.IsRestartRequired()) {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Signal that one or more settings have changed
    /// </summary>
    public void SettingsChanged() {
        SetBackButton(BUTTON_CANCEL);
        if (IsRestartRequired()) {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            SetSaveButton(BUTTON_WILL_RESTART);
#else
                SetSaveButton(BUTTON_NEEDS_RESTART);
#endif
        } else {
            SetSaveButton(BUTTON_SAVE);
        }
    }


    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /// <summary>
    /// Instantiate settings menu prefabs
    /// </summary>
    private void AddBuildPrefabs() {

        int siblingIndex = 0;
        GameObject menu = null;

        //Add game/mission control settings
#if DECO3801_GAME_BUILD && !DECO3801_MISSIONCONTROL_BUILD

        menu = Instantiate(gameSettingsPrefab, transform, false);
        menu.transform.SetSiblingIndex(siblingIndex++);

#elif !DECO3801_GAME_BUILD && DECO3801_MISSIONCONTROL_BUILD

        menu = Instantiate(missionControlSettingsPrefab, transform, false);
        menu.transform.SetSiblingIndex(siblingIndex++);
#endif

        //Add windows/android settings
#if UNITY_STANDALONE_WIN
        
        menu = Instantiate(windowsSettingsPrefab, transform, false);
        menu.transform.SetSiblingIndex(siblingIndex++);

#elif UNITY_ANDROID

        menu = Instantiate(androidSettingsPrefab, transform, false);
        menu.transform.SetSiblingIndex(siblingIndex++);
#endif
    }

    /// <summary>
    /// Set the text inside the back button to "Back" or "Cancel" when appropriate
    /// </summary>
    /// <param name="value"> value to determine if the button should say 'BACK' or 'CANCEL' </param>
    private void SetBackButton(int value) {

        Text buttonText = backButton.GetComponentInChildren<Text>();

        switch (value) {
            case BUTTON_BACK:
                buttonText.text = "BACK";
                break;

            case BUTTON_CANCEL:
                buttonText.text = "CANCEL";
                break;

            default:
                buttonText.text = "BACK";
                break;
        }
    }

    /// <summary>
    /// Set the text and enable interaction of the save button when appropriate
    /// </summary>
    /// <param name="value">A value to determine what the text should read</param>
    private void SetSaveButton(int value) {

        Text buttonText = saveButton.GetComponentInChildren<Text>();
        saveButton.GetComponentInChildren<Button>().interactable = true;

        switch (value) {
            case BUTTON_OFF:
                saveButton.GetComponentInChildren<Button>().interactable = false;
                buttonText.text = "SAVE";
                break;

            case BUTTON_SAVE:
                buttonText.text = "SAVE";
                break;

            case BUTTON_WILL_RESTART:
                buttonText.text = "SAVE (Will restart)";
                break;

            case BUTTON_NEEDS_RESTART:
                buttonText.text = "SAVE (Requires Restart)";
                break;

            default:
                buttonText.text = "SAVE";
                break;
        }

    }
}
