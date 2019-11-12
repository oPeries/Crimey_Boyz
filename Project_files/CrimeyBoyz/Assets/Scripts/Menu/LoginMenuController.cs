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
    public class LoginMenuController : MonoBehaviour {

        public InputField usernameInput;
        public InputField passwordInput;
        public Toggle rememberMeButton;
        public GameObject loginFailed;
        public GameObject loadingWheel;
        public CanvasGroup disableWhenLoading;
        public MissionControlLobbyController myContoller;


        private void OnEnable() {
            ResetFields();
        }


        public void UpdateLoginPage(DbResponse response) {
            SetLoading(false);
            if (response == null) {
                ResetFields();

            } else if (response.state == DbResponseState.NOTFOUND) {
                SetLoginFailed(true);
                SetEnabled(true);

            } else if (response.state == DbResponseState.SUCCESS) {
                SetLoginFailed(false);
                if (rememberMeButton.isOn) {
                    Debug.Log("Remembering this user");
                    SavedAccounts.SaveUser(usernameInput.text, passwordInput.text);
                }

            } else {
                Debug.LogWarning("Unexpected response when logging in user");
                SetLoginFailed(true);
                SetEnabled(false);
            }
        }

        public void AttemptLogin() {
            myContoller.LoginUser(usernameInput.text, passwordInput.text);
            SetLoading(true);
            SetLoginFailed(false);
            SetEnabled(false);
        }

        public void OnCancel() {
            myContoller.BackToSignIn();
        }


        private void SetLoading(bool loading) {
            loadingWheel.SetActive(loading);

        }

        private void SetLoginFailed(bool failed) {
            loginFailed.SetActive(failed);
        }

        private void SetEnabled(bool enabled) {
            float alpha = enabled ? 1f : 0.5f;
            disableWhenLoading.alpha = alpha;
            disableWhenLoading.interactable = enabled;
        }

        private void ResetFields() {
            usernameInput.text = "";
            passwordInput.text = "";
            rememberMeButton.isOn = false;
            SetEnabled(true);
            SetLoginFailed(false);
            SetLoading(false);
        }
    }
}
