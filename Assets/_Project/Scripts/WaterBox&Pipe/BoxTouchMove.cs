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

    #region Unity Methods
    public void OnStart()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        handleCollider = GetComponent<BoxHandleCollider>();

        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // siêu mượt
    }

    public void OnUpdate()
    {
        if (IsFilling || IsFillMax) return;

        if (dragging)
        {
            Vector3 wp = cam.ScreenToWorldPoint(Input.mousePosition);
            wp.z = zDepth;
            targetPos = wp + offset;

            // ✅ Giới hạn hướng di chuyển
            if (handleCollider.BoxData.type == HolderType.Direction || handleCollider.BoxData.type == HolderType.Stone)
            {
                if (handleCollider.BoxData.direction == HolderDirection.Horizontal)
                {
                    // chỉ đổi X
                    targetPos.y = rb.position.y;
                }
                else if (handleCollider.BoxData.direction == HolderDirection.Vertical)
                {
                    // chỉ đổi Y
                    targetPos.x = rb.position.x;
                }
            }

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
            Vector2 newPos = Vector2.SmoothDamp(
                rb.position,
                snapTargetPos,
                ref smoothVelocity,
                snapSmoothTime
            );

            rb.MovePosition(newPos);

            if (Vector2.Distance(rb.position, snapTargetPos) < 0.01f)
            {
                rb.MovePosition(snapTargetPos);
                snapping = false;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    #endregion

    public void OnPointerDown(PointerEventData eventData)
    {
        if (handleCollider.BoxData.type == HolderType.Ice && handleCollider.BoxData.iceBreak > 0) return;

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
