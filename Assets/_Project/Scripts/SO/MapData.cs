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
}
