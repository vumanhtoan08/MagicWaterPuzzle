using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoxTouchMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Main REF")]
    private BoxHandleCollider handleCollider;
    private BoxVisual boxVisual;

    private Camera cam;
    private Rigidbody2D rb;

    private Vector3 offset;
    private float zDepth;

    private bool dragging = false;
    private bool snapping = false;

    private Vector2 targetPos;
    private Vector2 snapTargetPos;
    private Vector2 smoothVelocity;

    [SerializeField] private float smoothTime = 0.02f;
    [SerializeField] private float snapSmoothTime = 0.06f;

    public bool IsFilling { get; set; }
    public bool IsFillMax { get; set; }
    public Rigidbody2D Rb => rb;

    private bool isPointerDown = false;

    [Header("Tilt")]
    [SerializeField] float tiltAmount = 1f;
    [SerializeField] float tiltSmooth = 8f;
    [SerializeField] float resetTiltSmooth = 6f;
    private float maxDegRotate = 7f;

    #region Unity Methods

    GameplayScreen gameplayScreen;
    public void OnStart()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        handleCollider = GetComponent<BoxHandleCollider>();
        boxVisual = GetComponent<BoxVisual>();

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public void OnUpdate()
    {
        if (IsFilling || IsFillMax) return;

        if (dragging)
        {
            Vector3 wp = cam.ScreenToWorldPoint(Input.mousePosition);
            wp.z = zDepth;
            targetPos = wp + offset;

            if (handleCollider.BoxData.type == HolderType.Direction || handleCollider.BoxData.type == HolderType.Stone)
            {
                if (handleCollider.BoxData.direction == HolderDirection.Horizontal) targetPos.y = rb.position.y;
                else if (handleCollider.BoxData.direction == HolderDirection.Vertical) targetPos.x = rb.position.x;
            }

            Vector2 newPos = Vector2.SmoothDamp(rb.position, targetPos, ref smoothVelocity, smoothTime);
            rb.MovePosition(newPos);

            Vector2 velocity = smoothVelocity;

            float tiltX = Mathf.Clamp(velocity.y * tiltAmount, -maxDegRotate, maxDegRotate);
            float tiltY = Mathf.Clamp(-velocity.x * tiltAmount, -maxDegRotate, maxDegRotate);

            Quaternion targetRot = Quaternion.Euler(tiltX, tiltY, boxVisual.CenterPos.transform.localRotation.z);
            boxVisual.CenterPos.localRotation = Quaternion.Lerp(boxVisual.CenterPos.localRotation,
                targetRot,
                Time.deltaTime * tiltSmooth
            );
        }
        else if (snapping)
        {
            Vector2 newPos = Vector2.SmoothDamp(rb.position, snapTargetPos, ref smoothVelocity, snapSmoothTime);
            rb.MovePosition(newPos);

            if (Vector2.Distance(rb.position, snapTargetPos) < 0.01f)
            {
                rb.MovePosition(snapTargetPos);
                snapping = false;
            }
        }
    }

    #endregion

    public void OnPointerDown(PointerEventData eventData)
    {
        if (LevelManager.Instance.IsBoxTouched) return;
        LevelManager.Instance.IsBoxTouched = true;

        if (IsFillMax) return;
        if (!LevelManager.Instance.IsTimeRunning) LevelManager.Instance.StartTimer();


        gameplayScreen = UIManager.Instance.GetScreen<GameplayScreen>();
        if (!gameplayScreen.IsHasPlayerInput)
        {
            gameplayScreen.IsHasPlayerInput = true;
            gameplayScreen.OnListButtonInteract(true);
        }

        if (TutorialManager.Instance.IsTutorialActive) TutorialManager.Instance.OnHasPlayerInput();

        if (LevelManager.Instance.IsHammerWaiting && !LevelManager.Instance.IsHammerActive)
        {
            LevelManager.Instance.OnHammerActive(handleCollider);
            return;
        }
        if (handleCollider.BoxData.type == HolderType.Ice && handleCollider.BoxData.iceBreak > 0) return;

        transform.DOMoveZ(-1f, 0.1f);

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
        if (IsFillMax) return;

        OnHandlePointerUp();
    }

    public void OnHandlePointerUp()
    {
        LevelManager.Instance.IsBoxTouched = false;
        transform.DOKill();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        boxVisual.CenterPos.transform.DOLocalRotate(Vector3.zero, 0.3f);

        dragging = false;
        SnapBoxToGrid();
        snapping = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
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
