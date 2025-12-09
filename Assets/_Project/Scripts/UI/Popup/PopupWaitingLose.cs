using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupWaitingLose : PopupUI, IPointerDownHandler, IPointerUpHandler
{
    GameManager gameManager;
    DataManager dataManager;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button keepPlaying;
    [SerializeField] private Button exit;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        gameManager = GameManager.Instance;
        dataManager = DataManager.Instance;

        keepPlaying.onClick.RemoveAllListeners();
        keepPlaying.onClick.AddListener(OnKeepPlayingButtonClick);

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
        canvasGroup.DOFade(0, 0.3f).SetTarget(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        DOTween.Kill(this);
        canvasGroup.DOFade(1, 0.3f).SetTarget(this);
    }
}