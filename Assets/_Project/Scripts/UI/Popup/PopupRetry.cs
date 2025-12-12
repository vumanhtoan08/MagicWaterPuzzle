using UnityEngine;
using System;
using UnityEngine.UI;
using PathologicalGames;

public class PopupRetry : PopupUI
{
    DataManager dataManager;
    HeartManager heartManager;

    [SerializeField] private Button giveUpButton;
    [SerializeField] private Button exit;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        giveUpButton.onClick.RemoveAllListeners();
        giveUpButton.onClick.AddListener(OnGiveUpButtonClick);

        exit.onClick.RemoveAllListeners();
        exit.onClick.AddListener(OnExitButtonClick);

        heartManager = HeartManager.Instance;
        dataManager = DataManager.Instance;
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
        int currentHeart = dataManager.GetLifeData();
        if(currentHeart > 0)
        {
            heartManager.ChangeLife(-1);
            GameManager.Instance.ChangeState(GameState.Playing);
            LevelManager.Instance.ClearDataInLevel();
            LevelManager.Instance.IsLevelGenComplete = false;
            uiManager.CloseAllPopup();
        }
        else
        {
            uiManager.ShowPopup<PopupOutOfHeart>(null);
        }
    }

    private void OnExitButtonClick()
    {
        GameManager.Instance.ChangeState(GameState.Playing, false);
        Hide();
    }
}