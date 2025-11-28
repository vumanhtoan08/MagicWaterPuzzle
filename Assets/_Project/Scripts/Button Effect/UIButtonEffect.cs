using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Settings")]
    public float pressedScale = 0.9f;       // scale khi nhấn
    public float normalScale = 1f;          // scale khi thả
    public float tweenTime = 0.15f;         // thời gian tween

    [Header("Optional Effects")]
    public bool punchEffectOnRelease = false;
    public float punchStrength = 0.2f;

    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // stop previous tweens
        rect.DOKill();

        // scale down
        rect.DOScale(pressedScale, tweenTime).SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rect.DOKill();

        // scale back to normal
        rect.DOScale(normalScale, tweenTime).SetEase(Ease.OutBack);

        // optional punch effect (bounce)
        if (punchEffectOnRelease)
        {
            rect.DOPunchScale(Vector3.one * punchStrength, 0.2f, 10, 1);
        }
    }
}
