using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoxVisual : MonoBehaviour
{
    [Header("Box Visual")]
    [SerializeField] public HalfBox half_01;
    [SerializeField] public HalfBox half_02;

    [Header("For Direction")]
    [SerializeField] private List<GameObject> directionObj;

    [Header("For Ice")]
    [SerializeField] private GameObject iceVisual;
    [SerializeField] private TextMeshPro iceText;

    [Header("For Stone")]
    [SerializeField] private GameObject stone;
    [SerializeField] private List<GameObject> stoneDirection;

    [Header("For Key")]
    [SerializeField] private MeshRenderer keyMesh;

    [Header("For Stack2")]
    [SerializeField] private GameObject stack2Layer; 
    [SerializeField] private HalfBox half_01_Second;
    [SerializeField] private HalfBox half_02_Second;

    public void OnUpdateVisualOfHolderType(HolderData data)
    {
        ResetVisual();

        switch (data.type)
        {
            case HolderType.Basic:
                ApplyMaterials(data);
                break;

            case HolderType.Ice:
                UpdateIceVisual(data);
                ApplyMaterials(data);
                break;

            case HolderType.Direction:
                UpdateDirectionVisual(data);
                ApplyMaterials(data);
                break;

            case HolderType.Stone:
                UpdateStoneVisual(data);
                break;

            case HolderType.Key:
                UpdateKeyVisual(data);
                ApplyMaterials(data);
                break;

            case HolderType.Lock:
                // TODO: visual lock
                break;

            case HolderType.Stack2:
                stack2Layer.SetActive(true);
                ApplyMaterialsSecondaryStack(data);
                ApplyMaterials(data);
                break;

            case HolderType.MergeColor:
                ApplyMaterials(data);
                ApplyMaterialsToHalf(half_02.meshRenderers, data, false, data.secondaryColor);
                break;
        }
    }

    #region Material Apply

    private void ApplyMaterials(HolderData data)
    {
        ApplyMaterialsToHalf(half_01.meshRenderers, data, true);
        ApplyMaterialsToHalf(half_02.meshRenderers, data, false);
    }

    private void ApplyMaterialsSecondaryStack(HolderData data)
    {
        Debug.Log($"Update Visual: {data.secondaryHolder.color}");
        ApplyMaterialsToHalf(half_01_Second.meshRenderers, data, true, data.secondaryHolder.color);
        ApplyMaterialsToHalf(half_02_Second.meshRenderers, data, false, data.secondaryHolder.color);
    }

    private void ApplyMaterialsToHalf(List<MeshRenderer> renderers, HolderData data, bool isHalf01)
    {
        if (renderers == null || renderers.Count == 0) return;

        for (int i = 0; i < renderers.Count; i++)
        {
            MeshRenderer mesh = renderers[i];
            Material[] materials;

            bool useSingle =
                (data.shapeType == HolderShape.ThreeSquare || data.shapeType == HolderShape.TwoSquare)
                && i == 0
                && isHalf01
                ||
                (data.shapeType == HolderShape.TwoSquare && i == 0 && !isHalf01);

            if (useSingle)
            {
                materials = new Material[]
                {
                    SOMaterialColor.GetMaterialTrans(data.color)
                };
            }
            else
            {
                materials = new Material[]
                {
                    SOMaterialColor.GetMaterialTrans(data.color),
                    SOMaterialColor.GetMaterial(data.color)
                };
            }

            mesh.materials = materials;
        }
    }

    private void ApplyMaterialsToHalf(List<MeshRenderer> renderers, HolderData data, bool isHalf01, EnumColor secondaryColor)
    {
        if (renderers == null || renderers.Count == 0) return;

        for (int i = 0; i < renderers.Count; i++)
        {
            MeshRenderer mesh = renderers[i];
            Material[] materials;

            bool useSingle =
                (data.shapeType == HolderShape.ThreeSquare || data.shapeType == HolderShape.TwoSquare)
                && i == 0
                && isHalf01
                ||
                (data.shapeType == HolderShape.TwoSquare && i == 0 && !isHalf01);

            if (useSingle)
            {
                materials = new Material[]
                {
                    SOMaterialColor.GetMaterialTrans(secondaryColor)
                };
            }
            else
            {
                materials = new Material[]
                {
                    SOMaterialColor.GetMaterialTrans(secondaryColor),
                    SOMaterialColor.GetMaterial(secondaryColor)
                };
            }

            mesh.materials = materials;
        }
    }

    #endregion

    #region Ice Visual

    private void UpdateIceVisual(HolderData data)
    {
        iceVisual.SetActive(true);
        iceText.text = $"{data.iceBreak}";
        iceText.transform.localRotation =
            Quaternion.Euler(0, 0, iceText.transform.localRotation.z - data.rotation);
    }

    public void UpdateVisualForIceHolder(int iceBreak)
    {
        iceText.text = $"{iceBreak}";
    }

    public void OnIceBreak()
    {
        iceVisual.SetActive(false);
    }

    #endregion

    #region Direction Visual

    private void UpdateDirectionVisual(HolderData data)
    {
        if (data.direction == HolderDirection.Horizontal)
        {
            if (data.rotation == 0) directionObj[1].SetActive(true);
            if (data.rotation == 90) directionObj[0].SetActive(true);
        }

        if (data.direction == HolderDirection.Vertical)
        {
            if (data.rotation == 0) directionObj[0].SetActive(true);
            if (data.rotation == 90) directionObj[1].SetActive(true);
        }
    }

    #endregion

    #region Stone Visual

    private void UpdateStoneVisual(HolderData data)
    {
        stone.SetActive(true);

        if (data.direction == HolderDirection.Horizontal)
        {
            if (data.rotation == 0) stoneDirection[1].SetActive(true);
            if (data.rotation == 90) stoneDirection[0].SetActive(true);
        }

        if (data.direction == HolderDirection.Vertical)
        {
            if (data.rotation == 0) stoneDirection[0].SetActive(true);
            if (data.rotation == 90) stoneDirection[1].SetActive(true);
        }
    }

    #endregion

    #region Key Visual

    private void UpdateKeyVisual(HolderData data)
    {
        keyMesh.gameObject.SetActive(true);
        keyMesh.transform.localRotation = Quaternion.Euler(0, 0, keyMesh.transform.localRotation.z - data.rotation);            // đảm bảo key nằm ngang với mọi trường hợp 

        Material[] materials;
        materials = new Material[]
        {
            SOMaterialColor.GetMaterial(data.color)
        };

        keyMesh.materials = materials;
    }

    #endregion

    private void ResetVisual()
    {
        iceVisual.SetActive(false);

        foreach (var obj in stoneDirection)
            obj.SetActive(false);
    }
}

[System.Serializable]
public class HalfBox
{
    public List<MeshRenderer> meshRenderers;
}
