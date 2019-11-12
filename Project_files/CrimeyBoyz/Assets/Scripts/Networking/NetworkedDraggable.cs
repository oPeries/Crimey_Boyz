/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;

//Add to a game object to enabled it to be dragged (in world space) by its network owner
//Behaviour:
//  - On the owners screen, holding down on the object and moving the cursor drags the object with the cursor. Then when letting go, the object stays in that new position
//  - On all other clients, the object snaps its new position once the owner has moved it and released their cursor
//Note: only tested with 2D projects
public class NetworkedDraggable : NetworkedBehaviour {

    private NetworkedVar<Vector2> syncedPos = new NetworkedVar<Vector2>(); //Synced between the server and client

    private bool isBeingDragged = false; //true when being dragged
    private Vector2 mousePosOffset; //offset between mouse click and the centre of the object. Used for dragging

    //Initialise the networked var
    void Start() {
        syncedPos.Settings.WritePermission = NetworkedVarPermission.OwnerOnly; //Owner is the only entity that can change the active status of this object
        syncedPos.OnValueChanged = OnSyncedPosChanged;

        //Initialise the position of this object
        if(IsOwner) {
            syncedPos.Value = new Vector2(transform.position.x, transform.position.y);
        }
    }

    /// <summary>
    /// When being dragged (owner only), set the transform pos to be under the cursor (adjusted for mousedown offset)
    /// </summary>
    void Update() {
        if(isBeingDragged && IsOwner) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            Vector2 adjustedPos = mousePos2D + mousePosOffset;
            transform.position = new Vector3(adjustedPos.x, adjustedPos.y, transform.position.z);

            //Note: this is where checking for collisions could be done
        }
    }

    /// <summary>
    /// When moused down, enable dragging (owner only)
    /// </summary>
    private void OnMouseDown() {
        //Note: instead of immediately setting isBeingDragged, could start a timer that will enabled isBeingDragged once player has held down for a specified amount of time
        if (IsOwner) {
            isBeingDragged = true;

            //Calculate the difference between the transform pos and the actual moused down pos (This will stop the object from snapping to the centre of the mouse when dragged)
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            Vector2 transformPos2D = new Vector2(transform.position.x, transform.position.y);
            mousePosOffset = transformPos2D - mousePos2D;
        }
    }

    /// <summary>
    /// When mouse is released, the new dragged-to position should be synced across all clients (owner only)
    /// </summary>
    private void OnMouseUp() {
        if(IsOwner) {
            isBeingDragged = false;
            syncedPos.Value = new Vector2(transform.position.x, transform.position.y);
        }
    }

    /// <summary>
    /// If not the owner, change the position of this object to match its new location. 
    /// The owner is the one to set the synced pos, assume it is correct on the owner side
    /// </summary>
    /// <param name="previous"> Previous position </param>
    /// <param name="updated"> Updated position </param>
    void OnSyncedPosChanged(Vector2 previous, Vector2 updated) {
        if(!IsOwner) {
            transform.position = new Vector3(updated.x, updated.y, transform.position.z);
        }
    }
}
