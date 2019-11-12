/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using UnityEngine;

//Testing non-networked Fling-able scene objects
public class Flingable : Draggable {

    //how many world units this can be dragged
    public float maxDistance = 3;

    private Rigidbody2D rb2d;

    private Vector2 initialPos;

    // Start is called before the first frame update
    new void Start() {
        //TODO: Check ux-yness of this - will this mess with initial object placement?
        initialPos = gameObject.transform.position;
        rb2d = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    override protected void Update() {

        //call superclass methods
        base.Update();

        Vector2 currentPos = gameObject.transform.position;

        Debug.Log((currentPos - initialPos).magnitude);

        if (currentPos != initialPos) {
            if (!isActive) {
                float distance = (currentPos - initialPos).magnitude;
                if (distance > maxDistance) {
                    //then let go at max speed
                    fling((currentPos - initialPos).normalized * maxDistance);
                }

                fling(currentPos);
                //it has been let go - let go proportionate to distance.

            }
        }
    }

    /// <summary>
    /// Accelerates the object in a specified direction
    /// </summary>
    /// <param name="position"> The direction that the object will be 'flung' in </param>
    private void fling(Vector2 position) {
        //rapidly accelerates based on input
        SetIsActive(false);
        rb2d.AddForce(position, ForceMode2D.Impulse);

        Vector2 currentPos = gameObject.transform.position;



    }
}
