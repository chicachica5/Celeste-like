using UnityEngine;

public class Spikes : Solid
{
    override public void BeingCollided(Agent agent)
    {
        if(agent.gameObject.tag == "Player") agent.SetToDie();
    }
}
