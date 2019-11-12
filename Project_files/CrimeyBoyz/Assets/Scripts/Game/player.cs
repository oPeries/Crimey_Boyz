/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.Messaging;
using System;
using System.Collections;

//Controller for the player objects
public class player : NetworkedBehaviour
{

    private NetworkedVar<string> playerName; //Synced between instances
    private NetworkedVar<bool> facingForward; //Sync the facing direction of the player over the network (to flip the sprite)
    private NetworkedVar<int> playerNumber; //the player number of this player (1 to 5) - use this for networked game state
    private int controllerNum;
    private Rigidbody2D rb;
    private BoxCollider2D bc2d;
    private collCheck coll;
    private SpriteRenderer spriteRenderer;
    private FollowPlayers playerTracker;
    private Animator anim;

    //----------Movement Variables--------//
    private float speed = 10, jumpVelocity = 15, deathSpeed = 15;
    private float loadJumpTimer = 0, graceJumpTimer = 0, loadJumpLimit = 0.15f, graceJumpLimit = 0.15f;
    private float fallMulti = 2.5f, shortHopMulti = 5f;
    private float currentXaxis = 0;
    private float currentYaxis = 0;
    private bool buttonAHeld = false;
    private bool buttonATriggered = false;
    private bool buttonXTriggered = false;

    //----------Death Variables--------//
    private NetworkedVar<bool> IsDead;
    float RespawnTimer = 0;
    public float RespawnTimerLimit = 0.5f;
    //TODO: Align this with camera
    public float respawnY = 3.3f;
    public float respawnX = -7;
    Vector3 respawnLocation;

    //----------Score Variables--------//
    private bool isLoaded = false;
    //private GameObject moneyBag;
    private GameObject pickup;
    public GameObject arrow;
    private float bagCoolDown;
    private bool atElevator = false;
    public Transform moneyBagPos;

    //----------Effects Variables------//
    bool isFrozen = false;


    // Start is called before the first frame update
    private void Awake()
    {
        controllerNum = -1; //By default set to a controller that does not exist. This should ensure the spawned player cannot move until assigned a specific controller

        //Init networked vars
        playerName = new NetworkedVar<string>();
        playerName.Settings.WritePermission = NetworkedVarPermission.ServerOnly;
        playerName.OnValueChanged = updateName;

        playerNumber = new NetworkedVar<int>();
        playerNumber.Settings.WritePermission = NetworkedVarPermission.ServerOnly;
        //playerNumber.OnValueChanged = UpdatePlayerNumber;

        facingForward = new NetworkedVar<bool>();
        facingForward.Settings.WritePermission = NetworkedVarPermission.OwnerOnly;
        facingForward.OnValueChanged = updateFacingDirection;

        IsDead = new NetworkedVar<bool>();
        IsDead.Settings.WritePermission = NetworkedVarPermission.OwnerOnly;
        IsDead.OnValueChanged = updateDeathStatus;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<collCheck>();
        bc2d = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        IsDead.Value = false;
        arrow.SetActive(false);
    }

    void Start()
    {
        //If not the owner (i.e. tablet build) then don't process physics for this object.
        if (!IsOwner)
        {
            disablePlayer(true);
        }

    }

