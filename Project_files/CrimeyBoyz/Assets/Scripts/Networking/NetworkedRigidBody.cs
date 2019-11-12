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

//Add to any environment object to be networked
//Synchronises the attached RigidBody over the network
public class NetworkedRigidBody : NetworkedBehaviour {

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //Settings
    public float sendRate = 10f; //Max #times per second to update position data

    public float movementThreshold = 0.001f; //Delta before values are updated over the network

    public float posSnapThreshold = 0.5f; //snap directly to the target position if further away than this value

    public float interpolateMovement = 1; //degree to interpolate between movement values (0 <= snap)

    public float interpolateRotation = 1; //degree to interpolate between angle values (0 <= snap)

    public bool isRigidBody2D = false; //set to true to sync attached RigidBody2D instead

    //Attached Body to Sync
    private Rigidbody attachedRigidbody = null; //The rigid body to synchronise
    private Rigidbody2D attachedRigidbody2D = null; //The rigid body to synchronise

    //Synced values (Same between server & all clients)
    private NetworkedVar<Vector3> syncedPos = new NetworkedVar<Vector3>();
    private NetworkedVar<Vector3> syncedRotation = new NetworkedVar<Vector3>();
    private NetworkedVar<Vector3> syncedVelocity = new NetworkedVar<Vector3>();

    //Previous values (for interpolation)
    private Vector3 previousPos;
    private Quaternion previousRotation;
    private float previousVelocity;

    //Projected values (for interpolation)
    private Vector3 targetPos;
    private Quaternion targetRotation;
    private Vector3 targetVelocity;

    private float lastOwnerSendTime; //last time the owner updated the synced var

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Behavioural ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //Initialise
    private void Awake() {
        //
        if (isRigidBody2D) {
            attachedRigidbody2D = GetComponent<Rigidbody2D>();
        } else {
            attachedRigidbody = GetComponent<Rigidbody>();
        }

        //Initial pos and rotation values
        previousPos = transform.position;
        previousRotation = transform.rotation;
        previousVelocity = 0;

        //Setup synced vars
        syncedPos.Settings.WritePermission = NetworkedVarPermission.OwnerOnly;
        syncedRotation.Settings.WritePermission = NetworkedVarPermission.OwnerOnly;
        syncedVelocity.Settings.WritePermission = NetworkedVarPermission.OwnerOnly;

        syncedPos.OnValueChanged = UpdateTargetPos;
        syncedRotation.OnValueChanged = UpdateTargetRotation;
        syncedVelocity.OnValueChanged = UpdateTargetVelovity;

        lastOwnerSendTime = 0.0f;
    }

    //If send rate is 0, send the values once
    void Start() {
        if (sendRate <= 0.0) {
            SetSyncedVars();
        }
    }

    //Interpolate at fixed update times
    private void FixedUpdate() {
        if (!IsOwner) {
            Interpolate();
        }
    }

