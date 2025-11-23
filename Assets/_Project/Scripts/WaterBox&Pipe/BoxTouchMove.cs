using UnityEngine;
using UnityEngine.EventSystems;

public class BoxTouchMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Camera cam;
    private Rigidbody2D rb; 

    private Vector3 offset;
    private float zDepth;

    private bool dragging = false;
    private bool snapping = false;

    private Vector2 targetPos;       // vị trí mục tiêu khi kéo
    private Vector2 snapTargetPos;   // vị trí snap sau khi thả
    private Vector2 smoothVelocity;  // cho SmoothDamp

    [SerializeField] private float smoothTime = 0.02f;   // mượt khi kéo
    [SerializeField] private float snapSmoothTime = 0.06f;  // mượt khi snap

    bool hasTrigged = false;

    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // siêu mượt
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        snapping = false;
        rb.bodyType = RigidbodyType2D.Dynamic;

        Vector3 wp = cam.ScreenToWorldPoint(eventData.position);
        zDepth = transform.position.z;
        wp.z = zDepth;

        offset = transform.position - wp;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;

        // tính vị trí snap
        SnapBoxToGrid();

        snapping = true;
        rb.bodyType = RigidbodyType2D.Dynamic; // để MovePosition vẫn hoạt động
    }

    private void FixedUpdate()
    {
        if (dragging)
        {
            // cập nhật target theo chuột
            Vector3 wp = cam.ScreenToWorldPoint(Input.mousePosition);
            wp.z = zDepth;
            targetPos = wp + offset;

            // Smooth kéo
            Vector2 newPos = Vector2.SmoothDamp(
                rb.position,
                targetPos,
                ref smoothVelocity,
                smoothTime
            );

            rb.MovePosition(newPos);
        }
        else if (snapping)
        {
            // Smooth snap về vị trí ô grid
            Vector2 newPos = Vector2.SmoothDamp(
                rb.position,
                snapTargetPos,
                ref smoothVelocity,
                snapSmoothTime
            );

            rb.MovePosition(newPos);

            // Nếu gần đến vị trí snap thì dừng lại
            if (Vector2.Distance(rb.position, snapTargetPos) < 0.01f)
            {
                rb.MovePosition(snapTargetPos);
                snapping = false;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    private void SnapBoxToGrid()
    {
        // grid size = 2
        float gx = Mathf.Round(transform.position.x / 2f) * 2f;
        float gy = Mathf.Round(transform.position.y / 2f) * 2f;

        snapTargetPos = new Vector2(gx, gy);
    }

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
