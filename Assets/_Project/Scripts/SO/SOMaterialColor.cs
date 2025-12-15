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
    Dictionary<EnumColor, Material> dicMaterialWaterColors = new();
    Dictionary<EnumColor, Material> dicMaterialWaterFlows = new();
    public List<ColorMatchMaterial> materialColors;

    public static Material GetMaterial(EnumColor color) => Instance.dicMaterialColors[color];
    public static Material GetMaterialTrans(EnumColor color) => Instance.dicMaterialTransColors[color];
    public static Material GetMaterialWater(EnumColor color) => Instance.dicMaterialWaterColors[color];
    public static Material GetMaterialWaterFlow(EnumColor color) => Instance.dicMaterialWaterFlows[color];

    public override void Init()
    {
        base.Init();
        foreach (var material in materialColors) 
        {
            dicMaterialColors[material.color] = material.materialColors;
            dicMaterialTransColors[material.color] = material.materialTransColors;
            dicMaterialWaterColors[material.color] = material.materialWaterColors;
            dicMaterialWaterFlows[material.color] = material.materialWaterFlows;
        }
    }
}

[System.Serializable]
public class ColorMatchMaterial
{
    public EnumColor color;
    public Material materialColors;
    public Material materialTransColors;
    public Material materialWaterColors;
    public Material materialWaterFlows;
}
