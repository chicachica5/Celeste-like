using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Agent agent;
    [SerializeField] PlayerInput input;

    float xMoveSpeed = 0.2f;

    // Update is called once per frame
    void FixedUpdate()
    {
        float move = input.actions["Move"].ReadValue<float>();

        if(move != 0) 
        {
            agent.MoveX(Mathf.Sign(move)*xMoveSpeed, null);
        }
    }
}
