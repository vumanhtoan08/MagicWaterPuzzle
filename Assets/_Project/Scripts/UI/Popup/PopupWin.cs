using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupWin : PopupUI
{
    GameManager gameManager;
    DataManager dataManager;
    AudioManager audioManager;
    [SerializeField] private Button claimCoin;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        dataManager = DataManager.Instance;
        gameManager = GameManager.Instance;
        audioManager = AudioManager.Instance;

        claimCoin.onClick.RemoveAllListeners();
        claimCoin.onClick.AddListener(OnClaimButtonClick);
    }

    public override void Show(Action onClose)
    {
        base.Show(onClose);
        audioManager.PlayOneShot(SoundKey.Win, 0.7f);
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
        uiManager.ShowPopup<PopupReciveCoin>(null);
    }
}