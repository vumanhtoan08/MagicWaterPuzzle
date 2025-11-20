using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("GameState")]
    [SerializeField] private GameState currentGameState; 
    public GameState CurrentGameState => currentGameState;
    
    [Header ("Manager REF")]
    [SerializeField] private DataManager dataManager;
    [SerializeField] private LevelManager levelManager; 
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioManager audioManager;      

    public void ChangeState(GameState newState)
    {
        if (newState == currentGameState) return;
        currentGameState = newState;

        switch (currentGameState)
        {
            case GameState.Init:
                DataManager.Instance.StartGameData();
                break;
            case GameState.MainMenu:
                break;
            case GameState.Playing:
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
    }

    #region Unity Methods

    protected override void Awake()
    {
        base.Awake();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        DontDestroyOnLoad(gameObject);

        if (!dataManager.IsNull("DataManager đang null")) dataManager.OnAwake();
    }

    private void Start()
    {
        ChangeState(GameState.Init);

        if (!levelManager.IsNull("Level Manager đang null")) levelManager.OnStart();
    }

    private void Update()
    {
        
    }

    #endregion
}