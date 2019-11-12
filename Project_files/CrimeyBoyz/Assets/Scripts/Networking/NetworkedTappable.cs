/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using MLAPI;
using MLAPI.NetworkedVar;
using UnityEngine;

//Add to an object to enable it being tapped / click (Works with both mouse or finger depending on device)
//If this instance of the object is the network owner, will detect being tapped and set itself as "active" when clicked and no "active" when not clicked
public class NetworkedTappable :  NetworkedBehaviour {

    public delegate void OnActiveChangedCallback (bool isActive); //Callback for when isActive status changes

    public OnActiveChangedCallback onActiveCallback;

    private NetworkedVar<bool> isActive = new NetworkedVar<bool>(); //Synced between the server and client


    //Initiate networked var
    private void Awake() {

        isActive.Settings.WritePermission = NetworkedVarPermission.OwnerOnly; //Owner is the only entity that can change the active status of this object
        isActive.OnValueChanged = UpdateActiveStatus;

    }
    

    private void Update()
    {
       

    }

    /// <summary>
    /// Callback function for when the synced var is changed. Called on all clients (including the server)
    /// </summary>
    /// <param name="previous"> Previous state of the object </param>
    /// <param name="updated"> Updated state of the object </param>
    private void UpdateActiveStatus(bool previous, bool updated) {
        onActiveCallback?.Invoke(updated); //Call the delegate if it has been specified

    }


    /// <summary>
    /// Activate the object when tapped (owner only)
    /// </summary>
    protected void OnMouseDown() {

        Debug.Log("tap");

        if (IsOwner) {
            isActive.Value = true;
        }
    }

    /// <summary>
    /// Deactivate the object when mouse release (owner only)
    /// </summary>
    protected void OnMouseUp() {
        if (IsOwner) {
            isActive.Value = false;
        }
    }

}
