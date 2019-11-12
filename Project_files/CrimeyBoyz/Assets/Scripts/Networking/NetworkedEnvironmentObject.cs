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
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.Messaging;

//Add to any environment object to be networked
//Manages the attached object's ownership
//Can set the object such that it is visible to the owner only
//Used to sync the object's initial position in the world
public class NetworkedEnvironmentObject : NetworkedBehaviour {

    public bool isOwnedByTablet = true; //set to have the owner of this object as the tablet player (when calling ResetOwnership)
    public bool disableChildrenWhenNotOwned = false; //if true, all children of this game object will be disabled when not the owner

    private NetworkedVar<Vector3> syncedPosition; //synced position of this object (currently only synced when spawned) - server has authority of this one
    public bool childrenEnabled = true;

    void Start() {
        syncedPosition = new NetworkedVar<Vector3>();
        syncedPosition.Settings.WritePermission = NetworkedVarPermission.ServerOnly;
        syncedPosition.OnValueChanged = OnPositionChange;

        setChildrenState(true);

        if (GetComponent<NetworkedObject>().IsSpawned) {
            InvokeServerRpc(ResetOwnership);
        }
    }

    // Update is called once per frame
    void Update() {
        if(disableChildrenWhenNotOwned) {
            if(IsOwner && !childrenEnabled) {
                setChildrenState(true);

            }

            if (!IsOwner && childrenEnabled) {
                setChildrenState(false);
            }
        }
    }

    /// <summary>
    /// Sets the state of an object's children
    /// </summary>
    /// <param name="enabled"> updated state </param>
    private void setChildrenState(bool enabled) {
        //Debug.Log("Set Children State: " + enabled.ToString());
        foreach(Transform child in transform) {
            child.gameObject.SetActive(enabled);
        }
        childrenEnabled = enabled;
    }


    /// <summary>
    /// Update the synced position (server only)
    /// </summary>
    public void UpdatePosition() {
        if (IsServer) {
            syncedPosition.Value = transform.position;
        } else {
            Debug.LogWarning("Tried to sync environment object's position when not the server");
        }
    }

    /// <summary>
    /// If not the server, snap to the updated position
    /// </summary>
    /// <param name="previous"> Previous position </param>
    /// <param name="updated"> Updated position </param>
    private void OnPositionChange(Vector3 previous, Vector3 updated) {

        if(!IsServer) {
            transform.position = updated;
        }
    }

    /// <summary>
    /// Reset the ownership of this object (server side call only). 
    /// If this object is not to be owned by the tablet, will be owned by the server
    /// </summary>
    [ServerRPC(RequireOwnership = false)]
    public void ResetOwnership() {
        if(!IsServer) {
            Debug.LogWarning("Tried to reset object's ownership as a client");
            return;
        } else if(!GetComponent<NetworkedObject>().IsSpawned) {
            return; //If this object is not yet spawned, do nothing
        }

        if(isOwnedByTablet) {
            GetComponent<NetworkedObject>().ChangeOwnership(ConnectionManager.Singleton.GetMissionControlID());
        } else {
            GetComponent<NetworkedObject>().ChangeOwnership(NetworkingManager.Singleton.ServerClientId);
        }
    }
}
