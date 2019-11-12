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

//Manages the "Lobby" section in the main menu
//Initialises lobby prefabs based on current build
//Will initialise prefabs as children of the attached GameObject
public class LobbyController : MonoBehaviour {

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public GameObject gameLobbyPrefab;
    public GameObject controllerLobbyPrefab;

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Behavioural ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void Awake() {
        AddBuildPrefabs();
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /// <summary>
    /// Initialise lobby prefabs based on current build
    /// </summary>
    private void AddBuildPrefabs() {

#if DECO3801_GAME_BUILD && !DECO3801_MISSIONCONTROL_BUILD
        GameObject lobby = Instantiate(gameLobbyPrefab);

        lobby.transform.SetParent(transform, false);
        lobby.transform.SetSiblingIndex(0);
#else
        GameObject lobby = Instantiate(controllerLobbyPrefab);

        lobby.transform.SetParent(transform, false);
        lobby.transform.SetSiblingIndex(0);
#endif
    }
}
