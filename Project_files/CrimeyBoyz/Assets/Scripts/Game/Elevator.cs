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
using TMPro;
using UnityEngine.Experimental.Rendering.LWRP;
using MLAPI;

//Controller for elevator behaviour
public class Elevator : MonoBehaviour {

    public enum ElevatorState {
        OPEN,
        CLOSED,
        OPENING,
        CLOSING
    }

    public bool isSpawnElevator = false; //set to true when this instance is to be the elevator where players spawn at the start of the level
    //Current behaviour when set - starts with doorsclosed, starts opening on level started. Once opened, "spits out" any players inside the elevator

    //Visual game objects
    public GameObject doorLeft;
    public GameObject doorRight;
    //public GameObject elevator;
    public ParticleSystem doorLeftSpark;
    public ParticleSystem doorRightSpark;
    public BoxCollider2D elevatorBox;
    public GameObject elevatorButton;
    public GameObject xButtonUI;
    public GameObject bButtonUI;
    public GameObject gradientBar;
    public GameObject gradientBarIndicator;
    public GameObject XLight;
    public GameObject BLight;
    public TextMeshPro text;
    public Light2D globalLight;
    public GameObject tutorialText;

    //Elevator state variables
    private float timer; //How long the elevator has been opening/closing for
    private float timeToFinish; //When timer reaches this value, the elevator is fully closed / open - players adjust this value when spamming buttons
    private ElevatorState currentState;

    private float timeToButtonOn = 0.5F;
    private float buttonTimer = 0;
    private bool button = false;
    private bool buttonOn = false;
    private float count = 0;

    //x and b button variables
    private float gradient = 0;

    private Color current;

    List<player> inElevator = new List<player>();
    private Dictionary<int, int> playerOpenCount = new Dictionary<int, int>();
    private Dictionary<int, int> playerCloseCount = new Dictionary<int, int>();

    void Start() {
        elevatorButton.SetActive(false);

        XLight.SetActive(false);
        BLight.SetActive(false);

        if (isSpawnElevator) {
            SetClosed();
            timeToFinish = 3;
            timer = 0;
            TurnOnButton();
            tutorialText.SetActive(false);
            currentState = ElevatorState.OPENING; //Immediately start opening the elevator
        } else {
            timeToFinish = 60;
            timer = 0;
            SetOpened();
            Color current = globalLight.color;
        }
    }

