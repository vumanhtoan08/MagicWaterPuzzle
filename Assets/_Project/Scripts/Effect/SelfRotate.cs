using UnityEngine;

public class SelfRotate : MonoBehaviour
{
    public float rotateSpeed = 45f; // độ/giây

    void Update()
    {
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime, Space.Self);
    }
}
