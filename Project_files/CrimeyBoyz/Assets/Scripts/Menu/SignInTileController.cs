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
    //Call to control the signin tile prefab
    //Controls what is displayed (such as the text, image and loading wheel) on the tile
    //Also used to enable / disable the tile
    public class SignInTileController : MonoBehaviour {

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Defines ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public Text tileName; //The text displaying this tile's name (eg "Create New Account")
        public GameObject loadingWheel; //Enables / disables this object when the tile is "loading"
        public GameObject loggedInDisplay; //The image/text to enabled or disable when the player is logged in
        public CanvasGroup accountButtons; //disables this object if the tile is a non-user account tile && dims + turns off interaction when "loading" or "loggedIn"
        public CanvasGroup mainTile; //dims + turns off interaction when "loading" or "loggedIn"

        private string currentName = "Default"; //current name of this tile
        private bool isUserAccount = true; //Sets this tile to be a user account or not (non user accounts will have certain parts disabled)
        private SignInMenuController myController; //The controller that this prefab will use to call when the tile is interacted with

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Behavioural ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //Set the text on the top of the tile
        //If isUserAccount is false, will disable the accountButtons object
        public void SetTileText(string tileText, bool isUserAccount, SignInMenuController controller) {
            myController = controller;
            //this.isUserAccount = isUserAccount;
            currentName = tileText;
            tileName.text = tileText;
            this.isUserAccount = isUserAccount;

            if (isUserAccount) {
                accountButtons.gameObject.SetActive(true);
            } else {
                accountButtons.gameObject.SetActive(false);
            }
        }

        //Return the current name for this tile
        public string GetTileName() {
            return currentName;
        }

        //Return the current name for this tile
        public bool IsUserTile() {
            return isUserAccount;
        }

        //Set current state to "loading" or not
        //When loading, the loading wheel is displayed (hidden otherwise)
        //Calling this hides the loggedIn section
        //Also lowers the alpha & turns off interaction of tile components (mainTile & accountButtons) or sets to normal when not loading
        public void SetLoading(bool loading) {
            loadingWheel.SetActive(loading);
            loggedInDisplay.SetActive(false);
            SetEnabled(!loading);
        }

        //Set the current state to "loggedIn"
        //When logged in, the logged in section is displayed (hidden otherwise)
        //Calling this hides the loading wheel
        //Also lowers the alpha & turns off interaction of tile components (mainTile & accountButtons) or sets to normal when not loading
        public void SetLoggedIn(bool loggedIn) {
            loggedInDisplay.SetActive(loggedIn);
            loadingWheel.SetActive(false);
            SetEnabled(!loggedIn);
        }

        //Callback for the "Edit" button
        public void OnEditPressed() {
            myController.EditUser(currentName);
        }

        //Callback for the "Forget" button
        public void OnForgetPressed() {
            myController.ForgetUser(currentName);
        }

        //Callback for pressing the tile itself
        public void OnTilePressed() {
            myController.TilePressed(this);
        }


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //Sets the alpha and intractability of the tile (mainTile section & accountButtons section)
        private void SetEnabled(bool enabled) {
            float alpha = 1f;
            if(!enabled) {
                alpha = 0.5f;
            }
            mainTile.alpha = alpha;
            mainTile.interactable = enabled;

            accountButtons.alpha = alpha;
            accountButtons.interactable = enabled;
        }
    }
}
