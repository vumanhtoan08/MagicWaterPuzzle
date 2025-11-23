using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewMySingleton",
    menuName = "SOSingleton/SOMaterialColor"
)]
public class SOMaterialColor : SOSingleton<SOMaterialColor>
{
    Dictionary<EnumColor, Material> dicMaterialColors = new(); 
    Dictionary<EnumColor, Material> dicMaterialTransColors = new(); 
    public List<ColorMatchMaterial> materialColors;

    public static Material GetMaterial(EnumColor color) => Instance.dicMaterialColors[color];
    public static Material GetMaterialTrans(EnumColor color) => Instance.dicMaterialTransColors[color];

    public override void Init()
    {
        base.Init();
        foreach (var material in materialColors) 
        {
            dicMaterialColors[material.color] = material.materialColors;
            dicMaterialTransColors[material.color] = material.materialTransColors;
        }
    }
}

[System.Serializable]
public class ColorMatchMaterial
{
    public EnumColor color;
    public Material materialColors;
    public Material materialTransColors;
}
