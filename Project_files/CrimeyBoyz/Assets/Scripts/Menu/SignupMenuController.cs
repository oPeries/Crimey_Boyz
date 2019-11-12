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

namespace CrimeyBoyz.Menu.MissionControl {
    public class SignupMenuController : MonoBehaviour {

        public InputField usernameInput;
        public InputField nameInput;
        public InputField emailInput;
        public InputField passwordInput;
        public InputField passwordVerify;
        public Toggle rememberMeButton;

        public GameObject usernameError;
        public GameObject nameError;
        public GameObject emailError;
        public GameObject passwordError;

        public GameObject loadingWheel;
        public CanvasGroup disableWhenLoading;
        public MissionControlLobbyController myContoller;

        private void OnEnable() {
            ResetFields();
        }

        //set response to null to reset
        public void UpdateSignUpPage(DbResponse response) {
            SetLoading(false);
            if (response == null) {
                ResetFields();

            } else if (response.state == DbResponseState.EMAIL_EXISTS) {
                SetEmailError("Email already in use");
                SetEnabled(true);

            } else if (response.state == DbResponseState.USERNAME_EXISTS) {
                SetUsernameError("Username already exists");
                SetEnabled(true);

            } else if (response.state == DbResponseState.PASSWORDS_DONT_MATCH) {
                SetPasswordError("Verify password does not match");
                SetEnabled(true);

            } else if (response.state == DbResponseState.SUCCESS) {
                ClearErrors();
                if (rememberMeButton.isOn) {
                    Debug.Log("Remembering this user");
                    SavedAccounts.SaveUser(usernameInput.text, passwordInput.text);
                }

            } else {
                Debug.LogWarning("Unexpected response when signing up as new user");
                Debug.Log(response.message);
                SetUsernameError("Something");
                SetNameError("went wrong!");
                SetEmailError("Please");
                SetPasswordError("try again");
                SetEnabled(true);
            }
        }


        public void AttemptSignup() {
            //TODO: match the signup verification with the same as the website
            if(usernameInput.text.Length < 5) {
                SetUsernameError("Username too short");

            } else if (nameInput.text.Length < 1) {
                SetNameError("Please enter a name");

            } else if (emailInput.text.Length < 1) {
                SetEmailError("Please enter an email");

            } else if (!passwordVerify.text.Equals(passwordInput.text)) {
                SetPasswordError("Verify password does not match");

            } else { 
                myContoller.SignUpUser(usernameInput.text, nameInput.text, emailInput.text, passwordInput.text, passwordVerify.text);
                SetLoading(true);
                ClearErrors();
                SetEnabled(false);
            }
        }

        public void OnCancel() {
            myContoller.BackToSignIn();
        }



        private void SetLoading(bool loading) {
            loadingWheel.SetActive(loading);

        }

        private void SetEnabled(bool enabled) {
            float alpha = enabled ? 1f : 0.5f;
            disableWhenLoading.alpha = alpha;
            disableWhenLoading.interactable = enabled;
        }

        //set error to "" or null to clear error box
        private void SetUsernameError(string error) {
            if(error == null || error.Equals("")) {
                usernameError.SetActive(false);
            } else {
                usernameError.GetComponentInChildren<Text>().text = error;
                usernameError.SetActive(true);
            }
        }

        //set error to "" or null to clear error box
        private void SetNameError(string error) {
            if (error == null || error.Equals("")) {
                nameError.SetActive(false);
            } else {
                nameError.GetComponentInChildren<Text>().text = error;
                nameError.SetActive(true);
            }
        }

        //set error to "" or null to clear error box
        private void SetEmailError(string error) {
            if (error == null || error.Equals("")) {
                emailError.SetActive(false);
            } else {
                emailError.GetComponentInChildren<Text>().text = error;
                emailError.SetActive(true);
            }
        }

        //set error to "" or null to clear error box
        private void SetPasswordError(string error) {
            if (error == null || error.Equals("")) {
                passwordError.SetActive(false);
            } else {
                passwordError.GetComponentInChildren<Text>().text = error;
                passwordError.SetActive(true);
            }
        }

        private void ClearErrors() {
            SetUsernameError("");
            SetNameError("");
            SetEmailError("");
            SetPasswordError("");
        }

        private void ResetFields() {
            usernameInput.text = "";
            nameInput.text = "";
            usernameInput.text = "";
            passwordInput.text = "";
            passwordVerify.text = "";
            rememberMeButton.isOn = false;
            ClearErrors();
            SetLoading(false);
            SetEnabled(true);
        }
    }
}
