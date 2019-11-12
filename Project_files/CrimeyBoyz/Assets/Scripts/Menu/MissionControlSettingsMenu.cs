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
using System;
using System.Net;

public class MissionControlSettingsMenu : MonoBehaviour, SettingsMenuItem {

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public Dropdown discoveryMethodDropdown;
    public Text ipAddressText;
    public InputField directIPInput;
    public InputField multicastIPInput;
    public GameObject invalidDirectIp;
    public GameObject invalidMulticastGroup;

    private bool discoveryRestartRequired;

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Behavioural ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void Awake() {

        //Add event listeners
        //Want these event listeners to be before those added in settings menu controller (so put these in Awake() instead of start)
        discoveryMethodDropdown.onValueChanged.AddListener(delegate { OnDiscoveryMethodChanged(); });
        directIPInput.onValueChanged.AddListener(delegate { CheckDirectIPField(); });
        multicastIPInput.onValueChanged.AddListener(delegate { CheckMulticastField(); });

        //Populate discovery method dropdown
        discoveryMethodDropdown.options.Clear();
        discoveryMethodDropdown.options.Add(new Dropdown.OptionData("DEFAULT"));
        foreach (LANdiscovery.DiscoveryMethod method in Enum.GetValues(typeof(LANdiscovery.DiscoveryMethod))) {
            discoveryMethodDropdown.options.Add(new Dropdown.OptionData(method.ToString()));
        }

        discoveryRestartRequired = false;
    }

