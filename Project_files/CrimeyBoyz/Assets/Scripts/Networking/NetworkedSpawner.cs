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
using MLAPI.Messaging;
using MLAPI.NetworkedVar;

//Object that enables a client spawn networked objects in the game
//Behaviour:
// - On the client side: click and drag this object for a networked object to spawn where the player lets go of the mouse / finger
// - On all other clients: a newly spawned object appears, owned by this player
public class NetworkedSpawner : NetworkedBehaviour {

    public GameObject prefabToSpawn; //prefab to spawn upon clicking & dragging. Note: this prefab must be added to the spawnable prefabs list in the NetworkingManager

    private bool parentSet = false; //Track if this object has been set to a 
    private bool addedToCameraDragChecker = false;
    private bool isBeingDragged = false; //true when being dragged
    private Vector2 mousePosOffset; //offset between mouse click and the centre of the object. Used for dragging
    private GameObject localPrefabInstance = null; //
    private NetworkedSpawnerUIController UIController;

    NetworkedVar<int> stashedAmount = new NetworkedVar<int>();
    public int stashedAmountLimit = 3;
    //private bool isSpawningDisabled = false;

    public float cooldownTime = 3;
    bool isCoolingDown;
    float cooldownTimer = 0;

    private void Start()
    {
        stashedAmount.Settings.WritePermission = NetworkedVarPermission.ServerOnly;
        stashedAmount.OnValueChanged = stashAmountChanged;

        UIController = gameObject.GetComponent<NetworkedSpawnerUIController>();

        stashedAmount.Value = 0;
        UIController.setStashAmount(0);

        //this MUST be called
        startCooldown();
    }

    

    //When being dragged, set the transform pos of the spawned object to be under the cursor (adjusted for mousedown offset)
    void Update() {

        if (isCoolingDown) {

            cooldownTimer += Time.deltaTime;
            UIController.changeCooldownPosition(cooldownTimer/cooldownTime);
            

            if (cooldownTimer > cooldownTime) {
                if (IsServer) {
                    stashedAmount.Value += 1;

                    if (stashedAmount.Value < stashedAmountLimit) {
                        InvokeClientRpcOnEveryone(startCooldown);
                    } else {
                        isCoolingDown = false;
                    }

                } else {

                    isCoolingDown = false;
                    //cooldownTimer = 0;
                }
            }
        }

        if (!addedToCameraDragChecker) {
            //DragToChangePosition dragger = Camera.main.GetComponent<DragToChangePosition>();

            //if (dragger != null) { 
            //    dragger.SetNetworkedSpawner(this);
            //    addedToCameraDragChecker = true;
            //}

            DragToChangePosition[] results = Resources.FindObjectsOfTypeAll<DragToChangePosition>();
            if (results.Length > 0) {
                results[0].SetNetworkedSpawner(this);
                addedToCameraDragChecker = true;
            }
        }

        if(IsOwner && !parentSet) {
            GameObject spawners = GameObject.Find("Spawners");
            if(spawners != null) {
                transform.SetParent(spawners.transform);
                parentSet = true;
            }
            
        }

        if (isBeingDragged && IsOwner) {
            //Spawn the prefab (this client only, for the purposes of displaying where it will be placed)
            if(localPrefabInstance == null) {
                localPrefabInstance = Instantiate(prefabToSpawn);
                if (localPrefabInstance.GetComponent<NetworkedDespawn>()) {
                    localPrefabInstance.GetComponent<NetworkedDespawn>().changeColourOfChildren(Color.white);
                }
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            Vector2 adjustedPos = mousePos2D + mousePosOffset;
            localPrefabInstance.transform.position = new Vector3(adjustedPos.x, adjustedPos.y, localPrefabInstance.transform.position.z);

            //Note: this is where checking for collisions could be done
        }
    }

    /// <summary>
    /// When moused down, enable dragging (only if a prefab has been specified)
    /// </summary>
    private void OnMouseDown() {
        if(!IsOwner) {
            return;
        }

        if(prefabToSpawn == null) {
            return;
        }

        if (stashedAmount.Value > 0)
        {
            //Note: instead of immediately setting isBeingDragged, could start a timer that will enabled isBeingDragged once player has held down for a specified amount of time
            isBeingDragged = true;
        }
        else {
            UIController.flashRed();
        }
        
        //Calculate the difference between the transform pos and the actual moused down pos (This will stop the object from snapping to the centre of the mouse when dragged)
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        Vector2 transformPos2D = new Vector2(transform.position.x, transform.position.y);
        mousePosOffset = transformPos2D - mousePos2D;
    }

    /// <summary>
    /// When mouse is released, request that the server spawn a new object at the position of the locally spawned object. 
    /// Delete the locally spawned object (server will re-create)
    /// </summary>
    private void OnMouseUp() {
        if (!IsOwner) {
            return;
        }

        if (isBeingDragged) {
            //Called on the client but executed on the server
            //Vector3 pos = new Vector3(localPrefabInstance.transform.position.x, localPrefabInstance.transform.position.y, localPrefabInstance.transform.position.z);
            InvokeServerRpc(RequestSpawnObject, localPrefabInstance.transform.position);

            Destroy(localPrefabInstance);
            localPrefabInstance = null;
        }

        isBeingDragged = false;
    }

    /// <summary>
    /// If not the owner, change the position of this object to match its new location. 
    /// The owner is the one to set the synced pos, assume it is correct on the owner side
    /// </summary>
    /// <param name="previous"> The previous position </param>
    /// <param name="updated"> The updated position </param>
    void OnSyncedPosChanged(Vector2 previous, Vector2 updated) {
        if (!IsOwner) {
            transform.position = new Vector3(updated.x, updated.y, transform.position.z);
        }
    }

    /// <summary>
    /// Called on the client but executed on the server
    /// </summary>
    /// <param name="position"></param>
    [ServerRPC(RequireOwnership = false)]
    void RequestSpawnObject(Vector3 position) {
        //Note: checking could be done here to ensure the given position of the object is valid and wont break things
        
        //Spawn in scene (initially server scene only with this command)
        if (stashedAmount.Value > 0)
        {
            GameObject newObject = Instantiate(prefabToSpawn);
            newObject.transform.position = position;

            //Spawn over the network
            newObject.GetComponent<NetworkedObject>().Spawn(destroyWithScene: true); //by default spawns with the server as the owner

            stashedAmount.Value -= 1;
            //if there isn't already a cooldown in effect then start a cooldown
            if (!isCoolingDown) {
                InvokeClientRpcOnEveryone(startCooldown);
            }
        }
    }

    [ClientRPC]
    void startCooldown() {
        cooldownTimer = 0;
        UIController.resetMask();
        isCoolingDown = true;
    }

    //Called on all clients (including the server) when the stach amount is changed
    void stashAmountChanged(int previous, int updated) {
        if (UIController != null) {
            UIController.setStashAmount(updated);
        }
    }

    public bool getIsBeingDragged() {
        //gets if the player is using the networked spawner - i.e if they're dragging something into the screen or touching the spawner
        return isBeingDragged;
    }
}
