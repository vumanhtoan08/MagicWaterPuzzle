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



    public MapData CurrentMap => currentMap;

    public void OnStart()
    {
        if (!gridGenerator.IsNull($"chưa gán {gridGenerator.name}")) gridGenerator.OnStart();
    }
}