    //Process movement only if a controller has been assigned to this player
    //Only the owner of this object should process player inputs. Other clients should sync this object's position with the server
    void Update()
    {
        if (playerTracker == null)
        {
            FollowPlayers[] results = Resources.FindObjectsOfTypeAll<FollowPlayers>();
            if (results.Length > 0)
            {
                playerTracker = results[0];
                addToCameraTracker();
            }
        }

        if (IsOwner)
        {

            if (IsDead.Value)
            {
                RespawnTimer += Time.deltaTime;
                if (gameObject.transform.position.x > Camera.main.transform.position.x) {
                    gameObject.transform.position = new Vector2(Camera.main.transform.position.x - 5, gameObject.transform.position.y);
                }
            }

            if (bagCoolDown > 0)
            {
                bagCoolDown -= Time.deltaTime;
            }

            if (controllerNum != -1)
            {
                currentXaxis = Input.GetAxis("Horizontal" + controllerNum);
                currentYaxis = Input.GetAxis("Vertical" + controllerNum);
                buttonAHeld = getButton("A");
                if (Input.GetButtonDown("A" + controllerNum))
                {
                    buttonATriggered = true;
                }
                if (Input.GetButtonDown("X" + controllerNum))
                {
                    buttonXTriggered = true;
                    Vector3 direction = getJoystickDirection(1f);
                    if (direction != new Vector3(0, 0, 0))
                    {

                        arrow.SetActive(true);
                        arrow.transform.position = gameObject.transform.position + direction;

                        //rotate by O/A where A = x and O = y
                        //rotate Z
                        //float theta = Mathf.Asin(getJoystickDirection(1f).y / 1) * Mathf.Rad2Deg;

                        //TODO: Make this point backwards properly
                        //theta = (Mathf.Abs(theta) < 90) ? theta : - theta;
                        float theta = Mathf.Atan2(currentYaxis, currentXaxis) * Mathf.Rad2Deg;
                        

                        arrow.transform.eulerAngles = new Vector3(0, 0, theta);
                    }
                }
                else
                {
                    arrow.SetActive(false);
                }
            }

            //Update character direction
            if (facingForward.Value && currentXaxis < -0.05)
            {
                facingForward.Value = false;
            }
            else if (!facingForward.Value && currentXaxis > 0.05)
            {
                facingForward.Value = true;
            }
            restrictPositionToCamera();
        }
    }

    private void FixedUpdate()
    {

        if (IsOwner && !isFrozen)
        {
            if (IsDead.Value && RespawnTimer > RespawnTimerLimit)
            {
                //respawning sequence

                moveRespawn();
                if (buttonATriggered)
                {
                    spawnPlayer();
                    buttonATriggered = false;
                }
            }
            else
            {
                walk();
                jump();
                if (buttonXTriggered)
                {
                    //change to button held

                    if (pickup != null)
                    {
                        throwPickup();
                    }
                    

                    buttonXTriggered = false;
                }
            }
        }

        if (rb.velocity.x != 0 && coll.isGrounded())
        {
            anim.SetBool("walking", true);
        }
        else
        {
            anim.SetBool("walking", false);
        }
    }

    /// <summary>
    /// When the player is destroyed, remove it from the camera tracking
    /// </summary>
    private void OnDestroy()
    {
        removeFromCameraTracker();
    }

    //If dead and clicked by the tablet player, try spawn the player
    private void OnMouseDown()
    {
        if (IsDead.Value)
        {
            InvokeServerRpc(spawnPlayer);
        }
    }

    /// <summary>
    /// Checks to see if the player can walk, then has them walk in accordance to player input
    /// </summary>
    private void walk()
    {
        if (rb.bodyType == RigidbodyType2D.Static)
        {
            return;
        }
        if ((currentXaxis < 0 && coll.wallLeft()) || (currentXaxis > 0 && coll.wallRight()))
            return;
        rb.velocity = new Vector2(currentXaxis * speed, rb.velocity.y);
    }

    /// <summary>
    /// Has the player jump upward, or checks if they are currently in the air while holding the jump button
    /// </summary>
    private void jump()
    {
        //FALL CONTROL by 'Board to Bits Games' from https://youtu.be/7KiK0Aqtmzc
        //FALL CONTROL START
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics.gravity.y * fallMulti * Time.fixedDeltaTime;
        if (rb.velocity.y > 0 && !buttonAHeld)
            rb.velocity += Vector2.up * Physics.gravity.y * (shortHopMulti) * Time.fixedDeltaTime;
        //FALL CONTROL END

        updateJumpTimers();
        if (loadJumpTimer > 0 && graceJumpTimer > 0 && !(rb.velocity.y > 0))
        {
            if (pickup == null || pickup.tag != "Moneybag")
            {
                killJumpTimers();
                rb.velocity = Vector2.up * jumpVelocity;
                SessionManager.Singleton.dataRecorder.RecordInteraction("Jump", GetPlayerNumber(), transform.position.x, transform.position.y, rb.velocity.ToString("0.000"));
            }
        }

    }
    /// <summary>
    /// Updates the timers associated with loaded and grace jumps
    /// </summary>
    private void updateJumpTimers()
    {
        loadJumpTimer -= Time.deltaTime;
        graceJumpTimer -= Time.deltaTime;
        if (buttonATriggered)
        {
            loadJumpTimer = loadJumpLimit;
            buttonATriggered = false;
        }
        if (coll.isGrounded())
            graceJumpTimer = graceJumpLimit;
    }

