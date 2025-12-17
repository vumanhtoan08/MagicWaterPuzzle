using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupHammerTutorial : PopupUI
{
    DataManager dataManager;

    [SerializeField] private Button claim;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        claim.onClick.RemoveAllListeners();
        claim.onClick.AddListener(OnClaimClick);

        dataManager = DataManager.Instance;
    }

    public override void Show(Action onClose)
    {
        base.Show(onClose);
    }

    public override void Hide()
    {
        base.Hide();
        TutorialManager.Instance.OnLevel13TutorialTrigger();
        dataManager.SetHammerData(2);
        GameplayScreen gameplayScreen = uiManager.GetScreenActive<GameplayScreen>();
        gameplayScreen.OnUpdateUIFooter();
    }

    protected override void OnPopupDestroyed()
    {
        base.OnPopupDestroyed();
    }

    private void OnClaimClick()
    {
        TutorialManager.Instance.OnLevel13TutorialTrigger();
        Hide();
    }
}