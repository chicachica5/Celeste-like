using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Player agent;
    [SerializeField] PlayerInput input;

    float xMoveSpeed = 1.7f;

    float maxVelocityY = 2.0f;
    float gravAcceleration = 0.4f;
    public bool disableGravity = false;

    int fulljumpFrames = 15;
    int jumpTimer = 0;
    int finishJumpFrames;
    float jumpSpeed = 1.8f;
    bool isJumping = false;
    bool canJump = true;

    float speedY = 0.0f;

    void Start()
    {
        finishJumpFrames = 15 + (int)Mathf.Round(jumpSpeed / gravAcceleration);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float move = input.actions["Move"].ReadValue<float>();

        if(move != 0) //this will change in the future
        {
            agent.MoveX(Mathf.Sign(move)*xMoveSpeed, null, agent.checkDownCollition);
        }

        if(isJumping)
        {
            jumpTimer++;

            if(jumpTimer >= fulljumpFrames)
            {
                disableGravity = false; 
            }
            else if(jumpTimer >= finishJumpFrames)
            {
                isJumping = false;
            }

            if(input.actions["Jump"].ReadValue<float>() == 0)
            {
                if(speedY > 0.0f) speedY = 0.0f;
                disableGravity = false;
                isJumping = false;
            }
        }

        if(!agent.IsRidingAny() && disableGravity == false)
        {
            //Apply gravity
            speedY -= gravAcceleration;
        }
        else if(agent.IsRidingAny() && speedY < 0.0f)
        {  
            speedY = 0.0f;
        }

        if(Mathf.Abs(speedY) > maxVelocityY) speedY = Mathf.Sign(speedY)*maxVelocityY;

        agent.MoveY(speedY, touchedFloor, OnMoveY);
    }

    public void touchedFloor()
    {
        //All things that refresh over touching floors
        Debug.Log("touched called");
        if(!isJumping) canJump = true;
    }

    public void OnMoveY()
    {
        if(speedY > 0.0f) agent.ridingObject = null;
    }

    void OnJump()
    {
        if(canJump)
        {
            Debug.Log("jumping");
            //jumping things
            canJump = false;
            isJumping = true;
            jumpTimer = 0;
            disableGravity = true;

            speedY = jumpSpeed;
        }
    }
}
