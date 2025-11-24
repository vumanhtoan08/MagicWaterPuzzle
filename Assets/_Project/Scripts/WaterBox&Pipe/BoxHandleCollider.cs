using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxHandleCollider : MonoBehaviour
{
    [SerializeField] private BoxTouchMove boxTouchMove;
    [SerializeField] private HolderData boxData;
    [SerializeField] private List<WaterColor> currentColor;
    [SerializeField] private List<NodeBoxCheckCollider> childColliders;

    public void GetHolderData(HolderData data)
    {
        boxData = data;
        InitStartValueWaterColor();
        boxTouchMove = GetComponent<BoxTouchMove>();
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

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (hasTrigged) return;

    //    if (collision.CompareTag("Pipe"))
    //    {
    //        hasTrigged = true;

    //        // Lay component cua pipe
    //        PipeBase pipeBase = collision.GetComponent<PipeBase>();
    //        if (pipeBase.CheckBoxCondition(boxData))
    //        {
    //            boxTouchMove.OnPointerUp();
    //            Transform hitObject = collision.transform;
    //            Debug.Log("Va chạm với: " + hitObject.name);


    //            boxTouchMove.SnapBoxToGrid(hitObject.position);
    //            pipeBase.FillWater(boxTouchMove, boxData, currentColor);

    //        }
    //        // Goi ham check

    //        // Neu true thi fill nuoc bang coroutine
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    hasTrigged = false;
    //}
}
