using UnityEngine;

public class TriggerSetRespawn : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerMovement>().SetRespawn(gameObject.transform.parent.transform.position);
        }
    }
}
