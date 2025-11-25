using UnityEngine;
using UnityEngine.EventSystems;

public class BoxTouchMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Main REF")]
    private BoxHandleCollider handleCollider;

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

    public bool IsFilling { get; set; }
    public bool IsFillMax { get; set; }

    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        handleCollider = GetComponent<BoxHandleCollider>();

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

    public void OnPointerUp(PointerEventData eventData = default)
    {
        dragging = false;

        // tính vị trí snap
        SnapBoxToGrid();

        snapping = true;
        rb.bodyType = RigidbodyType2D.Dynamic; // để MovePosition vẫn hoạt động
    }

    private void FixedUpdate()
    {
        if (IsFilling || IsFillMax) return;

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

    public void SnapBoxToGrid(Vector3? pos = null)
    {
        Vector3 p = pos ?? transform.position;

        float gx = Mathf.Round(p.x / 2f) * 2f;
        float gy = Mathf.Round(p.y / 2f) * 2f;

        snapTargetPos = new Vector2(gx, gy);
    }

    public Vector3 GetSnappedPosition(Vector3 pos)
    {
        float gx = Mathf.Round(pos.x / 2f) * 2f;
        float gy = Mathf.Round(pos.y / 2f) * 2f;

        return new Vector3(gx, gy, transform.position.z);
    }
}