    //Update the synced variables if enough time has passed. (Only the owner can update pos)
    void Update() {
        if (!IsOwner || sendRate <= 0.0) {
            return;
        }
        if ((Time.time - lastOwnerSendTime) >= (1 / sendRate)) {
            if (HasMoved()) {
                SetSyncedVars();
            }
            lastOwnerSendTime = Time.time;
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /// <summary>
    /// Interpolate between the current position and the target position
    /// </summary>
    private void Interpolate() {
        if (interpolateMovement != 0.0) {
            if (isRigidBody2D) {
                attachedRigidbody2D.velocity = ((Vector2)targetPos - attachedRigidbody2D.position) * interpolateMovement * sendRate;
            } else {
                attachedRigidbody.velocity = (targetPos - attachedRigidbody.position) * interpolateMovement * sendRate;
            }
        }
        if (interpolateRotation != 0.0) {
            if (isRigidBody2D) {
                //Rotate 2D about z axis only
                attachedRigidbody2D.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 0.0f, targetRotation.z), Time.fixedDeltaTime * interpolateRotation).eulerAngles.z);
            } else {
                attachedRigidbody.MoveRotation(Quaternion.Slerp(attachedRigidbody.rotation, targetRotation, Time.fixedDeltaTime * interpolateRotation));
            }
        }
        targetPos += targetVelocity * Time.fixedDeltaTime * 0.1f;
    }

    /// <summary>
    /// Gets if an objects values have changed enough to be worth sending over the network
    /// </summary>
    /// <returns>
    /// Return true if the object's values have changed enough to be worth sending over the network
    /// </returns>
    private bool HasMoved() {

        if (isRigidBody2D) {

            if((attachedRigidbody2D.position - (Vector2) previousPos).magnitude >= movementThreshold) {
                return true;
            } else if (Mathf.Abs(attachedRigidbody2D.rotation - previousRotation.z) >= movementThreshold) {
                return true;
            } else if (Mathf.Abs(attachedRigidbody2D.velocity.sqrMagnitude - previousVelocity) >= movementThreshold) {
                return true;
            }

        } else {

            if ((attachedRigidbody.position - previousPos).magnitude >= movementThreshold) {
                return true;
            } else if (Quaternion.Angle(attachedRigidbody.rotation, previousRotation) >= movementThreshold) {
                return true;
            } else if (Mathf.Abs(attachedRigidbody.velocity.sqrMagnitude - previousVelocity) >= movementThreshold) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Set the synced variables to reflect the current state
    /// </summary>
    private void SetSyncedVars() {
        if (isRigidBody2D) {
            syncedPos.Value = attachedRigidbody2D.position;
            syncedRotation.Value = new Vector3(0.0f, 0.0f, attachedRigidbody2D.rotation);
            syncedVelocity.Value = attachedRigidbody2D.velocity;

            previousPos = attachedRigidbody2D.position;
            previousRotation.z = attachedRigidbody2D.rotation;
            previousVelocity = attachedRigidbody2D.velocity.sqrMagnitude;

        } else {
            syncedPos.Value = attachedRigidbody.position;
            syncedRotation.Value = attachedRigidbody.rotation.eulerAngles;
            syncedVelocity.Value = attachedRigidbody.velocity;

            previousPos = attachedRigidbody.position;
            previousRotation = attachedRigidbody.rotation;
            previousVelocity = attachedRigidbody.velocity.sqrMagnitude;
        }
    }

    /// <summary>
    /// Called whenever the "syncedPos" variable is changed (both locally and over the network)
    /// </summary>
    /// <param name="previous"> Previous position of the target </param>
    /// <param name="updated"> Updated position of the target </param>
    private void UpdateTargetPos(Vector3 previous, Vector3 updated) {
        targetPos = updated;
        if (IsOwner) {
            return;
        }
        if (IsServer && !IsClient) {
            //Server doesn't care about smooth movement
            if (isRigidBody2D) {
                attachedRigidbody2D.MovePosition((Vector2) targetPos);
            } else {
                attachedRigidbody.MovePosition(targetPos);
            }
        } else if (interpolateMovement <= 0.0) {
            //Snap if interpolate disabled
            if (isRigidBody2D) {
                attachedRigidbody2D.position = (Vector2)targetPos;
            } else {
                attachedRigidbody.position = targetPos;
            }
        }
    }

    /// <summary>
    /// Called whenever the "syncedRotation" variable is changed (both locally and over the network)
    /// </summary>
    /// <param name="previous"> The target's previous rotation </param>
    /// <param name="updated"> The target's updated rotation </param>
    private void UpdateTargetRotation(Vector3 previous, Vector3 updated) {
        targetRotation.eulerAngles = updated;
        if (IsOwner) {
            return;
        }
        if (IsServer && !IsClient) {
            //Server doesn't care about smooth movement
            if (isRigidBody2D) {
                attachedRigidbody2D.MoveRotation(targetRotation.z);
            } else {
                attachedRigidbody.MoveRotation(targetRotation);
            }
        } else if (interpolateRotation <= 0.0) {
            //Snap if interpolate disabled
            if (isRigidBody2D) {
                attachedRigidbody2D.rotation = targetRotation.z;
            } else {
                attachedRigidbody.rotation = targetRotation;
            }
        }
    }

    /// <summary>
    /// Called whenever the "syncedVelocity" variable is changed (both locally and over the network)
    /// </summary>
    /// <param name="previous"> The target's previous velocity </param>
    /// <param name="updated"> The target's updated velocity </param>
    private void UpdateTargetVelovity(Vector3 previous, Vector3 updated) {
        targetVelocity = updated;
        if (IsOwner) {
            return;
        }
        if (isRigidBody2D) {
            attachedRigidbody2D.velocity = (Vector2)updated;
        } else {
            attachedRigidbody.velocity = updated;
        }
    }
}
