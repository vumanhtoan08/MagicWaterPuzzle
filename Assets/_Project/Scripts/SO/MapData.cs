using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Custom/Grid Map Data")]
public class MapData : ScriptableObject
{
    public int width;
    public int height;

    public int level; 
    public LevelDifficult levelDifficult;
    public float time; 

    public List<NodeData> nodes = new List<NodeData>();
    public List<PipeData> pipes = new List<PipeData>();
    public List<HolderData> holders = new List<HolderData>();
}

[System.Serializable]
public class NodeData
{
    public int x;
    public int y;
    public bool isSpawn;

    public NodeData(int x, int y, bool isSpawn)
    {
        this.x = x;
        this.y = y;
        this.isSpawn = isSpawn;
    }
}

[System.Serializable]
public class PipeData
{
    public int x;
    public int y;
    public PipeType type;
    public EnumColor keyColor;
    public List<WaterColor> waterColors = new();

    public PipeData() { }

    // Deep copy constructor
    public PipeData(PipeData other)
    {
        x = other.x;
        y = other.y;
        type = other.type;
        keyColor = other.keyColor;

        // Copy deep từng WaterColor
        waterColors = new List<WaterColor>();
        foreach (var wc in other.waterColors)
        {
            waterColors.Add(new WaterColor(wc));
        }
    }
}

[System.Serializable]
public class WaterColor
{
    public EnumColor color;
    public float Value;

    public WaterColor() { }

    public WaterColor(WaterColor other)
    {
        color = other.color;
        Value = other.Value;
    }
}

[System.Serializable]
public class HolderData
{
    public int x;
    public int y;
    public float rotation;

    public HolderType type;                  // basic, ice, stone...
    public List<WaterColor> holderValue;     // giá trị fill tối đa của holder

    public HolderShape shapeType;            // tên prefab
    public EnumColor color;

    public HolderDirection direction;
    public int iceBreak;                     // số lượt holder cần để phá băng
    public EnumColor keyColor;               // nếu None thì là không có khóa
    public EnumColor secondaryColor;
    public SecondaryHolder secondaryHolder;

    public HolderData() { }

    // DEEP COPY CONSTRUCTOR
    public HolderData(HolderData other)
    {
        x = other.x;
        y = other.y;
        rotation = other.rotation;

        type = other.type;
        shapeType = other.shapeType;
        color = other.color;

        direction = other.direction;
        iceBreak = other.iceBreak;
        keyColor = other.keyColor;
        secondaryColor = other.secondaryColor;
        secondaryHolder = other.secondaryHolder != null ? new SecondaryHolder(other.secondaryHolder) : null;

        // IMPORTANT: Deep copy List<WaterColor>
        holderValue = new List<WaterColor>();
        if (other.holderValue != null)
        {
            foreach (var wc in other.holderValue)
            {
                holderValue.Add(new WaterColor(wc));
            }
        }
    }
}

[System.Serializable]
public class SecondaryHolder
{
    public HolderType type;
    public EnumColor color;
    public List<WaterColor> holderValue;
    public EnumColor keyColor;

    public SecondaryHolder() 
    { 
        type = HolderType.Basic;
        color = EnumColor.red;
        holderValue = new List<WaterColor>();
        keyColor = EnumColor.None;
    }

    public SecondaryHolder(SecondaryHolder other)
    {
        type = other.type;
        color = other.color;
        keyColor = other.keyColor;

        holderValue = new List<WaterColor>();
        if (other.holderValue != null)
        {
            foreach (var wc in other.holderValue)
                holderValue.Add(new WaterColor(wc));
        }
    }
}

public enum LevelDifficult
{
    Normal, 
    Hard,
    SuperHard
}