using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuScreen : ScreenUI
{
    DataManager dataManager;
    HeartManager heartManager;

    [Header("Header Property")]
    [SerializeField] private Button moneyHeaderBtn;
    [SerializeField] private Button settingHeaderBtn;
    [SerializeField] private Text moneyValueHeaderTxt;
    [SerializeField] private Text lifeValueHeaderTxt;
    [SerializeField] private Text lifeCounterHeaderTxt;

    [Header("Footer Property")]
    [SerializeField] private RectTransform hightlightRect;
    [SerializeField] private Button homeFooterBtn;
    [SerializeField] private Button shopFooterBtn;
    [SerializeField] private Button rankFooterBtn;
    [SerializeField] private Text homeFooterTxt;
    [SerializeField] private Text shopFooterTxt;
    [SerializeField] private Text rankFooterTxt;
    [SerializeField] private Image homeFooterImg;
    [SerializeField] private Image shopFooterImg;
    [SerializeField] private Image rankFooterImg;

    [Header("Main Content")]
    [SerializeField] private RectTransform mainContent;
    [SerializeField] private RectTransform homeRect;
    [SerializeField] private RectTransform shopRect;
    [SerializeField] private RectTransform rankRect;
    public override void Initialize(UIManager uiManager)
    {
        base.Initialize(uiManager);
        OnHomeButtonClick();
        dataManager = DataManager.Instance;
        heartManager = HeartManager.Instance;

    }
    public override void Active()
    {
        base.Active();
        InitHeader();
        InitHome();

        #region Header
        moneyHeaderBtn.onClick.RemoveAllListeners();
        moneyHeaderBtn.onClick.AddListener(OnShopButtonClick);

        settingHeaderBtn.onClick.RemoveAllListeners();
        settingHeaderBtn.onClick.AddListener(() =>
        {
            uiManager.ShowPopup<PopupSetting>(null);
        });
        #endregion

        #region Footer
        homeFooterBtn.onClick.RemoveAllListeners();
        homeFooterBtn.onClick.AddListener(OnHomeButtonClick);

        shopFooterBtn.onClick.RemoveAllListeners();
        shopFooterBtn.onClick.AddListener(OnShopButtonClick);

        rankFooterBtn.onClick.RemoveAllListeners();
        rankFooterBtn.onClick.AddListener(OnRankButtonClick);
        #endregion
    }
    public override void Deactive()
    {
        base.Deactive();
    }

    protected override void OnScreenDestroyed()
    {

    }

    private void Update()
    {
        UpdateHeartCountVisual();
    }

    #region Header

    private float timeCounter;

    public void InitHeader()
    {
        int currentMoney = dataManager.GetMoneyData();
        int currentLife = dataManager.GetLifeData();

        moneyValueHeaderTxt.text = $"{currentMoney}";
        lifeValueHeaderTxt.text = $"{currentLife}";

        var resulHeart = heartManager.NumberOfRecoveryHearts();
        int timeCoolDownLeft = dataManager.GetCoolDownTime();
        timeCounter = timeCoolDownLeft - resulHeart.timeOverflow;
        UpdateHeartCountWhenActive(resulHeart.recoveryHeartsCount, resulHeart.timeOverflow);
    }

    public void UpdateHeartCountVisual()
    {
        int currentHeart = dataManager.GetLifeData();
        if (currentHeart >= 5)
        {
            lifeValueHeaderTxt.text = $"{currentHeart}";
            lifeCounterHeaderTxt.text = $"MAX";
        }
        else
        {
            timeCounter -= Time.deltaTime;
            int mins = (int)timeCounter / 60;
            int seconds = (int)timeCounter % 60;
            lifeCounterHeaderTxt.text = $"{mins:00}:{seconds:00}";

            if (timeCounter <= 0)
            {
                mins = (int)heartManager.CoolDownHeart / 60;
                seconds = (int)heartManager.CoolDownHeart % 60;
                lifeCounterHeaderTxt.text = $"{mins:00}:{seconds:00}";
                heartManager.ChangeLife(1);
                currentHeart = dataManager.GetLifeData();
                lifeValueHeaderTxt.text = $"{currentHeart}";
                timeCounter = heartManager.CoolDownHeart;
            }
        }
    }

    public void UpdateHeartCountWhenActive(int recoveryHeartCount, int timeLeft = 0)
    {
        int currentHeart = dataManager.GetLifeData();
        if (currentHeart >= 5)
        {
            lifeValueHeaderTxt.text = $"{currentHeart}";
            lifeCounterHeaderTxt.text = $"MAX";
        }
        else
        {
            currentHeart = Mathf.Clamp(currentHeart + recoveryHeartCount, 0, 5);
            dataManager.SetLifeData(currentHeart);
            lifeValueHeaderTxt.text = $"{currentHeart}";

            int time = (int)heartManager.CoolDownHeart - timeLeft;
            int minutes = time / 60;
            int seconds = time % 60;

            lifeCounterHeaderTxt.text = $"{minutes:00}:{seconds:00}";
            dataManager.SetDateTimeData();
            dataManager.SetCoolDownTime((int)timeCounter);
        }
    }

    #endregion

    #region Footer

    public void OnHomeButtonClick()
    {
        Sequence seq = DOTween.Sequence();
        hightlightRect.DOKill();
        mainContent.DOKill();
        homeRect.gameObject.SetActive(true);

        seq.Append(hightlightRect.DOAnchorPosX(0f, 0.15f).SetEase(Ease.Linear))
            .Join(homeFooterImg.rectTransform.DOAnchorPos(new Vector2(homeFooterImg.rectTransform.anchoredPosition.x, 84f), 0.1f).SetEase(Ease.Linear))
            .Join(homeFooterTxt.DOFade(1f, 0.1f).SetEase(Ease.Linear))
            .JoinCallback(() =>
            {
                OnShopButtonUnActive();
                OnRankButtonUnActive();
            })
            .Join(mainContent.DOAnchorPosX(0f, 0.15f).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                shopRect.gameObject.SetActive(false);
                rankRect.gameObject.SetActive(false);
            });
    }

    public void OnShopButtonClick()
    {
        Sequence seq = DOTween.Sequence();
        hightlightRect.DOKill();
        mainContent.DOKill();
        shopRect.gameObject.SetActive(true);

        seq.Append(hightlightRect.DOMoveX(shopFooterImg.transform.position.x, 0.15f).SetEase(Ease.Linear))
            .Join(shopFooterImg.rectTransform.DOAnchorPos(new Vector2(shopFooterImg.rectTransform.localPosition.x, 84f), 0.1f).SetEase(Ease.Linear))
            .Join(shopFooterTxt.DOFade(1f, 0.1f).SetEase(Ease.Linear))
            .JoinCallback(() =>
            {
                OnHomeButtonUnActive();
                OnRankButtonUnActive();
            })
            .Join(mainContent.DOAnchorPosX(1500f, 0.15f).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                homeRect.gameObject.SetActive(false);
                rankRect.gameObject.SetActive(false);
            });
    }

    public void OnRankButtonClick()
    {
        Sequence seq = DOTween.Sequence();
        hightlightRect.DOKill();
        mainContent.DOKill();
        rankRect.gameObject.SetActive(true);

        seq.Append(hightlightRect.DOMoveX(rankFooterImg.transform.position.x, 0.15f).SetEase(Ease.Linear))
            .Join(rankFooterImg.rectTransform.DOAnchorPos(new Vector2(rankFooterImg.rectTransform.localPosition.x, 84f), 0.1f).SetEase(Ease.Linear))
            .Join(rankFooterTxt.DOFade(1f, 0.1f).SetEase(Ease.Linear))
            .JoinCallback(() =>
            {
                OnHomeButtonUnActive();
                OnShopButtonUnActive();
            })
            .Join(mainContent.DOAnchorPosX(-1500f, 0.15f).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                homeRect.gameObject.SetActive(false);
                shopRect.gameObject.SetActive(false);
            });
    }

    private void OnHomeButtonUnActive()
    {
        homeFooterImg.rectTransform.DOAnchorPos(new Vector2(homeFooterImg.rectTransform.anchoredPosition.x, 14f), 0.1f).SetEase(Ease.Linear);
        homeFooterTxt.DOFade(0f, 0.1f).SetEase(Ease.Linear);
    }

    private void OnShopButtonUnActive()
    {
        shopFooterImg.rectTransform.DOAnchorPos(new Vector2(0f, 14f), 0.1f).SetEase(Ease.Linear);
        shopFooterTxt.DOFade(0f, 0.1f).SetEase(Ease.Linear);
    }

    private void OnRankButtonUnActive()
    {
        rankFooterImg.rectTransform.DOAnchorPos(new Vector2(0f, 14f), 0.1f).SetEase(Ease.Linear);
        rankFooterTxt.DOFade(0f, 0.1f).SetEase(Ease.Linear);
    }

    #endregion

    #region Home Ability

    [Header("Home Property")]
    [SerializeField] private Button playHomeBtn;
    [SerializeField] private List<Text> displayText;

    private void InitHome()
    {
        playHomeBtn.onClick.RemoveAllListeners();
        playHomeBtn.onClick.AddListener(ChangePlayScreen);
        UpdateUIForLevel();
    }

    private void ChangePlayScreen()
    {
        int currentHeart = dataManager.GetLifeData();
        if (currentHeart > 0)
        {
            GameManager.Instance.ChangeState(GameState.Playing);
            if (timeCounter <= 0)
                dataManager.SetCoolDownTime((int)heartManager.CoolDownHeart);
            else
                dataManager.SetCoolDownTime((int)timeCounter);
            dataManager.SetDateTimeData();
        }
        else
        {
            // Show Popup
            uiManager.ShowPopup<PopupOutOfHeart>(null);
        }
    }

    private void UpdateUIForLevel()
    {
        int currentLevel = dataManager.GetLevelData();

        for (int i = 0; i < displayText.Count; i++)
        {
            displayText[i].text = currentLevel.ToString();
            currentLevel++;
        }
    }

    #endregion


}
