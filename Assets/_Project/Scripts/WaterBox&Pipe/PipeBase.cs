using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeBase : MonoBehaviour
{
    [Header("Pipe Data")]
    [SerializeField] private PipeData pipeData;

    [Header("Visual Pipe")]
    [SerializeField] private Transform waterHolder;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private Renderer headRenderer;

    public bool IsFilling { get; set; }

    /// <summary>
    /// Lấy Data cho Pipe ở trong MapData khi Init
    /// </summary>
    public void GetPipeData(PipeData newPipeData)
    {
        pipeData = new PipeData(newPipeData);
    }

    public virtual void RemoveWater(WaterColor waterColor)
    {
        pipeData.waterColors.Remove(waterColor);
    }

    public virtual void FillWater(BoxTouchMove boxTouchMove, HolderData data, List<WaterColor> waterColors)
    {
        // Coroutine
        StartCoroutine(FillingWater(boxTouchMove, data, waterColors));

        // Lấy water trong list ở index = 0. lấy value của nó trừ đi value trong box. nếu value trong water = 0 thì remove ở index = 0 đi. sửa lại currentColor = color ở vị trí 0
    }

    private IEnumerator FillingWater(BoxTouchMove boxTouchMove, HolderData data, List<WaterColor> waterColors)
    {
        IsFilling = true;                   // ngăn không bị lặp lại 

        yield return new WaitForSeconds(1f); // chờ 1 giây

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

            positionY += waterColor.Value;
        }
    }

}