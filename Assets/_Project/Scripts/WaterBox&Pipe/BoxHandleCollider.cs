using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gần như là Controller quản lý việc Move, Visual và điều kiện 
public class BoxHandleCollider : MonoBehaviour
{
    [Header("Main REF")]
    [SerializeField] private BoxTouchMove boxTouchMove;
    [SerializeField] private BoxVisual boxVisual;

    [SerializeField] private HolderData boxData;
    [SerializeField] private List<WaterColor> currentColor;
    [SerializeField] private List<NodeBoxCheckCollider> childColliders;

    public HolderData BoxData => boxData;

    public void GetHolderData(HolderData data)
    {
        boxData = new HolderData(data);
        InitStartValueWaterColor();
        boxTouchMove = GetComponent<BoxTouchMove>();
        boxVisual = GetComponent<BoxVisual>();
    }

    private void InitStartValueWaterColor()
    {
        currentColor = new List<WaterColor>();

        foreach (WaterColor color in boxData.holderValue)
        {
            WaterColor newCurrentColor = new WaterColor();
            newCurrentColor.color = color.color;
            newCurrentColor.Value = 0f;
            currentColor.Add(newCurrentColor);
        }
    }

    private void HandleChildTrigger(Collider2D other, Transform childTransform)
    {
        PipeBase pipeBase = other.GetComponent<PipeBase>();
        if (pipeBase.CheckBoxCondition(boxData))
        {
            boxTouchMove.OnPointerUp();

            Vector3 offset = transform.position - childTransform.position;
            Vector3 snappedChild = boxTouchMove.GetSnappedPosition(childTransform.position);
            Vector3 finalPos = snappedChild + offset;

            Sequence seq = DOTween.Sequence().AppendCallback(() => boxTouchMove.SnapBoxToGrid(finalPos))            // ngăn không cho di chuyển lúc fill
                .AppendInterval(0.2f).OnComplete(() => boxTouchMove.IsFilling = true);

            // Clone ra them HolderData truyen vao Pipe
            HolderData cloneHolderData = new HolderData(boxData);
            ReceiveWater(pipeBase);
            pipeBase.FillWater(boxTouchMove, cloneHolderData);
        }
    }

    private void ReceiveWater(PipeBase pipeBase)
    {
        // fill 
        WaterColor fillValue = boxData.holderValue.Find(x => x.color == pipeBase.PipeData.waterColors[0].color);
        float subValue = Mathf.Min(pipeBase.PipeData.waterColors[0].Value, fillValue.Value);
        fillValue.Value -= subValue;

        // check không còn phần tử trong list
        if (fillValue.Value <= 0)
        {
            Debug.Log("Fill het");
            boxData.holderValue.Remove(fillValue);
        }
        else
        {
            Debug.Log("Fill chua het");
        }
        // nếu không còn nữa destroy holder
        if (boxData.holderValue.Count <= 0)
        {
            DOVirtual.DelayedCall(subValue * 0.5f, () =>
            {
                boxTouchMove.IsFillMax = true;
                transform.DOScale(0, 0.5f)
                    .OnComplete(() =>
                    {
                        Destroy(gameObject);
                    });
            });
        }
    }

    private void OnEnable()
    {
        childColliders = new List<NodeBoxCheckCollider>(
            GetComponentsInChildren<NodeBoxCheckCollider>()
        );

        foreach (var child in childColliders)
            child.OnChildTriggerEnter = HandleChildTrigger;
    }

    private void OnDisable()
    {
        foreach (var child in childColliders)
        {
            child.OnChildTriggerEnter = null;
        }
    }
}
