using UnityEngine;
using UnityEngine.InputSystem;

public enum playerState {
    normal = 0,
    prepareDashing,
    dashing,
    climbing,
    swing,
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

    Hitstop HitstopManager;
    playerState state = playerState.normal;

    float walkingSpeed = 0f;
    float walkingAcceleration = 0.2778f;
    float floorDeceleration = 0.12f;
    float maxWalkingSpeed = 1.5f;

    float maxVelocityY = 3.0f;
    float gravAcceleration = 0.4f;
    public bool disableGravity = false;

    int fulljumpFrames = 12;
    int jumpTimer = 0;
    int finishJumpFrames;
    float jumpSpeed = 1.8f;
    bool isJumping = false;
    bool canJump = true;

    bool canDash = true;
    int fullDashFrames = 13;
    int dashTimer = 0;
    float dashSpeedX = 2f;
    float dashSpeedY = 2f;
    float dashNormalSpeed = 4f;
    float dashDiagonalSpeed = 3.1f;

    float inputX = 0f;
    float inputY = 0f;
    float inputJump = 0f;
    float inputDash = 0f;
    float inputClimb = 0f;
    float inputSwing = 0f;

    float climbSpeed = 1.0f;

    Vector2 velocity;

    float coyoteTime = 0.10f;
    float jumpBufferTime = 0.10f;
    float dashBufferTime = 0.10f;
 

    float coyoteTimer = 0f;
    float jumpBufferTimer = 0f;
    float dashBufferTimer = 0f;
    float grabBufferTimer = 0f;

    bool jumpHeld = false;
    bool dashHeld = false;
    bool swingHeld = false;

    float speedY = 0.0f;

    Vector3 respawnPoint;

    directions dashDir;

    Vector2 swingAnchor;
    float swingLength;
    Vector2 swingVelocity;
    LineRenderer swingRope;
    bool preserveReleaseMomentum = false;

    void Start()
    {
        HitstopManager = GameObject.Find("GameLoop Manager").GetComponent<Hitstop>();
        finishJumpFrames = 15 + (int)Mathf.Round(jumpSpeed / gravAcceleration);

        swingRope = GetComponent<LineRenderer>();
        if(swingRope == null)
            swingRope = gameObject.AddComponent<LineRenderer>();

        swingRope.positionCount = 2;
        swingRope.useWorldSpace = true;
        swingRope.startWidth = 0.08f;
        swingRope.endWidth = 0.08f;
        swingRope.material = new Material(Shader.Find("Sprites/Default"));
        swingRope.startColor = Color.white;
        swingRope.endColor = Color.white;
        swingRope.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
        BufferManager();
    }

    void BufferManager()
    {
        float delta = Time.deltaTime;

        if(agent.IsGrounded())
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer = Mathf.Max(0f, coyoteTimer - delta);
        }

        jumpBufferTimer = Mathf.Max(0f, jumpBufferTimer - delta);
        dashBufferTimer = Mathf.Max(0f, dashBufferTimer - delta);
        grabBufferTimer = Mathf.Max(0f, grabBufferTimer - delta);

        bool jumpPressedThisFrame = inputJump > 0f && !jumpHeld;
        bool dashPressedThisFrame = inputDash > 0f && !dashHeld;
        bool swingPressedThisFrame = inputSwing > 0f && !swingHeld;

        if(jumpPressedThisFrame)
        {
            jumpBufferTimer = jumpBufferTime;
        }

        if(dashPressedThisFrame)
        {
            dashBufferTimer = dashBufferTime;
        }

        if(jumpBufferTimer > 0f && coyoteTimer > 0f && state == playerState.normal && !isJumping)
        {
            jumpBufferTimer = 0f;
            OnJump();
        }

        if(dashBufferTimer > 0f
            && canDash
            && state == playerState.normal
            && !HitstopManager.IsFrozen)
        {
            dashBufferTimer = 0f;
            canDash = false;
            state = playerState.prepareDashing;
            dashTimer = 0;
            HitstopManager.Freeze(3);
        }

        if(swingPressedThisFrame && state == playerState.normal && !HitstopManager.IsFrozen)
            TryStartSwing();

