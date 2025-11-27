using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class GameplayScreen : ScreenUI
{
    [Header("Properties Level & Time")]
    [SerializeField] private Image iconLevelImg;
    [SerializeField] private List<Sprite> levelSprites;
    [SerializeField] private Text levelTxt;
    [SerializeField] private Text timeTxt;

    [Header("Properties Button Action")]
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button retryBtn;
    [SerializeField] private Button pauseBtn;

    [Header("Properties Button Booster")]
    [SerializeField] private Button frozenBtn;
    [SerializeField] private Button bombBtn;
    [SerializeField] private Button hammerBtn;

    private LevelManager levelManager = LevelManager.Instance;

    #region Unity Methods

    private void Update()
    {
        timeTxt.text = FormatTimeMMSS(levelManager.CurrentTime);
    }

    #endregion

    public override void Active()
    {
        base.Active();

        Color32 textColor = new Color32(255, 255, 255, 255);
        // Level & Time
        switch (levelManager.CurrentMap.levelDifficult)
        {
            case LevelDifficult.Normal:
                iconLevelImg.gameObject.SetActive(false);
                textColor = new Color32(255, 255, 255, 255);
                break;
            case LevelDifficult.Hard:
                iconLevelImg.gameObject.SetActive(true);
                iconLevelImg.sprite = levelSprites[0];
                textColor = new Color32(255, 56, 59, 255);
                break;
            case LevelDifficult.SuperHard:
                iconLevelImg.gameObject.SetActive(true);
                iconLevelImg.sprite = levelSprites[1];
                textColor = new Color32(139, 58, 251, 255);
                break;
        }
        levelTxt.text = $"LEVEL {levelManager.CurrentMap.level}";
        levelTxt.color = textColor;

        timeTxt.text = FormatTimeMMSS(levelManager.CurrentTime);

        // Button Action

        // Button Booster
        frozenBtn.onClick.RemoveAllListeners();
        frozenBtn.onClick.AddListener(LevelManager.Instance.OnFrozeBoosterActive);
    }

    protected override void OnScreenDestroyed()
    {

    }

    #region Time Convert

    public static string FormatTimeMMSS(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return $"{m:00}:{s:00}";
    }

    #endregion
}
