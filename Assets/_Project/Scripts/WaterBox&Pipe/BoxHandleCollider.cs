using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxHandleCollider : MonoBehaviour
{
    bool hasTrigged = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTrigged) return;

        if (collision.CompareTag("Pipe"))
        {
            hasTrigged = true;
            Debug.Log("Va cham voi Pipe" + collision.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hasTrigged = false;
    }
}
