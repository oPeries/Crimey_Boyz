/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrimeyBoyz.GameState;
using UnityEngine.SceneManagement;

//Records game data to be sent to the DB upon game completion
public class DataRecorder : MonoBehaviour {

    public struct SessionInfo {
        public string startTime;
        public Dictionary<int, string> usersInSession;
    }

    public struct RoundInfo {
        public string floorName;
        public float floorStartTime;
        public Dictionary<int, int> startingScores;
        public int tableyPlayerNum;
        public List<InteractionInfo> interactions;
    }

    public struct InteractionInfo {
        public string metricName;
        public int initiatingPlayer;
        public float actionTime;
        public float? actionXPos;
        public float? actionYPos;
        public string actionSpecificData;
    }

    private SessionInfo session;
    private List<RoundInfo> rounds;
    private List<InteractionInfo> currentRoundActions;

    private DateTime sessionStart;

    private void Start() {
        rounds = new List<RoundInfo>();
        currentRoundActions = null;
    }

    public void ResetData() {
        session = new SessionInfo();
        rounds.Clear();
        currentRoundActions = null;
    }

    public void DisableInteractionsUntilNextRound() {
        currentRoundActions = null;
    }

    public void StartSession(NetworkedGameState gameState) {
        sessionStart = DateTime.Now;
        session.startTime = sessionStart.ToString("yyyy-MM-dd HH:mm:ss");
        Debug.Log("Session start time: " + session.startTime);

        Dictionary<int, PlayerInfo> players = gameState.GetAllPlayers();

        if(session.usersInSession == null) {
            session.usersInSession = new Dictionary<int, string>();
        } else {
            session.usersInSession.Clear();
        }

        foreach(KeyValuePair<int, PlayerInfo> player in players) {
            session.usersInSession[player.Key] = player.Value.username;
            Debug.Log("Player" + player.Key + "'s username recorded as \"" + session.usersInSession[player.Key] + "\"");
        }

        currentRoundActions = null;
    }

    public void StartNextRound(NetworkedGameState gameState) {
        RoundInfo info = new RoundInfo();

        info.floorName = SceneManager.GetActiveScene().name;
        //Debug.Log("Adding floor info for scene: \"" + info.floorName + "\"");

        info.floorStartTime = (float) (DateTime.Now - sessionStart).TotalSeconds;
        //Debug.Log("Floor started at: " + info.floorStartTime.ToString("0.000") + " seconds");

        info.startingScores = new Dictionary<int, int>();

        foreach(KeyValuePair<int, int> score in gameState.GetAllScores()) {
            info.startingScores[score.Key] = score.Value;
            //Debug.Log("Player" + score.Key + "'s starting score recorded as: " + score.Value);
        }

        info.tableyPlayerNum = SessionManager.Singleton.tabletPlayer;
        //Debug.Log("Mission Control player set to: " + info.tableyPlayerNum);

        info.interactions = new List<InteractionInfo>();

        currentRoundActions = info.interactions;
        rounds.Add(info);
    }

    public void RecordInteraction(string metricName, int initiatingPlayer, float? xPos, float? yPos, string actionData) {
        if(currentRoundActions == null) {
            return;
        }

        InteractionInfo info = new InteractionInfo();

        info.metricName = metricName;
        info.initiatingPlayer = initiatingPlayer;
        info.actionXPos = xPos;
        info.actionYPos = yPos;
        info.actionSpecificData = actionData;
        info.actionTime = (float)(DateTime.Now - sessionStart).TotalSeconds;

        //Debug.Log(string.Format("Recording interaction - Name: \"{0}\" Player: {1} Time: {2} XPos: {3} YPos: {4} Data: {5}", 
        //    metricName, initiatingPlayer, info.actionTime, xPos, yPos, actionData));

        currentRoundActions.Add(info);
    }

    public string ToJSON() {
        string result = "{";

        //Convert session info to a json object
        Dictionary<string, string> sessionDict = new Dictionary<string, string>();
        sessionDict["startTime"] = EscapeString(session.startTime);
        foreach (KeyValuePair<int, string> player in session.usersInSession) {
            sessionDict["player" + player.Key] = EscapeString(player.Value);
        }
        result += ToJSONObject("session", sessionDict) + ",";


        //Convert round info to a json array of json objects
        List<string> roundsJsonList = new List<string>();

        foreach(RoundInfo floor in rounds) {

            Dictionary<string, string> roundDictionary = new Dictionary<string, string>();

            roundDictionary["name"] = EscapeString(floor.floorName);
            roundDictionary["startTime"] = floor.floorStartTime.ToString("0.000");

            foreach (KeyValuePair<int, int> player in floor.startingScores) {
                roundDictionary["player" + player.Key + "Score"] = player.Value.ToString();
            }

            roundDictionary["tabletPlayer"] = floor.tableyPlayerNum.ToString();

            //convert all recorded metrics into a json array
            if (floor.interactions.Count > 0) {
                List<string> interactionsJsonList = new List<string>();
                foreach (InteractionInfo interaction in floor.interactions) {

                    Dictionary<string, string> interactionDict = new Dictionary<string, string>();

                    interactionDict["name"] = EscapeString(interaction.metricName);
                    interactionDict["player"] = interaction.initiatingPlayer.ToString();
                    interactionDict["time"] = interaction.actionTime.ToString("0.000");

                    if (interaction.actionXPos != null) {
                        interactionDict["x"] = ((float)interaction.actionXPos).ToString("0.000");
                    }
                    if (interaction.actionYPos != null) {
                        interactionDict["y"] = ((float)interaction.actionYPos).ToString("0.000");
                    }
                    if (interaction.actionSpecificData != null) {
                        interactionDict["data"] = EscapeString(interaction.actionSpecificData);
                    }

                    interactionsJsonList.Add(ToJSONObject(null, interactionDict));
                }
                roundDictionary["interactions"] = ToJSONArray(null, interactionsJsonList);
            }

            roundsJsonList.Add(ToJSONObject(null, roundDictionary));
        }

        result += ToJSONArray("rounds", roundsJsonList);

        result += "}";

        return result;
    }

    //Add quotation marks around the given string
    private string EscapeString(string toEscape) {
        return "\"" + toEscape + "\"";
    }

    //Convert dictionary into a json object
    //Expects the values in properties to be correctly escaped (if needed)
    //Output format: "objectName":{"key1":value1,"key2":value2,...}
    //If objName is null will return {"key1":value1,"key2":value2,...}
    private string ToJSONObject(string objName, Dictionary<string, string> properties) {
        string result = "";

        if (objName != null) {
            result += EscapeString(objName) + ":";
        }
        result += "{";
        

        foreach (KeyValuePair<string, string> value in properties) {
            result += EscapeString(value.Key) + ":" + value.Value + ",";
        }
        result = result.Remove(result.Length - 1);

        result += "}";

        return result;
    }

    //Convert the given string to a json array
    //Expects the values in properties to be correctly escaped (if needed)
    //Output format: "arrayName":[value1,value2,....]
    //If arrayName is null will return [value1,value2,....]
    private string ToJSONArray(string arrayName, List<string> values) {
        string result = "";

        if (arrayName != null) {
            result += EscapeString(arrayName) + ":";
        }
        result += "[";

        foreach (string s in values) {
            result += s + ",";
        }
        result = result.Remove(result.Length - 1);

        result += "]";

        return result;
    }
}
