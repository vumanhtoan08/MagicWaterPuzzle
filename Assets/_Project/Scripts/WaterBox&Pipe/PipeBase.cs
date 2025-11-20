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

    public virtual void FillWater()
    {
        // Coroutine
        // Lấy water trong list ở index = 0. lấy value của nó trừ đi value trong box. nếu value trong water = 0 thì remove ở index = 0 đi. sửa lại currentColor = color ở vị trí 0
    }

    public void GenWater()
    {
        headRenderer.material = SOMaterialColor.Instance.GetMaterial(pipeData.waterColors[0].color);

        float positionY = 0;
        foreach (var waterColor in pipeData.waterColors)
        {
            GameObject water = Instantiate(waterPrefab, waterHolder);
            water.transform.position = new Vector3(waterHolder.position.x,
                                                    waterHolder.position.y,
                                                    waterHolder.position.z + positionY * 3);
            water.transform.localScale = new Vector3(0.8f, waterColor.Value * 3, 1f);

            WaterBase waterBase = water.GetComponent<WaterBase>();
            waterBase.meshRenderer.material = SOMaterialColor.Instance.GetMaterial(waterColor.color);

            positionY += waterColor.Value;
        }
    }
}