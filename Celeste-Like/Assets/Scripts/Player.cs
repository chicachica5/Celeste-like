using UnityEngine;

public class Player : Agent
{
    [SerializeField] PlayerMovement pMovement;

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
            ridingObject = col.gameObject.GetComponent<Solid>(); // we asume this is already right but just in case
            pMovement.touchedFloor();
        }
    }
}
