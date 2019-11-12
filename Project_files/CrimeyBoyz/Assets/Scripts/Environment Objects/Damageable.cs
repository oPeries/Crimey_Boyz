/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using UnityEngine;

//Script added to hostile objects to kill players that collide with it.
//Must be used on an object with a BoxCollider component with isTrigger = false;
public class Damageable : MonoBehaviour {

    public string causeOfDeath = "Damageable";

    bool isActive = true;

    private void Start()
    {
    }

    /// <summary>
    /// Kills a player if they come into contact with this object
    /// </summary>
    /// <param name="collision"> The object that has collided with the damageable </param>
    protected void OnCollisionEnter2D(Collision2D collision) {
        
        player gamer = collision.gameObject.GetComponent<player>();
        if (gamer != null && isActive) {
            gamer.killPlayer(causeOfDeath);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision) {
        player gamer = collision.gameObject.GetComponent<player>();
        if (gamer != null && isActive)
        {
            gamer.killPlayer(causeOfDeath);
        }
        

    }

    public void activate(bool shouldActivate) {
        isActive = shouldActivate;
    }

    protected bool getActiveStatus() {
        return isActive;
    }

}
