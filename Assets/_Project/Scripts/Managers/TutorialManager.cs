using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    UIManager uiManager;

    [SerializeField] private Transform handTutSlide;
    [SerializeField] private Transform boxChatDirection;
    [SerializeField] private Transform boxChatIce;
    [SerializeField] private Transform boxChatStack; 
    [SerializeField] private Transform boxChatRock; 

    public bool IsTutorialActive { get; set; }

    public void OnStart()
    {
        uiManager = UIManager.Instance;
    }

    public void OnUpdate()
    {
    }

    public void OnTutorialTrigger(int level)
    {
        switch (level)
        {
            case 1:
                OnLevel1TutorialTrigger();
                break;
            case 7:
                uiManager.ShowPopup<PopupTutorial>(null);
                OnLevel7TutorialTrigger();
                break;
            case 8:
                uiManager.ShowPopup<PopupFrozenTutorial>(null);
                break;
            case 10:
                uiManager.ShowPopup<PopupBombTutorial>(null);
                break;
            case 13:
                uiManager.ShowPopup<PopupHammerTutorial>(null);
                break;
            case 18:
                uiManager.ShowPopup<PopupTutorial>(null);
                OnLevel18TutorialTrigger();
                break;
            case 31:
                uiManager.ShowPopup<PopupTutorial>(null);
                OnLevel31TutorialTrigger();
                break;
            case 43:
                uiManager.ShowPopup<PopupTutorial>(null);
                OnLevel43TutorialTrigger();
                break;
        }
    }

    private void OnLevel1TutorialTrigger()
    {
        Transform boxColliderHandTut = LevelManager.Instance.BoxHandleColliders[0].transform;
        IsTutorialActive = true;
        handTutSlide.transform.position = new Vector3(boxColliderHandTut.transform.position.x - 2f, boxColliderHandTut.transform.position.y, 0);
        handTutSlide.gameObject.SetActive(true);    
    }

    private void OnLevel7TutorialTrigger()
    {
        Transform boxColliderHandTut = LevelManager.Instance.BoxHandleColliders[1].transform;
        IsTutorialActive = true;
        boxChatDirection.transform.position = new Vector3(boxColliderHandTut.transform.position.x, boxColliderHandTut.transform.position.y + 3, -2);
        boxChatDirection.gameObject.SetActive(true);
    }

    public void OnLevel8TutorialTrigger()
    {
        IsTutorialActive = true;
        GameplayScreen gameplayScreen = uiManager.GetScreenActive<GameplayScreen>();
        gameplayScreen.ChangeUnmaskFrozen(true);
    }

    public void OnLevel10TutorialTrigger()
    {
        IsTutorialActive = true;
        GameplayScreen gameplayScreen = uiManager.GetScreenActive<GameplayScreen>();
        gameplayScreen.ChangeUnmaskBomb(true);
    }

    public void OnLevel13TutorialTrigger()
    {
        IsTutorialActive = true;
        GameplayScreen gameplayScreen = uiManager.GetScreenActive<GameplayScreen>();
        gameplayScreen.ChangeUnmaskHammer(true);
    }

    private void OnLevel18TutorialTrigger()
    {
        Transform boxColliderHandTut = LevelManager.Instance.BoxHandleColliders[3].transform;
        IsTutorialActive = true;
        boxChatIce.transform.position = new Vector3(boxColliderHandTut.transform.position.x - 0.5f, boxColliderHandTut.transform.position.y + 2.5f, -2);
        boxChatIce.gameObject.SetActive(true);
    }

    public void OnLevel31TutorialTrigger()
    {
        Transform boxColliderHandTut = LevelManager.Instance.BoxHandleColliders[0].transform;
        IsTutorialActive = true;
        boxChatStack.transform.position = new Vector3(boxColliderHandTut.transform.position.x - 0.5f, boxColliderHandTut.transform.position.y + 2.5f, -2);
        boxChatStack.gameObject.SetActive(true);
    }

    public void OnLevel43TutorialTrigger()
    {
        Transform boxColliderHandTut = LevelManager.Instance.BoxHandleColliders[0].transform;
        IsTutorialActive = true;
        boxChatRock.transform.position = new Vector3(boxColliderHandTut.transform.position.x - 1.5f, boxColliderHandTut.transform.position.y + 2.5f, -2);
        boxChatRock.gameObject.SetActive(true);
    }

    public void OnHasPlayerInput()
    {
        handTutSlide.gameObject.SetActive(false);
        boxChatDirection.gameObject.SetActive(false);
        boxChatIce.gameObject.SetActive(false);
        boxChatStack.gameObject.SetActive(false);
        boxChatRock.gameObject.SetActive(false);
        IsTutorialActive = false;
    }
}
