using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupTutorial : PopupUI, IPointerDownHandler
{
    DataManager dataManager;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        dataManager = DataManager.Instance;
    }

    public override void Show(Action onClose)
    {
        base.Show(onClose);
        int level = dataManager.GetLevelData();
        OnShowPopup(level);
    }

    public override void Hide()
    {
        base.Hide();
    }

    protected override void OnPopupDestroyed()
    {
        base.OnPopupDestroyed();
    }

    private void OnShowPopup(int level)
    {
        switch (level)
        {
            case 7:
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TutorialManager.Instance.OnHasPlayerInput();
        Hide();
    }
}