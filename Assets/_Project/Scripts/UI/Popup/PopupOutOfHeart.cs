using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupOutOfHeart : PopupUI
{
    [SerializeField] private Button buyButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private ButtonBuy buttonBuy;
    [SerializeField] private UIButtonEffect effect;
    [SerializeField] private ButtonSoundEffect sound;

    [Header("Money")]
    [SerializeField] private Text money;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(OnExitButtonClick);

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyButtonClick);
    }

    public override void Show(Action onClose)
    {
        base.Show(onClose);
        int currentMoney = DataManager.Instance.GetMoneyData();
        if (currentMoney < buttonBuy.Cost)
        {
            buyButton.interactable = false;
            effect.enabled = false;
            sound.enabled = false;
        }
        else
        {
            buyButton.interactable = true;
            effect.enabled = true;
            sound.enabled = true;
        }
        money.text = $"{currentMoney}";
    }

    public override void Hide()
    {
        base.Hide();
    }

    protected override void OnPopupDestroyed()
    {
        base.OnPopupDestroyed();
    }

    private void OnExitButtonClick()
    {
        Hide();
    }

    private void OnBuyButtonClick()
    {
        Hide();
    }
}