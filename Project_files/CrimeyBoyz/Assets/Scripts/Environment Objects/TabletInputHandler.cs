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

public class TabletInputHandler : MonoBehaviour
{

    Camera thisCamera;
    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("Load Tablet");
        thisCamera = this.gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            //Vector2 touchPos = 

            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Ray touchRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            //RaycastHit2D hit;

            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                //touch on
                //handle collisions
                {

                    // Do something with the object that was hit by the raycast.
                }

            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                //touch on
            }
            

            if (Input.GetTouch(0).phase == TouchPhase.Moved) {
                //move
            }

        }
    }


}
