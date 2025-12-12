using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewMySingleton",
    menuName = "SOSingleton/SODictionaryEffect"
)]
public class SODictionaryEffect : SOSingleton<SODictionaryEffect>
{
    Dictionary<EffectType, Transform> dictionaryEffects = new();
    public List<EffectData> effects;

    public static Transform GetEffectByType(EffectType effectType) => Instance.dictionaryEffects[effectType];

    public override void Init()
    {
        base.Init();
        foreach (var effect in effects)
        {
            dictionaryEffects[effect.effectType] = effect.effect;
        }
    }
}

[System.Serializable]
public class EffectData
{
    public EffectType effectType;
    public Transform effect;
}

public enum EffectType
{
    BoxHit,
    BoxBreak,
    BombExplosion,
    BoxClear,
    Coin
}
