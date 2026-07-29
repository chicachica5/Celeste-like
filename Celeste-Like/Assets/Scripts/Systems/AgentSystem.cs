using UnityEngine;
using System.Collections.Generic;

public class AgentSystem : MonoBehaviour
{
    Player p;
    List<Agent> agentList = new List<Agent>();

    public void Step() 
    {
        //first player
        p.Step();

        //second everything else
        foreach(Agent agent in agentList)
        {
            agent.Step();
        }
    }

    public void AddToList(Agent a)
    {
        agentList.Add(a);
    }

    public void AddPlayer(Player p)
    {
        this.p = p;
    }
}
