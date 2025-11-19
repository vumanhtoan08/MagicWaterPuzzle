using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeBase : MonoBehaviour
{
    [Header("Data Contain Water")]
    [SerializeField] protected List<WaterColor> waterColors = new();
    [SerializeField] protected Vector2Int correctFillInGrid;
    [SerializeField] protected EnumColor currentColor; 

    [Header("Visual Pipe")]
    [SerializeField] protected float rotationZ;

    public virtual void RemoveWater(WaterColor waterColor)
    {
        waterColors.Remove(waterColor);
    }

    public virtual void FillWater()
    {
        // Coroutine
        // Lấy water trong list ở index = 0. lấy value của nó trừ đi value trong box. nếu value trong water = 0 thì remove ở index = 0 đi. sửa lại currentColor = color ở vị trí 0
    }
}

[System.Serializable]
public class WaterColor
{
    public EnumColor color;
    public int Value; 
}