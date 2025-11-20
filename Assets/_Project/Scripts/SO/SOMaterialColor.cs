using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SOMaterialColor : SOSingleton<SOMaterialColor>
{
    public List<ColorMatchMaterial> materialColors;
}

[System.Serializable]
public class ColorMatchMaterial
{
    public EnumColor color;
    public Material materialColors;
}
