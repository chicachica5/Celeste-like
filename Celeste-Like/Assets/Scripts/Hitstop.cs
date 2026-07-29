using UnityEngine;

public class Hitstop : MonoBehaviour
{
    public int FramesRemaining;

    public bool IsFrozen => FramesRemaining > 0;

    void LateUpdate()
    {
        if (FramesRemaining > 0)
            FramesRemaining--;
    }

    public void Freeze(int frames)
    {
        FramesRemaining = frames;
    }
}
