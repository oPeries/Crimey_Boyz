/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using UnityEngine;

//Attach to an object to change its quad's colour when active (Tappable Objects only)
public class ChangeColourWhenActive : MonoBehaviour {

    //Set the callback function to run when tapped on
    void Start() {
        NetworkedTappable script = gameObject.GetComponent<NetworkedTappable>();

        if(script != null) {
            script.onActiveCallback = OnActiveChanged;
        } else {
            Debug.LogWarning("ChangeColourWhenActive must be on an object with a NetworkedTappable component");
        }
    }

    /// <summary>
    /// Callback for when isActive changes (Gameobject is tapped / clicked). 
    /// This is where animation / behaviour should be run (Will be run on all instances of this object)
    /// </summary>
    /// <param name="isActive"> Value for if the object is active or not </param>
    private void OnActiveChanged(bool isActive) {
        if(isActive) {
            gameObject.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.red;
        } else {
            gameObject.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.white;
        }
    }
}
