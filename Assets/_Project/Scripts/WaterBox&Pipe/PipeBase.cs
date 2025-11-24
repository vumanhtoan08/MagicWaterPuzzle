using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PipeBase : MonoBehaviour
{
    [Header("Pipe Data")]
    [SerializeField] private PipeData pipeData;

    [Header("Visual Pipe")]
    [SerializeField] private Transform waterHolder;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private Renderer headRenderer;
    private float durationTime = 0.5f;

    public bool IsFilling { get; set; }

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

    public virtual void FillWater(BoxTouchMove boxTouchMove, HolderData data)
    {
        if (IsFilling) return;
        IsFilling = true;

        StartCoroutine(FillingWater(boxTouchMove, data));
    }

    private IEnumerator FillingWater(BoxTouchMove boxTouchMove, HolderData data)
    {
        float subValue = Mathf.Min(pipeData.waterColors[0].Value, data.holderValue[0].Value);
        Debug.Log($"Sub Value: {subValue}");
        pipeData.waterColors[0].Value -= subValue;

        Transform firstWater = waters[0];
        //firstWater.DOKill();

        if (pipeData.waterColors[0].Value <= 0)
        {
            firstWater.DOScaleY(0f, subValue * durationTime).OnComplete(() =>
            {
                RemoveWater();
                RemoveWaterTrans(firstWater);
                headRenderer.material = SOMaterialColor.GetMaterial(pipeData.waterColors[0].color);
            });

            float positionY = 0;

            for (int i = 0; i < waters.Count; i++)
            {
                //waters[i].DOKill();
                waters[i].DOLocalMoveZ(positionY * -3, subValue * durationTime);
                positionY += pipeData.waterColors[i].Value;
            }
        }
        else
        {
            firstWater.DOScaleY(pipeData.waterColors[0].Value * 3f, subValue * durationTime);

            float positionY = pipeData.waterColors[0].Value;

            for (int i = 1; i < waters.Count; i++)
            {
                //waters[i].DOKill();
                waters[i].DOLocalMoveZ(positionY * -3, subValue * durationTime);
                positionY += pipeData.waterColors[i].Value;
            }
        }

        yield return new WaitForSeconds(subValue * durationTime);

        IsFilling = false;
    }

    public bool CheckBoxCondition(HolderData data)
    {
        if (pipeData.waterColors.Count <= 0) return false;

        if (data.color == pipeData.waterColors[0].color && pipeData.waterColors[0].Value > 0)
        {
            return true;
        }

        return false;
    }

    public void GenWater()
    {
        if (pipeData.waterColors.Count <= 0) return;

        headRenderer.material = SOMaterialColor.GetMaterial(pipeData.waterColors[0].color);

        float positionY = 0;
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
        }
    }


    #region Waters Transform

    [Header("Trans water in Pipe")]
    [SerializeField] private List<Transform> waters;

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

}