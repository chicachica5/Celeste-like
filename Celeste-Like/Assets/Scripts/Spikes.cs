using UnityEngine;

public class Spikes : Solid
{
    override public void BeingCollided(Agent agent)
    {
        Debug.Log("haha");
        if(agent.gameObject.tag == "Player") agent.SetToDie();
    }
}
