using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.LightingExplorerTableColumn;

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

    //[SerializeField] private GameObject lockVisual;   
    //[SerializeField] private GameObject keyVisual;
    //[SerializeField] private GameObject lidsVisual;

    public void OnUpdateVisualOfHolderType(HolderData data)
    {
        switch (data.type)
        {
            case HolderType.Basic:
                if (half_01.meshRenderers.Count <= 0) return;

                int indexHalf01 = 0;

                foreach (var mesh in half_01.meshRenderers)
                {
                    Material[] materials;

                    if ((data.shapeType == HolderShape.ThreeSquare || data.shapeType == HolderShape.TwoSquare) && indexHalf01 == 0)
                    {
                        materials = new Material[1];
                        materials[0] = SOMaterialColor.GetMaterialTrans(data.color);
                        mesh.materials = materials;
                        Debug.Log("1 Material");

                        indexHalf01++; 
                        continue;
                    }

                    materials = new Material[2];
                    materials[0] = SOMaterialColor.GetMaterialTrans(data.color);
                    materials[1] = SOMaterialColor.GetMaterial(data.color);
                    mesh.materials = materials;

                    indexHalf01++;
                }
                if (half_02.meshRenderers.Count <= 0) return;

                int indexHalf02 = 0;

                foreach (var mesh in half_02.meshRenderers)
                {
                    Material[] materials;

                    if (data.shapeType == HolderShape.TwoSquare && indexHalf02 == 0)
                    {
                        materials = new Material[1];
                        materials[0] = SOMaterialColor.GetMaterialTrans(data.color);
                        mesh.materials = materials;

                        indexHalf02++;
                        continue;
                    }

                    materials = new Material[2];
                    materials[0] = SOMaterialColor.GetMaterialTrans(data.color);
                    materials[1] = SOMaterialColor.GetMaterial(data.color);
                    mesh.materials = materials;

                    indexHalf02++;
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