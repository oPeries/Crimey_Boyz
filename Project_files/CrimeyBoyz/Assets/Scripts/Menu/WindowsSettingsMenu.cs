/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using UnityEngine.UI;
using UnityEngine;

//Manages the windows specific settings prefab
//Currently has the following settings:
//  - Select Monitor (Choose which monitor to display the game on)
public class WindowsSettingsMenu : MonoBehaviour, SettingsMenuItem {

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public Dropdown monitorSelectionDropdown;

    private bool restartRequired;

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Behavioural ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void Awake() {
        restartRequired = false;
        LoadSettings();
        monitorSelectionDropdown.onValueChanged.AddListener(delegate { SetRestartRequired(); });
    }

    private void OnEnable() {
        restartRequired = false;
    }


    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public void SaveSettings() {
        SaveMonitorSettings();
        //Can add more setting here
    }

    public void LoadSettings() {
        LoadMonitorSettings();
        //Can add more setting here
    }

    public bool IsRestartRequired() {
        return restartRequired;
    }

    

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void SaveMonitorSettings() {
        int selectedValue = monitorSelectionDropdown.value;
        int savedValue = PlayerPrefs.GetInt("UnitySelectMonitor");

        if (selectedValue != savedValue) {
            PlayerPrefs.SetInt("UnitySelectMonitor", selectedValue);
            Debug.Log(string.Format("Monitor Saved to {0}", selectedValue));
            restartRequired = true;
        }
    }

    private void LoadMonitorSettings() {
        //Debug.Log(string.Format("Monitor read as {0}", PlayerPrefs.GetInt("UnitySelectMonitor")));
        int savedValue = PlayerPrefs.GetInt("UnitySelectMonitor");
        monitorSelectionDropdown.value = savedValue;
    }

    private void SetRestartRequired() {
        restartRequired = true;
    }
}
