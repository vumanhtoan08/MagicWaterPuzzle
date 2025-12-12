using System;
using UnityEngine; 

public class HeartManager : Singleton<HeartManager> 
{
    private DataManager dataManager;
    private int heartCount;
    private float coolDownHeart = 300f; 
    public float CoolDownHeart => coolDownHeart;

    public void OnStart()
    {
        dataManager = DataManager.Instance;
        heartCount = dataManager.GetLifeData();
    }

    public void OnUpdate()
    {

    }

    public (int recoveryHeartsCount,int timeOverflow) NumberOfRecoveryHearts()
    {
        int timePass = dataManager.GetTimePassData();

        int recoveryHeartsCount = timePass / (int)coolDownHeart;       // tính toán số lượng trái tim nhân được
        int timeOverflow = timePass % (int)coolDownHeart;              // chia lấy dư để có được thời gian dư thừa

        return (recoveryHeartsCount, timeOverflow);
    }

    // thay đổi life lưu 1 lần date time
    public void ChangeLife(int value)
    {
        int currentLife = dataManager.GetLifeData();
        currentLife += value;
        currentLife = Mathf.Clamp(currentLife, 0, 10000);
        dataManager.SetLifeData(currentLife);
    }
}