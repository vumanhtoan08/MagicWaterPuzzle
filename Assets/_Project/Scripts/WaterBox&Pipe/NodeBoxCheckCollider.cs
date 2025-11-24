using UnityEngine;

public class NodeBoxCheckCollider : MonoBehaviour
{
    public System.Action<Collider2D, Transform> OnChildTriggerEnter;

    bool isTrigged = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTrigged) return;

        if (collision.CompareTag("Pipe"))
        {
            isTrigged = true;
            OnChildTriggerEnter?.Invoke(collision, transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isTrigged = false;
    }
}

