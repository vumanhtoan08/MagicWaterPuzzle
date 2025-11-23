using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxVisual : MonoBehaviour
{
    [Header("Box Visual")]
    [SerializeField] public HalfBox half_01;
    [SerializeField] public HalfBox half_02;

    [Header("For Box Type")]
    [SerializeField] private List<GameObject> directionObj;
    [SerializeField] private GameObject iceVisual;
    [SerializeField] private GameObject stone;  
    [SerializeField] private List<GameObject> stoneDirection;  

    [SerializeField] private GameObject lockVisual;   
    [SerializeField] private GameObject keyVisual;
    [SerializeField] private GameObject lidsVisual;

    public void OnUpdateVisualOfHolderType(HolderType type, EnumColor color)
    {
        switch (type)
        {
            case HolderType.Basic:
                Debug.Log("Thay màu");
                if (half_01.meshRenderers.Count <= 0) return;
                foreach (var mesh in half_01.meshRenderers)
                {
                    Material[] materials = new Material[2];
                    materials[0] = SOMaterialColor.GetMaterialTrans(color);
                    materials[1] = SOMaterialColor.GetMaterial(color);
                    mesh.materials = materials;
                }
                if (half_02.meshRenderers.Count <= 0) return;
                foreach (var mesh in half_02.meshRenderers)
                {
                    Material[] materials = new Material[2];
                    materials[0] = SOMaterialColor.GetMaterialTrans(color);
                    materials[1] = SOMaterialColor.GetMaterial(color);
                    mesh.materials = materials;
                }
                break;
            case HolderType.Ice:
                break;
            case HolderType.Direction:
                break;
            case HolderType.Stone:
                break;
            case HolderType.Key:
                break;
            case HolderType.Lock:
                break;
        }
    }
}
    [System.Serializable]
public class HalfBox
{
    public List<MeshRenderer> meshRenderers;
}