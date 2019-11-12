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

public class SyncObjectScale : NetworkedBehaviour {

    private NetworkedVar<Vector3> syncedScale = new NetworkedVar<Vector3>();

    private bool scaleSynced = true;

    // Start is called before the first frame update
    void Start() {

        syncedScale.Settings.WritePermission = NetworkedVarPermission.OwnerOnly;
        syncedScale.Settings.SendTickrate = 4;
        syncedScale.OnValueChanged += OnScaleSchanged;

        if (IsOwner) {
            scaleSynced = false;
        }
    }

    // Update is called once per frame
    void Update() {
        if(!scaleSynced) {
            if(IsOwner && NetworkedObject.IsSpawned) {
                syncedScale.Value = transform.localScale;
                scaleSynced = true;
            }
        }
    }

    private void OnScaleSchanged(Vector3 previous, Vector3 updated) {
        if(!IsOwner) {
            transform.localScale = updated;
        }
    }


}
