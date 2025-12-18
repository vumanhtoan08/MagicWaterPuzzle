using Coffee.UIExtensions;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleEffect : MonoBehaviour
{
    public void ReceiveCoin()
    {
        transform.DOKill();

        transform.localScale = Vector3.one;
        transform.DOScale(1.2f, 0.1f).OnComplete(() =>
        {
            float pitch = Random.Range(1f, 1.2f);
            AudioManager.Instance.PlayOneShot(SoundKey.Coin, 1, pitch);
            MobileVibration.Vibrate(10);
            transform.DOScale(1, 0.05f);
        });
    }
}
