using UnityEngine;
using System;

public class Agent : MonoBehaviour
{
    float xRemainder = 0;
    float yRemainder = 0;

    public int sizeX = 8;
    public int sizeY = 8;

    public Solid ridingObject = null;

    public LayerMask SolidLayer;

    void Start()
    {
        SolidLayer = LayerMask.GetMask("Solids");
        AddToAgentList();
    }

    virtual public void Step() {}
    virtual public void AddToAgentList() 
    {
        GameObject.Find("Systems").GetComponent<AgentSystem>().AddToList(this);
    }

    public void MoveX(float amount, Action OnCollide, Action OnMove)
    {
        xRemainder += amount; //add change of movement to tracker

        int move = (int)Mathf.Round(xRemainder);

        if(move != 0) //movement was added and we move agenta pixel or so
        {
            xRemainder -= move;
            int sign = (int)Mathf.Sign(move);

            while(move != 0) //we move all the pixels that need to be moved
            {
                
                Vector2 nextPos = new Vector2(transform.position.x, transform.position.y) + Vector2.right * sign;
                float padding = 0.03f;

                Collider2D col = Physics2D.OverlapArea(
                    nextPos + new Vector2(-sizeX / 2f, -sizeY / 2f) + Vector2.one * padding,
                    nextPos + new Vector2( sizeX / 2f,  sizeY / 2f) - Vector2.one * padding,
                    SolidLayer);

                if(col == null)
                { //does not collide
                    transform.position = new Vector3 (transform.position.x +sign, transform.position.y, transform.position.z);
                    move -= sign;

                    if(OnMove != null)
                        OnMove();
                }
                else
                {
                    col.gameObject.GetComponent<Solid>().BeingCollided(this);

                    if(OnCollide != null)
                        OnCollide();

                    break;
                }
            }
        }
    }

    public void MoveY(float amount, Action OnCollide, Action OnMove)
    {
        yRemainder += amount; //add change of movement to tracker

        int move = (int)Mathf.Round(yRemainder);

        if(move != 0) //movement was added and we move agenta pixel or so
        {
            yRemainder -= move;
            int sign = (int)Mathf.Sign(move);

            while(move != 0) //we move all the pixels that need to be moved
            {
                Vector2 nextPos = new Vector2(transform.position.x, transform.position.y) + Vector2.up * sign;

                float padding = 0.03f;

                Collider2D col = Physics2D.OverlapArea(
                    nextPos + new Vector2(-sizeX / 2f, -sizeY / 2f) + Vector2.one * padding,
                    nextPos + new Vector2( sizeX / 2f,  sizeY / 2f) - Vector2.one * padding,
                    SolidLayer);

                if(col == null)
                { //does not collide
                    transform.position = new Vector3 (transform.position.x, transform.position.y + sign, transform.position.z);
                    move -= sign;

                    if(OnMove != null)
                        OnMove();
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

    public virtual void SetToDie() {}
    public virtual bool IsRiding(Solid solid) 
    {
        if(ridingObject == solid) return true;
        
        return false;
    }

    public virtual void Squish() {}

    public bool IsRidingAny()
    {
        if(ridingObject != null) return true;

        return false;
    }
}
