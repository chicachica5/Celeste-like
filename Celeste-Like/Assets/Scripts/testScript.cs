using UnityEngine;

public class testScript : MonoBehaviour
{
    [SerializeField] Solid sol;

    // Update is called once per frame
    void FixedUpdate()
    {
        sol.Move(-1.0f, 0.0f);
    }
}
