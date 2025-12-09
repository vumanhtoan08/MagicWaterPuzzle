using System.Collections.Generic;
using System;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    DataManager dataManager;

    public void OnStart()
    {
        dataManager = DataManager.Instance;
    }

    public bool CheckMoney(int compareValue)
    {
        int currentMoney = dataManager.GetMoneyData();
        if (currentMoney >= compareValue)
        {
            return true;
        }
        return false;
    }

    public void ChangeMoney(int value)
    {
        int currentMoney = dataManager.GetMoneyData();
        currentMoney = (int)Mathf.Clamp(currentMoney + value, 0, Mathf.Infinity);
        dataManager.SetMoneyData(currentMoney);
    }

    public void ChangeLife(int value)
    {
        int currentLife = dataManager.GetLifeData();
        currentLife = (int)Mathf.Clamp(currentLife + value, 0, Mathf.Infinity);
        dataManager.SetLifeData(currentLife);
    }
    public void ChangeFrozenBooster(int value)
    {
        int currentFrozen = dataManager.GetFrozenData();
        currentFrozen = (int)Mathf.Clamp(currentFrozen + value, 0, Mathf.Infinity);
        dataManager.SetFrozenData(currentFrozen);
    }

    public void ChangeBombBooster(int value)
    {
        int currentBomb = dataManager.GetBombData();
        currentBomb = (int)Mathf.Clamp(currentBomb + value, 0, Mathf.Infinity);
        dataManager.SetBombData(currentBomb);
    }

    public void ChangeHammerBooster(int value)
    {
        int currentHammer = dataManager.GetHammerData();
        currentHammer = (int)Mathf.Clamp(currentHammer + value, 0, Mathf.Infinity);
        dataManager.SetHammerData(currentHammer);
    }


    public int GetCurrentMoney()
    {
        return dataManager.GetMoneyData();
    }
}
