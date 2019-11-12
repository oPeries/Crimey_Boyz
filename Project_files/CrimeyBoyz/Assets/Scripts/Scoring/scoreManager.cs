﻿/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using System.Collections.Generic;
using UnityEngine;

//Controls the scoreboard
public class scoreManager : MonoBehaviour {

    private GameManager gm;
    private moneyDistribution md;
    public GameObject scoreboard;
    
    void Start() {
        //gm = GetComponent<GameManager>();
        //md = scoreboard.GetComponent<moneyDistribution>();
    }

    /// <summary>
    /// Opens the share menu for players to see who they can share money with
    /// </summary>
    /// <param name="loadedPlayer"> The player who has all the money </param>
    /// <param name="inElevator"> A list of all players in the elevator </param>
    //public void openShareMenu(player loadedPlayer, List<player> inElevator) {
        //md.boardMove(true, loadedPlayer, inElevator);
    //}
}