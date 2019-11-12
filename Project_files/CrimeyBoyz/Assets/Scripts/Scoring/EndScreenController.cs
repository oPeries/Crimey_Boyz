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
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndScreenController : MonoBehaviour
{
    public bool isSolo;
    public bool isLoseScreen;
    public TextMeshPro text;
    int groupWin;
    int soloWin;
    public GameObject pScoreUI;
    // Start is called before the first frame update

    public GameObject menuButton;

    private EventSystem eventSystem; //Current event system, used to set the currently selected object

    private float startTime;
    private float startButtonDelay = 10f;

    void Start()
    {
        eventSystem = EventSystem.current;

        groupWin = SessionManager.Singleton.groupWin;
        soloWin = SessionManager.Singleton.soloWin;
        //groupWin = 10;
        //soloWin = 1;
        if (isSolo)
        {
            int winnerPlayerNumber = getPlayers()[0].Key;
            text.text = "Player "+ winnerPlayerNumber + " won with over $" + soloWin + "! But at what cost?";
        }
        else if (isLoseScreen)
        {
            text.text = "You didn't profit enough!";
        }
        else {
            text.text = "You all successfully paid off your debt of $" + groupWin + "!";
        }

        orderPlayers();

        startTime = Time.time;
    }

    private void Update() {
#if UNITY_STANDALONE_WIN
        if (eventSystem.currentSelectedGameObject == null) {
            for (int i = 1; i < 6; i++) {
                if (Mathf.Abs(Input.GetAxis("Vertical" + i)) >= 0.7) {
                    eventSystem.SetSelectedGameObject(menuButton);
                    break;
                }
            }
        }
#endif
    }


    public List<KeyValuePair<int, int>> getPlayers() {
        Dictionary<int, int> players = SessionManager.Singleton.networkedGameState.GetAllScores();
        //Dictionary<int, int> players = new Dictionary<int, int>();
        List<KeyValuePair<int, int>> playerList = new List<KeyValuePair<int, int>>();

        foreach (KeyValuePair<int, int> player in players)
        {
            playerList.Add(player);
        }


        playerList.Sort((a, b) => a.Value.CompareTo(b.Value));
        playerList.Reverse();
        return playerList;

    }

    public void orderPlayers() {


        Canvas parentCanvas = gameObject.GetComponentInChildren<Canvas>();
        float parentCanvasHeight = parentCanvas.GetComponent<RectTransform>().rect.height;
        float parentCanvasWidth = parentCanvas.GetComponent<RectTransform>().rect.width;
        float playerScoreUIStartPosition = -parentCanvasWidth / 2 + (parentCanvasWidth) / 5;
        float playerScoreUIIncrement = parentCanvasWidth * 0.8f * (1f / getPlayers().Count);
        int i = 0;
        foreach (KeyValuePair<int, int> player in getPlayers())
        {
            
            GameObject UIElement = Instantiate(pScoreUI);

            UIElement.transform.SetParent(parentCanvas.transform, false);

            //set position along width of parent canvas + 20% margin on both sides
            UIElement.transform.localPosition = new Vector3(playerScoreUIStartPosition + (playerScoreUIIncrement * i), -parentCanvasHeight/8 + 100, gameObject.transform.position.z);
            UIElement.transform.localScale = new Vector3(200, 200, 0);

            i++;

            endScoreUIController control = UIElement.GetComponent<endScoreUIController>();
            control.setPlace(i);
            control.getScore().GetComponent<TextMeshPro>().text = "$" + player.Value.ToString();
            control.getPName().GetComponent<TextMeshPro>().text = SessionManager.Singleton.networkedGameState.GetPlayer(player.Key).username;
            control.getSprite().sprite = AllCharacters.Singleton.getSprite(SessionManager.Singleton.networkedGameState.GetPlayer(player.Key).characterName);
            control.setColours(player.Key);


        }
    }
    public void BackToMainMenu() {
        if (Time.time > (startTime + startButtonDelay)) {
            SessionManager.Singleton.ReturnToMainMenu();
        }
    }
}
