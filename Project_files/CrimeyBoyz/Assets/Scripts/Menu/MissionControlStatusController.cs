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

public class MissionControlStatusController : MonoBehaviour {

    public Text statusText;
    public Image border;



    public void UpdateMissionControlStatus() {

        ConnectionState state = ConnectionManager.Singleton.GetState();

        if (state == ConnectionState.CONNECTED) {
            border.color = Color.green;
            statusText.text = "Connected!";

        } else if (state == ConnectionState.LISTENING) {
            border.color = Color.yellow;
            statusText.text = "Please press \"Play\" on mission control";

        } else if (state == ConnectionState.SEARCHING) {
            border.color = Color.yellow;
            statusText.text = "Searching for a game";

        } else if (state == ConnectionState.CONNECTING) {
            border.color = Color.yellow;
            statusText.text = "Connecting to the game";

        } else if (state == ConnectionState.DENIED) {
            border.color = Color.red;
            statusText.text = "Connecting failed, will retry soon";

        } else if (state == ConnectionState.DISCONNECTED) {
            border.color = Color.red;
            statusText.text = "Disconnected from game, will retry soon";

        } else if (state == ConnectionState.STOPPED) {
            border.color = Color.red;
            statusText.text = "Stopped, Timed out?";

        } else {
            border.color = new Color(255, 128, 0);
            statusText.text = "Unexpected: ¯\\_(ツ)_/¯";
            Debug.LogWarning("Unexpected connection state while in mission control status");
        }
    }
}
