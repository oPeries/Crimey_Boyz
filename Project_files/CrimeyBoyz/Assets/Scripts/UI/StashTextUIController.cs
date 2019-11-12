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
using TMPro;

public class StashTextUIController : MonoBehaviour
{

    //Small controller for the stash display

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().sortingOrder = 6;
        gameObject.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void writeToStash(int value) {
        if (value <= 0)
        {
            darken();
            value = 0;
        }
        else {
            brighten();
        }

        string newText = value.ToString();
        gameObject.GetComponent<TextMeshPro>().text = newText;
    }

    //darkens the stash indicator background
    public void darken() {
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
    }

    //brightens the stash indicator background
    public void brighten() {
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
    
}
