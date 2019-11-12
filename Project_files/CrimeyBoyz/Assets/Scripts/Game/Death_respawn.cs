/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using UnityEngine;

//[Obsolete]
//Initial implementation for death and respawning. Moved to Player script
public class Death_respawn : MonoBehaviour {
    
    bool IsDead;
    public bool IsPlayer;
    float RespawnTimer = 0;
    public float RespawnTimerLimit = 1;

    // Start is called before the first frame update
    void Start() {
        IsDead = false;
    }

    // Update is called once per frame
    void Update() {
        if (IsDead) {
            RespawnTimer += Time.deltaTime;
        }

        if (IsPlayer && IsDead && RespawnTimer > RespawnTimerLimit) {

        }
    }

    /// <summary>
    /// Kill the player by destroying their gameobject
    /// </summary>
    public void KillPlayer() {
        //kills the current gameObject
        Destroy(gameObject);
    }
}