    void Update() {

        if (!isSpawnElevator && timeToFinish - timer <= 10)
        {

            float t = Mathf.PingPong(Time.time, 1) / 1;
            globalLight.color = Color.Lerp(current, Color.red, t);

            float timerText = Mathf.Round((timeToFinish - timer) * 10.0f) / 10.0f;
            text.SetText(timerText.ToString());

        }


        if (!isSpawnElevator && currentState != ElevatorState.CLOSING) {
            if (timer < timeToFinish) {
                timer += Time.deltaTime;
            }
            
            if (timer >= timeToFinish && currentState != ElevatorState.CLOSED) {
                if (count < 1) {
                    count += 0.1f;
                    SetDoorPos(count);
                } else if (count >= 1) {
                    SetDoorPos(1);
                    SetClosed();
                    TurnOnButton();
                    SessionManager.Singleton.LoadNextFloor();
                }
            }



        }

        if (buttonTimer < timeToButtonOn && button == true) {
            buttonTimer += Time.deltaTime;
        } else if (buttonTimer >= timeToButtonOn && buttonOn == false) {
            TurnOnButton();
        }

        //If the elevator is not opening, allow the player to get out
        if (currentState != ElevatorState.OPENING) {
            foreach (player gamer in inElevator) {
                if (gamer.getButtonDown("Y")) {
                    RemovePlayerFromElevator(gamer);
                    break;
                }
            }
        }

        if (currentState == ElevatorState.CLOSING) {

            if (gradient > 0) {
                timer += Time.deltaTime / gradient;
            } else {
                timer += Time.deltaTime;
            }

            if (!xButtonUI.activeSelf) {
                xButtonUI.SetActive(true);
                bButtonUI.SetActive(true);
                gradientBar.SetActive(true);
                gradientBarIndicator.SetActive(true);
            }

            float timerText = Mathf.Round((timeToFinish - timer) * 10.0f) / 10.0f;
            text.SetText(timerText.ToString());

            if (timer < timeToFinish) {
                foreach (player gamer in inElevator) {
                    if (gamer.getButtonDown("X")) {

                        if (gradient > -2 && gradient - (0.2f + (timer / 1000)) != -2) {
                            gradient -= 0.2f + (timer / 1000);
                        } else {
                            gradient = -2;
                        }
                        XLight.SetActive(true);

                        if(!playerCloseCount.ContainsKey(gamer.GetPlayerNumber())) {
                            playerCloseCount[gamer.GetPlayerNumber()] = 0;
                        }
                        playerCloseCount[gamer.GetPlayerNumber()] += 1;

                    } else {
                        XLight.SetActive(false);
                    }

                    if (gamer.getButtonDown("B")) {
                        if (gradient < 2 && gradient + (0.2f - (timer / 1000)) != 2) {
                            gradient += 0.2f - (timer / 1000);
                        } else {
                            gradient = 2;
                        }

                        if (!playerOpenCount.ContainsKey(gamer.GetPlayerNumber())) {
                            playerOpenCount[gamer.GetPlayerNumber()] = 0;
                        }
                        playerOpenCount[gamer.GetPlayerNumber()] += 1;

                        BLight.SetActive(true);

                    } else {
                        BLight.SetActive(false);
                    }

                    count = 0.25f - (gradient / 4);
                }

                gradientBarIndicator.transform.position = new Vector3(transform.position.x + gradient, gradientBarIndicator.transform.position.y, gradientBarIndicator.transform.position.z);
                SetDoorPos(0.25f - (gradient / 4));
            }


            if (!doorLeftSpark.isEmitting) {
                doorLeftSpark.Play();
            }
            if (!doorRightSpark.isEmitting) {
                doorRightSpark.Play();
            }



            if (timer >= timeToFinish || gradient <= -2) {
                timer = timeToFinish;
                if (count < 1) {
                    count += 0.1f;
                    SetDoorPos(count);
                } else if (count >= 1) {
                    SetDoorPos(1);
                    SetClosed();
                    TurnOnButton();

                    if (NetworkingManager.Singleton.IsServer || NetworkingManager.Singleton.IsHost) {
                        foreach (KeyValuePair<int, int> count in playerOpenCount) {
                            SessionManager.Singleton.dataRecorder.RecordInteraction("ElevatorOpens", count.Key, null, null, count.Value.ToString());
                        }
                        foreach (KeyValuePair<int, int> count in playerCloseCount) {
                            SessionManager.Singleton.dataRecorder.RecordInteraction("ElevatorCloses", count.Key, null, null, count.Value.ToString());
                        }
                        SessionManager.Singleton.LoadNextFloor();
                    }
                }

            }

        } else if (currentState == ElevatorState.OPENING) {
            timer += Time.deltaTime;

            float timerText = Mathf.Round((timeToFinish - timer) * 10.0f) / 10.0f;
            text.SetText(timerText.ToString());


            if (!doorLeftSpark.isEmitting) {
                doorLeftSpark.Play();
            }
            if (!doorRightSpark.isEmitting) {
                doorRightSpark.Play();
            }

            SetDoorPos(1 - timer / timeToFinish);

            if (timer >= timeToFinish) {
                SetOpened();
                EmptyElevator();
            }
        }
    }

    /// <summary>
    /// Set the door position as a ratio from 0 to 1. Where 1 is fully closed, 0 is fully opened
    /// </summary>
    /// <param name="pos"></param>
    private void SetDoorPos(float pos) {
        doorLeft.transform.position = new Vector3(((transform.position.x) + 1.5f * (pos - 1)), transform.position.y, transform.position.z + 1);
        doorRight.transform.position = new Vector3(((transform.position.x) + 1.5f * (1 - pos)), transform.position.y, transform.position.z + 1);
        doorLeftSpark.transform.position = new Vector3(doorLeft.transform.position.x, doorLeft.transform.position.y+1, transform.position.z - 2);
        doorRightSpark.transform.position = new Vector3(doorRight.transform.position.x, doorRight.transform.position.y+1, transform.position.z - 2);
    }

    /// <summary>
    /// Set the elevator to be fully closed
    /// </summary>
    private void SetClosed() {
        SetDoorPos(1);
        doorLeftSpark.Stop();
        doorRightSpark.Stop();
        currentState = ElevatorState.CLOSED;
        xButtonUI.SetActive(false);
        bButtonUI.SetActive(false);
        gradientBar.SetActive(false);
        gradientBarIndicator.SetActive(false);
        
    }

