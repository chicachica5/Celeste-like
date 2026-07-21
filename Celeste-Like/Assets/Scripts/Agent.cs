using UnityEngine;

public class Agent : MonoBehaviour
{
    float xRemainder = 0;
    //float yRemainder = 0;

    int width = 8;
    int height = 8;

    LayerMask SolidLayer;

    void Start()
    {
        SolidLayer = LayerMask.GetMask("Solid");
    }

    public void MoveX(float amount)
    {
        xRemainder += amount; //add change of movement to tracker

        int move = (int)Mathf.Round(xRemainder);

        if(move != 0) //movement was added and we move agenta pixel or so
        {
            xRemainder -= move;
            int sign = (int)Mathf.Sign(move);

            while(move != 0) //we move all the pixels that need to be moved
            {
                Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
                Debug.Log(width/2);
                Debug.DrawLine(playerPos + new Vector2(sign*width/2 + move, height/2), playerPos + new Vector2(sign*width/2, -height/2), Color.white, 5.0f);
                if(!Physics2D.OverlapArea(playerPos + new Vector2(sign*width/2 + move, height/2), playerPos + new Vector2(sign*width/2, -height/2), SolidLayer))
                { //does not collide
                    transform.position = new Vector3 (transform.position.x +sign, transform.position.y, transform.position.z);
                    move -= sign;
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void MoveY(float amount)
    {

    }
}
