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
    [SerializeField] private List<BoxHandleCollider> boxHandleColliders = new();                    // chứa Box có mặt trong map để check thắng thua.
    [SerializeField] private List<PipeBase> pipes = new();                                          // chứa Pipe có mặt tỏng map.

    public MapData CurrentMap => currentMap;

    public void OnStart()
    {
        if (!gridGenerator.IsNull($"chưa gán {gridGenerator.name}")) gridGenerator.OnStart();
    }

    #region BoxWater Handle 

    public void AddBoxWater(BoxHandleCollider boxHandleCollider)
    {
        boxHandleColliders.Add(boxHandleCollider);
    }

    public void RemoveBoxWater(BoxHandleCollider boxHandleCollider)
    {
        boxHandleColliders.Remove(boxHandleCollider);
    }

    #region Ice

    public void SubIceBreakAllHolder()
    {
        List<BoxHandleCollider> iceBoxs = boxHandleColliders.FindAll(x => x.BoxData.type == HolderType.Ice);
        
        if(iceBoxs.Count <= 0) return;

        iceBoxs.ForEach(i => i.SubIceBreak());                                                                                     // ForEach
    }

    #endregion

    #endregion
}