    /// <summary>
    /// Set the elevator to be fully opened
    /// </summary>
    private void SetOpened() {
        SetDoorPos(0);
        doorLeftSpark.Stop();
        doorRightSpark.Stop();
        currentState = ElevatorState.OPEN;
        xButtonUI.SetActive(false);
        bButtonUI.SetActive(false);
        gradientBar.SetActive(false);
        gradientBarIndicator.SetActive(false);
        timer = 0;
    }

    /// <summary>
    /// Set the given play to be inside the elevator
    /// </summary>
    /// <param name="gamer"></param>
    public void MovePlayerIntoElevator(player gamer) {
        gamer.disablePlayer(true);
        inElevator.Add(gamer);
        if (NetworkingManager.Singleton.IsServer || NetworkingManager.Singleton.IsHost) {
            SessionManager.Singleton.networkedGameState.inElevator.Add(gamer.GetPlayerNumber());
            SessionManager.Singleton.dataRecorder.RecordInteraction("ElevatorEntry", gamer.GetPlayerNumber(), gamer.gameObject.transform.position.x, gamer.gameObject.transform.position.y, null);
        }
        int playersInElevator = inElevator.Count;
        if (tutorialText.activeSelf)
        {
            tutorialText.SetActive(false);
        }
        gamer.gameObject.transform.position = new Vector3((transform.position.x - 0.5f) + (playersInElevator % 2), transform.position.y - 1.3f + ((playersInElevator - 1) / 2), transform.position.z + 1);
    }

    /// <summary>
    /// Remove the given player from the elevator (if already inside)
    /// </summary>
    /// <param name="gamer"></param>
    public void RemovePlayerFromElevator(player gamer) {
        if (inElevator.Contains(gamer)) {
            inElevator.Remove(gamer);
            if (NetworkingManager.Singleton.IsServer || NetworkingManager.Singleton.IsHost) {
                SessionManager.Singleton.networkedGameState.inElevator.Remove(gamer.GetPlayerNumber());
                SessionManager.Singleton.dataRecorder.RecordInteraction("ElevatorExit", gamer.GetPlayerNumber(), gamer.gameObject.transform.position.x, gamer.gameObject.transform.position.y, null);
            }
            gamer.gameObject.transform.position = transform.position; //Stack players on top of each other for the memes (Lets see what happens...)
            gamer.disablePlayer(false);
        }
    }

    /// <summary>
    /// Removes all players from the elevator
    /// </summary>
    public void EmptyElevator() {
        while (inElevator.Count != 0) {
            RemovePlayerFromElevator(inElevator[0]);
        }
    }


    /// <summary>
    /// Move the player inside the elevator when they press "X" button
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision) {

        player gamer = collision.gameObject.GetComponent<player>();

        if (gamer && gamer.getButtonDown("X")) {
            //Player infront of elevator and wants to enter (if not already inside)
            //Player cannot enter an elevator when it is closed
            if ((currentState == ElevatorState.OPEN|| currentState == ElevatorState.CLOSING)&&!isSpawnElevator) {
                if (!inElevator.Contains(gamer)) {
                    MovePlayerIntoElevator(gamer);
                }
            }

            //Start closing the elevator (is it's not the spawn elevator)
            if (currentState == ElevatorState.OPEN && !isSpawnElevator) {
                currentState = ElevatorState.CLOSING;
                playerOpenCount.Clear();
                playerCloseCount.Clear();
                if (timeToFinish - timer >= 10)
                {
                    timer = 0;
                    timeToFinish = 10;
                }
                else
                {
                    timeToFinish-=timer;
                    timer = 0;
                }
            }
        }
    }

    /// <summary>
    /// If a player walks in front of the elevator, set them to be "At Elevator"
    /// </summary>
    /// <param name="collision"> The object the elevator has collided with </param>
    private void OnTriggerEnter2D(Collider2D collision) {

        player collidedPlayer = collision.gameObject.GetComponent<player>();
        if (collidedPlayer) {
            collidedPlayer.setAtElevator(true);
        }
    }

    /// <summary>
    /// If a player walks away from the elevator, set them to be not "At Elevator"
    /// </summary>
    /// <param name="collision"> The object the elevator has stopped colliding with </param>
    private void OnTriggerExit2D(Collider2D collision) {
        player collidedPlayer = collision.gameObject.GetComponent<player>();
        if (collidedPlayer && !inElevator.Contains(collidedPlayer)) {
            collidedPlayer.setAtElevator(false);
        }
    }
    
    private void TurnOnButton()
    {
        buttonOn = true;
        elevatorButton.SetActive(true);
    }


}
