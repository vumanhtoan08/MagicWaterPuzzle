using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    [SerializeField] private HeartManager heartManager;
    [SerializeField] private TutorialManager tutorialManager;
    public bool IsInited { get; set; }

    public void ChangeState(GameState newState, bool isAuto = true)
    {
        //if (newState == currentGameState) return;
        currentGameState = newState;
        Debug.Log($"Chuyển trạng thái {currentGameState}");

        if (!isAuto) return;

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
                break;
            case GameState.Playing:
                int currentLevel = dataManager.GetLevelData();
                if (currentLevel <= 15 && !IsInited)
                {
                    UIManager.Instance.ShowScreen<GameplayScreen>();
                    IsInited = true;
                }
                else
                {
                    uiManager.ShowScreen<LoadingScreen>();
                }
                break;
            case GameState.Pause:
                uiManager.ShowPopup<PopupPause>(null);
                break;
            case GameState.Win:
                DOVirtual.DelayedCall(2.5f, () =>
                {
                    LevelManager.Instance.ClearDataInLevel();
                });
                DOVirtual.DelayedCall(3f, () =>
                {
                    UIManager.Instance.ShowPopup<PopupWin>(null);
                });
                break;
            case GameState.Lose:
                UIManager.Instance.ShowPopup<PopupWaitingLose>(null);
                break;
            case GameState.GiveUp:
                break;
        }
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
        if (!heartManager.IsNull("Heart Manager đang null")) heartManager.OnStart();
        if (!tutorialManager.IsNull("Tutorial Manager đang null")) tutorialManager.OnStart();

    }

    private void Update()
    {
        if (!levelManager.IsNull("Level Manager đang null")) levelManager.OnUpdate();
        if (!heartManager.IsNull("Heart Manager đang null")) heartManager.OnUpdate();
        if (!tutorialManager.IsNull("Turorial Manager đang null")) tutorialManager.OnUpdate();

        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("Vibrate");
            Handheld.Vibrate();
        }
    }

    #endregion

    private void OnApplicationFocus(bool focus)
    {
        Debug.Log(focus);
        //if (!focus)
        //{
        //    dataManager.SetDateTimeData();
        //}
        //else
        //{
        //    var resulHeart = heartManager.NumberOfRecoveryHearts();
        //    UpdateHeartCountWhenActive(resulHeart.recoveryHeartsCount, resulHeart.timeOverflow);
        //}
    }
}