using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupSuperHard : PopupUI
{
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
    }

    public override void Show(Action onClose)
    {
        base.Show(onClose);
        AudioManager.Instance.PlayOneShot(SoundKey.Warning, 1f);
        DOVirtual.DelayedCall(1.3f, () =>
        {
            Hide();
        });
    }

    public override void Hide()
    {
        base.Hide();
    }

    protected override void OnPopupDestroyed()
    {
        base.OnPopupDestroyed();
        Destroy(gameObject);
    }
}