        jumpHeld = inputJump > 0f;
        dashHeld = inputDash > 0f;
        swingHeld = inputSwing > 0f;
    }

    public void Step()
    {
        switch(state)
        {
            case playerState.normal:
            {
                if(isJumping) //jumpstate
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

                    if(inputJump == 0)
                    {
                        if(speedY > 0.0f) speedY = 0.0f;
                        disableGravity = false;
                        isJumping = false;
                    }
                }

                if(!agent.IsGrounded() && disableGravity == false) //apply gravity
                {
                    speedY -= gravAcceleration;
                }
                else if(agent.IsGrounded() && speedY < 0.0f)
                {  
                    speedY = 0.0f;
                }

                //walking shenanigans
                if(inputX != 0)
                {
                    walkingSpeed += inputX * walkingAcceleration;
                }
                else
                {
                    if(Mathf.Abs(walkingSpeed) < floorDeceleration) walkingSpeed = 0;
                    else 
                    {
                        walkingSpeed -= Mathf.Sign(walkingSpeed)*floorDeceleration;
                    }
                }

                //moving x later to avoid collision problems onCollide
                if(!preserveReleaseMomentum && Mathf.Abs(speedY) > maxVelocityY)
                    speedY = Mathf.Sign(speedY)*maxVelocityY;

                if(!preserveReleaseMomentum && Mathf.Abs(walkingSpeed) > maxWalkingSpeed)
                    walkingSpeed = Mathf.Sign(walkingSpeed)*maxWalkingSpeed;
                preserveReleaseMomentum = false;

                velocity = new Vector2(walkingSpeed, speedY);

                agent.MoveY(velocity.y, agent.CheckGroundCollision, OnMoveY);

                if(inputX != 0 || Mathf.Abs(walkingSpeed) > 0.001f)
                {
                    agent.MoveX(velocity.x, OnWallCollision, agent.CheckGroundCollision);
                }
                break;
            }
            case playerState.climbing:
            {
                speedY = inputY * climbSpeed;

                velocity = new Vector2(0f, speedY);
                agent.MoveY(velocity.y, agent.CheckGroundCollision, OnMoveY);


                if(inputClimb == 0.0f)
                {
                    state = playerState.normal;
                    disableGravity = false;
                }
                break;
            }
            case playerState.prepareDashing:
            {
                //check for grabing place
                Vector2 origin = new Vector2(transform.position.x, transform.position.y);
                RaycastHit2D hit = Physics2D.Raycast(origin, new Vector2(inputX, inputY), dashNormalSpeed*fullDashFrames, LayerMask.GetMask("Solids")); 
                if(hit == false) 
                {
                    state = playerState.normal;
                }
                else
                {
                    dashDir = DecideDirection();
                    state = playerState.dashing;
                    Dash();
                }
                break;
            }
            case playerState.dashing:
            {
                Dash();
                break;
            }
            case playerState.swing:
            {
                Swing();
                break;
            }
            default: return;
        }
    }

    void ReadInput()
    {
        if(input == null)
            return;

        inputX = input.actions["Move"].ReadValue<float>();
        inputY = input.actions["Look"].ReadValue<float>();

        inputJump = input.actions["Jump"].ReadValue<float>();
        inputDash = input.actions["Dash"].ReadValue<float>();
        inputClimb = input.actions["Climb"].ReadValue<float>();

        inputSwing = input.actions["Swing"].ReadValue<float>();
    }

    public void touchedFloor()
    {
        //All things that refresh over touching floors
        canJump = true;
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
        swingRope.enabled = false;
    }

    directions DecideDirection()
    {
        if(inputX == 1 && inputY == 1)
        {
            dashSpeedX = dashDiagonalSpeed;
            dashSpeedY = dashDiagonalSpeed;
            return directions.up_right;
        }
        else if(inputX == 1 && inputY == -1)
        {
            dashSpeedX = dashDiagonalSpeed;
            dashSpeedY = -dashDiagonalSpeed;
            return directions.up_left;
        }
        else if(inputX == -1 && inputY == 1)
        {
            dashSpeedX = -dashDiagonalSpeed;
            dashSpeedY = dashDiagonalSpeed;
            return directions.down_right;
        }
        else if(inputX == -1 && inputY == -1)
        {
            dashSpeedX = -dashDiagonalSpeed;
            dashSpeedY = -dashDiagonalSpeed;
            return directions.down_left;
        }
        else if(inputX == 1)
        {
            dashSpeedX = dashNormalSpeed;
            dashSpeedY = 0f;
            return directions.up;
        }
        else if(inputX == -1)
        {
            dashSpeedX = -dashNormalSpeed;
            dashSpeedY = 0f;
            return directions.down;
        }
        else if(inputY == 1)
        {
            dashSpeedX = 0f;
            dashSpeedY = dashNormalSpeed;
            return directions.right;
        }
        else if(inputY == -1)
        {
            dashSpeedX = 0f;
            dashSpeedY = -dashNormalSpeed;
            return directions.left;
        }

        dashSpeedX = 0f;
        dashSpeedY = dashNormalSpeed;
        return directions.right;
    }

    void OnWallCollision()
    {
        if(state != playerState.normal)
            return;

        if(inputClimb != 0.0f) //enter climbstate
        {
            disableGravity = true;
            state = playerState.climbing;
        }
    }

    void TryStartSwing()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y);
        
        RaycastHit2D hit = Physics2D.Raycast(origin, new Vector2(inputX, inputY), dashNormalSpeed*fullDashFrames, LayerMask.GetMask("Solids")); 
        
        if(hit.collider == null)
            return;
        swingAnchor = hit.point;
        swingLength = Vector2.Distance(origin, swingAnchor);
        if(swingLength <= 0.01f)
            return;
        swingVelocity = new Vector2(walkingSpeed, speedY);
        state = playerState.swing;
        swingRope.enabled = true;
        UpdateSwingRope();
    }

    void Swing()
    {
        if(inputSwing <= 0f)
        {
            ReleaseSwing();
            return;
        }
        Vector2 oldPosition = transform.position;
        Vector2 offset = oldPosition - swingAnchor;
        if(offset.sqrMagnitude <= 0.0001f)
            offset = Vector2.down * swingLength;
        Vector2 radial = offset.normalized;
        Vector2 tangent = new Vector2(-radial.y, radial.x);
        swingVelocity += Vector2.down * gravAcceleration;
        swingVelocity = Vector2.Dot(swingVelocity, tangent) * tangent;
        Vector2 target = swingAnchor + (offset + swingVelocity).normalized * swingLength;
        Vector2 movement = target - (Vector2)transform.position;
        agent.MoveX(movement.x, null, null);
        agent.MoveY(movement.y, null, null);
        Vector2 actualOffset = (Vector2)transform.position - swingAnchor;
        if(actualOffset.sqrMagnitude > 0.0001f)
        {
            Vector2 constrainedPosition = swingAnchor + actualOffset.normalized * swingLength;
            Vector2 correction = constrainedPosition - (Vector2)transform.position;
            agent.MoveX(correction.x, null, null);
            agent.MoveY(correction.y, null, null);
        }
        Vector2 newOffset = (Vector2)transform.position - swingAnchor;
        Vector2 newRadial = newOffset.sqrMagnitude > 0.0001f ? newOffset.normalized : radial;
        Vector2 intendedVelocity = target - oldPosition;
        swingVelocity = intendedVelocity - Vector2.Dot(intendedVelocity, newRadial) * newRadial;
        UpdateSwingRope();
    }

    void ReleaseSwing()
    {
        Vector2 offset = ((Vector2)transform.position - swingAnchor).normalized;
        Vector2 tangent = new Vector2(-offset.y, offset.x);
        Vector2 releaseVelocity = Vector2.Dot(swingVelocity, tangent) * tangent;
        walkingSpeed = releaseVelocity.x;
        speedY = releaseVelocity.y;
        preserveReleaseMomentum = true;
        state = playerState.normal;
        swingRope.enabled = false;
    }

    void UpdateSwingRope()
    {
            swingRope.SetPosition(0, swingAnchor);
            swingRope.SetPosition(1, transform.position);
    }

    void Dash() //this way it can be called from the previous frame
    {
        dashTimer++;

        if(dashSpeedX != 0) agent.MoveX(dashSpeedX, null, agent.CheckGroundCollision);
        if(dashSpeedY != 0) agent.MoveY(dashSpeedY, agent.CheckGroundCollision, agent.CheckGroundCollision);

        if(dashTimer >= fullDashFrames)
        {
            state = playerState.normal;
        }
    }

    void OnJump()
    {
        if(canJump)
        {
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
            dashBufferTimer = dashBufferTime;
        }
    }
}
