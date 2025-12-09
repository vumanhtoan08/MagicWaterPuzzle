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

    public MapData CurrentMap => currentMap;
    public bool IsBoxTouched { get; set; }

    public bool IsLevelGenComplete { get; set; }

    #region Unity Methods

    public void OnStart()
    {
        LoadAllMapsInResouces();
        LoadMapToDictinary();
        //LoadLevel(levelTest);
    }

    public void OnUpdate()
    {
        Debug.Log(IsLevelGenComplete);
        if (!IsLevelGenComplete) return;

        boxHandleColliders.ForEach((b) =>
        {
            if (!b.IsNull("Không có BoxHandleCollider")) b.OnUpdate();
        });

        SubCounterTime();
        SubFrozeCounterTime();
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
        //iceBoxs.ForEach(i => i.SubIceBreak());  
        foreach (BoxHandleCollider box in iceBoxs)
        {
            box.SubIceBreak();
            if (box.BoxData.iceBreak <= 0) box.BoxData.type = HolderType.Basic;
        }
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

    #region Froze Đóng băng: đóng băng thời gian trong 20s
    [Header("Froze Setting")]
    [SerializeField] private float frozeDuration = 20f;                                            // hiệu lực của trạng thái đóng băng
    [SerializeField] private float frozeTimeCouter = 0f;                                                // bộ đếm hiệu lực 
    public float FrozeTimeCouter => frozeTimeCouter;
    public bool IsFroze { get; set; }
    public void OnFrozeBoosterActive()
    {
        InitFrozeBooster();
    }

    private void InitFrozeBooster()
    {
        frozeTimeCouter += frozeDuration;
        IsFroze = true;
        AudioManager.Instance.PlayOneShot(SoundKey.Freeze, 1f);
    }

    private void SubFrozeCounterTime()
    {
        if (!IsFroze) return;

        frozeTimeCouter -= Time.deltaTime;
        frozeTimeCouter = Mathf.Clamp(frozeTimeCouter, 0, Mathf.Infinity);

        if (frozeTimeCouter <= 0)
        {
            AudioManager.Instance.PlayOneShot(SoundKey.EndFreeze, 1f);
            IsFroze = false;
        }
    }

    #endregion

    [SerializeField] private Camera mainCam;

    #region Bomb: phá hủy random 1 khối trong map
    // Búa: phá random một box trong map

    [SerializeField] private GameObject bombPrefab;
    private bool isBombActive;

    public void OnBombBoosterActive()
    {
        if (boxHandleColliders.Count <= 0) return;
        if (isBombActive) return;
        isBombActive = true;

        List<BoxHandleCollider> boxEnableDestroy = boxHandleColliders.FindAll(x => x.BoxData.type != HolderType.Ice && x.BoxData.type != HolderType.Stone);         // lấy hết tất cả box trên sân trừ ice và stone
        if (boxEnableDestroy.Count <= 0) return;
        BoxHandleCollider boxHandleCollider = boxEnableDestroy[Random.Range(0, boxEnableDestroy.Count)];

        GameplayScreen gameplayScreen = UIManager.Instance.GetScreenActive<GameplayScreen>();
        gameplayScreen.ListObjInScreen.ForEach(x => x.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        gameplayScreen.FrozenBtn.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
        gameplayScreen.BombBtn.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
        gameplayScreen.HammerBtn.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() => gameplayScreen.gameObject.SetActive(false));

        Vector3 startPos = new Vector3(mainCam.transform.position.x, -mainCam.orthographicSize * 1.5f, -5f);
        Vector3 endPos = boxHandleCollider.BoxVisual.CenterPos.position;

        GameObject bomb = Instantiate(bombPrefab, startPos, Quaternion.identity);

        if (mainCam.orthographicSize >= 20f)
        {
            bomb.transform.localScale = new Vector3(20f / mainCam.orthographicSize, 20 / mainCam.orthographicSize, 20f / mainCam.orthographicSize);
        }
        else
        {
            bomb.transform.localScale = new Vector3((20f / mainCam.orthographicSize) / 2, (20f / mainCam.orthographicSize) / 2, (20f / mainCam.orthographicSize) / 2);
        }

        MoveParabola(
             bomb.transform,
             startPos,
             endPos,
             height: 4f,
             duration: 0.8f,
             onComplete: () =>
             {
                 boxHandleCollider.BoxBreak(pipes);
                 Destroy(bomb);
                 AudioManager.Instance.PlayOneShot(SoundKey.Explo, 1f);
                 ShakeCamera();
                 Destroy(boxHandleCollider.gameObject);
                 Transform effect = ObjectPooling.GetObject(SODictionaryEffect.GetEffectByType(EffectType.BombExplosion), new Vector3(boxHandleCollider.BoxVisual.CenterPos.position.x, boxHandleCollider.BoxVisual.CenterPos.position.y, -2f));
                 DOVirtual.DelayedCall(0.5f, () =>
                 {
                     gameplayScreen.BombBtn.transform.localScale = Vector3.zero;
                     gameplayScreen.gameObject.SetActive(true);
                     gameplayScreen.ListObjInScreen.ForEach(x => x.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
                     ObjectPooling.ReturnObject(effect);

                     DOVirtual.DelayedCall(0.5f, () =>
                     {
                         gameplayScreen.FrozenBtn.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
                         gameplayScreen.BombBtn.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
                         gameplayScreen.HammerBtn.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
                     }).OnComplete(() => isBombActive = false);
                 });
             }
         );

    }
    public void MoveParabola(Transform obj, Vector3 startPos, Vector3 endPos, float height, float duration, TweenCallback onComplete = null)
    {
        float t = 0;

        DOTween.To(() => t, x =>
        {
            t = x;

            // Lerp ngang (x,y) theo tuyến tính
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);

            // Công thức parabol (đỉnh giữa đường đi)
            float parabola = -4 * height * (t - 0.5f) * (t - 0.5f) + height;

            // Chỉ cộng vào trục Y, không xoay hệ trục
            pos.y += parabola;

            pos.z = -5f;

            obj.position = pos;

        }, 1f, duration)
        .SetEase(Ease.Linear)
        .OnComplete(onComplete);
    }
    private void ShakeCamera(float duration = 0.3f, float strength = 0.5f, int vibrato = 20)
    {
        Camera mainCam = this.mainCam;

        if (mainCam == null) return;

        Vector3 originalPos = mainCam.transform.localPosition;

        mainCam.transform.DOShakePosition(duration, strength, vibrato, 90f, false, true)
            .OnComplete(() =>
            {
                mainCam.transform.localPosition = originalPos;
            });
    }
    #endregion

    #region Búa: phá hủy 1 khối chỉ định trên map 

    [SerializeField] private GameObject hammerPrefab;
    public bool IsHammerWaiting { get; set; }

    public void OnHammerActive(BoxHandleCollider boxHandleCollider)
    {
        GameObject hammerObj = Instantiate(hammerPrefab, new Vector3(boxHandleCollider.BoxVisual.CenterPos.position.x, boxHandleCollider.BoxVisual.CenterPos.position.y, -3), Quaternion.identity);
        AudioManager.Instance.PlayOneShot(SoundKey.HammerHit, 1f);

        if (mainCam.orthographicSize >= 20f)
        {
            hammerObj.transform.localScale = new Vector3(20f / mainCam.orthographicSize, 20 / mainCam.orthographicSize, 20f / mainCam.orthographicSize);
        }
        else
        {
            hammerObj.transform.localScale = new Vector3((20f / mainCam.orthographicSize) / 2, (20f / mainCam.orthographicSize) / 2, (20f / mainCam.orthographicSize) / 2);
        }

        GameplayScreen gameplayScreen = UIManager.Instance.GetScreenActive<GameplayScreen>();

        DOVirtual.DelayedCall(0.67f, () =>
        {
            boxHandleCollider.BoxBreak(pipes);
            ShakeCamera();
            Destroy(boxHandleCollider.gameObject);
            Transform effect = ObjectPooling.GetObject(SODictionaryEffect.GetEffectByType(EffectType.BoxHit), new Vector3(boxHandleCollider.BoxVisual.CenterPos.position.x, boxHandleCollider.BoxVisual.CenterPos.position.y, -1f));
            Transform boxBreak = ObjectPooling.GetObject(SODictionaryEffect.GetEffectByType(EffectType.BoxBreak), new Vector3(boxHandleCollider.BoxVisual.CenterPos.position.x, boxHandleCollider.BoxVisual.CenterPos.position.y, -1f));
            ParticleSystemRenderer colorBox = boxBreak.transform.Find("Color").GetComponent<ParticleSystemRenderer>();
            colorBox.material = SOMaterialColor.GetMaterial(boxHandleCollider.BoxData.color);

            ParticleSystemRenderer colorGalssBox = boxBreak.transform.Find("Glass").GetComponent<ParticleSystemRenderer>();
            colorGalssBox.material = SOMaterialColor.GetMaterialTrans(boxHandleCollider.BoxData.color);

            DOVirtual.DelayedCall(0.33f, () =>

            {
                Destroy(hammerObj);
                IsHammerWaiting = false;
                gameplayScreen.OnHammerClose();
                ObjectPooling.ReturnObject(effect);
            });
        });
    }

    #endregion

    #endregion

    #region Game Running

    [Header("Game para")]
    [SerializeField] private float currentTime;
    public float CurrentTime => currentTime;
    public bool IsTimeRunning { get; set; }

    #region TimeInGame

    public void InitTimer()
    {
        currentTime = currentMap.time;
        frozeTimeCouter = 0;
        IsTimeRunning = false;
    }
    public void StartTimer() => IsTimeRunning = true;
    private void SubCounterTime()
    {
        if (!IsTimeRunning || GameManager.Instance.CurrentGameState != GameState.Playing) return;
        if (IsFroze) return;

        currentTime -= Time.deltaTime;
        currentTime = Mathf.Clamp(currentTime, 0, currentMap.time);

        CheckGameWinLose();
    }

    public void AddCounterTime(float value)
    {
        Debug.Log(currentTime);
        currentTime += value; 
    }

    #endregion
    public void CheckGameWinLose()                                              // Check liên tục theo delta time, vì thằng thua có liên quan tới thời gian 
    {
        List<BoxHandleCollider> currentHolder = boxHandleColliders.FindAll(x => x.BoxData.type != HolderType.Stone);

        if (currentTime > 0)
        {
            //if (currentHolder.Count <= 0) UIManager.Instance.ShowPopup<PopupWin>(() => LoadLevel(1)); // dùng hảm ẩn danh 
            if (currentHolder.Count <= 0)
            {
                GameManager.Instance.ChangeState(GameState.Win);
                IsLevelGenComplete = false;
            }
        }
        else
        {
            if (currentHolder.Count > 0)
            {
                GameManager.Instance.ChangeState(GameState.Lose);
                //IsLevelGenComplete = false;
            }
        }
    }

    #endregion

    #region Game Load Level

    public void LoadLevel(int level)
    {
        // Clear old map
        ClearDataInLevel();

        // SetUp dữ liệu ban đầu
        currentMap = dictionaryMapDatas[level];
        InitTimer();                                                       // đặt thời gian khi load map 

        gridGenerator.OnGenerateMap();

        boxHandleColliders.ForEach((b) =>
        {
            if (!b.IsNull("Không có BoxHandleCollider")) b.OnStart();
        });
        //GameManager.Instance.ChangeState(GameState.Playing);
    }

    public void ClearDataInLevel()
    {
        // Clear old map
        gridGenerator.ClearGrid();
        RemoveAllBoxWater();
        RemoveAllPupeWater();
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

    private int levelTest = 1;

    #region Build Test

    public void OnNextLevel()
    {
        levelTest++;
        if (levelTest > dictionaryMapDatas.Count)
        {
            levelTest = 1;
        }
        LoadLevel(levelTest);
    }

    public void OnPreviourLevel()
    {
        levelTest--;
        if (levelTest <= dictionaryMapDatas.Count)
        {
            levelTest = dictionaryMapDatas.Count;
        }
        LoadLevel(levelTest);
    }

    #endregion
}
