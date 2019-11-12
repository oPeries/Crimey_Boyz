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

public class endScoreUIController : MonoBehaviour
{
    public GameObject score;
    public GameObject pname;
    SpriteRenderer sprite;
    public GameObject pPlace;

    private void Awake() {
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    public void setPlace(int place) {

        string output = place.ToString();

        switch (place)
        {
            case 1:
                output += "st";
                break;
            case 2:
                output += "nd";
                break;
            case 3:
                output += "rd";
                break;
            default:
                output += "th";
                break;
        }
       
        pPlace.GetComponent<TextMeshPro>().text = output;
    }

    public GameObject getScore() { return score; }

    public SpriteRenderer getSprite() { return sprite; }

    public GameObject getPName() { return pname; }

    public void setColours(int pNum) {

            Color32[] playerColours = {
            new Color32(240, 65, 65, 255),
            new Color32(58, 107, 242, 255),
            new Color32(237, 223, 69, 255),
            new Color32(110, 219, 88, 255),
            new Color32(247, 125, 2, 255)
        };

        getScore().GetComponent<TextMeshPro>().color = playerColours[pNum - 1];
        getPName().GetComponent<TextMeshPro>().color = playerColours[pNum - 1];
    }
}
