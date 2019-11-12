using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonMash : MonoBehaviour
{
    /**
     * Listens to ALL button presses and changes the sprite accordingly.
     * Has a default animation that is interrupted by a button press.
     * Human button presses will have a "splat" underlay.
     * 
     * After timeOutTime seconds of no button pressing, the idle animation will start again.
     *
     */
     
         

    public string button = "A";
    public float idleTimer = 0.25f;
    public Sprite switchTo = null;
    //time to wait before the button animation starts again
    public float timeOutTime = 2f;
    GameObject emphasis;
    SpriteRenderer emphasisSR;
    

    private float timer;
    SpriteRenderer spriteRenderer;
    Sprite originalSprite;

    bool changeToOriginal = false;
    bool wasPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;
        //emphasis = GetComponentInChildren<SpriteRenderer>();
        emphasis = gameObject.transform.Find("emphasisUnderlay").gameObject;
        
        emphasisSR = emphasis.GetComponent<SpriteRenderer>();
        emphasisSR.enabled = false;
       
    }

    // Update is called once per frame
    void Update() {

        timer += Time.deltaTime;

        //if any controller button is down
        for (int i = 1; i < 6; i++) {
            if (Input.GetButtonDown(button + i.ToString())){

                wasPressed = true;
                //make the button go down

                spriteRenderer.sprite = changeSprite(false);
                emphasisSR.enabled = true;
                timer = 0;
            }
        }

        for (int i = 1; i < 6; i++) {
            if (Input.GetButtonUp(button + i.ToString())) {
                //Debug.Log("up");
                //make the button go up

                spriteRenderer.sprite = changeSprite(true);
                emphasisSR.enabled = false;
            }
        }


        if (timer > timeOutTime && wasPressed == true) {
            //a timeout for the button: if it hasn't been pressed for a while then restart idle animation
            timer = 0;
            wasPressed = false;

        }

        if (timer > idleTimer && !wasPressed) {
            //switch the sprite if the button hasn't been pressed
            changeToOriginal = !changeToOriginal;
            spriteRenderer.sprite = changeSprite(changeToOriginal);

            timer = 0;
        }






    }

    private Sprite changeSprite(bool isOriginal) {
        if (isOriginal)
        {
            return originalSprite;
        }
        else {
         
            return switchTo;
        }

    }
}
