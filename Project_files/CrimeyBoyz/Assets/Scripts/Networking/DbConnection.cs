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
using UnityEngine.Networking;
using UnityEngine;
using System.Text;

//Class for communicating with the database
//Used to check and add to data in the database
//Uses http POST requests to communicate with the server
public class DbConnection : MonoBehaviour {

    //private const string baseURL = "http://192.168.33.58/DB_Interface/"; //the base site for http requests
    private const string baseURL = "https://scruffle.uqcloud.net/DB_Interface/"; //the base site for http requests
    private const string knownUsername = "Cameron"; //Known user credentials used to ensure there is a DB connection. Change to a test user later
    private const string knownPass = "Cameron1";
    private bool disregardNext = false;

    public static DbConnection Singleton { get; private set; }

    private enum DbConnectionState {
        READY,
        AWAITING_RESPONSE,
        RESPONSE_RECEIVED
    }

    private DbConnectionState currentState;
    private DbResponse lastResponse;

    // Start is called before the first frame update
    void Start() {
        currentState = DbConnectionState.READY;
        lastResponse = null;
    }

    /// <summary>
    /// Sets the singleton for this class. Ensures only one copy of this script is instantiated.
    /// </summary>
    private void OnEnable() {

        if (Singleton != null && this != Singleton) {
            Destroy(this.gameObject);
        } else {
            Singleton = this;
        }
    }

    public void DisregardNextResponse() {
        disregardNext = true;
    }

    //Return the last response received (or null if response not yet received)
    //If a response has been received, calling this function resets the state to "READY"
    public DbResponse GetResponse() {

        if (currentState == DbConnectionState.RESPONSE_RECEIVED) {
            currentState = DbConnectionState.READY;
        }

        DbResponse result = lastResponse;
        lastResponse = null;

        return result;
    }

    public void CheckLogin(string username, string password) {
        StartCoroutine(CheckUserCredentials(username, password));
    }

    public void CreateUser(string username, string name, string email, string password1, string password2) {
        StartCoroutine(CreateNewUser(username, name, email, password1, password2));
    }

    public void SaveSessionData(DataRecorder recorder) {
        StartCoroutine(SaveData(recorder));
    }


    //Ensure there is a connection to the db by querying a known user account
    //If connection cannot be established, stops the coroutine, sets to the "RESPONSE_RECEIVED" state and set response to null;
    IEnumerator CheckDbConnection() {
        WWWForm form = new WWWForm();
        form.AddField("username", knownUsername);
        form.AddField("password", knownPass);

        using (UnityWebRequest www = UnityWebRequest.Post(baseURL + "check_user.php", form)) {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {
                Debug.LogWarning("Cannot connect to the database: " + www.error);
                Debug.Log(www.downloadHandler.text);
                if (disregardNext) {
                    currentState = DbConnectionState.READY;
                    lastResponse = null;
                } else {
                    currentState = DbConnectionState.RESPONSE_RECEIVED;
                    lastResponse = new DbResponse(DbResponseState.NOCONNECTION, www.downloadHandler.text, www.GetResponseHeaders());
                }
                yield break;
            } //else {
                //Debug.Log("Database connection is available");
            //}
        }
    }

    //Check if the given username and credentials match an existing user
    //When called, sets to the AWAITING_RESPONSE state
    //Once a response is received, changes to RESPONSE_RECEIVED
    //Sets response based on
    // 200 - SUCCESS (credentials are valid)
    // 404 - NOTFOUND (credentials not valid)
    // ?   - OTHER (something else)
    IEnumerator CheckUserCredentials(string username, string password) {
        UnityWebRequest.ClearCookieCache();
        Debug.Log("Checking user credentials are valid");

        if(currentState != DbConnectionState.READY) {
            Debug.LogWarning("Tried to connect to the DB before receiving response, will not process this request");
            yield break;
        }

        currentState = DbConnectionState.AWAITING_RESPONSE;
        yield return StartCoroutine(CheckDbConnection());

        if(currentState != DbConnectionState.AWAITING_RESPONSE) {
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(baseURL + "check_user.php", form)) {
            yield return www.SendWebRequest();

            DbResponseState state;

            if (www.responseCode == 200) {
                state = DbResponseState.SUCCESS;
            } else if(www.responseCode == 404) {
                state = DbResponseState.NOTFOUND;
            } else {
                Debug.LogError("Unexpected response when checking user credentials: " + www.error);
                Debug.Log(www.downloadHandler.text);
                state = DbResponseState.OTHER;
            }

            if (disregardNext) {
                currentState = DbConnectionState.READY;
                lastResponse = null;
            } else {
                currentState = DbConnectionState.RESPONSE_RECEIVED;
                lastResponse = new DbResponse(state, www.downloadHandler.text, www.GetResponseHeaders());
            }
        }
    }

