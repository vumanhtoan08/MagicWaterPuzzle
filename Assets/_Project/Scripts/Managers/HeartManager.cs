using System;
using UnityEngine; 

public class HeartManager : Singleton<HeartManager> 
{
    private DataManager dataManager;
    private int heartCount;

    public void OnStart()
    {
        dataManager = DataManager.Instance;
        heartCount = dataManager.GetLifeData();
        CalculateWhenFocusGame();
    }

    public void OnUpdate()
    {

    }

    public int HeartCount
    {
        get { return heartCount; }
        set 
        { 
            heartCount = value;
            dataManager.SetLifeData(heartCount);
        }
    }

    public void CalculateWhenFocusGame()
    {
        int lasttime = dataManager.GetDateTimeData();              // thời trong trong data được lưu 

    
    }
}