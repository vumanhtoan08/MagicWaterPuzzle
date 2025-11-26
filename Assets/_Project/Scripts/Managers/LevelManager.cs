using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class này sẽ dùng để quản lý việc genmap, các list Holder, Pipe có trong map. Từ đó check điều kiện thắng thua. <br/>
/// Là Controller để giúp người chơi quản lý Pipe và Holder. 
/// </summary>
public class LevelManager : Singleton<LevelManager>
{
    [Header("Component REF")]
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private MapData currentMap;

    [Header("List Manager")]
    [SerializeField] private List<MapData> mapDatas = new();                                        // là nơi chưa SO của Level. 
    private Dictionary<int, MapData> dictionaryMapDatas = new(); 
    [SerializeField] private List<BoxHandleCollider> boxHandleColliders = new();                    // chứa Box có mặt trong map để check thắng thua.
    [SerializeField] private List<PipeBase> pipes = new();                                          // chứa Pipe có mặt tỏng map.   

    [Header("Game para")]
    [SerializeField] private float currentTime;
    public bool IsStart { get; set; }
    
    public MapData CurrentMap => currentMap;

    #region Unity Methods

    public void OnStart()
    {
        LoadAllMapsInResouces();

        LoadMapToDictinary();
        //if (!gridGenerator.IsNull($"chưa gán {gridGenerator.name}")) gridGenerator.OnGenerateMap();

        boxHandleColliders.ForEach((b) =>
        {
            if (!b.IsNull("Không có BoxHandleCollider")) b.OnStart();
        });
    }

    public void OnUpdate()
    {
        boxHandleColliders.ForEach((b) =>
        {
            if (!b.IsNull("Không có BoxHandleCollider")) b.OnUpdate();
        });
    }

    #endregion

    #region BoxWater Handle 

    public void AddBoxWater(BoxHandleCollider boxHandleCollider)
    {
        boxHandleColliders.Add(boxHandleCollider);
    }

    public void RemoveBoxWater(BoxHandleCollider boxHandleCollider)
    {
        boxHandleColliders.Remove(boxHandleCollider);
    }

    public void RemoveAllBoxWater()
    {
        boxHandleColliders.Clear();
    }

    #region Ice

    public void SubIceBreakAllHolder()
    {
        List<BoxHandleCollider> iceBoxs = boxHandleColliders.FindAll(x => x.BoxData.type == HolderType.Ice);

        if (iceBoxs.Count <= 0) return;

        iceBoxs.ForEach(i => i.SubIceBreak());                                                                                     // ForEach
    }

    #endregion

    #region Key 

    public void KeyBreakDown(HolderData data)
    {
        List<PipeBase> pipeLock = pipes.FindAll(x => x.PipeData.type == PipeType.Lock);

        foreach (PipeBase pipe in pipeLock)
        {
            if (pipe.PipeData.keyColor == data.keyColor || pipe.PipeData.keyColor == data.secondaryHolder.keyColor)
            {
                // BreakDown Key di
                pipe.LockVisual.transform.DOScale(0, 0.5f);
                pipe.PipeData.type = PipeType.Basic;
            }
        }
    }

    #endregion

    #endregion

    #region Pipe Handle

    public void AddPipeWater(PipeBase pipeBase)
    {
        pipes.Add(pipeBase);
    }

    public void RemovePipeWater(PipeBase pipeBase)
    {
        pipes.Remove(pipeBase);
    }

    public void RemoveAllPupeWater()
    {
        pipes.Clear();
    }

    #endregion

    #region Boosters

    // Đóng băng: đóng băng thời gian trong 20s

    // Bom: phá hủy random một khối trên map 

    // Búa: phá hủy 1 khối chỉ định trên map 

    #endregion

    #region Game Running

    // Quản lý việc giảm thời gian của game

    // Quản lý thắng thua của game

    #endregion

    #region Game Load Level

    public void LoadLevel(int level)
    {
        // Clear old map
        gridGenerator.ClearGrid();
        RemoveAllBoxWater();
        RemoveAllPupeWater();

        currentMap = mapDatas[level];

        gridGenerator.OnGenerateMap();

        OnStart(); // chạy init cho Box / Pipe
    }

    public void LoadMapToDictinary()
    {
        int index = 1; 

        foreach (var mapData in mapDatas)
        {
            dictionaryMapDatas[index] = mapData;
            index++;
        }
        foreach (var item in dictionaryMapDatas)
        {
            Debug.Log($"MapData: {item.Key} --- {item.Value.name}");
        }
    }

    private void LoadAllMapsInResouces()
    {
        mapDatas.Clear();

        var loadedMaps = Resources.LoadAll<MapData>("Level");

        foreach (var map in loadedMaps)
        {
            mapDatas.Add(map);
        }
    }

    #endregion

}
