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

public class NetworkedSpawnerUIController : MonoBehaviour
{
    public SpriteMask spriteMask;
    public SpriteRenderer background;
    Color backgroundColour;

    public bool isCoolingDown = false;
    
    float filledPosY;
    float emptyPosY;

    //flashes red for a warning
    bool shouldFlashRed = false;
    int flashCountMax = 3;
    int flashCount = 0;
    float flashTimer = 0.1f;
    float flashTimerCurrent = 0;

    StashTextUIController stashController;

    // Start is called before the first frame update
    void Start()
    {
        //filledPosY = spriteMask.transform.localPosition.y; //Position when completely filled
        filledPosY = spriteMask.transform.localScale.y * 2;
        //basically starting position + the size of the boxCollider - spriteMask should be just below the object
        emptyPosY = 0;
        //saves the original bg colour
        backgroundColour = background.color;

        //spriteMask.transform.position = new Vector2(gameObject.transform.position.x, filledPosY);
        resetMask();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldFlashRed) {
            if (flashCount >= flashCountMax) {
                shouldFlashRed = false;
                background.color = backgroundColour;
                flashCount = 0;
            }
            //flashes between red and original
            flashTimerCurrent += Time.deltaTime;
            if (flashTimerCurrent > flashTimer) {

                if (background.color != Color.red)
                {
                    background.color = Color.red;
                    flashCount += 1;
                }
                else {
                    background.color = backgroundColour;
                }

                flashTimerCurrent = 0;
            }

            
        }

    }

    private void OnEnable() {
        stashController = gameObject.GetComponentInChildren<StashTextUIController>();
    }

    public void resetMask() {
        //spriteMask.transform.position = new Vector3(gameObject.transform.position.x,filledPosY, gameObject.transform.position.z);
        //when complete:
        changeCooldownPosition(0f);
    }
    public StashTextUIController getStashController() {
        return stashController;
    }

    public void setStashAmount(int value) {
        stashController.writeToStash(value);
    }

    //Set the position of the cooldown mask
    //amount done is between 0 and 1
    //0 == not cooled down at all (just started)
    //1 == completed cooled down (ready to get the next item)
    public void changeCooldownPosition(float amountDone) {
        float posY = emptyPosY + amountDone * (filledPosY - emptyPosY);
        spriteMask.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, posY, gameObject.transform.localPosition.z);
    }

    public void flashRed() {
        shouldFlashRed = true;
    }
}
