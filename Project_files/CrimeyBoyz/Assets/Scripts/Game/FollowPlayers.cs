/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using System.Collections.Generic;
using UnityEngine;
using System;

//Attach to an object (E.g. the Camera) to enable it to follow the players as they move
public class FollowPlayers : MonoBehaviour {
    
    public List<Transform> players = new List<Transform>(); //players to follow with this camera
    //public float smoothTime = 0.5f;

    public Vector2 xAxisBounds = new Vector2(float.MinValue, float.MaxValue); //transform will not exceed these limits

    private Vector3 velocity; //Used by SmoothDamp to track velocity between updates
    private bool recentlyChanged = false; //true if a player has been recently added/removed (will smooth the movement)

    private void Start() {
    }

    /// <summary>
    /// Update the position of this game object (with camera attached) to be the centre point of all players. Currently only follows on the x axis
    /// </summary>
    private void Update() {

        if (players.Count == 0) {
            return;
        }

        moveToPlayers();
    }

    public void moveToPlayers() {
        Vector3 centre = getCentrePont();
        if (centre.x < xAxisBounds.x) {
            centre.x = xAxisBounds.x;
        }
        if (centre.x > xAxisBounds.y) {
            centre.x = xAxisBounds.y;
        }
        centre.y = transform.position.y;
        centre.z = transform.position.z;

        if (!recentlyChanged) {
            transform.position = centre;

        } else {
            transform.position = Vector3.SmoothDamp(transform.position, centre, ref velocity, 0.5f);
            if (velocity.magnitude < 0.5) {
                recentlyChanged = false;
            }
        }
    }

    /// <summary>
    /// Add a player to the camera's tracking
    /// </summary>
    /// <param name="target"> The player to be targetted by the camera </param>
    public void addPlayer(Transform target) {
        players.Add(target);
        recentlyChanged = true;
    }

    /// <summary>
    /// Remove a player from the camera's tracking
    /// </summary>
    /// <param name="target"> The player to be removed from the camera </param>
    public void removePlayer(Transform target) {
        players.Remove(target);
        recentlyChanged = true;
    }

    /// <summary>
    /// Return the centre point of all target players
    /// </summary>
    /// <returns></returns>
    private Vector3 getCentrePont() {

        try {
            var bounds = new Bounds(players[0].position, Vector3.zero);

            for (int i = 0; i < players.Count; i++) {
                bounds.Encapsulate(players[i].position);
            }

            return bounds.center;
        } catch (MissingReferenceException) {
            return transform.position;
        } catch (ArgumentOutOfRangeException) {
            return transform.position;
        }
    }

    /// <summary>
    /// Get the bounds of the camera in world coordinates
    /// </summary>
    /// <returns>
    /// Returns Vector4 of minX, minY, maxX, maxY
    /// </returns>
    public Vector4 GetBounds() {
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, Camera.main.nearClipPlane));

        Vector3 center = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, Camera.main.nearClipPlane));
        Vector3 offset = transform.position - center;

        //Debug.Log(topRight.ToString());
        //Debug.Log(bottomLeft.ToString());

        return new Vector4(bottomLeft.x + offset.x, bottomLeft.y + offset.y, topRight.x + offset.x, topRight.y + offset.y);
    }
}
