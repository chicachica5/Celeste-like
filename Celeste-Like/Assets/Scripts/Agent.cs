using UnityEngine;
using System;

public class Agent : MonoBehaviour
{
    float xRemainder = 0;
    float yRemainder = 0;

    public int sizeX = 8;
    public int sizeY = 8;

    public Solid ridingObject = null;

    LayerMask SolidLayer;

    void Start()
    {
        SolidLayer = LayerMask.GetMask("Solids");
    }

    public void MoveX(float amount, Action OnCollide)
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

                if(!Physics2D.OverlapArea(playerPos + new Vector2(sign*sizeX/2 + sign, sizeY/2), playerPos + new Vector2(sign*sizeX/2, -sizeY/2), SolidLayer))
                { //does not collide
                    transform.position = new Vector3 (transform.position.x +sign, transform.position.y, transform.position.z);
                    move -= sign;
                }
                else
                {
                    if(OnCollide != null)
                        OnCollide();

                    break;
                }
            }
        }
    }

    public void MoveY(float amount, Action OnCollide)
    {
        yRemainder += amount; //add change of movement to tracker

        int move = (int)Mathf.Round(yRemainder);

        if(move != 0) //movement was added and we move agenta pixel or so
        {
            yRemainder -= move;
            int sign = (int)Mathf.Sign(move);

            while(move != 0) //we move all the pixels that need to be moved
            {
                Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);

                if(!Physics2D.OverlapArea(playerPos + new Vector2(sizeX/2, sign*sizeY/2 + sign), playerPos + new Vector2(-sizeX/2, sign*sizeY/2), SolidLayer))
                { //does not collide
                    transform.position = new Vector3 (transform.position.x, transform.position.y + sign, transform.position.z);
                    move -= sign;
                }
                else
                {
                    if(OnCollide != null)
                        OnCollide();
                    
                    break;
                }
            }
        }
    }

    public virtual bool IsRiding(Solid solid) {return false;}
    public virtual void Squish() {}
}
