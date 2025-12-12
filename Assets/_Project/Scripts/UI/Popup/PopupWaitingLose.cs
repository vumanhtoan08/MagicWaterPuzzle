using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupWaitingLose : PopupUI, IPointerDownHandler, IPointerUpHandler
{
    AudioManager audioManager;

    [SerializeField] private CanvasGroup canvasGroupFade;
    [SerializeField] private Button keepPlaying;
    [SerializeField] private Button exit;
    [SerializeField] private ButtonBuy buttonBuy;
    [SerializeField] private UIButtonEffect effect;
    [SerializeField] private ButtonSoundEffect sound;

    [Header("Money")]
    [SerializeField] private Text money;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        audioManager = AudioManager.Instance;

        keepPlaying.onClick.RemoveAllListeners();
        keepPlaying.onClick.AddListener(OnKeepPlayingButtonClick);

        exit.onClick.RemoveAllListeners();
        exit.onClick.AddListener(OnExitButtonClick);
    }

    public override void Show(Action onClose)
    {
        base.Show(onClose);
        audioManager.PlayOneShot(SoundKey.Lose, 1f);

        int currentMoney = DataManager.Instance.GetMoneyData();
        if (currentMoney < buttonBuy.Cost)
        {
            keepPlaying.interactable = false;
            effect.enabled = false;
            sound.enabled = false;
        }
        else
        {
            keepPlaying.interactable = true;
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

    public void OnExitButtonClick()
    {
        uiManager.ShowPopup<PopupLose>(null);
    }

    public void OnKeepPlayingButtonClick()
    {
        Hide();
        LevelManager.Instance.AddCounterTime(20f);
        GameManager.Instance.ChangeState(GameState.Playing, false);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        DOTween.Kill(this);
        canvasGroupFade.DOFade(0, 0.3f).SetTarget(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        DOTween.Kill(this);
        canvasGroupFade.DOFade(1, 0.3f).SetTarget(this);
    }
}