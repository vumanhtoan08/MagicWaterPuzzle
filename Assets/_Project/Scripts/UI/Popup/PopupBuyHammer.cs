using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupBuyHammer : PopupUI
{
    [SerializeField] private Button buyButton;
    [SerializeField] private Button exitButton;

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
        GameManager.Instance.ChangeState(GameState.Playing, false);
        Hide();
    }

    private void OnBuyButtonClick()
    {
        GameManager.Instance.ChangeState(GameState.Playing, false);
        Hide();
    }
}