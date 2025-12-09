using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuScreen : ScreenUI
{
    DataManager dataManager;

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

    }
    public override void Active()
    {
        base.Active();
        InitHeader();
        InitHome();

        #region Header
        moneyHeaderBtn.onClick.RemoveAllListeners();
        moneyHeaderBtn.onClick.AddListener(OnShopButtonClick);
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

    #region Header

    public void InitHeader()
    {
        int currentMoney = dataManager.GetMoneyData();
        int currentLife = dataManager.GetLifeData();

        moneyValueHeaderTxt.text = $"{currentMoney}";
        lifeValueHeaderTxt.text = $"{currentLife}";
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
        GameManager.Instance.ChangeState(GameState.Playing);
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
