
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
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
            }, fillAmountAfter, subValue * 0.25f);

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
            }, fillAmountAfter, subValue * 0.25f);
        }
        // nếu không còn nữa destroy holder
        if (boxData.holderValue.Count <= 0)
        {
            DOVirtual.DelayedCall(subValue * 0.25f, () =>
            {
                boxTouchMove.IsFillMax = true;

                // voi truong hop la ice holder
                LevelManager.Instance.SubIceBreakAllHolder();

                // voi truong hop la key holder
                if (boxData.type == HolderType.Key) LevelManager.Instance.KeyBreakDown(boxData);

                LevelManager.Instance.RemoveBoxWater(this);
                //transform.DOScale(0, 0.25f)
                //    .OnComplete(() =>
                //    {
                //        Destroy(gameObject);
                //    });
                AnimWhenBoxFillMax();
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
            }, fillAmountAfter, subValue * 0.25f);

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
            }, fillAmountAfter, subValue * 0.25f);
        }
        // nếu không còn nữa destroy holder
        if (boxData.secondaryHolder.holderValue.Count <= 0)
        {
            DOVirtual.DelayedCall(subValue * 0.25f, () =>
            {
                // voi truong hop la ice holder
                LevelManager.Instance.SubIceBreakAllHolder();

                // voi truong hop la key holder
                if (boxData.secondaryHolder.type == HolderType.Key)
                {
                    boxVisual.KeyMesh.transform.DOScale(0, 0.25f);
                    LevelManager.Instance.KeyBreakDown(boxData);
                }
                boxVisual.Stack2Layer.transform.DOScale(0, 0.5f);
            });
        }
    }
    public void BoxBreak(List<PipeBase> pipeBases)
    {
        List<WaterColor> waterColorsInBox = new List<WaterColor>();

        if (boxData.type == HolderType.Stack2)
        {
            waterColorsInBox.AddRange(boxData.secondaryHolder.holderValue);
            waterColorsInBox.AddRange(boxData.holderValue);
        }
        else
            waterColorsInBox.AddRange(boxData.holderValue);

        LevelManager.Instance.SubIceBreakAllHolder();
        if (boxData.type == HolderType.Key) LevelManager.Instance.KeyBreakDown(boxData);
        if (boxData.type == HolderType.Stack2)
        {
            if (boxData.secondaryHolder.type == HolderType.Key) LevelManager.Instance.KeyBreakDown(boxData);
        }

        foreach (PipeBase pipeBase in pipeBases)
        {
            pipeBase.FillingWaterWhenBoxBreak(pipeBase, waterColorsInBox, this);
            //pipeBase.UpdateWaterWhenBoxBreak(pipeBase, waterColorsInBox, this);
        }

        /*
        // Duyệt từng pipe
        foreach (var pipe in pipeBases)
        {
            // Duyệt từng water của pipe
            foreach (var water in pipe.PipeData.waterColors.ToList())
            {
                // Không còn waterBox nào -> dừng hẳn
                if (waterColorsInBox.Count == 0)
                    break;

                // Duyệt Box Water để trừ
                for (int i = 0; i < waterColorsInBox.Count; i++)
                {
                    var waterBox = waterColorsInBox[i];

                    // Nếu khác màu -> bỏ qua
                    if (water.color != waterBox.color)
                        continue;

                    // Trừ nước
                    float sub = Mathf.Min(waterBox.Value, water.Value);
                    waterBox.Value -= sub;
                    water.Value -= sub;

                    Debug.Log($"[BoxBreak] Box:{gameObject.name} - Pipe:{pipe.gameObject.name} - Color:{water.color} - Sub:{sub} - BoxLeft:{waterBox.Value} - PipeLeft:{water.Value}");

                    // Nếu nước trong box hết
                    if (waterBox.Value <= 0)
                    {
                        waterColorsInBox.RemoveAt(i);
                        i--; // dồn lại index

                        // Nếu nước trong pipe cũng hết → dừng xử lý water này, chuyển sang water khác / pipe khác
                        if (water.Value <= 0)
                        {
                            pipe.RemoveWater(water);
                            break;  // break vòng for waterBox → chuyển sang water tiếp theo hoặc pipe tiếp theo
                        }

                        // Nếu pipe còn nước → tiếp tục lặp lại pipe này và box water kế tiếp
                        continue;
                    }
                    else
                    {
                        // Box chưa hết nhưng pipe hết → remove water và chuyển sang PIPE tiếp theo
                        if (water.Value <= 0)
                        {
                            pipe.RemoveWater(water);
                            goto NextPipe; // NHẢY RA KHỎI water hiện tại và SANG PIPE TIẾP THEO
                        }
                    }
                }
            NextPipe:
                continue;
            }
        }

        LevelManager.Instance.RemoveBoxWater(this);
        */
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

    [Header("Effect")]
    [SerializeField] private GameObject box_hit_effect;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float duration;

    public void AnimWhenBoxFillMax()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMoveZ(-2f, 0.1f).SetEase(Ease.InCubic))
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                Transform effect = ObjectPooling.GetObject(SODictionaryEffect.GetEffectByType(EffectType.BoxClear), new Vector3(boxVisual.CenterPos.position.x, boxVisual.CenterPos.position.y, -3));
                DOVirtual.DelayedCall(0.5f, () => ObjectPooling.ReturnObject(effect));
            })
            .AppendCallback(() =>
            {
                // Bay sang phải
                if (Mathf.RoundToInt(transform.position.x) > Mathf.RoundToInt(LevelManager.Instance.CurrentMap.width / 2))
                {
                    transform.DOLocalJump(new Vector3(-10, transform.localPosition.y - 40f, transform.localPosition.z), 25, 1, 3);
                }
                // Bay sang trái
                else
                {
                    transform.DOLocalJump(new Vector3(-10, transform.localPosition.y + 40f, transform.localPosition.z), -25, 1, 3);
                }
            });
        //.OnComplete(() =>
        //{
        //    Destroy(gameObject);
        //});
    }
}
