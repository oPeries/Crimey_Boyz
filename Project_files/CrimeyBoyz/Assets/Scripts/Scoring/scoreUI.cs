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
using UnityEngine.UI;

public class scoreUI : MonoBehaviour {

    Dictionary<int, int> playerScores;
    private Text UIText;

    public GameObject playerScoreElementReference;
    //y position in screen units
    //public float positionY;
    List<playerUICommander> listOfPlayerScoreElements = new List<playerUICommander>();


    void Start() {
        //UIText = GetComponent<Text>();
        //float positionY = -Camera.main.orthographicSize * 2;
        //Debug.Log(playerScoreUIStartPosition);
        
        if(SessionManager.Singleton.IsNetworkedGameStateReady()) {
            UpdateUiItems();
        }
        SessionManager.Singleton.PlayerStateChangedCallback += UpdateUiItems;
    }

    // Update is called once per frame
    void Update() {
    }

    private void OnDestroy() {
        SessionManager.Singleton.PlayerStateChangedCallback -= UpdateUiItems;
    }
    
    private void UpdateUiItems() {
        if(!SessionManager.Singleton.IsNetworkedGameStateReady()) {
            return;
        }

        //Remove all existing items
        while(listOfPlayerScoreElements.Count > 0) {
            playerUICommander ui = listOfPlayerScoreElements[0];
            listOfPlayerScoreElements.Remove(ui);
            Destroy(ui.gameObject);
        }

        playerScores = SessionManager.Singleton.networkedGameState.GetAllScores();

        Canvas parentCanvas = gameObject.GetComponent<Canvas>();
        float parentCanvasHeight = parentCanvas.GetComponent<RectTransform>().rect.height;
        float parentCanvasWidth = parentCanvas.GetComponent<RectTransform>().rect.width;
        float playerScoreUIStartPosition = -parentCanvasWidth / 2 + (parentCanvasWidth) / 5;
        float playerScoreUIIncrement = parentCanvasWidth * 0.8f * (1f / playerScores.Count);

        int i = 0;

        //Debug.Log(playerScores.Count);
        foreach (KeyValuePair<int, int> player in playerScores) {


            GameObject UIElement = Instantiate(playerScoreElementReference);

            UIElement.transform.SetParent(parentCanvas.transform, false);

            //set position along width of parent canvas + 20% margin on both sides
            UIElement.transform.localPosition = new Vector2(playerScoreUIStartPosition + (playerScoreUIIncrement * i), -parentCanvasHeight / 2 + 20);


            playerUICommander playerCommander = UIElement.GetComponent<playerUICommander>();
            playerCommander.playerIndex = player.Key - 1; //The key returned by GetPlayerScores is the player number. (1-5) index is one less (0-4)
            listOfPlayerScoreElements.Add(playerCommander);

            i++;
        }
    }
}
