using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("GameState")]
    [SerializeField] private GameState currentGameState;
    public GameState CurrentGameState => currentGameState;

    [Header("Manager REF")]
    [SerializeField] private DataManager dataManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private ShopManager shopManager;
    public bool IsInited { get; set; }

    public void ChangeState(GameState newState)
    {
        //if (newState == currentGameState) return;
        currentGameState = newState;

        switch (currentGameState)
        {
            case GameState.Init:
                DataManager.Instance.CheckInitData();
                uiManager.ShowScreen<FlashScreen>();
                break;
            case GameState.MainMenu:
                if (IsInited)
                    uiManager.ShowScreen<LoadingScreen>();
                else
                {
                    uiManager.ShowScreen<MainMenuScreen>();
                    IsInited = true;
                }
                AudioManager.Instance.PlayMusic(SoundKey.MainMusic, 0.3f, true);
                break;
            case GameState.Playing:
                uiManager.ShowScreen<LoadingScreen>();
                break;
            case GameState.Pause:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            case GameState.GiveUp:
                break;
        }

        Debug.Log($"Chuyển trạng thái {currentGameState}");
    }

    #region Unity Methods

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        IsInited = false;

        if (!dataManager.IsNull("DataManager đang null")) dataManager.OnAwake();
        if (!uiManager.IsNull("UIManager đang null")) uiManager.Initialize();
    }

    private void Start()
    {
        ChangeState(GameState.Init);

        if (!levelManager.IsNull("Level Manager đang null")) levelManager.OnStart();
        if (!shopManager.IsNull("Wallet Manager đang null")) shopManager.OnStart();
    }

    private void Update()
    {
        if (!levelManager.IsNull("Level Manager đang null")) levelManager.OnUpdate();
    }

    #endregion
}