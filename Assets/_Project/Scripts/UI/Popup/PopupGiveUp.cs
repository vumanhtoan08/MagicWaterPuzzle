using UnityEngine;
using System;
using UnityEngine.UI;
using PathologicalGames;

public class PopupGiveUp : PopupUI
{
    [SerializeField] private Button giveUpButton;
    [SerializeField] private Button exit;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        giveUpButton.onClick.RemoveAllListeners();
        giveUpButton.onClick.AddListener(OnGiveUpButtonClick);

        exit.onClick.RemoveAllListeners();
        exit.onClick.AddListener(OnExitButtonClick);
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

    private void OnGiveUpButtonClick()
    {
        GameManager.Instance.ChangeState(GameState.MainMenu);
        LevelManager.Instance.ClearDataInLevel();
        LevelManager.Instance.IsLevelGenComplete = false;
        uiManager.CloseAllPopup();
    }

    private void OnExitButtonClick()
    {
        Hide();
    }
}