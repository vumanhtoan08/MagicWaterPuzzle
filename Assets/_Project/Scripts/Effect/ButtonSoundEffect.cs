using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSoundEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private SoundType soundType = SoundType.Button;

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (soundType)
        {
            case SoundType.None:
                break;
            case SoundType.Button:
                AudioManager.Instance.PlayOneShot(SoundKey.ClickButton, 1f);
                break;
            case SoundType.Cast:
                AudioManager.Instance.PlayOneShot(SoundKey.ReceiveCoin, 1f);
                break;
            case SoundType.ReceiveBooster:
                AudioManager.Instance.PlayOneShot(SoundKey.CollectBooster, 1f);
                break;
        }
    }
}

public enum SoundType
{
    Button,
    None,
    Cast,
    ReceiveBooster
}