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
using MLAPI;

public class InitDBUpload : MonoBehaviour {

    public GameObject loadingWheel;

    private bool waitingForDbResponse;

    // Start is called before the first frame update
    void Start() {
        if(NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer) {
            StartDbUpload();
        }
    }

    // Update is called once per frame
    void Update() {
        if (waitingForDbResponse) {
            DbResponse response = DbConnection.Singleton.GetResponse();
            if (response != null) {
                waitingForDbResponse = false;
                SetLoading(false);
                Debug.Log("response received");
                Debug.Log(response.state);
                Debug.Log(response.message);
            }
        }
    }

    public void StartDbUpload() {
        if (waitingForDbResponse) {
            Debug.LogWarning("Tried to start db upload when already waiting for upload");
            return;
        }

        //dataCollectedText.text = SessionManager.Singleton.dataRecorder.ToJSON();
        DbConnection.Singleton.SaveSessionData(SessionManager.Singleton.dataRecorder);
        waitingForDbResponse = true;
        SetLoading(true);
    }

    private void SetLoading(bool isLoading) {
        //retryUploadButton.gameObject.SetActive(!isLoading);
        loadingWheel.SetActive(isLoading);
    }

}
