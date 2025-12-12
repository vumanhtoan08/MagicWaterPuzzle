using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadingScreen : ScreenUI
{
    [SerializeField] private CanvasGroup canvasGroup;

    public override void Active()
    {
        base.Active();
        OnActiveLoading();
        //AudioManager.Instance.PlayOneShot(SoundKey.Intro, 1f);
    }

    protected override void OnScreenDestroyed()
    {
    }

    public void OnActiveLoading()
    {
        Sequence seq = DOTween.Sequence();
        gameObject.SetActive(true);

        seq.Append(canvasGroup.DOFade(1f, 0.2f))
            .AppendInterval(2f)
            .Append(canvasGroup.DOFade(0f, 0.2f))
            .OnComplete(() =>
            {
                switch (GameManager.Instance.CurrentGameState)
                {
                    case GameState.Init:
                        break;
                    case GameState.MainMenu:
                        UIManager.Instance.ShowScreen<MainMenuScreen>();
                        break;
                    case GameState.Playing:
                        UIManager.Instance.ShowScreen<GameplayScreen>();
                        break;
                }
                gameObject.SetActive(false);
            });
    }
}