    //Save the values of the data recorder to the DB.
    //When called, sets to the AWAITING_RESPONSE state
    //Once a response is received, changes to RESPONSE_RECEIVED
    //Sets response based on
    // 200 - SUCCESS (scores saved)
    // 404 - NOTFOUND (scores not saved)
    // ?   - OTHER (something else)
    IEnumerator SaveData(DataRecorder recorder) {
        UnityWebRequest.ClearCookieCache();

        if (currentState != DbConnectionState.READY) {
            Debug.LogWarning("Tried to connect to the DB before receiving response, will not process this request");
            yield break;
        }

        currentState = DbConnectionState.AWAITING_RESPONSE;
        yield return StartCoroutine(CheckDbConnection());

        if (currentState != DbConnectionState.AWAITING_RESPONSE) {
            yield break;
        }

        var www = new UnityWebRequest(baseURL + "save_data.php", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(recorder.ToJSON());
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();
        DbResponseState state;

        if (www.responseCode == 200) {
            state = DbResponseState.SUCCESS;
        } else if (www.responseCode == 404) {
            state = DbResponseState.NOTFOUND;
        } else {
            Debug.LogError("Unexpected response when trying to save data: " + www.error);
            Debug.Log(www.downloadHandler.text);
            state = DbResponseState.OTHER;
        }

        if (disregardNext) {
            currentState = DbConnectionState.READY;
            lastResponse = null;
        } else {
            currentState = DbConnectionState.RESPONSE_RECEIVED;
            lastResponse = new DbResponse(state, www.downloadHandler.text, www.GetResponseHeaders());
        }
    }


    //Try create a new user in the DB
    //When called, sets to the AWAITING_RESPONSE state
    //Once a response is received, changes to RESPONSE_RECEIVED
    //Sets response based on
    // 200 & "ok" - SUCCESS (user created)
    // 200 & "username" - username already exists in db
    // 200 & "email" - email already exists in db
    // 200 & "pass" - passwords did not match
    // 404 - NOTFOUND (malformed post request)
    // ?   - OTHER (something else)
    IEnumerator CreateNewUser(string username, string name, string email, string password1, string password2) {
        Debug.Log("Checking user credentials are valid");

        if (currentState != DbConnectionState.READY) {
            Debug.LogWarning("Tried to connect to the DB before receiving response, will not process this request");
            yield break;
        }

        currentState = DbConnectionState.AWAITING_RESPONSE;
        yield return StartCoroutine(CheckDbConnection());

        if (currentState != DbConnectionState.AWAITING_RESPONSE) {
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("name", name);
        form.AddField("password1", password1);
        form.AddField("password2", password2);
        form.AddField("email", email);

        using (UnityWebRequest www = UnityWebRequest.Post(baseURL + "create_new_user.php", form)) {
            yield return www.SendWebRequest();
            DbResponseState state;

            string responseText = www.downloadHandler.text;

            if (www.responseCode == 200) {
                if (responseText.ToLower().Equals("username")) {
                    state = DbResponseState.USERNAME_EXISTS;

                } else if (responseText.ToLower().Equals("email")) {
                    state = DbResponseState.EMAIL_EXISTS;

                } else if (responseText.ToLower().Equals("pass")) {
                    state = DbResponseState.PASSWORDS_DONT_MATCH;

                } else if (responseText.ToLower().Equals("ok")) {
                    state = DbResponseState.SUCCESS;

                } else {
                    Debug.Log("Unexpected HTTP message when adding new user: " + responseText);
                    state = DbResponseState.OTHER;
                }

            } else if (www.responseCode == 404) {
                state = DbResponseState.NOTFOUND;

            } else {
                Debug.LogError("Unexpected response when trying to save scores: " + www.error);
                Debug.Log(www.downloadHandler.text);
                state = DbResponseState.OTHER;
            }

            if (disregardNext) {
                currentState = DbConnectionState.READY;
                lastResponse = null;
            } else {
                currentState = DbConnectionState.RESPONSE_RECEIVED;
                lastResponse = new DbResponse(state, www.downloadHandler.text, www.GetResponseHeaders());
            }
        }
    }

}
