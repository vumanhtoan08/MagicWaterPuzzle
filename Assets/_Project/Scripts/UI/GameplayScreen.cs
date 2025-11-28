using DG.Tweening;
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
    [SerializeField] private Button timeBtn;

    [Header("Properties Button Booster")]
    [SerializeField] private Button frozenBtn;
    [SerializeField] private Button bombBtn;
    [SerializeField] private Button hammerBtn;

    private LevelManager levelManager = LevelManager.Instance;

    #region Unity Methods

    private void Update()
    {
        timeTxt.text = FormatTimeMMSS(levelManager.CurrentTime);
        OnScreenFroze();
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

    #region Game Visual

    [Header("Frozen Screen")]
    [SerializeField] private GameObject frozeScreen;
    [SerializeField] private Image timeButtonImg;
    [SerializeField] private List<Sprite> timeButtonFroze;
    [SerializeField] private GameObject clockObj;                       // đồng hồ lúc bình thường
    [SerializeField] private GameObject frezenObj;                       // đồng hồ lúc đóng băng
    [SerializeField] private GameObject iceCounter;
    [SerializeField] private Text timeFrozeTxt;
 
    private bool isFrozenScreenActive = false;
    private bool isReskinFrozen = false;
    private void OnScreenFroze()
    {
        //if (!LevelManager.Instance.IsFroze) return;

        if (!isFrozenScreenActive) isFrozenScreenActive = true;

        if (!isReskinFrozen)
        {
            frozeScreen.SetActive(LevelManager.Instance.IsFroze ? true : false);
            timeButtonImg.sprite = LevelManager.Instance.IsFroze ? timeButtonFroze[1] : timeButtonFroze[0];
            timeBtn.interactable = LevelManager.Instance.IsFroze ? false : true;
            clockObj.SetActive(LevelManager.Instance.IsFroze ? false : true);
            frezenObj.SetActive(LevelManager.Instance.IsFroze ? true : false);
            iceCounter.SetActive(LevelManager.Instance.IsFroze ? true : false);
        }

        timeFrozeTxt.text = $"{(int)LevelManager.Instance.FrozeTimeCouter}";
    }

    #endregion

    #region Time Convert

    public static string FormatTimeMMSS(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return $"{m:00}:{s:00}";
    }

    #endregion
}
