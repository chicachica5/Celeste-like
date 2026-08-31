using UnityEngine;

public class GameLoop : MonoBehaviour
{
    float stepDuration = 1f/60f;
    float accumulator = 0;

    //all systems
    [SerializeField] AgentSystem agents;
    [SerializeField] SolidSystem solids;

    [SerializeField] Hitstop HitstopManager;
    int safeguard = 10;

    void Update()
    {
        accumulator += Time.deltaTime;
        int i = 0;

        while (accumulator >= stepDuration)
        {
            GameplayStep();
            accumulator = accumulator - stepDuration;

            i++;
            if(i > safeguard) break;
        }
    }

    void GameplayStep()
    {
        if (HitstopManager.IsFrozen)
        {
            HitstopManager.Step();
            return;
        }

        agents.Step();
        solids.Step();
        //enemySystem.Step();
        //platformSystem.Step();
        // projectileSystem.Step();
    }
}