    /// <summary>
    /// Sets both jump timers back to 0
    /// </summary>
    private void killJumpTimers()
    {
        loadJumpTimer = 0;
        graceJumpTimer = 0;
    }

    /// <summary>
    /// Gets if a certain button has been pressed down on a frame
    /// </summary>
    /// <param name="button"> The name of the button being checked (IE A,B,X,Y e.t.c) </param>
    /// <returns>
    /// Returns true if the button has been pressed on this frame, false if otherwise
    /// </returns>
    public bool getButtonDown(string button)
    {
        return Input.GetButtonDown(button + controllerNum);
    }

    /// <summary>
    /// Gets if a certain button is being held down on a frame
    /// </summary>
    /// <param name="button"> The name of the button being checked (IE A,B,X,Y e.t.c) </param>
    /// <returns>
    /// Returns true if the button is being held down on this frame, false if otherwise
    /// </returns>
    public bool getButton(string button)
    {
        return Input.GetButton(button + controllerNum);
    }

    //Set this player's name (server only. All other instances get updated to match)
    public void setName(string newName)
    {
        if (IsServer)
        {
            playerName.Value = newName;
        }
    }

    //When character name changes, update the sprite model to match
    private void updateName(string previous, string updated)
    {

        spriteRenderer.sprite = AllCharacters.Singleton.getSprite(updated);
        anim.SetInteger("character", AllCharacters.Singleton.getSpriteNumber(updated));
    }

    private void updateFacingDirection(bool previous, bool updated)
    {
        spriteRenderer.flipX = !updated;
    }

    /// <summary>
    /// Function to set the player's controller number
    /// </summary>
    /// <param name="controllerNum"> The number controller the player is using </param>
    public void setController(int controllerNum)
    {
        if (controllerNum <= 0 || controllerNum > 4)
        {
            Debug.LogError("Player Controller set to a non valid controller number, ignoring assignment");
        }
        else
        {
            this.controllerNum = controllerNum;
        }
    }

    /// <summary>
    /// Get the player's controller number
    /// </summary>
    /// <returns> Returns in int that contains the player's controller number </returns>
    public int getController()
    {
        return controllerNum;
    }

    /// <summary>
    /// Function to set the player's player number (e.g. P1, p2, p3, ...)
    /// </summary>
    public void SetPlayerNumber(int pNumber) {
        playerNumber.Value = pNumber;
    }

    /// <summary>
    /// Get the player's controller player number (e.g. P1, p2, p3, ...)
    /// </summary>
    public int GetPlayerNumber() {
        return playerNumber.Value;
    }

    /// <summary>
    /// Get the player's name
    /// </summary>
    /// <returns> 
    /// Returns a string that contains the player's name 
    /// </returns>
    public string getName()
    {
        return playerName.Value;
    }

    /// <summary>
    /// Function to set if the player has a moneybag or not
    /// </summary>
    /// <param name="loaded"> True if the player is holding a money bag, false if otherwise </param>
    public void setLoaded(GameObject pickedUp)
    {

        if (pickup != null) {
            dropPickup();
            Debug.LogWarning("Dropping a currently picked up item to setLoaded as something else, is this intended?");
        }


        //can't pick up when dead
        if (!IsDead.Value)
        {
            bagCoolDown = .2f;
            isLoaded = true;

            pickedUp.transform.SetParent(moneyBagPos);
            pickedUp.transform.localPosition = Vector3.zero;
            pickedUp.GetComponent<Rigidbody2D>().simulated = false;
            pickup = pickedUp;

            string itemType;
            if (pickup.GetComponentInChildren<moneyBag>() != null) {
                SessionManager.Singleton.networkedGameState.richPlayer.Value = playerNumber.Value;
                itemType = "MoneyBag";
                //Debug.Log("Moneybag picked up");

            } else if(pickup.GetComponent<PickupLogic>() != null) {
                pickup.GetComponent<PickupLogic>().SetInactiveBehaviour();
                itemType = "Alien";
                //Debug.Log("Pickup picked up");

            } else {
                itemType = "Unknown";
                Debug.LogWarning("Object pickedup that wasnt a money bag or pickup logic");
            }

            SessionManager.Singleton.dataRecorder.RecordInteraction("ItemPickUp", GetPlayerNumber(), transform.position.x, transform.position.y, itemType);
        }
    }

