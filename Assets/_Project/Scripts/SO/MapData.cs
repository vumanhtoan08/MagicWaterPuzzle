using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Custom/Grid Map Data")]
public class MapData : ScriptableObject
{
    public int width;
    public int height;
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
    public List<WaterColor> waterColors = new();

    public PipeData() { }

    public PipeData(PipeData other)
    {
        x = other.x;
        y = other.y;
        type = other.type;
        waterColors = new List<WaterColor>(other.waterColors);
    }
}


[System.Serializable]
public class WaterColor
{
    public EnumColor color;
    public float Value;
}

[System.Serializable]
public class HolderData
{
    public int x;
    public int y;
    public float rotation;

    public HolderType type;                  // basic, ice, stone...
    public List<WaterColor> holderValue;             // giá trị fill tối đa của holder

    public HolderShape shapeType;            // tên prefab
    public EnumColor color;
    
    public HolderDirection direction;
    public int iceBreak;                     // số lượt holder cần để phá băng
    public EnumColor keyColor;               // nếu None thì là không có khóa 
}
