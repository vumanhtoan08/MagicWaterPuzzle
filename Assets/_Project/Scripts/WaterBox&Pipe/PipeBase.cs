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
    [SerializeField] private float durationTime = 0.25f;

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
        pipeData.waterColors.RemoveAt(0);
    }

    public virtual void FillWater(BoxTouchMove boxTouchMove, HolderData data)
    {
        // Coroutine
        StartCoroutine(FillingWater(boxTouchMove, data));

        // Lấy water trong list ở index = 0. lấy value của nó trừ đi value trong box. nếu value trong water = 0 thì remove ở index = 0 đi. sửa lại currentColor = color ở vị trí 0
    }

    // Xử lý Logic và Visual trong lúc Fill 
    private IEnumerator FillingWater(BoxTouchMove boxTouchMove, HolderData data)
    {
        IsFilling = true;                   // ngăn không bị lặp lại 

        // Tính toán lượng giá trị còn lại 
        // tính giá trị thực tế khi trừ 
        float subValue = Mathf.Min(pipeData.waterColors[0].Value, data.holderValue[0].Value);       // giá trị thực tế phải trừ trong Pipe
        Debug.Log($"Sub Value: {subValue}");
        pipeData.waterColors[0].Value -= subValue;

        // Xử lý việc giảm nước của Visual
        Transform firstWater = waters[0];

        // Pipe Fill hết nước
        if (pipeData.waterColors[0].Value <= 0)
        {
            // xử lý visual
            firstWater.DOScaleY(0f, subValue * durationTime).OnComplete(() =>
            {
                RemoveWater();
                RemoveWaterTrans(firstWater);
            });

            float positionY = 0;

            for (int i = 0; i < waters.Count; i++)
            {
                waters[i].DOLocalMoveZ(positionY * -3, subValue * durationTime);
                positionY += pipeData.waterColors[i].Value;
            }
        }
        // Pipe fill nhưng trong Pipe vẫn còn nước
        else
        {
            firstWater.DOScaleY(pipeData.waterColors[0].Value * 3f, subValue * durationTime);
        }

        yield return new WaitForSeconds(subValue * durationTime); // chờ 1 giây

        IsFilling = false;
    }

    // điều kiện để box được fill là có cùng màu và trong watercolor còn giá trị 
    public bool CheckBoxCondition(HolderData data)
    {
        if (pipeData.waterColors.Count <= 0) return false;

        if (data.color == pipeData.waterColors[0].color && pipeData.waterColors[0].Value > 0)
        {
            return true;
        }

        return false;
    }

    // Dùng cho Visual
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
        waters.Remove(transform);
    }

    #endregion

}