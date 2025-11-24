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
            boxTouchMove.SnapBoxToGrid(finalPos);

            pipeBase.FillWater(boxTouchMove, boxData);
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
