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

public class centreCamera : MonoBehaviour {

    //private GameObject mainCamera;
    Color initialColour;
    bool isBeingTouched = false;

    DragToChangePosition changePosController;

    private bool isMissionControl = false;

    // Start is called before the first frame update
    void Start() {
        UpdateIsMissionControl();

        initialColour = gameObject.GetComponent<SpriteRenderer>().color;
        //mainCamera = Camera.main.gameObject;

        gameObject.SetActive(isMissionControl);

        DragToChangePosition[] results = Resources.FindObjectsOfTypeAll<DragToChangePosition>();
        if (results.Length > 0) {
            changePosController = results[0];
        }

    }

    public bool getTouched() {
        return isBeingTouched;
    }

    protected void OnMouseDown() {
        //mainCamera.GetComponent<DragToChangePosition>().resetCameraPosition();
        isBeingTouched = true;
        changePosController.resetCameraPosition();
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }

    protected void OnMouseUp() {
        gameObject.GetComponent<SpriteRenderer>().color = initialColour;
        isBeingTouched = false;
    }

    private void UpdateIsMissionControl() {
        if (NetworkingManager.Singleton.LocalClientId == ConnectionManager.Singleton.GetMissionControlID()) {
            isMissionControl = true;
        } else {
            isMissionControl = false;
        }
        //Debug.Log("Is Mission Control: " + isMissionControl.ToString());
    }
}
