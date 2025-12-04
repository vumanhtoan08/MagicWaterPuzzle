using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : ScreenUI
{
    [Header("Header Property")]
    [SerializeField] private Button moneyHeaderBtn;
    [SerializeField] private Button settingHeaderBtn;
    [SerializeField] private Text moneyValueHeaderTxt; 
    [SerializeField] private Text lifeValueHeaderTxt; 
    [SerializeField] private Text lifeCounterHeaderTxt; 


    public override void Initialize(UIManager uiManager)
    {
        base.Initialize(uiManager);
    }
    public override void Active()
    {
        base.Active();
    }
    public override void Deactive()
    {
        base.Deactive();
    }

    protected override void OnScreenDestroyed()
    {

    }
}
