using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashScreen : ScreenUI
{
    [SerializeField] private Image logoFillImg;
    [SerializeField] private RectTransform loadingRect;
    [SerializeField] private Text percentTxt;

    private float maxWidth = 775f;
    private int percent = 5;
    private int currentLevel;

    public override void Initialize(UIManager uiManager)
    {
        float startFill = percent / 100f;
        logoFillImg.fillAmount = startFill;

        float startWidth = maxWidth * startFill;
        loadingRect.sizeDelta = new Vector2(startWidth, loadingRect.sizeDelta.y);

        percentTxt.text = $"{percent}%";
    }

    public override void Active()
    {
        base.Active();
        PlayLoadingTween();
        currentLevel = DataManager.Instance.GetLevelData();
    }

    protected override void OnScreenDestroyed()
    {
    }

    public void PlayLoadingTween()
    {
        int targetPercent = 100;

        logoFillImg
            .DOFillAmount(1f, 1.2f)
            .SetEase(Ease.Linear);

        loadingRect
            .DOSizeDelta(
                new Vector2(maxWidth, loadingRect.sizeDelta.y),
                1.2f
            )
            .SetEase(Ease.Linear);

        DOTween.To(
            () => percent,
            x =>
            {
                percent = x;
                percentTxt.text = $"{x}%";
            },
            targetPercent,
            1.2f
        ).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (currentLevel <= 15)
            {
                GameManager.Instance.ChangeState(GameState.Playing);
            }
            else
            {
                GameManager.Instance.ChangeState(GameState.MainMenu);
            }
            AudioManager.Instance.PlayMusic(SoundKey.MainMusic, 0.3f, true);
        });
    }

}
