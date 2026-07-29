using UnityEngine;

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

    public void checkDownCollition() //check if theres something down
    {
        Vector2 nextPos = new Vector2(transform.position.x, transform.position.y) + Vector2.down;

        float padding = 0.03f;

        Collider2D col = Physics2D.OverlapArea(
            nextPos + new Vector2(-sizeX / 2f, -sizeY / 2f) + Vector2.one * padding,
            nextPos + new Vector2( sizeX / 2f,  sizeY / 2f) - Vector2.one * padding,
            SolidLayer);

        if(col == null) ridingObject = null;
        else 
        {
            col.gameObject.GetComponent<Solid>().BeingCollided(this);
            
            ridingObject = col.gameObject.GetComponent<Solid>(); // we asume this is already right but just in case
            pMovement.touchedFloor();
        }
    }

    public void checkDownCollition2() //check if theres something down
    {
        Debug.Log("TRIGGER 2");
        Vector2 nextPos = new Vector2(transform.position.x, transform.position.y) + Vector2.down;

        float padding = 0.03f;

        Collider2D col = Physics2D.OverlapArea(
            nextPos + new Vector2(-sizeX / 2f, -sizeY / 2f) + Vector2.one * padding,
            nextPos + new Vector2( sizeX / 2f,  sizeY / 2f) - Vector2.one * padding,
            SolidLayer);

        if(col == null) ridingObject = null;
        else 
        {
            Debug.Log("Touching");
            col.gameObject.GetComponent<Solid>().BeingCollided(this);
            
            ridingObject = col.gameObject.GetComponent<Solid>(); // we asume this is already right but just in case
            pMovement.touchedFloor();
        }
    }
}
