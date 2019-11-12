/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using UnityEngine;

//Player collision checking. (Is a given player on the ground / against a wall?)
public class collCheck : MonoBehaviour {
    private float Distx, Disty, Posx, Posy;
    private float boxSize = 0.01f;
    private LayerMask groundLayer;
    private float distSoftener = 0.8f;
    private BoxCollider2D bc;
    // Start is called before the first frame update
    void Start() {
        groundLayer = LayerMask.GetMask("Ground");
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update() {
        Distx = bc.size.x/2;
        Disty = bc.size.y/2;
        Posx = transform.position.x;
        Posy = transform.position.y;
    }

    /// <summary>
    /// Get if the player is grounded
    /// </summary>
    /// <returns>
    /// True if the player is grounded, false if otherwise
    /// </returns>
    public bool isGrounded() {
        return Physics2D.OverlapArea(new Vector2(Posx - distSoftener * Distx, Posy - Disty - boxSize),
            new Vector2(Posx + distSoftener * Distx, Posy), groundLayer);
    }

    /// <summary>
    /// Get if a player is on the left wall
    /// </summary>
    /// <returns>
    /// Returns true if the player is on the left wall, false if otherwise
    /// </returns>
    public bool wallLeft() {
        return Physics2D.OverlapArea(new Vector2(Posx - Distx - boxSize, Posy + distSoftener * Disty),
            new Vector2(Posx - Distx, Posy - distSoftener * Disty), groundLayer);
    }

    /// <summary>
    /// Get if a player is on the right wall
    /// </summary>
    /// <returns>
    /// Returns true if a player is on the right wall, false if otherwise
    /// </returns>
    public bool wallRight() {
        return Physics2D.OverlapArea(new Vector2(Posx + Distx + boxSize, Posy + distSoftener * Disty),
            new Vector2(Posx + Distx, Posy - distSoftener * Disty), groundLayer);
    }

}
