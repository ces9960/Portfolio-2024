using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    //parameters for player's height, movement speed, and jump physics
    const float standHeight = 2f;
    const float jumpVelocity = 15f;
    const float gravAccel = -15f;
    const float walkSpeed = 15f;
    const float runMult = 1.5f;

    //used for hit detection
    Ray ray;
    RaycastHit hit;

    //enables/disables jumping based on grounded/midair state
    bool canJump;

    //mouse movement
    const float mouseSpeed = 1.515f;

    //vectors used for movement/rotation
    Vector3 velocity = Vector3.zero;
    Vector2 rotation = Vector2.zero;
    Vector3 pos;

    //vectors used for movement based on angle
    Vector3 moveVectorF = Vector3.zero;
    Vector3 moveVectorB = Vector3.zero;
    Vector3 moveVectorL = Vector3.zero;
    Vector3 moveVectorR = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Prevents cursor movement
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position; //updates pos to transform's current position

        //handles inputs
        handleKeyboardInput(); //keyboard

        handleMouseLook(); //mouse movement

        fireGun(); //mouse click

        checkCamHeight(); //checks grounded/midair status

        pos += velocity * Time.deltaTime; //updates pos based on velocity and time

        transform.position = pos; //updates transform's position
    }

    private void OnGUI()
    {
        //displays controls on screen
        GUI.Box(new Rect(0, 0, 200, 150), "Controls:\nWASD: Move\nSpace: Jump\nMouse: Look\nLeft Click: Play Sounds\nEscape: Quit\n\nClick on boxes to play sounds");
    }

    void checkCamHeight()
    {
            if (transform.position.y < standHeight) //prevents falling through the ground and enables jumping upon touching the ground
            {
                velocity.y = 0;
                pos.y = standHeight;
                transform.position = pos;
                canJump = true;
            }
            else //accelerates downwards
            {
                velocity.y += gravAccel * Time.deltaTime;
            }
    }

    void handleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //quits when escape is pressed
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Space)) //when space is pressed, jumps and disables jumping until the player lands
        {
            if (canJump)
            {
                pos.y += 0.2f;
                velocity.y = jumpVelocity;
                canJump = false;
            }
        }
        
        //movement
        if (Input.GetKey(KeyCode.W))//forwards
        {
            moveVectorF = new Vector3(transform.forward.x, 0, transform.forward.z).normalized * walkSpeed;
        }
        else
        {
            moveVectorF = Vector3.zero;
        }
        if (Input.GetKey(KeyCode.A))//left
        {
            moveVectorL = new Vector3(-transform.right.x, 0, -transform.right.z).normalized * walkSpeed;
        }
        else
        {
            moveVectorL = Vector3.zero;
        }
        if (Input.GetKey(KeyCode.S))//right
        {
            moveVectorB = new Vector3(-transform.forward.x, 0, -transform.forward.z).normalized * walkSpeed;
        }
        else
        {
            moveVectorB = Vector3.zero;
        }
        if (Input.GetKey(KeyCode.D))//backwards
        {
            moveVectorR = new Vector3(transform.right.x, 0, transform.right.z).normalized * walkSpeed;
        }
        else
        {
            moveVectorR = Vector3.zero;
        }

        if (Input.GetKey(KeyCode.LeftShift)) //hold shift to walk (reduce speed)
        {
            velocity.x = moveVectorF.x + moveVectorB.x + moveVectorL.x + moveVectorR.x;
            velocity.z = moveVectorF.z + moveVectorB.z + moveVectorL.z + moveVectorR.z;
        }
        else
        {
            velocity.x = (moveVectorF.x + moveVectorB.x + moveVectorL.x + moveVectorR.x) * runMult;
            velocity.z = (moveVectorF.z + moveVectorB.z + moveVectorL.z + moveVectorR.z) * runMult;
        }
    }

    void handleMouseLook()
    {
        //gets current rotation
        float rotX = rotation.x;
        float rotY = rotation.y;

        //modifies rotation by raw mouse input
        rotX -= Input.GetAxisRaw("Mouse Y");
        rotY += Input.GetAxisRaw("Mouse X");

        //updates rotation
        rotation.x = rotX;
        rotation.y = rotY;

        //rotates transform
        transform.eulerAngles = new Vector2(Mathf.Clamp(rotation.x, -6 * (15f/1.515f), 6 * (15f/1.515f)), rotation.y) * mouseSpeed;
    }

    void fireGun()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit)) //raycasts in the current facing direction
            {
                if (hit.collider.gameObject) //checks if it hits anything
                {
                    soundObject hitObj = hit.collider.GetComponent<soundObject>(); //gets the soundObject component
                    if (hitObj != null) //if the soundObject component exists, plays the sound associated with that object
                    {
                        hitObj.playSound();
                    }
                }
            }
        }
    }
}
