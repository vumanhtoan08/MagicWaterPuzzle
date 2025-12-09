using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupWin : PopupUI
{
    GameManager gameManager;
    DataManager dataManager;
    [SerializeField] private Button claimCoin;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        dataManager = DataManager.Instance;
        gameManager = GameManager.Instance;

        claimCoin.onClick.RemoveAllListeners();
        claimCoin.onClick.AddListener(OnClaimButtonClick);
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

    private void OnClaimButtonClick()
    {
        int currentLevel = dataManager.GetLevelData();
        currentLevel++; 
        dataManager.SetLevelData(currentLevel);

        if (currentLevel <= 5)
        {
            gameManager.ChangeState(GameState.Playing);
        }
        else
        {
            gameManager.ChangeState(GameState.MainMenu);
        }

        Hide();
    }
}