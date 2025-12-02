using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class GameplayScreen : ScreenUI
{
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

    private LevelManager levelManager = LevelManager.Instance;

    #region Unity Methods

    private void Update()
    {
        timeTxt.text = FormatTimeMMSS(levelManager.CurrentTime);
        OnScreenFroze();
    }

    #endregion

    public override void Active()
    {
        base.Active();

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

        frozeScreen = UIManager.Instance.GetScreen<FrozeScreen>();

        levelTxt.text = $"LEVEL {levelManager.CurrentMap.level}";
        levelTxt.color = textColor;

        timeTxt.text = FormatTimeMMSS(levelManager.CurrentTime);

        // Button Booster
        frozenBtn.onClick.RemoveAllListeners();
        frozenBtn.onClick.AddListener(LevelManager.Instance.OnFrozeBoosterActive);

        bombBtn.onClick.RemoveAllListeners();
        bombBtn.onClick.AddListener(LevelManager.Instance.OnBombBoosterActive);

        hammerBtn.onClick.RemoveAllListeners();
        hammerBtn.onClick.AddListener(OnHammerReady);
        closeHammer.onClick.RemoveAllListeners();
        closeHammer.onClick.AddListener(OnHammerClose);

        // Test
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(LevelManager.Instance.OnNextLevel);

        previourButton.onClick.RemoveAllListeners();
        previourButton.onClick.AddListener(LevelManager.Instance.OnPreviourLevel);

    }

    protected override void OnScreenDestroyed()
    {

    }

    #region Ice Visual

    [Header("Frozen Screen")]
    [SerializeField] private FrozeScreen frozeScreen;
    [SerializeField] private Image timeButtonImg;
    [SerializeField] private List<Sprite> timeButtonFroze;
    [SerializeField] private GameObject clockObj;                       // đồng hồ lúc bình thường
    [SerializeField] private GameObject frezenObj;                       // đồng hồ lúc đóng băng
    [SerializeField] private GameObject iceCounter;
    [SerializeField] private Text timeFrozeTxt;

    private bool isFrozenScreenActive = false;
    private bool isReskinFrozen = false;
    private void OnScreenFroze()
    {
        if (!isFrozenScreenActive) isFrozenScreenActive = true;

        if (!isReskinFrozen)
        {
            frozeScreen.gameObject.SetActive(LevelManager.Instance.IsFroze ? true : false);
            timeButtonImg.sprite = LevelManager.Instance.IsFroze ? timeButtonFroze[1] : timeButtonFroze[0];
            timeBtn.interactable = LevelManager.Instance.IsFroze ? false : true;
            clockObj.SetActive(LevelManager.Instance.IsFroze ? false : true);
            frezenObj.SetActive(LevelManager.Instance.IsFroze ? true : false);
            iceCounter.SetActive(LevelManager.Instance.IsFroze ? true : false);
        }

        timeFrozeTxt.text = $"{(int)LevelManager.Instance.FrozeTimeCouter}";
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
        LevelManager.Instance.IsHammerWaiting = true;

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

    public void OnHammerClose()
    {
        LevelManager.Instance.IsHammerWaiting = false;

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

    #endregion

}