    /// <summary>
    /// Method for the player to drop a moneybag if they are holding it
    /// </summary>
    private void throwPickup()
    {
        if (isLoaded && bagCoolDown <= 0 && !atElevator)
        {
            //Instantiate(moneyBag, transform);
            if (pickup != null)
            {
                Vector3 direction = getJoystickDirection(15f);
                if (direction != new Vector3(0, 0, 0))
                {
                    //pickup.transform.position = arrow.transform.position;
                    pickup.transform.Translate(gameObject.transform.position - arrow.transform.position + direction.normalized * 1.5f);
                    pickup.transform.SetParent(null);
                    //teleport the pickup away from the player before setting active - not doing this will kill the player
                    pickup.GetComponent<Rigidbody2D>().simulated = true;
                    //moneyBag.GetComponent<Rigidbody2D>().velocity = rb.velocity;
                    pickup.GetComponent<Rigidbody2D>().AddForce(direction, ForceMode2D.Impulse);
                    //pickup.SendMessage("SetActive");

                    string itemType;
                    if (pickup.GetComponentInChildren<moneyBag>() != null) {
                        SessionManager.Singleton.networkedGameState.richPlayer.Value = -1;
                        itemType = "MoneyBag";
                        //Debug.Log("Moneybag thrown");

                    } else if (pickup.GetComponent<PickupLogic>() != null) {


                        pickup.GetComponent<PickupLogic>().SetActiveBehaviour();
                        itemType = "Alien";
                        //Debug.Log("threw picked up");

                    } else {
                        itemType = "Unknown";
                        Debug.LogWarning("Object thrown that wasnt a money bag or pickup logic");
                    }

                    float theta = Mathf.Atan2(currentYaxis, currentXaxis) * Mathf.Rad2Deg;

                    pickup = null;
                    isLoaded = false;

                    SessionManager.Singleton.dataRecorder.RecordInteraction("ItemThrow", GetPlayerNumber(), transform.position.x, transform.position.y, itemType + "," + theta.ToString("0.0"));


                } //else don't drop the moneybag as there is no joystick direction set
            }
        }
    }
    

    /// <summary>
    /// Function to set if the player is currently standing outside the elevator doors
    /// </summary>
    /// <param name="value"> True if the player is outside the elevator, false if otherwise </param>
    public void setAtElevator(bool value)
    {
        atElevator = value;
    }

    /// <summary>
    /// Get if the player has a money bag on them
    /// </summary>
    /// <returns>
    /// Returns true if the player has a money bag, false if otherwise
    /// </returns>
    public bool getIsLoaded()
    {
        return isLoaded;
    }

    //PLAYER DEATH IMPLEMENTATIONS:

    /// <summary>
    /// Despawns the player, sets isDead to false, and starts the respawning sequence.
    /// </summary>
    public void killPlayer(string cause)
    {
        SessionManager.Singleton.networkedGameState.UpdateScore(playerNumber.Value, -50);
        dropPickup(); //if loaded, drop the bag

        gameObject.GetComponentInChildren<ParticleSystem>().Play();
        IsDead.Value = true;
        disablePlayer(true);
        RespawnTimer = 0;
        gameObject.transform.position = new Vector2(Camera.main.transform.position.x - 5, Camera.main.transform.position.y);

        SessionManager.Singleton.dataRecorder.RecordInteraction("PlayerDeath", GetPlayerNumber(), transform.position.x, transform.position.y, cause);
    }

    /// <summary>
    /// Spawns the player in their current despawned position. Makes sure they aren't collided
    /// </summary>
    [ServerRPC(RequireOwnership = false)]
    public void spawnPlayer()
    {
        if (!coll.wallLeft() && !coll.wallRight())
        {
            RespawnTimer = 0;
            disablePlayer(false);
            IsDead.Value = false;

            SessionManager.Singleton.dataRecorder.RecordInteraction("PlayerSpawn", GetPlayerNumber(), transform.position.x, transform.position.y, null);
        }

    }

