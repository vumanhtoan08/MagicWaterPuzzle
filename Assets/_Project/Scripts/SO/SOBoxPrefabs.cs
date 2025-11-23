using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewMySingleton",
    menuName = "SOSingleton/SOBoxPrefabs"
)]
public class SOBoxPrefabs : SOSingleton<SOBoxPrefabs>
{
    Dictionary<HolderShape, GameObject> dicBoxPrefabs = new();
    public List<BoxPrefabs> boxPrefabs;

    public static GameObject GetBoxPrefabs(HolderShape shape) => Instance.dicBoxPrefabs[shape];

    public override void Init()
    {
        base.Init();
        foreach (var boxPrefab in boxPrefabs)
        {
            dicBoxPrefabs[boxPrefab.shape] = boxPrefab.prefab;
        }
    }
}

[System.Serializable]
public class BoxPrefabs
{
    public HolderShape shape;
    public GameObject prefab;
}
