using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupLose : PopupUI
{
    GameManager gameManager;

    [SerializeField] private Button retryBtn;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        gameManager = GameManager.Instance;

        retryBtn.onClick.RemoveAllListeners();
        retryBtn.onClick.AddListener(OnRetryButtonClick);
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

    private void OnRetryButtonClick()
    {
        gameManager.ChangeState(GameState.Playing);
        uiManager.CloseAllPopup();
        LevelManager.Instance.ClearDataInLevel();
        LevelManager.Instance.IsLevelGenComplete = false;

        AudioManager.Instance.StopSound();
    }
}
