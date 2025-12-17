using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PipeBase : MonoBehaviour
{
    [Header("Pipe Data")]
    [SerializeField] private PipeData pipeData;

    [Header("Visual Pipe")]
    [SerializeField] private GameObject lockVisualObj;
    [SerializeField] private Renderer lockMaterials;

    [SerializeField] private Transform waterHolder;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private Renderer headRenderer;

    [Header("Pipe Bubble")]
    [SerializeField] private ParticleSystem idleBubble;
    [SerializeField] private ParticleSystem activeBubble;
    private float durationTime = 0.4f;

    [Header("Pipe Water_Flow")]
    [SerializeField] private Renderer waterFlow_U;
    [SerializeField] private Renderer waterFlow_R;
    [SerializeField] private Renderer waterFlow_L;

    public bool IsFilling { get; set; }
    public PipeData PipeData => pipeData;
    public GameObject LockVisual => lockVisualObj;
    public Renderer LockMaterials => lockMaterials;
    /// <summary>
    /// Lấy Data cho Pipe ở trong MapData khi Init
    /// </summary>
    public void GetPipeData(PipeData newPipeData)
    {
        pipeData = new PipeData(newPipeData);
    }

    public virtual void RemoveWater()
    {
        if (waters.Count > 0)
        {
            waters[0].DOKill();
        }

        pipeData.waterColors.RemoveAt(0);
    }

    public void RemoveWater(WaterColor waterColor)
    {
        pipeData.waterColors.Remove(waterColor);
    }

    public virtual void FillWater(BoxTouchMove boxTouchMove, HolderData data)
    {
        if (IsFilling) return;
        IsFilling = true;
        activeBubble.Play();

        StartCoroutine(FillingWater(boxTouchMove, data));
        AudioManager.Instance.PlayOneShot(SoundKey.WaterPOUR2, 0.7f);
        DOVirtual.DelayedCall(0.63f, () => AudioManager.Instance.PlayOneShot(SoundKey.WaterPOUR1, 0.7f));
    }

    public void UpdateVisualWaterFlow(HolderData data)
    {
        float fillAmountBefore;
        float fillAmountAfter;
        float z = Mathf.Round(transform.eulerAngles.z);

        if (z == 180f)
        {
            waterFlow_U.material = SOMaterialColor.GetMaterialWaterFlow(pipeData.waterColors[0].color);
            SetFillAmountToLayerMesh(waterFlow_U, 0);
            waterFlow_U.gameObject.SetActive(true);

            fillAmountBefore = GetFillAmountToLayerMesh(waterFlow_U);
            fillAmountAfter = 1f;
            DOTween.To(() => fillAmountBefore, x =>
            {
                fillAmountBefore = x;
                SetFillAmountToLayerMesh(waterFlow_U, fillAmountBefore);
            }, fillAmountAfter, 0.25f);
        }
        else if (z == 90f)
        {
            waterFlow_R.material = SOMaterialColor.GetMaterialWaterFlow(pipeData.waterColors[0].color);
            SetFillAmountToLayerMesh(waterFlow_R, 0);
            waterFlow_R.gameObject.SetActive(true);

            fillAmountBefore = GetFillAmountToLayerMesh(waterFlow_R);
            fillAmountAfter = 1f;
            DOTween.To(() => fillAmountBefore, x =>
            {
                fillAmountBefore = x;
                SetFillAmountToLayerMesh(waterFlow_R, fillAmountBefore);
            }, fillAmountAfter, 0.25f);
        }
        else if (z == 270f)
        {
            waterFlow_L.material = SOMaterialColor.GetMaterialWaterFlow(pipeData.waterColors[0].color);
            SetFillAmountToLayerMesh(waterFlow_L, 0);
            waterFlow_L.gameObject.SetActive(true);

            fillAmountBefore = GetFillAmountToLayerMesh(waterFlow_L);
            fillAmountAfter = 1f;
            DOTween.To(() => fillAmountBefore, x =>
            {
                fillAmountBefore = x;
                SetFillAmountToLayerMesh(waterFlow_L, fillAmountBefore);
            }, fillAmountAfter, 0.25f);
        }
    }

    // sửa lại để có thể thực hiện với nhiều màu nước
    private IEnumerator FillingWater(BoxTouchMove boxTouchMove, HolderData data)
    {

        WaterColor goalWaterColor;
        float subValue;

        UpdateVisualWaterFlow(data);

        if (data.type == HolderType.Stack2 && data.secondaryHolder.holderValue.Count > 0)
        {
            goalWaterColor = data.secondaryHolder.holderValue.Find(x => x.color == pipeData.waterColors[0].color);
            subValue = Mathf.Min(pipeData.waterColors[0].Value, goalWaterColor.Value);
            pipeData.waterColors[0].Value -= subValue;
        }
        else
        {
            goalWaterColor = data.holderValue.Find(x => x.color == pipeData.waterColors[0].color);
            subValue = Mathf.Min(pipeData.waterColors[0].Value, goalWaterColor.Value);
            pipeData.waterColors[0].Value -= subValue;
        }

        Transform firstWater = waters[0];
        //firstWater.DOKill();

        if (pipeData.waterColors[0].Value <= 0)
        {
            firstWater.DOScaleY(0f, subValue * durationTime).OnComplete(() =>
            {
                RemoveWater();
                RemoveWaterTrans(firstWater);
                if (pipeData.waterColors.Count <= 0)
                {
                    idleBubble.Stop();
                    headRenderer.material = SOMaterialColor.GetMaterial(EnumColor.None);
                }
                else
                    headRenderer.material = SOMaterialColor.GetMaterial(pipeData.waterColors[0].color);
            });

            float positionY = 0;

            if (waters.Count > 0)
            {
                for (int i = 1; i < waters.Count; i++)
                {
                    //waters[i].DOKill();
                    waters[i].DOLocalMoveZ(positionY * -3, subValue * durationTime);
                    positionY += pipeData.waterColors[i].Value;
                }
            }
        }
        else
        {
            firstWater.DOScaleY(pipeData.waterColors[0].Value * 3f, subValue * durationTime);

            float positionY = pipeData.waterColors[0].Value;

            if (waters.Count > 0)
            {
                for (int i = 1; i < waters.Count; i++)
                {
                    //waters[i].DOKill();
                    waters[i].DOLocalMoveZ(positionY * -3, subValue * durationTime);
                    positionY += pipeData.waterColors[i].Value;
                }
            }
        }

        yield return new WaitForSeconds(subValue * durationTime);

        IsFilling = false;
        boxTouchMove.IsFilling = false;
        activeBubble.Stop(false);
        waterFlow_U.gameObject.SetActive(false);
        waterFlow_R.gameObject.SetActive(false);
        waterFlow_L.gameObject.SetActive(false);
    }

    public void FillingWaterWhenBoxBreak(PipeBase pipeBase, List<WaterColor> waterColorsInBox, BoxHandleCollider boxCollider)
    {
        foreach (var water in pipeBase.PipeData.waterColors.ToList())
        {
            if (waterColorsInBox.Count == 0) break;

            for (int i = 0; i < waterColorsInBox.Count; i++)
            {
                var waterBox = waterColorsInBox[i];

                if (water.color != waterBox.color) continue;

                float sub = Mathf.Min(waterBox.Value, water.Value);
                waterBox.Value -= sub;
                water.Value -= sub;

                //Debug.Log($"[BoxBreak] Box:{gameObject.name} - Pipe:{pipeBase.gameObject.name} - Color:{water.color} - Sub:{sub} - BoxLeft:{waterBox.Value} - PipeLeft:{water.Value}");
                int waterIndex = pipeBase.PipeData.waterColors.IndexOf(water);

                if (waterBox.Value <= 0)
                {
                    waterColorsInBox.RemoveAt(i);
                    i--;

                    if (water.Value <= 0)
                    {
                        pipeBase.RemoveWater(water);
                        pipeBase.RemoveWaterTrans(waters[waterIndex]);
                        break;
                    }

                    continue;
                }
                else
                {
                    if (water.Value <= 0)
                    {
                        pipeBase.RemoveWater(water);
                        pipeBase.RemoveWaterTrans(waters[waterIndex]);
                        goto NextPipe;
                    }
                }
            }
        NextPipe:
            continue;
        }

        LevelManager.Instance.RemoveBoxWater(boxCollider);

        UpdateWaterWhenBoxBreak(pipeBase, waterColorsInBox, boxCollider);
    }

    public void UpdateWaterWhenBoxBreak(PipeBase pipeBase, List<WaterColor> waterColorsInBox, BoxHandleCollider boxCollider)
    {
        Dictionary<Transform, float> childrenDic = new();
        List<Transform> children = new List<Transform>();

        foreach (Transform child in waterHolder)
        {
            children.Add(child);
        }

        foreach (Transform child in children.ToList())
        {
            if (waters.Contains(child))
            {
                int childIndex = waters.IndexOf(child);
                childrenDic[waters[childIndex]] = pipeBase.pipeData.waterColors[childIndex].Value;

                children.Remove(child);
            }
        }

        // Scale những vị trí bị hết trong Pipe
        foreach (var child in children)
        {
            child.DOScaleY(0, 0.5f).OnComplete(() =>
            {
                if (pipeData.waterColors.Count <= 0)
                {
                    idleBubble.Stop();
                    headRenderer.material = SOMaterialColor.GetMaterial(EnumColor.None);
                }
                else
                    headRenderer.material = SOMaterialColor.GetMaterial(pipeData.waterColors[0].color);
            });
        }

        foreach (var child in childrenDic)
        {
            //Debug.Log($"Name: {child.Key.gameObject.name} -- Value: {child.Value}");
            child.Key.DOScaleY(child.Value * 3f, 0.5f);
        }

        float positionY = 0;

        if (waters.Count > 0)
        {
            for (int i = 0; i < waters.Count; i++)
            {
                //waters[i].DOKill();
                waters[i].DOLocalMoveZ(positionY * -3, 0.5f);
                positionY += pipeData.waterColors[i].Value;
            }
        }
    }

    public bool CheckBoxCondition(HolderData data)
    {
        if (pipeData.type == PipeType.Lock) return false;                   // đang là Lock không cho Fill luôn 

        if (pipeData.waterColors.Count <= 0) return false;

        switch (data.type)
        {
            case HolderType.Basic:
                foreach (var holder in data.holderValue)
                {
                    if (pipeData.waterColors[0].color == holder.color) return true;
                }
                break;

            case HolderType.Ice:
                if (data.iceBreak > 0) return false;
                foreach (var holder in data.holderValue)
                {
                    if (pipeData.waterColors[0].color == holder.color) return true;
                }
                break;

            case HolderType.Direction:
                foreach (var holder in data.holderValue)
                {
                    if (pipeData.waterColors[0].color == holder.color) return true;
                }
                break;

            case HolderType.Stone:
                return false;

            case HolderType.Key:
                foreach (var holder in data.holderValue)
                {
                    if (pipeData.waterColors[0].color == holder.color) return true;
                }
                break;

            case HolderType.Lock:
                break;

            case HolderType.MergeColor:
                foreach (var holder in data.holderValue)
                {
                    if (pipeData.waterColors[0].color == holder.color) return true;
                }
                break;

            case HolderType.Stack2:
                if (data.secondaryHolder.holderValue.Count <= 0)
                {
                    foreach (var holder in data.holderValue)
                    {
                        if (pipeData.waterColors[0].color == holder.color) return true;
                    }
                }

                if (pipeData.waterColors[0].color == data.secondaryHolder.color) return true;

                break;
        }

        return false;
    }

    public void GenWater()
    {
        if (pipeData.waterColors.Count <= 0) return;

        headRenderer.material = SOMaterialColor.GetMaterial(pipeData.waterColors[0].color);

        float positionY = 0;
        int index = 0;

        foreach (var waterColor in pipeData.waterColors)
        {
            GameObject water = Instantiate(waterPrefab, waterHolder);
            water.transform.localPosition = new Vector3(
                                                            0,
                                                            0,
                                                            positionY * -3
                                                        );
            water.transform.localScale = new Vector3(0.8f, waterColor.Value * 3, 1f);

            WaterBase waterBase = water.GetComponent<WaterBase>();
            waterBase.meshRenderer.material = SOMaterialColor.GetMaterial(waterColor.color);
            waters.Add(water.transform);

            positionY += waterColor.Value;

            water.name = $"Water {index}";
            index++;
        }
    }


    #region Waters Transform

    [Header("Trans water in Pipe")]
    [SerializeField] private List<Transform> waters;
    public List<Transform> Waters => waters;

    public void AddWaterTrans(Transform transform)
    {
        waters.Add(transform);
    }

    public void RemoveWaterTrans(Transform transform)
    {
        transform.DOKill();
        waters.Remove(transform);
    }

    #endregion

    private float GetFillAmountToLayerMesh(Renderer mesh) => mesh.material.GetFloat("_FillAmount");
    private void SetFillAmountToLayerMesh(Renderer mesh, float amount) => mesh.material.SetFloat("_FillAmount", amount);
}