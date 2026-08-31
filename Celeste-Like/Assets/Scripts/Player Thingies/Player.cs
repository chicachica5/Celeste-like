using UnityEngine;

public enum wallSide {
    none = 0,
    left,
    right,
}

public class Player : Agent
{
    [SerializeField] PlayerMovement pMovement;

    bool isDying = false;

    int waitFrames = 60;
    int waitTimer = 0;

    override public void Step()
    {
        if(isDying)
        {
            waitTimer++;

            if(waitTimer >= waitFrames)
            {
                Die();
            }
        }
        else
        {
            pMovement.Step();
        }
    }

    override public void AddToAgentList()
    {
        GameObject.Find("Systems").GetComponent<AgentSystem>().AddPlayer(this);
    }

    override public void SetToDie()
    {
        isDying = true;
        pMovement.SetPlayerState(playerState.dying);
        waitTimer = 0;
    }

    public void Die()
    {
        //set player position
        pMovement.RestartToSpawn();

        //reset death state
        isDying = false;
    }

    public bool IsGrounded()
    {
        return ridingObject != null;
    }

    public bool IsTouchingWall(out wallSide side)
    {
        side = wallSide.none;

        Vector2 checkCenter = new Vector2(transform.position.x, transform.position.y);
        Vector2 boxSize = new Vector2(0.15f, sizeY * 0.8f);

        Vector2 leftCenter = checkCenter + new Vector2(-(sizeX / 2f + 0.1f), 0f);
        Vector2 rightCenter = checkCenter + new Vector2((sizeX / 2f + 0.1f), 0f);

        Collider2D colLeft = Physics2D.OverlapBox(leftCenter, boxSize, 0f, SolidLayer);
        if(colLeft != null)
        {
            side = wallSide.left;
            Debug.DrawLine(leftCenter - (Vector2)(boxSize * 0.5f), leftCenter + (Vector2)(boxSize * 0.5f), Color.yellow, 2f);
            Debug.DrawLine(leftCenter - new Vector2(boxSize.x * 0.5f, -boxSize.y * 0.5f), leftCenter + new Vector2(boxSize.x * 0.5f, -boxSize.y * 0.5f), Color.yellow, 2f);
            Debug.Log("Wall grab check: left wall detected.");
            return true;
        }

        Collider2D colRight = Physics2D.OverlapBox(rightCenter, boxSize, 0f, SolidLayer);
        if(colRight != null)
        {
            side = wallSide.right;
            Debug.DrawLine(rightCenter - (Vector2)(boxSize * 0.5f), rightCenter + (Vector2)(boxSize * 0.5f), Color.yellow, 2f);
            Debug.DrawLine(rightCenter - new Vector2(boxSize.x * 0.5f, -boxSize.y * 0.5f), rightCenter + new Vector2(boxSize.x * 0.5f, -boxSize.y * 0.5f), Color.yellow, 2f);
            Debug.Log("Wall grab check: right wall detected.");
            return true;
        }

        Debug.Log("Wall grab check: no wall collision detected.");
        return false;
    }

    public void CheckGroundCollision()
    {
        Vector2 nextPos = new Vector2(transform.position.x, transform.position.y) + Vector2.down;

        float padding = 0.03f;

        Collider2D col = Physics2D.OverlapArea(
            nextPos + new Vector2(-sizeX / 2f, -sizeY / 2f) + Vector2.one * padding,
            nextPos + new Vector2( sizeX / 2f,  sizeY / 2f) - Vector2.one * padding,
            SolidLayer);

        if(col == null)
        {
            ridingObject = null;
            return;
        }

        Solid solid = col.gameObject.GetComponent<Solid>();
        if(solid == null)
            return;

        solid.BeingCollided(this);
        ridingObject = solid;
        pMovement.touchedFloor();
    }

    public void checkDownCollition() //check if theres something down
    {
        CheckGroundCollision();
    }

    public void checkDownCollition2() //check if theres something down
    {
        CheckGroundCollision();
    }
}
