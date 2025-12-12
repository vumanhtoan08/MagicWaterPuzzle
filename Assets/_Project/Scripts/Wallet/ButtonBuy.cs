using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBuy : MonoBehaviour, IPointerClickHandler
{
    [Header("Config Data")]
    [SerializeField] private int coin = 0;
    [SerializeField] private int life = 0;
    [SerializeField] private int frozenBooter = 0; 
    [SerializeField] private int bombBooter = 0; 
    [SerializeField] private int hammerBooter = 0;

    [SerializeField] private int cost = 0; 

    public int Cost => cost;

    ShopManager shopManager;

    public void OnEnable()
    {
        shopManager = ShopManager.Instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int currentMoney = shopManager.GetCurrentMoney();
        if (currentMoney < cost) return;

        shopManager.ChangeMoney(-cost);

        if (coin > 0) shopManager.ChangeMoney(coin);
        if (life > 0) shopManager.ChangeLife(life);
        if (frozenBooter > 0) shopManager.ChangeFrozenBooster(frozenBooter);
        if (bombBooter > 0) shopManager.ChangeBombBooster(bombBooter);
        if (hammerBooter > 0) shopManager.ChangeHammerBooster(hammerBooter);

        MainMenuScreen mainMenuScreen = UIManager.Instance.GetScreenActive<MainMenuScreen>();
        mainMenuScreen.InitHeader();
    }
}
