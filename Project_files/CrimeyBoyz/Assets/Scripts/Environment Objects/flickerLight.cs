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
using MLAPI.NetworkedVar;

public class flickerLight : MonoBehaviour
{
    //flickers 2D lights by changing their scale

    public float scaleChange = 0.1f;
    public float timeToFlicker = 0.25f;

    NetworkedVar<float> nwScaleChange = new NetworkedVar<float>();
    NetworkedVar<float> nwTimeToFlicker = new NetworkedVar<float>();

    float timer = 0;
    Vector3 origScale;

    // Start is called before the first frame update
    void Start()
    {
        nwScaleChange.Value = scaleChange;
        nwTimeToFlicker.Value = timeToFlicker;
        origScale = gameObject.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > nwTimeToFlicker.Value) {
            gameObject.transform.localScale += (origScale * nwScaleChange.Value);
            //invert scaleChange
            nwScaleChange.Value = nwScaleChange.Value * -1;

            timer = 0;
        }
    }
}
