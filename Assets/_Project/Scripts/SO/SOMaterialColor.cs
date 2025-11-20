using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SOMaterialColor : SOSingleton<SOMaterialColor>
{
    public List<ColorMatchMaterial> materialColors;

    public Material GetMaterial(EnumColor color)
    {
        foreach (var mat in materialColors)
        {
            if (mat.color == color)
            {
                return mat.materialColors;
            }
        }
        return null;
    }
}

[System.Serializable]
public class ColorMatchMaterial
{
    public EnumColor color;
    public Material materialColors;
}
