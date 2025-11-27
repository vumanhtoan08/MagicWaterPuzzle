using System.Collections;
using System.Collections.Generic;
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

        // Level & Time
        switch (levelManager.CurrentMap.levelDifficult)
        {
            case LevelDifficult.Normal:
                iconLevelImg.gameObject.SetActive(false);
                break;
            case LevelDifficult.Hard:
                iconLevelImg.gameObject.SetActive(true);
                iconLevelImg.sprite = levelSprites[0];
                break;
            case LevelDifficult.SuperHard:
                iconLevelImg.gameObject.SetActive(true);
                iconLevelImg.sprite = levelSprites[1];
                break;
        }
        levelTxt.text = $"Level {levelManager.CurrentMap.level}";
        timeTxt.text = FormatTimeMMSS(levelManager.CurrentTime);

        // Button Action

        // Button Booster
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
