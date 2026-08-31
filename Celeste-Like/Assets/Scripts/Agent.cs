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
        MoveAxis(ref xRemainder, Vector2.right, amount, OnCollide, OnMove);
    }

    public void MoveY(float amount, Action OnCollide, Action OnMove)
    {
        MoveAxis(ref yRemainder, Vector2.up, amount, OnCollide, OnMove);
    }

    void MoveAxis(ref float remainder, Vector2 axis, float amount, Action onCollide, Action onMove)
    {
        remainder += amount;

        int move = Mathf.RoundToInt(remainder);

        if(move == 0)
            return;

        remainder -= move;
        int sign = (int)Mathf.Sign(move);

        while(move != 0)
        {
            Vector2 nextPos = new Vector2(transform.position.x, transform.position.y) + axis * sign;
            float padding = 0.03f;

            Collider2D col = Physics2D.OverlapArea(
                nextPos + new Vector2(-sizeX / 2f, -sizeY / 2f) + Vector2.one * padding,
                nextPos + new Vector2( sizeX / 2f,  sizeY / 2f) - Vector2.one * padding,
                SolidLayer);

            if(col == null)
            {
                transform.position += new Vector3(axis.x, axis.y, 0f) * sign;
                move -= sign;

                if(onMove != null)
                    onMove();
            }
            else
            {
                Solid solid = col.gameObject.GetComponent<Solid>();
                if(solid != null)
                    solid.BeingCollided(this);

                if(onCollide != null)
                    onCollide();

                break;
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
