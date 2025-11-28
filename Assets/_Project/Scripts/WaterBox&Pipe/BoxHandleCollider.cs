using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    private HolderData boxDataTemp; 

    #region Unity Methods

    public void OnStart()
    {
        if (!boxTouchMove.IsNull("Không có BoxTouchMove")) boxTouchMove.OnStart();
    }

    public void OnUpdate()
    {
        if (!boxTouchMove.IsNull("Không có BoxTouchMove")) boxTouchMove.OnUpdate();
    }

    #endregion

    public void GetHolderData(HolderData data)
    {
        boxData = new HolderData(data);
        boxDataTemp = new HolderData(data);
        boxTouchMove = GetComponent<BoxTouchMove>();
        boxVisual = GetComponent<BoxVisual>();
    }

    // Kiểm tra va chạm của các Box con
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
                .AppendInterval(0.1f).OnComplete(() => boxTouchMove.IsFilling = true);

            // Clone ra them HolderData truyen vao Pipe
            HolderData cloneHolderData = new HolderData(boxData);
            if (boxData.secondaryHolder.holderValue.Count > 0)
            {
                ReceiveSecondaryWater(pipeBase);
            }
            else
            {
                ReceiveWater(pipeBase);
            }

            pipeBase.FillWater(boxTouchMove, cloneHolderData);
        }
    }

    // Hàm nhận nước dùng chung được với nhiều nước màu
    private void ReceiveWater(PipeBase pipeBase)
    {
        // fill 
        WaterColor fillValue = boxData.holderValue.Find(x => x.color == pipeBase.PipeData.waterColors[0].color);
        float subValue = Mathf.Min(pipeBase.PipeData.waterColors[0].Value, fillValue.Value);

        fillValue.Value -= subValue;

        WaterColor tempColor;
        float fillAmountValueMax;
        float fillAmountBefore; 
        float fillAmountAfter; 

        // check không còn phần tử trong list
        if (fillValue.Value <= 0)
        {
            fillAmountBefore = boxVisual.GetFillAmountToMainMesh();
            fillAmountAfter = 1f;
            DOTween.To(() => fillAmountBefore, x =>
            {
                fillAmountBefore = x;
                boxVisual.SetFillAmountToMainMesh(fillAmountBefore);
            }, fillAmountAfter, subValue * 0.5f);

            boxData.holderValue.Remove(fillValue);
        }
        else
        {
            tempColor = boxDataTemp.holderValue.Find(x => x.color == pipeBase.PipeData.waterColors[0].color);
            fillAmountValueMax = tempColor.Value;
            fillAmountBefore = boxVisual.GetFillAmountToMainMesh();
            fillAmountAfter = fillAmountBefore + subValue / fillAmountValueMax;
            DOTween.To(() => fillAmountBefore, x =>
            {
                fillAmountBefore = x;
                boxVisual.SetFillAmountToMainMesh(fillAmountBefore);
            }, fillAmountAfter, subValue * 0.5f);
        }
        // nếu không còn nữa destroy holder
        if (boxData.holderValue.Count <= 0)
        {
            DOVirtual.DelayedCall(subValue * 0.5f, () =>
            {
                boxTouchMove.IsFillMax = true;

                // voi truong hop la ice holder
                LevelManager.Instance.SubIceBreakAllHolder();

                // voi truong hop la key holder
                if (boxData.type == HolderType.Key) LevelManager.Instance.KeyBreakDown(boxData);

                LevelManager.Instance.RemoveBoxWater(this);
                transform.DOScale(0, 0.5f)
                    .OnComplete(() =>
                    {
                        Destroy(gameObject);
                    });
            });
        }
    }

    private void ReceiveSecondaryWater(PipeBase pipeBase)
    {
        // fill 
        WaterColor fillValue = boxData.secondaryHolder.holderValue.Find(x => x.color == pipeBase.PipeData.waterColors[0].color);
        float subValue = Mathf.Min(pipeBase.PipeData.waterColors[0].Value, fillValue.Value);
        fillValue.Value -= subValue;

        WaterColor tempColor;
        float fillAmountValueMax;
        float fillAmountBefore;
        float fillAmountAfter;

        // check không còn phần tử trong list
        if (fillValue.Value <= 0)
        {
            fillAmountBefore = boxVisual.GetFillAmountToLayerMesh();
            fillAmountAfter = 1f;
            DOTween.To(() => fillAmountBefore, x =>
            {
                fillAmountBefore = x;
                boxVisual.SetFillAmountToLayerMesh(fillAmountBefore);
            }, fillAmountAfter, subValue * 0.5f);

            boxData.holderValue.Remove(fillValue);

            boxData.secondaryHolder.holderValue.Remove(fillValue);
        }
        else
        {
            tempColor = boxDataTemp.holderValue.Find(x => x.color == pipeBase.PipeData.waterColors[0].color);
            fillAmountValueMax = tempColor.Value;
            fillAmountBefore = boxVisual.GetFillAmountToLayerMesh();
            fillAmountAfter = fillAmountBefore + subValue / fillAmountValueMax;
            DOTween.To(() => fillAmountBefore, x =>
            {
                fillAmountBefore = x;
                boxVisual.SetFillAmountToLayerMesh(fillAmountBefore);
            }, fillAmountAfter, subValue * 0.5f);
        }
        // nếu không còn nữa destroy holder
        if (boxData.secondaryHolder.holderValue.Count <= 0)
        {
            DOVirtual.DelayedCall(subValue * 0.5f, () =>
            {
                // voi truong hop la ice holder
                LevelManager.Instance.SubIceBreakAllHolder();

                // voi truong hop la key holder
                if (boxData.secondaryHolder.type == HolderType.Key)
                {
                    boxVisual.KeyMesh.transform.DOScale(0, 0.5f);
                    LevelManager.Instance.KeyBreakDown(boxData);
                }
                boxVisual.Stack2Layer.transform.DOScale(0, 0.5f);
            });
        }
    }

    #region IceHolder

    public void SubIceBreak()
    {
        boxData.iceBreak -= 1;
        boxVisual.UpdateVisualForIceHolder(boxData.iceBreak);
        if (boxData.iceBreak <= 0) boxVisual.OnIceBreak();
    }

    #endregion

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
