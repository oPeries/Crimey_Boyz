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

public class playerUICommander : MonoBehaviour
{
    public int playerIndex = 0;
    
    Color playerColour;
    /*red, blue, yellow, green, orange*/
    Color32[] playerColours = {
        new Color32(240, 65, 65, 255),
        new Color32(58, 107, 242, 255),
        new Color32(237, 223, 69, 255),
        new Color32(110, 219, 88, 255),
        new Color32(247, 125, 2, 255)
    };


    private float score;
    private float visibleScore;

    //p1-p5
    public int playerNumber;
    private bool isMissionControl = false;
    private int actualscore;

    Image playerRoleIcon;
    Text playerIdentifier;
    Text playerScore;

    public Sprite gamepad_icon;
    public Sprite tablet_icon;

    // Start is called before the first frame update
    void Start()
    {

        //gamepad_icon = Resources.Load("UI/PlayerDiplay/gamepad_icon") as Sprite;
        //tablet_icon = Resources.Load("UI/PlayerDisplay/tablet_icon") as Sprite;

        playerColour = playerColours[playerIndex];
        //

        playerRoleIcon = gameObject.GetComponentInChildren<Image>();
        playerIdentifier = gameObject.GetComponentsInChildren<Text>()[0];
        playerScore = gameObject.GetComponentsInChildren<Text>()[1];

        playerNumber = (int) playerIndex + 1;
        playerIdentifier.text = "P" + playerNumber;

        playerScore.color = playerColour;
        playerIdentifier.color = playerColour;
        //Debug.Log(playerColour);
        score = SessionManager.Singleton.networkedGameState.GetPlayer(playerNumber).score;
        visibleScore = score;

        if (SessionManager.Singleton.tabletPlayer == playerNumber)
            isMissionControl = true;
        switchRoles(isMissionControl);
    }



    // Update is called once per frame
    void Update()
    {
        //Should use an action to do this rather than every frame (@Cameron said he was setting this up a bit ago)
        score = SessionManager.Singleton.networkedGameState.GetPlayer(playerNumber).score;
        visibleScore = Mathf.MoveTowards(visibleScore, score, 500 * Time.deltaTime);
        playerScore.text = "$" + Mathf.Floor(visibleScore);
    }

    private void updatePlayerScore(int newScore) {
        score += newScore;

    }

    public void switchRoles(bool isMC) {
        if (isMC)
        {
            playerRoleIcon.sprite = tablet_icon;
        }
        else {
            playerRoleIcon.sprite = gamepad_icon;
        }


    }
}
