using UnityEngine;
using System;
using UnityEngine.UI;

public class PopupSetting : PopupUI
{
    AudioManager audioManager;

    [SerializeField] private Button musicButton;
    [SerializeField] private Button soundButton;
    [SerializeField] private Button vibrateButton;
    [SerializeField] private Button exitButton;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        audioManager = AudioManager.Instance;

        musicButton.onClick.RemoveAllListeners();
        musicButton.onClick.AddListener(OnMusicButtonClick);

        soundButton.onClick.RemoveAllListeners();
        soundButton.onClick.AddListener(OnSoundButtonClick);

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    public override void Show(Action onClose)
    {
        base.Show(onClose);
        int musicVolume = AudioManager.MusicSetting;
        if (musicVolume == 1)
        {
            foreach (Transform item in musicButton.transform)
            {
                item.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (Transform item in musicButton.transform)
            {
                item.gameObject.SetActive(true);
            }
        }

        int soundVolume = AudioManager.SoundSetting;
        if (soundVolume == 1)
        {
            foreach (Transform item in soundButton.transform)
            {
                item.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (Transform item in soundButton.transform)
            {
                item.gameObject.SetActive(true);
            }
        }

    }

    public override void Hide()
    {
        base.Hide();
    }

    protected override void OnPopupDestroyed()
    {
        base.OnPopupDestroyed();
    }

    private void OnMusicButtonClick()
    {
        int musicVolume = AudioManager.MusicSetting;
        if (musicVolume == 1)
        {
            audioManager.EnableMusic(false);
            foreach (Transform item in musicButton.transform)
            {
                item.gameObject.SetActive(true);
            }
        }
        else
        {
            audioManager.EnableMusic(true, 0.3f);
            foreach (Transform item in musicButton.transform)
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    private void OnSoundButtonClick()
    {
        int soundVolume = AudioManager.SoundSetting;
        if (soundVolume == 1)
        {
            audioManager.EnableSound(false);
            foreach (Transform item in soundButton.transform)
            {
                item.gameObject.SetActive(true);
            }
        }
        else
        {
            audioManager.EnableSound(true);
            foreach (Transform item in soundButton.transform)
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    private void OnVibrateButtonClick()
    {

    }

    private void OnExitButtonClick()
    {
        Hide();
    }
}