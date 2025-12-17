using Coffee.UIExtensions;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupReciveCoin : PopupUI
{
    DataManager dataManager;
    GameManager gameManager;
    AudioManager audioManager;

    [SerializeField] private UIParticle uIParticle;
    [SerializeField] private Text moneyTxt;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        dataManager = DataManager.Instance;
        gameManager = GameManager.Instance;
        audioManager = AudioManager.Instance;
    }

    public override void Show(Action onClose)
    {
        base.Show(onClose);

        int currentMoney = dataManager.GetMoneyData();
        moneyTxt.text = $"{currentMoney + 60}";

        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
            uIParticle.Play();
        })
        .AppendInterval(2f)
        .AppendCallback(() =>
        {
            int currentLevel = dataManager.GetLevelData();
            currentLevel++;
            dataManager.SetLevelData(currentLevel);

            if (currentLevel <= 15)
            {
                gameManager.ChangeState(GameState.Playing);
            }
            else
            {
                gameManager.ChangeState(GameState.MainMenu);
            }

            audioManager.StopSound();
            uiManager.CloseAllPopup();
        });
    }

    public override void Hide()
    {
        base.Hide();
    }

    protected override void OnPopupDestroyed()
    {
        base.OnPopupDestroyed();
    }
}