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

//Controls the scoreboard
public class moneyDistribution : MonoBehaviour
{
    private bool down;
    public List<GameObject> sharerList;
    public GameObject distributor;
    private player controller;
    private Dictionary<int, player> addedPlayers = new Dictionary<int, player>();
    private List<player> sharedPlayers = new List<player>();
    private float timerMax = 6;
    private float timer;
    private int[] floorvalues = new int[] {500, 550, 600, 700, 900, 1250, 2000};
    public int bagValue = 1000;
    private int displayY = 0;
    private float boardSpeed = .3f;
    private float t = 0;

    void Start()
    {
        bagValue = floorvalues[SessionManager.Singleton.networkedGameState.GetCurrentFloor()];
    }

    private void Update()
    {
        if (ElevatorLevelController.Singleton.distribute == true)
        {
            ElevatorLevelController.Singleton.distribute = false;
            boardDown(ElevatorLevelController.Singleton.richPlayer, ElevatorLevelController.Singleton.playersInElevator);
        }
        //If the board is down, update the board values
        if (down)
            updateSharePlayers();
        if (ElevatorLevelController.Singleton.dontdist == true)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (down)
        {
            if (transform.position.y > displayY)
            {
                transform.position -= new Vector3(0, Mathf.Lerp(0, boardSpeed, t), 0);
                t += Time.fixedDeltaTime;
            }
        }
        else
        {
            if (transform.position.y < 13)
            {
                transform.position += new Vector3(0, Mathf.Lerp(0, boardSpeed, t), 0);
                t += Time.fixedDeltaTime;
            }
        }
    }

    private void updateSharePlayers()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            boardUp();
            timer = 0;
        }
        try
        {
            if (controller.getButtonDown("A"))
            {
                if (sharedPlayers.Contains(addedPlayers[0]))
                {
                    sharedPlayers.Remove(addedPlayers[0]);
                    sharerList[0].GetComponent<Image>().color = new Color32(122, 95, 90, 100);
                }
                else
                {
                    sharedPlayers.Add(addedPlayers[0]);
                    sharerList[0].GetComponent<Image>().color = new Color32(122, 255, 90, 100);
                }
            }
            if (controller.getButtonDown("B"))
            {
                if (sharedPlayers.Contains(addedPlayers[1]))
                {
                    sharedPlayers.Remove(addedPlayers[1]);
                    sharerList[1].GetComponent<Image>().color = new Color32(122, 95, 90, 100);
                }
                else
                {
                    sharedPlayers.Add(addedPlayers[1]);
                    sharerList[1].GetComponent<Image>().color = new Color32(122, 255, 90, 100);
                }
            }
            if (controller.getButtonDown("X"))
            {
                if (sharedPlayers.Contains(addedPlayers[2]))
                {
                    sharedPlayers.Remove(addedPlayers[2]);
                    sharerList[2].GetComponent<Image>().color = new Color32(122, 95, 90, 100);
                }
                else
                {
                    sharedPlayers.Add(addedPlayers[2]);
                    sharerList[2].GetComponent<Image>().color = new Color32(122, 255, 90, 100);
                }
            }
            if (controller.getButtonDown("Y"))
            {
                if (sharedPlayers.Contains(addedPlayers[3]))
                {
                    sharedPlayers.Remove(addedPlayers[3]);
                    sharerList[3].GetComponent<Image>().color = new Color32(122, 95, 90, 100);
                }
                else
                {
                    sharedPlayers.Add(addedPlayers[3]);
                    sharerList[3].GetComponent<Image>().color = new Color32(122, 255, 90, 100);
                }
            }
        }
        catch (KeyNotFoundException)
        {
            //Do nothing
        }
    }

    private void boardDown(player richPlayer, List<player> inElevator)
    {
        t = 0;
        down = true;
        controller = richPlayer;
        distributor.GetComponent<Text>().text = SessionManager.Singleton.networkedGameState.GetPlayer(richPlayer.GetPlayerNumber()).username;
        sharedPlayers.Add(controller);
        timer = timerMax;
        foreach (GameObject share in sharerList)
        {
            //If the number of players in the elevator is greater than the sharebox we are up to
            if (inElevator.Count > sharerList.IndexOf(share))
            {
                //Set the name
                share.GetComponent<scoreUIElement>().name.GetComponent<Text>().text = SessionManager.Singleton.networkedGameState.GetPlayer(inElevator[sharerList.IndexOf(share)].GetPlayerNumber()).username;
                //Set the image
                share.GetComponent<scoreUIElement>().playerImage.GetComponent<Image>().sprite = inElevator[sharerList.IndexOf(share)].GetComponent<SpriteRenderer>().sprite;
                //Start with red 'disabled' background
                share.GetComponent<Image>().color = new Color32(122, 95, 90, 100);
            }
            else
            {
                Destroy(share);
            }
        }
        foreach (player gamer in inElevator)
        {
            addedPlayers.Add(addedPlayers.Count, gamer);
        }
    }

    private void boardUp()
    {
        t = 0;
        down = false;
        foreach (player gamer in sharedPlayers)
        {
            SessionManager.Singleton.networkedGameState.UpdateScore(gamer.GetPlayerNumber(), (int)bagValue / sharedPlayers.Count);
        }

        addedPlayers = new Dictionary<int, player>();
        sharedPlayers = new List<player>();
        controller = null;
        ElevatorLevelController.Singleton.spawnRest();
    }
}