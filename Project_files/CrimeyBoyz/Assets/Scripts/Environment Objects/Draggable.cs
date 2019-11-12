/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using UnityEngine;

//Testing non-networked drag-able scene objects
public class Draggable : Touchable {

    bool isEntered;
    Vector2 enteredPoint;

    // Update is called once per frame
    new virtual protected void Update() {

        HandleEnteredCollisions(isEntered);
        //non-touch input
        if (isActive) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosFlattened = new Vector2(mousePos.x, mousePos.y);

            gameObject.transform.position = mousePosFlattened;
        }

        if (isEntered && !isActive) {
            gameObject.GetComponentsInChildren<Renderer>()[0].material.SetColor("_Color", Color.red);
            gameObject.transform.position = enteredPoint;
            isEntered = false;
            gameObject.GetComponentsInChildren<Renderer>()[0].material.SetColor("_Color", Color.white);
        }

    }

    /// <summary>
    /// Check if the draggable is being dragged into a position that it won't fit in
    /// </summary>
    /// <param name="collision"> The object the draggable is being dragged into </param>
    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        //should do a check if the object is static

        if (!isEntered) {

            Debug.Log("RECALC");

            Vector2 currentPos = gameObject.transform.position;
            Vector2 collidedPos = collision.GetContact(0).point;
            //TODO: CLEANUP MATHS SO IT WORKS IN ALL DIRECTIONS
            enteredPoint = currentPos + (currentPos - collidedPos) * 0.05f;

        }

        HandleEnteredCollisions(true);

        //may need to remove depending on implementation
        //freeze rotation and turn red
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
        gameObject.GetComponentsInChildren<Renderer>()[0].material.SetColor("_Color", Color.red);
    }

    /// <summary>
    /// Function for if the draggable is being held on another object
    /// </summary>
    /// <param name="collision"> The object being held over </param>
    protected virtual void OnCollisionStay2D(Collision2D collision) {
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    /// <summary>
    /// Function for if the draggable is being dragged away from another object
    /// </summary>
    /// <param name="collision"> The collider object being dragged away from </param>
    protected virtual void OnCollisionExit2D(Collision2D collision) {

        //SetIsActive(false);
        if (!isEntered) {
            gameObject.GetComponentsInChildren<Renderer>()[0].material.SetColor("_Color", Color.green);
            gameObject.transform.position = enteredPoint;
        }

        if (!isActive) {
            HandleEnteredCollisions(false);
        }
    }
    
    /// <summary>
    /// Sets if the draggable object handles enter collisions
    /// </summary>
    /// <param name="set"> Value for if the object should (true) or shouldn't (false) handle enter collisions </param>
    private void HandleEnteredCollisions(bool set) {
        isEntered = set;
    }
}