    //Synced across all instances (called when changing IsDead.value)
    private void updateDeathStatus(bool previous, bool updated)
    {
        Color faded = spriteRenderer.color;
        if (updated)
        {
            faded.a = 0.3f;
            removeFromCameraTracker();
        }
        else
        {
            faded.a = 1f;
            addToCameraTracker();
        }
        spriteRenderer.color = faded;
    }

    /// <summary>
    /// Locks the player to the X axis. Input method.
    /// </summary>
    private void moveRespawn()
    {
        
        Vector3 moveBy = new Vector3(currentXaxis * deathSpeed * Time.fixedDeltaTime, currentYaxis * deathSpeed * Time.fixedDeltaTime, 0);
        if (gameObject.transform.position.x + moveBy.x < Camera.main.transform.position.x) {
            gameObject.transform.Translate(moveBy);
        }
    }

    /// <summary>
    /// Disables the player but placing them in a ghost-like state. Collisions are disabled in this state. 
    /// To check collisions with this method use spawnPlayer
    /// </summary>
    /// <param name="input"></param>
    public void disablePlayer(bool input)
    {
        //puts the player in a ghost-like state
        //rb.simulated = !input;

        if (input)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

    }

    /// <summary>
    /// Add the attached game object to the camera tracker (camera will average the position of this object and all added).
    /// Use when spawning / respawning
    /// </summary>
    private void addToCameraTracker()
    {

        if (playerTracker != null)
        {
            playerTracker.addPlayer(transform);
        }
    }

    /// <summary>
    /// Remove the attached game object to the camera tracker (camera will no longer track this object). Use when player dies
    /// </summary>
    private void removeFromCameraTracker()
    {

        if (playerTracker != null)
        {
            playerTracker.removePlayer(transform);
        }
    }

    /// <summary>
    /// Restricts the positions that the camera can be in
    /// </summary>
    private void restrictPositionToCamera()
    {

        if (playerTracker == null || !playerTracker.enabled)
        {
            return;
        }

        Vector4 bounds = playerTracker.GetBounds();

        Vector3 newPos = transform.position;

        newPos.x = Mathf.Clamp(transform.position.x, bounds.x + transform.localScale.x / 2f, bounds.z - transform.localScale.x / 2f);
        newPos.y = Mathf.Clamp(transform.position.y, bounds.y + transform.localScale.y / 2f, bounds.w - transform.localScale.y / 2f);

        transform.position = newPos;
    }

    ///<summary>
    /// Gets the player's joystick direction as a Vector2
    /// </summary>
    private Vector3 getJoystickDirection(float magnitude)
    {
        float x = Input.GetAxisRaw("Horizontal" + controllerNum);
        float y = Input.GetAxisRaw("Vertical" + controllerNum);


        return new Vector3(x, y, 0).normalized * magnitude;

    }

    private void dropPickup() {
        //drops the moneybag no matter what
        //Debug.Log("Dropping pickup called");

        //Instantiate(moneyBag, transform);
        if (pickup != null)
        {
            //Debug.Log("Dropping pickup called");
            pickup.transform.SetParent(null);
            pickup.GetComponent<Rigidbody2D>().simulated = true;

            string itemType;
            if (pickup.GetComponent<PickupLogic>() != null) {
                pickup.GetComponent<PickupLogic>().SetInactiveBehaviour();
                itemType = "Alien";
                //Debug.Log("Pickup dropped");

            } else if (pickup.GetComponentInChildren<moneyBag>() != null) {
                SessionManager.Singleton.networkedGameState.richPlayer.Value = -1;
                itemType = "MoneyBag";
                //Debug.Log("Moneybag dropped");

            } else {
                itemType = "Unknown";
                Debug.LogWarning("Pickup dropped but wasnt a monebag or pickuplogic");
            }
            SessionManager.Singleton.dataRecorder.RecordInteraction("ItemDrop", GetPlayerNumber(), transform.position.x, transform.position.y, itemType);

            pickup = null;
        }
        
    }

    public void freezePlayer(bool shouldFreeze) {
        //Freezes player input
        isFrozen = shouldFreeze;
    }
    
}
