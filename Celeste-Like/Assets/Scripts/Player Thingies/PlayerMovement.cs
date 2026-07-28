using UnityEngine;
using UnityEngine.InputSystem;

public enum playerState {
    normal = 0,
    pause,
    dashing,
    dying,
}

public enum directions {
    up = 0,
    down,
    left,
    right,
    up_left,
    up_right,
    down_left,
    down_right
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

    bool canDash = true;
    int fullDashFrames = 13;
    int dashTimer = 0;
    float dashSpeedX = 2f;
    float dashSpeedY = 2f;
    float dashNormalSpeed = 4.4f;
    float dashDiagonalSpeed = 3.1f;


    float speedY = 0.0f;

    Vector3 respawnPoint;

    directions dashDir;

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

        else if(state == playerState.dashing)
        {
            dashTimer++;

            if(dashSpeedX != 0) agent.MoveX(dashSpeedX, null, agent.checkDownCollition);
            if(dashSpeedY != 0) agent.MoveY(dashSpeedY, agent.checkDownCollition, agent.checkDownCollition);



            if(dashTimer >= fullDashFrames)
            {
                state = playerState.normal;
            }
        }
    }

    public void touchedFloor()
    {
        //All things that refresh over touching floors
        if(!isJumping) canJump = true;

        canDash = true;
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

    directions DecideDirection(float move, float look)
    {
        if(move == 1 && look == 1)
        {
            dashSpeedX = dashDiagonalSpeed;
            dashSpeedY = dashDiagonalSpeed;
            return directions.up_right;
        }
        else if(move == 1 && look == -1)
        {
            dashSpeedX = dashDiagonalSpeed;
            dashSpeedY = -dashDiagonalSpeed;
            return directions.up_left;
        }
        else if(move == -1 && look == 1)
        {
            dashSpeedX = -dashDiagonalSpeed;
            dashSpeedY = dashDiagonalSpeed;
            return directions.down_right;
        }
        else if(move == -1 && look == -1)
        {
            dashSpeedX = -dashDiagonalSpeed;
            dashSpeedY = -dashDiagonalSpeed;
            return directions.down_left;
        }
        else if(move == 1)
        {
            dashSpeedX = dashNormalSpeed;
            dashSpeedY = 0f;
            return directions.up;
        }
        else if(move == -1)
        {
            dashSpeedX = -dashNormalSpeed;
            dashSpeedY = 0f;
            return directions.down;
        }
        else if(look == 1)
        {
            dashSpeedX = 0f;
            dashSpeedY = dashNormalSpeed;
            return directions.right;
        }
        else if(look == -1)
        {
            dashSpeedX = 0f;
            dashSpeedY = -dashNormalSpeed;
            return directions.left;
        }

        dashSpeedX = 0f;
        dashSpeedY = dashNormalSpeed;
        return directions.right;
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

    void OnDash()
    {
        if(canDash)
        {
            //dashing things
            canDash = false;
            state = playerState.dashing;
            dashTimer = 0;

            float move = input.actions["Move"].ReadValue<float>();
            float look = input.actions["Look"].ReadValue<float>();

            Debug.Log(move);
            Debug.Log(look);
            dashDir = DecideDirection(move, look);
        }
    }
}