    void OnEnable() {

        UpdateDevicesIPs();
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public void SaveSettings() {
        SaveDiscoveryMethod();
        //Can add more setting here
    }

    public void LoadSettings() {
        LoadDiscoveryMethod();
        //Can add more setting here
    }

    public bool IsRestartRequired() {
        return discoveryRestartRequired;
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void UpdateDevicesIPs() {
        string localIPs = "";
        try {
            foreach (IPAddress address in LANdiscovery.Singleton.GetLocalAddresses()) {
                localIPs = localIPs + address.ToString() + "; ";
            }

            ipAddressText.text = localIPs;
        } catch {
            ipAddressText.text = "rip";
        }
    }

    //Called when discovery method changed by user
    //Enables / disables direct @ multicast IP input fields
    private void OnDiscoveryMethodChanged() {

        bool enableDirectIPInput = false;
        bool enableMulticastInput = false;
        string selectedOption = discoveryMethodDropdown.options[discoveryMethodDropdown.value].text;

        if (!selectedOption.Equals("DEFAULT")) {
            if(Enum.TryParse(selectedOption, true, out LANdiscovery.DiscoveryMethod method)) {

                switch(method) {
                    case LANdiscovery.DiscoveryMethod.DIRECTIP:
                        enableDirectIPInput = true;
                        directIPInput.text = null;
                        break;

                    case LANdiscovery.DiscoveryMethod.MULTICAST:
                        enableMulticastInput = true;
                        multicastIPInput.text = null;
                        break;
                }
                discoveryRestartRequired = false;

            } else {
                Debug.Log("Failed to convert \"" + selectedOption + "\" to discovery method");
            }

        } else {

            //Setting back to default will require a restart
            discoveryRestartRequired = true;
        }

        //Enable or disable input components
        directIPInput.transform.parent.gameObject.SetActive(enableDirectIPInput);
        multicastIPInput.transform.parent.gameObject.SetActive(enableMulticastInput);
    }

    private bool IsValidIPAddress(string address) {
        if (address == null) {
            return false;
        }

        if(address.Split('.').Length != 4) {
            return false;
        }

        try {
            IPAddress.Parse(address);
            return true;
        } catch (FormatException) {
            return false;
        }
    }

    private bool IsValidMulticastGroup(string address) {
        if(address == null) {
            return false;
        }

        if (address.Split('.').Length != 4) {
            return false;
        }

        try {
            byte[] converted = IPAddress.Parse(address).GetAddressBytes();

            //All multicast addresses must be between 224.0.0.0 to 239.255.255.255
            if (converted[0] < 224 || converted[0] > 239) {
                return false;
            }
            return true;
        } catch (FormatException) {
            return false;
        }
    }

    //Check the user input is a valid ip address
    //If not valid, will turn on valid border / box
    private void CheckDirectIPField() {
        if(directIPInput.text == null || directIPInput.text == "") {
            return;
        }
        bool validIp = IsValidIPAddress(directIPInput.text);
        invalidDirectIp.SetActive(!validIp);
    }

    //Check the user input is a valid ip address
    //If not valid, will turn on valid border / box
    private void CheckMulticastField() {
        if (multicastIPInput.text == null || multicastIPInput.text == "") {
            return;
        }
        bool validIp = IsValidMulticastGroup(multicastIPInput.text);
        invalidMulticastGroup.SetActive(!validIp);
    }

    //Save discovery method choices to memory
    //does not set connection manager (that is done by the load function)
    private void SaveDiscoveryMethod() {

        //Get chosen option
        string selectedOption = discoveryMethodDropdown.options[discoveryMethodDropdown.value].text;

        if(selectedOption.Equals("DEFAULT")) {
            //Chosen to use default, save this and don't process any more
            PlayerPrefs.SetString("DiscoveryMethod", selectedOption);
            return;
        }
        //try convert method to enum value
        if (Enum.TryParse(selectedOption, true, out LANdiscovery.DiscoveryMethod method)) {

            //Save to memory
            PlayerPrefs.SetString("DiscoveryMethod", selectedOption);

        } else {
            Debug.Log("Failed to convert \"" + selectedOption + "\" to discovery method (while saving)");
            return;
        }

        //Here if chosen to override default discovery method

        //if direct IP selected, check if valid ip address
        //if valid, save value to memory
        if (method == LANdiscovery.DiscoveryMethod.DIRECTIP && IsValidIPAddress(directIPInput.text)) {
            PlayerPrefs.SetString("DirectIPOverride", directIPInput.text);
        }

        //if multicast selected, check if valid multicast group
        //if valid, save value to memory
        if (method == LANdiscovery.DiscoveryMethod.MULTICAST && IsValidMulticastGroup(multicastIPInput.text)) {
            PlayerPrefs.SetString("MulticastGroupOverride", multicastIPInput.text);
        }
    }

    //Load discovery settings from memory
    //If settings are valid, will set LAN discovery and input filed to match
    private void LoadDiscoveryMethod() {
        Debug.Log("loading discovery");

        string selectedOption = PlayerPrefs.GetString("DiscoveryMethod", "DEFAULT");

        List<Dropdown.OptionData> discoveryOptions = discoveryMethodDropdown.options;
        bool validOptionFound = false;
        for (int i = 0; i < discoveryOptions.Count; i++ ) {
            if(discoveryOptions[i].text.Equals(selectedOption)) {
                discoveryMethodDropdown.value = i;
                validOptionFound = true;
                break;
            }
        }

        if(!validOptionFound) {
            discoveryMethodDropdown.value = 0;
        }

        OnDiscoveryMethodChanged();

        //If set to default, don't set any discovery settings
        if (selectedOption.Equals("DEFAULT")) {
            return;
        }

        //try convert method to enum value
        if (!Enum.TryParse(selectedOption, true, out LANdiscovery.DiscoveryMethod method)) {
            Debug.Log("Failed to convert \"" + selectedOption + "\" to discovery method (while loading)");
            return;
        }

        //Here if chosen to override default discovery method
        LANdiscovery.Singleton.discoveryMethod = method;

        //if direct IP selected, load from memory and check if valid ip address
        //if valid, set LAN discovery & input filed
        if (method == LANdiscovery.DiscoveryMethod.DIRECTIP) {
            string directIP = PlayerPrefs.GetString("DirectIPOverride", null);
            if (IsValidIPAddress(directIP)) {
                LANdiscovery.Singleton.directIP = directIP;
                directIPInput.text = directIP;
            }
        }

        //if multicast selected, load from memory and check if valid multicast group
        //if valid, set LAN discovery & input filed
        if (method == LANdiscovery.DiscoveryMethod.MULTICAST) {
            string multicastAddr = PlayerPrefs.GetString("MulticastGroupOverride", null);
            if (IsValidMulticastGroup(multicastAddr)) {
                LANdiscovery.Singleton.multicastIP = multicastAddr;
                multicastIPInput.text = multicastAddr;
            }
        }
    }
}
