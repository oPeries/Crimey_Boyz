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
using Cinemachine;

public class DragToChangePosition : MonoBehaviour {

    public Vector2 xAxisBounds = new Vector2(-40, 40); //transform will not exceed these limits

    Vector3 mouseDownOrigin;
    Vector3 cameraOrigin;
    Vector3 trackerOrigin;

    Vector2 mouseOrigin;
    Vector2 touchOrigin;
    //float CameraWidth;
    //float CameraHeight;
    public float sensitivity = 2;
    public bool enableMouseTesting = false;
    private NetworkedSpawner spawner = null;
    public GameObject resetCameraButton;
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera missionControlCamera;
    public FollowPlayers playerTracker;

    bool isFollowingPlayer = false;

    private bool isMissionControl;

    // Start is called before the first frame update
    void Start() {
        UpdateIsMissionControl();

        //CameraWidth = Screen.width / 2.0f;
        //CameraHeight = Screen.height / 2.0f;
    }

    private void OnEnable() {
        UpdateIsMissionControl();
        resetCameraPosition();
    }

    // Update is called once per frame
    void Update() {
        if (isMissionControl && spawner != null) {
            //Mouse spoofing for testing
            if (enableMouseTesting) {
                CheckMouseDrag();
            }

            CheckTouchDrag();
        }

        if(isFollowingPlayer) {
            transform.position = playerTracker.transform.position;
        }
    }

    public void SetNetworkedSpawner(NetworkedSpawner spawner) {
        this.spawner = spawner;
    }

    public void resetCameraPosition() {
        //reset the camera's position to the default tracker
        transform.position = playerTracker.transform.position;
        missionControlCamera.enabled = false;
        playerCamera.enabled = true;
        isFollowingPlayer = true;
        //touchOrigin = new Vector2(0, 0);

    }

    private void CheckMouseDrag() {
        if (Input.mousePresent
                        && !spawner.getIsBeingDragged()
                        && !resetCameraButton.GetComponent<centreCamera>().getTouched()) {


            //in screen (pixel) units! need very strong sensitivity scaling
            Vector2 mouse = Input.mousePosition;

            if (Input.GetMouseButtonDown(0)) {
                isFollowingPlayer = false;
                mouseOrigin = Input.mousePosition;
                mouseDownOrigin = Camera.main.ScreenToWorldPoint(mouseOrigin);
                cameraOrigin = Camera.main.transform.position;
                trackerOrigin = transform.position;

                if (!missionControlCamera.enabled) {
                    transform.position = playerTracker.transform.position;
                    playerCamera.enabled = false;
                    missionControlCamera.enabled = true;
                }
            }

            if (Input.GetMouseButton(0) && mouse != mouseOrigin) {

                //Vector2 direction = (mouseOrigin - mouse) / CameraWidth;
                //gameObject.transform.Translate(new Vector2(direction.x * sensitivity, 0));

                Vector3 cameraDiff = Camera.main.transform.position - cameraOrigin;
                Vector3 mouseDiff = Camera.main.ScreenToWorldPoint(mouse) - mouseDownOrigin;
                Vector3 dragDiff = mouseDiff - cameraDiff;
                float newXPos = trackerOrigin.x - dragDiff.x;

                if (newXPos < xAxisBounds.x) {
                    newXPos = xAxisBounds.x;
                } else if (newXPos > xAxisBounds.y) {
                    newXPos = xAxisBounds.y;
                }

                transform.position = new Vector3(newXPos, transform.position.y, transform.position.z);

            }

            //if (Input.GetMouseButtonUp(0)) {
            //    mouseOrigin = new Vector2(0, 0);
            //}

        }
    }

    private void CheckTouchDrag() {
        if (Input.touchCount > 0
                    && !spawner.getIsBeingDragged()
                    && !resetCameraButton.GetComponent<centreCamera>().getTouched()) {

            //in screen (pixel) units! need very strong sensitivity scaling
            Touch touch = Input.GetTouch(0);


            if (touch.phase == TouchPhase.Began) {
                isFollowingPlayer = false;
                touchOrigin = touch.position;
                mouseDownOrigin = Camera.main.ScreenToWorldPoint(touchOrigin);
                cameraOrigin = Camera.main.transform.position;
                trackerOrigin = transform.position;

                if (!missionControlCamera.enabled) {
                    transform.position = playerTracker.transform.position;
                    playerCamera.enabled = false;
                    missionControlCamera.enabled = true;
                }
            }

            if (touch.phase == TouchPhase.Moved) {
                //Vector2 direction = (touchOrigin - touch.position) / CameraWidth;
                //gameObject.transform.Translate(new Vector2(direction.x * sensitivity, 0));

                Vector3 cameraDiff = Camera.main.transform.position - cameraOrigin;
                Vector3 mouseDiff = Camera.main.ScreenToWorldPoint(touch.position) - mouseDownOrigin;
                Vector3 dragDiff = mouseDiff - cameraDiff;
                float newXPos = trackerOrigin.x - dragDiff.x;

                if(newXPos < xAxisBounds.x) {
                    newXPos = xAxisBounds.x;
                } else if(newXPos > xAxisBounds.y) {
                    newXPos = xAxisBounds.y;
                }

                transform.position = new Vector3(newXPos, transform.position.y, transform.position.z);

            }

            //if (touch.phase == TouchPhase.Ended) {
            //    touchOrigin = new Vector2(0, 0);
            //}

        }
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
