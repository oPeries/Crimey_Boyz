/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using System;

namespace CrimeyBoyz.GameState {
    //Information for each player's state
    public class PlayerInfo {

        [Obsolete("Use PlayerInfo.assignedController instead")]
        public int controllerNum;

        [Obsolete("Use PlayerInfo.characterName instead")]
        public string name;

        public int assignedController; //-1 for not assigned
        public string username;
        public string characterName;
        public int score;

        public PlayerInfo() {
            assignedController = -1;
            username = null;
            characterName = null;
            score = 0;
        }

        [Obsolete("Use new PlayerInfo() instead")]
        public PlayerInfo(int controllerNumber, string playerName) {
            assignedController = controllerNumber;
            controllerNum = assignedController;

            characterName = playerName;
            name = playerName;

            score = 0;
        }

        //Convert this class to the obsolete one (for backwards compatibility)
        /*public static implicit operator SessionManager.PlayerInfo(PlayerInfo x) {
            SessionManager.PlayerInfo result = new SessionManager.PlayerInfo(x.assignedController, x.characterName);

            result.score = x.score;

            return result;
        }*/
    }
}
