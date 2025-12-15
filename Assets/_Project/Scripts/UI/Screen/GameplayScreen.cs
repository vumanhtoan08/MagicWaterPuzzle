
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayScreen : ScreenUI
{
    GameManager gameManager;
    DataManager dataManager;
    LevelManager levelManager;

    [Header("Properties Level & Time")]
    [SerializeField] private Image iconLevelImg;
    [SerializeField] private List<Sprite> levelSprites;
    [SerializeField] private Text levelTxt;
    [SerializeField] private Text timeTxt;

    [Header("Properties Button Action")]
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button retryBtn;
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Button timeBtn;

    [Header("Properties Button Booster")]
    [SerializeField] private Button frozenBtn;
    [SerializeField] private Button bombBtn;
    [SerializeField] private Button hammerBtn;
    [SerializeField] private List<GameObject> listObjInScreen;
    public Button FrozenBtn => frozenBtn;
    public Button BombBtn => bombBtn;
    public Button HammerBtn => hammerBtn;
    public List<GameObject> ListObjInScreen => listObjInScreen;

    #region Unity Methods

    int previourTime = -9999;

    private void Update()
    {
        timeTxt.text = FormatTimeMMSS(levelManager.CurrentTime);

        int currentTime = (int)levelManager.CurrentTime;

        if (levelManager.CurrentTime <= 30 && previourTime != currentTime)
        {
            timeTxt.color = Color.red;
            timeTxt.transform.DOScale(1.1f, 0.1f).OnComplete(() => timeTxt.transform.DOScale(1, 0.05f));
            previourTime = currentTime;
        }
        else if (levelManager.CurrentTime > 30)
        {
            timeTxt.color = Color.white;
        }
        OnScreenFroze();
    }

    #endregion
    public override void Initialize(UIManager uiManager)
    {
        base.Initialize(uiManager);
        frozeScreen = UIManager.Instance.GetScreen<FrozeScreen>();
        dataManager = DataManager.Instance;
        gameManager = GameManager.Instance;
        levelManager = LevelManager.Instance;
    }
    public override void Active()
    {
        base.Active();
        int currentLevel = dataManager.GetLevelData();

        levelManager.LoadLevel(currentLevel);
        OnUpdateUIFooter();

        MapData mapData = levelManager.CurrentMap;
        switch (mapData.levelDifficult)
        {
            case LevelDifficult.Normal:
                AudioManager.Instance.PlayOneShot(SoundKey.Intro, 0.7f);
                break;
            case LevelDifficult.Hard:
                uiManager.ShowPopup<PopupHard>(null);
                break;
            case LevelDifficult.SuperHard:
                uiManager.ShowPopup<PopupSuperHard>(null);
                break;
        }

        Color32 textColor = new Color32(255, 255, 255, 255);
        // Level & Time
        switch (levelManager.CurrentMap.levelDifficult)
        {
            case LevelDifficult.Normal:
                iconLevelImg.gameObject.SetActive(false);
                textColor = new Color32(255, 255, 255, 255);
                break;
            case LevelDifficult.Hard:
                iconLevelImg.gameObject.SetActive(true);
                iconLevelImg.sprite = levelSprites[0];
                textColor = new Color32(255, 56, 59, 255);
                break;
            case LevelDifficult.SuperHard:
                iconLevelImg.gameObject.SetActive(true);
                iconLevelImg.sprite = levelSprites[1];
                textColor = new Color32(139, 58, 251, 255);
                break;
        }

        levelTxt.text = $"LEVEL {levelManager.CurrentMap.level}";
        levelTxt.color = textColor;
        timeTxt.text = FormatTimeMMSS(levelManager.CurrentTime);

        homeBtn.onClick.RemoveAllListeners();
        homeBtn.onClick.AddListener(() =>
        {
            levelManager.ClearDataInLevel();
            gameManager.ChangeState(GameState.MainMenu);
        });

        retryBtn.onClick.RemoveAllListeners();
        retryBtn.onClick.AddListener(() =>
        {
            if (gameManager.CurrentGameState == GameState.Playing)
            {
                uiManager.ShowPopup<PopupRetry>(null);
                gameManager.ChangeState(GameState.Pause, false);
            }
        });

        pauseBtn.onClick.RemoveAllListeners();
        pauseBtn.onClick.AddListener(() =>
        {
            if (gameManager.CurrentGameState == GameState.Playing)
                gameManager.ChangeState(GameState.Pause);
        });

        frozenBtn.onClick.RemoveAllListeners();
        frozenBtn.onClick.AddListener(() =>
        {
            if (gameManager.CurrentGameState == GameState.Playing)
                levelManager.OnFrozeBoosterActive();
        });

        bombBtn.onClick.RemoveAllListeners();
        bombBtn.onClick.AddListener(() =>
        {
            if (gameManager.CurrentGameState == GameState.Playing)
                levelManager.OnBombBoosterActive();
        });

        hammerBtn.onClick.RemoveAllListeners();
        hammerBtn.onClick.AddListener(() =>
        {
            if (gameManager.CurrentGameState == GameState.Playing)
                OnHammerReady();
        });
        closeHammer.onClick.RemoveAllListeners();
        closeHammer.onClick.AddListener(OnHammerClose);

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(levelManager.OnNextLevel);
         previourButton.onClick.RemoveAllListeners();
        previourButton.onClick.AddListener(levelManager.OnPreviourLevel);
    }

    protected override void OnScreenDestroyed()
    {

    }

    #region Ice Visual

    [Header("Frozen Screen")]
    [SerializeField] private FrozeScreen frozeScreen;
    [SerializeField] private Image timeButtonImg;
    [SerializeField] private List<Sprite> timeButtonFroze;
    [SerializeField] private GameObject clockObj;                   
    [SerializeField] private GameObject frezenObj;                  
    [SerializeField] private GameObject iceCounter;
    [SerializeField] private Text timeFrozeTxt;

    private bool isFrozenScreenActive = false;
    private bool isReskinFrozen = false;
    private void OnScreenFroze()
    {
        if (!isFrozenScreenActive) isFrozenScreenActive = true;

        if (!isReskinFrozen)
        {
            frozeScreen.gameObject.SetActive(levelManager.IsFroze ? true : false);
            timeButtonImg.sprite = levelManager.IsFroze ? timeButtonFroze[1] : timeButtonFroze[0];
            timeBtn.interactable = levelManager.IsFroze ? false : true;
            clockObj.SetActive(levelManager.IsFroze ? false : true);
            frezenObj.SetActive(levelManager.IsFroze ? true : false);
            iceCounter.SetActive(levelManager.IsFroze ? true : false);
        }

        timeFrozeTxt.text = $"{(int)levelManager.FrozeTimeCouter}";
    }

    #endregion

    #region Time Convert

    public static string FormatTimeMMSS(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return $"{m:00}:{s:00}";
    }

    #endregion

    #region Building Test

    [SerializeField] private Button nextButton;
    [SerializeField] private Button previourButton;

    #endregion

    #region Hammer Visual

    [SerializeField] private CanvasGroup hammerGroup;
    [SerializeField] private Button closeHammer;

    private void OnHammerReady()
    {
        if (CheckBoosterFrozen())
        {
            levelManager.IsHammerWaiting = true;

            listObjInScreen.ForEach(x =>
            {
                x.transform.DOKill();
                x.transform.DOScale(0, 0.2f).SetEase(Ease.InBack);
            });

            frozenBtn.transform.DOKill();
            frozenBtn.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);

            bombBtn.transform.DOKill();
            bombBtn.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);

            hammerBtn.transform.DOKill();
            hammerBtn.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                hammerGroup.DOKill();
                hammerGroup.DOFade(1, 0.3f).SetEase(Ease.OutBack);
            });
        }
        else
        {
            gameManager.ChangeState(GameState.Pause, false);
            UIManager.Instance.ShowPopup<PopupBuyHammer>(() =>
            {
                GameplayScreen gameplayScreen = UIManager.Instance.GetScreenActive<GameplayScreen>();
                gameplayScreen.OnUpdateUIFooter();
            });
        }
    }

    public void OnHammerClose()
    {
        levelManager.IsHammerWaiting = false;

        hammerGroup.DOKill();
        hammerGroup.DOFade(0, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            listObjInScreen.ForEach(x =>
            {
                x.transform.DOKill();
                x.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
            });

            DOVirtual.DelayedCall(0.5f, () =>
            {
                frozenBtn.transform.DOKill();
                frozenBtn.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);

                bombBtn.transform.DOKill();
                bombBtn.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);

                hammerBtn.transform.DOKill();
                hammerBtn.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
            });
        });
    }
    public bool CheckBoosterFrozen()
    {
        int hammerValue = dataManager.GetHammerData();
        if (hammerValue > 0)
        {
            dataManager.SetHammerData(hammerValue - 1);
            return true;
        }
        return false;
    }

    #endregion

    #region Footer Visual

    [Header("Footer Visual")]
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private Image frozenValueImg;
    [SerializeField] private Text frozenText;
    [SerializeField] private Image bombValueImg;
    [SerializeField] private Text bombText;
    [SerializeField] private Image hammerValueImg;
    [SerializeField] private Text hammerText;

    public void OnUpdateUIFooter()
    {
        int frozenValue = dataManager.GetFrozenData();
        int bombValue = dataManager.GetBombData();
        int hammerValue = dataManager.GetHammerData();

        if (frozenValue > 0)
        {
            frozenValueImg.sprite = sprites[0];
            frozenText.gameObject.SetActive(true);
            frozenText.text = $"{frozenValue}";
        }
        else
        {
            frozenValueImg.sprite = sprites[1];
            frozenText.gameObject.SetActive(false);
        }

        if (bombValue > 0)
        {
            bombValueImg.sprite = sprites[0];
            bombText.gameObject.SetActive(true);
            bombText.text = $"{bombValue}";
        }
        else
        {
            bombValueImg.sprite = sprites[1];
            bombText.gameObject.SetActive(false);
        }

        if (hammerValue > 0)
        {
            hammerValueImg.sprite = sprites[0];
            hammerText.gameObject.SetActive(true);
            hammerText.text = $"{hammerValue}";
        }
        else
        {
            hammerValueImg.sprite = sprites[1];
            hammerText.gameObject.SetActive(false);
        }

    }

    #endregion
}
