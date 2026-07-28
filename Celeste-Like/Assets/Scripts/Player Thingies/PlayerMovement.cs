using UnityEngine;
using UnityEngine.InputSystem;

public enum playerState {
    normal = 0,
    pause,
    dying,
}

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Player agent;
    [SerializeField] PlayerInput input;

    playerState state = playerState.normal;

    float xMoveSpeed = 1.7f;

    float maxVelocityY = 3.0f;
    float gravAcceleration = 0.4f;
    public bool disableGravity = false;

    int fulljumpFrames = 12;
    int jumpTimer = 0;
    int finishJumpFrames;
    float jumpSpeed = 2f;
    bool isJumping = false;
    bool canJump = true;

    float speedY = 0.0f;

    Vector3 respawnPoint;

    void Start()
    {
        finishJumpFrames = 15 + (int)Mathf.Round(jumpSpeed / gravAcceleration);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(state == playerState.normal)
        {
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

            //moving x later to avoid collision problems onCollide
            if(Mathf.Abs(speedY) > maxVelocityY) speedY = Mathf.Sign(speedY)*maxVelocityY;

            agent.MoveY(speedY, agent.checkDownCollition, OnMoveY);


            float move = input.actions["Move"].ReadValue<float>();

            if(move != 0) //this will change in the future
            {
                agent.MoveX(Mathf.Sign(move)*xMoveSpeed, null, agent.checkDownCollition);
            }
        }
    }

    public void touchedFloor()
    {
        //All things that refresh over touching floors
        if(!isJumping) canJump = true;
    }

    public void OnMoveY()
    {
        if(speedY > 0.0f) agent.ridingObject = null;
    }

    public void SetRespawn(Vector3 vec)
    {
        respawnPoint = vec;
    }

    public void SetPlayerState(playerState s)
    {
        state = s;
    }

    public void RestartToSpawn()
    {
        //override normal movement to directly put player to respawn position
        gameObject.transform.position = respawnPoint;

        //after restart set player to normal state
        state = playerState.normal;
    }

    void OnJump()
    {
        if(canJump)
        {
            //jumping things
            canJump = false;
            isJumping = true;
            jumpTimer = 0;
            disableGravity = true;

            speedY = jumpSpeed;
        }
    }
}
