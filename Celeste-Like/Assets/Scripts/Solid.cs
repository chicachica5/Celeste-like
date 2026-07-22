using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Solid : MonoBehaviour
{
    float xRemainder = 0;
    float yRemainder = 0;

    int sizeX = 8;
    int sizeY = 8;

    BoxCollider2D col;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }
    public void Move(float x, float y)
    {
        xRemainder += x;
        yRemainder += y;   

        int moveX = (int)Mathf.Round(xRemainder);
        int moveY = (int)Mathf.Round(yRemainder);

        if (moveX != 0 || moveY != 0)
        {
            Agent[] allAgents = FindObjectsByType<Agent>(FindObjectsSortMode.None);

            List<Agent> riding = GetAllRidingActors(allAgents);

            col.enabled = false;
            
            if(moveX != 0)
            {
                xRemainder -= moveX;
                transform.position = new Vector3 (transform.position.x + moveX, transform.position.y, transform.position.z);

                foreach (Agent agent in allAgents)
                {
                    if(overlapCheck(agent))
                    {
                        int sign = (int)Mathf.Sign(moveX);

                        agent.MoveX(((transform.position.x + sign*sizeX/2) - (agent.transform.position.x - sign*agent.sizeX/2)), agent.Squish);
                    }
                    else if(riding.Contains(agent))
                    {
                        agent.MoveX(moveX, null);
                    }
                } 
            }
            else if(moveY != 0)
            {
                yRemainder -= moveY;
                transform.position = new Vector3 (transform.position.x, transform.position.y + moveY, transform.position.z);

                foreach (Agent agent in riding)
                {
                    if(overlapCheck(agent))
                    {
                        int sign = (int)Mathf.Sign(moveY);

                        agent.MoveY(((transform.position.y + sign*sizeY/2) - (agent.transform.position.y - sign*agent.sizeY/2)), agent.Squish);
                    }
                    else if(riding.Contains(agent))
                    {
                        agent.MoveY(moveY, null);
                    }
                } 
            }

            col.enabled = true;
        }

        bool overlapCheck(Agent agent)
        {
            return Mathf.Abs(transform.position.x - agent.transform.position.x) < (sizeX + agent.sizeX) * 0.5f &&
            Mathf.Abs(transform.position.y - agent.transform.position.y) < (sizeY + agent.sizeY) * 0.5f;
        }

        List<Agent> GetAllRidingActors(Agent[] allAgents)
        {
            List<Agent> finalList = new List<Agent>();

            foreach (Agent agent in allAgents)
            {
                if(agent.ridingObject == this)
                {
                    finalList.Add(agent);
                }
            }

            return finalList;
        }
    }
}
