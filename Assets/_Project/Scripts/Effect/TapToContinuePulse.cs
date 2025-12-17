using UnityEngine;

public class TapToContinuePulse : MonoBehaviour
{
    [SerializeField] float scaleMin = 0.95f;
    [SerializeField] float scaleMax = 1.05f;
    [SerializeField] float speed = 1.8f;

    Vector3 baseScale;

    void OnEnable()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        AnimatePulse();
    }

    void AnimatePulse()
    {
        float t = (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f;
        float scale = Mathf.Lerp(scaleMin, scaleMax, t);
        transform.localScale = baseScale * scale;
    }
}
