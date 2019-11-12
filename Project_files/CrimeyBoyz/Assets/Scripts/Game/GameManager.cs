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
using UnityEngine.SceneManagement;
using MLAPI;

//[Obsolete]
//Controls and manages the current game state
//Should only ever be one instance of this script
public class GameManager : MonoBehaviour {

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public GameObject playerPrefab;
    public GameObject draggablePrefab;
    public GameObject droppablePrefab;
    public GameObject tappablePrefab;
    public GameObject spawnerPrefab;

    private Dictionary<int, player> players = new Dictionary<int, player>(); //Players in the current game

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Behavioural ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //If is the server, spawn an initial game object for testing
    private void Start() {

        NetworkedObject[] networkedObjects = Resources.FindObjectsOfTypeAll<NetworkedObject>();
        foreach (NetworkedObject obj in networkedObjects) {
            if (!obj.gameObject.activeSelf) {
                //Debug.Log("Found inactive networkedObject, activating and spawning");
                if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer) {
                    obj.gameObject.SetActive(true);
                    obj.Spawn();

                } else {
                    Destroy(obj.gameObject);
                }
            }
        }

        //If running the host (Game), add a player for each connected controller
        if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsServer) {
            for (int i = 0; i < CountConnectedControllers(); i++) {
                spawnPlayer(i + 1, "Chad");
            }

            SpawnObject(draggablePrefab, new Vector3(-7.8f, 3, 0));
            SpawnObject(droppablePrefab, new Vector3(-6.4f, 3, 0));
            SpawnObject(tappablePrefab, new Vector3(-5f, 3, 0));
            SpawnObject(spawnerPrefab, new Vector3(5f, -3.7f, -2));

            //ConnectionManager.ResetTabletOwnership();
        }
    }

    void Update() {

        //Return to Main menu upon hitting "ESC" key
        //KeyCode.Escape works for Android & Windows Builds
        if (Input.GetKeyDown(KeyCode.Escape)) {
            StopGame();
        }

        //Return to main menu if the connection with the tablet is lost
        if(ConnectionManager.Singleton.GetState() == ConnectionState.DISCONNECTED) {
            Debug.Log("Stopping game due to disconnection");
            StopGame();
        }

    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /// <summary>
    /// Stop the current game and return to main menu (only if main menu added to build settings). 
    /// If menu not in build settings, reloads the current scene
    /// </summary>
    public void StopGame() {

        List<string> scenesInBuild = new List<string>();
        for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            int lastSlash = path.LastIndexOf("/");
            scenesInBuild.Add(path.Substring(lastSlash + 1, path.LastIndexOf(".") - lastSlash - 1));
        }

        ConnectionManager.Singleton.CloseConnection();

        if (scenesInBuild.Contains("Menu")) {
            SceneManager.LoadScene("Menu");

        } else {
            //ConnectionManager.Singleton.OpenConnection();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// Count how many controllers are connected to the current device
    /// </summary>
    /// <returns></returns>
    public static int CountConnectedControllers() {
        int count = 0;
        foreach (string name in Input.GetJoystickNames()) {
            if(!name.Equals("")) {
                count++;
            }
        }
        //Debug.Log("Connected controllers: " + count.ToString());
        return count;
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /// <summary>
    /// Spawn a new player in the game scene & assign them to the provided controller number
    /// </summary>
    /// <param name="controllerNum"> The number controller of the player being spawned</param>
    /// <param name="name"> The name of the player being spawned </param>
    private void spawnPlayer(int controllerNum, string name) {
        if(!NetworkingManager.Singleton.IsHost && !NetworkingManager.Singleton.IsServer) {
            Debug.LogError("Tried to spawn a player when not the server");
            return;
        }

        //Spawn in scene
        GameObject newPlayer = Instantiate(playerPrefab);

        //Set name and controller
        players.Add(controllerNum, newPlayer.GetComponent<player>());
        players[controllerNum].setName(name);
        players[controllerNum].setController(controllerNum);

        //Spawn over the network
        newPlayer.GetComponent<NetworkedObject>().Spawn(); //by default spawns with the server as the owner

        Debug.Log(players[controllerNum].getName() + " is using controller number " + controllerNum);
    }

    /// <summary>
    /// Spawn a networked object at the given position
    /// </summary>
    /// <param name="obj"> The obect being spawned </param>
    /// <param name="pos"> The position of the spawned object </param>
    private void SpawnObject(GameObject obj, Vector3 pos) {
        if (!NetworkingManager.Singleton.IsHost && !NetworkingManager.Singleton.IsServer) {
            Debug.LogError("Tried to spawn an object when not the server");
            return;
        }

        //Spawn in scene & set position
        GameObject instance = Instantiate(obj);
        instance.transform.position = pos;

        //Spawn over the network
        instance.GetComponent<NetworkedObject>().Spawn(); //by default spawns with the server as the owner

    }
}