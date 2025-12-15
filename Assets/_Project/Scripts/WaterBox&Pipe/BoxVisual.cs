using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoxVisual : MonoBehaviour
{
    [SerializeField] private Transform centerPos;
    public Transform CenterPos => centerPos;

    [Header("BoxVFX")]
    [SerializeField] public GameObject VFX_Fly;
    [SerializeField] public GameObject VFX_Fly_2;

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

    [Header("Water Mesh")]
    [SerializeField] private Renderer mainMesh;
    [SerializeField] private Renderer layerMesh;
    [SerializeField] private Renderer mesh1;
    [SerializeField] private Renderer mesh2;

    [Header("Water Mask")]
    [SerializeField] private GameObject waterMask;
    public void ChangeActiveWaterMesh(bool active) => waterMask.SetActive(active);

    public GameObject Stack2Layer => stack2Layer;
    public MeshRenderer KeyMesh => keyMesh;
    public void OnUpdateVisualOfHolderType(HolderData data)
    {
        ResetVisual();

        switch (data.type)
        {
            case HolderType.Basic:
                ApplyMaterials(data);
                ApplyMaterialsToMainWater(data);
                ApplyMaterialsToMergeWater(data);
                break;

            case HolderType.Ice:
                UpdateIceVisual(data);
                ApplyMaterials(data);
                ApplyMaterialsToMainWater(data);
                ApplyMaterialsToMergeWater(data);
                break;

            case HolderType.Direction:
                UpdateDirectionVisual(data);
                ApplyMaterials(data);
                ApplyMaterialsToMainWater(data);
                ApplyMaterialsToMergeWater(data);
                break;

            case HolderType.Stone:
                UpdateStoneVisual(data);
                break;

            case HolderType.Key:
                UpdateKeyVisual(data);
                ApplyMaterials(data);
                ApplyMaterialsToMainWater(data);
                ApplyMaterialsToMergeWater(data);
                break;

            case HolderType.Lock:
                // TODO: visual lock
                break;

            case HolderType.Stack2:
                stack2Layer.SetActive(true);
                ApplyMaterialsSecondaryStack(data);
                ApplyMaterials(data);

                ApplyMaterialsToMainWater(data);
                ApplyMaterialsToLayerWater(data);
                ApplyMaterialsToMergeWater(data);

                if (data.secondaryHolder.type == HolderType.Key)
                {
                    keyMesh.gameObject.SetActive(true);
                    keyMesh.transform.localRotation = Quaternion.Euler(0, 0, keyMesh.transform.localRotation.z - data.rotation);            // đảm bảo key nằm ngang với mọi trường hợp 

                    Material[] materials;
                    materials = new Material[]
                    {
                        SOMaterialColor.GetMaterial(data.secondaryHolder.color)
                    };

                    keyMesh.materials = materials;
                }
                break;

            case HolderType.MergeColor:
                ApplyMaterials(data);
                ApplyMaterialsToHalf(half_02.meshRenderers, data, false, data.secondaryColor);

                ApplyMaterialsToMainWater(data);
                ApplyMaterialsToMergeWater(data);
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

    private void ApplyMaterialsToMainWater(HolderData data)
    {
        mainMesh.material = SOMaterialColor.GetMaterialWater(data.color);
        switch (data.rotation)
        {
            case 0:
                mainMesh.material.SetVector("_FillDir", new Vector4(0, 0, 1, 0));
                if (data.shapeType == HolderShape.ShortL) mainMesh.material.SetVector("_FillDir", new Vector4(1, 0, 0, 0));
                if (data.shapeType == HolderShape.ReverseL || data.shapeType == HolderShape.L || data.shapeType == HolderShape.ShortT) 
                    mainMesh.material.SetVector("_FillDir", new Vector4(0, 0, -1, 0));
                break;
            case 90:
                mainMesh.material.SetVector("_FillDir", new Vector4(1, 0, 0, 0));
                if (data.shapeType == HolderShape.ShortL) mainMesh.material.SetVector("_FillDir", new Vector4(0, 0, -1, 0));
                if (data.shapeType == HolderShape.ReverseL || data.shapeType == HolderShape.L || data.shapeType == HolderShape.ShortT) 
                    mainMesh.material.SetVector("_FillDir", new Vector4(-1, 0, 0, 0));
                break;
            case 180:
                mainMesh.material.SetVector("_FillDir", new Vector4(0, 0, -1, 0));
                if (data.shapeType == HolderShape.ShortL) mainMesh.material.SetVector("_FillDir", new Vector4(-1, 0, 0, 0));
                if (data.shapeType == HolderShape.ReverseL || data.shapeType == HolderShape.L || data.shapeType == HolderShape.ShortT) 
                    mainMesh.material.SetVector("_FillDir", new Vector4(0, 0, 1, 0));
                break;
            case 270:
                mainMesh.material.SetVector("_FillDir", new Vector4(-1, 0, 0, 0));
                if (data.shapeType == HolderShape.ShortL) mainMesh.material.SetVector("_FillDir", new Vector4(0, 0, 1, 0));
                if (data.shapeType == HolderShape.ReverseL || data.shapeType == HolderShape.L || data.shapeType == HolderShape.ShortT) 
                    mainMesh.material.SetVector("_FillDir", new Vector4(1, 0, 0, 0));
                break;
        }
        SetFillAmountToMainMesh(0);
    }

    private void ApplyMaterialsToLayerWater(HolderData data)
    {
        layerMesh.material = SOMaterialColor.GetMaterialWater(data.secondaryHolder.color);
        switch (data.rotation)
        {
            case 0:
                layerMesh.material.SetVector("_FillDir", new Vector4(0, 0, 1, 0));
                if (data.shapeType == HolderShape.ShortL || data.shapeType == HolderShape.L) layerMesh.material.SetVector("_FillDir", new Vector4(1, 0, 0, 0));
                if (data.shapeType == HolderShape.ReverseL) layerMesh.material.SetVector("_FillDir", new Vector4(0, 0, -1, 0));
                break;
            case 90:
                layerMesh.material.SetVector("_FillDir", new Vector4(1, 0, 0, 0));
                if (data.shapeType == HolderShape.ShortL || data.shapeType == HolderShape.L) layerMesh.material.SetVector("_FillDir", new Vector4(0, 0, -1, 0));
                if (data.shapeType == HolderShape.ReverseL) layerMesh.material.SetVector("_FillDir", new Vector4(-1, 0, 0, 0));
                break;
            case 180:
                layerMesh.material.SetVector("_FillDir", new Vector4(0, 0, -1, 0));
                if (data.shapeType == HolderShape.ShortL || data.shapeType == HolderShape.L) layerMesh.material.SetVector("_FillDir", new Vector4(-1, 0, 0, 0));
                if (data.shapeType == HolderShape.ReverseL) layerMesh.material.SetVector("_FillDir", new Vector4(0, 0, 1, 0));
                break;
            case 270:
                layerMesh.material.SetVector("_FillDir", new Vector4(-1, 0, 0, 0));
                if (data.shapeType == HolderShape.ShortL || data.shapeType == HolderShape.L) layerMesh.material.SetVector("_FillDir", new Vector4(0, 0, 1, 0));
                if (data.shapeType == HolderShape.ReverseL) layerMesh.material.SetVector("_FillDir", new Vector4(1, 0, 0, 0));
                break;
        }
        SetFillAmountToLayerMesh(0);
    }

    private void ApplyMaterialsToMergeWater(HolderData data)
    {
        if (data.shapeType == HolderShape.Plus || data.shapeType == HolderShape.ThreeSquare) return;

        mesh1.material = SOMaterialColor.GetMaterialWater(data.color);
        mesh2.material = SOMaterialColor.GetMaterialWater(data.secondaryColor);
        switch (data.rotation)
        {
            case 0:
                mesh1.material.SetVector("_FillDir", new Vector4(0, 0, 1, 0));
                mesh2.material.SetVector("_FillDir", new Vector4(0, 0, 1, 0));
                if (data.shapeType == HolderShape.ShortL || data.shapeType == HolderShape.L) mesh1.material.SetVector("_FillDir", new Vector4(1, 0, 0, 0));
                if (data.shapeType == HolderShape.ShortL || data.shapeType == HolderShape.L) mesh2.material.SetVector("_FillDir", new Vector4(1, 0, 0, 0));
                if (data.shapeType == HolderShape.ReverseL) mesh1.material.SetVector("_FillDir", new Vector4(0, 0, -1, 0));
                if (data.shapeType == HolderShape.ReverseL) mesh2.material.SetVector("_FillDir", new Vector4(0, 0, -1, 0));
                break;
            case 90:
                mesh1.material.SetVector("_FillDir", new Vector4(1, 0, 0, 0));
                mesh2.material.SetVector("_FillDir", new Vector4(1, 0, 0, 0));
                if (data.shapeType == HolderShape.ShortL || data.shapeType == HolderShape.L) mesh1.material.SetVector("_FillDir", new Vector4(0, 0, -1, 0));
                if (data.shapeType == HolderShape.ShortL || data.shapeType == HolderShape.L) mesh2.material.SetVector("_FillDir", new Vector4(0, 0, -1, 0));
                if (data.shapeType == HolderShape.ReverseL) mesh1.material.SetVector("_FillDir", new Vector4(-1, 0, 0, 0));
                if (data.shapeType == HolderShape.ReverseL) mesh2.material.SetVector("_FillDir", new Vector4(-1, 0, 0, 0));
                break;
            case 180:
                mesh1.material.SetVector("_FillDir", new Vector4(0, 0, -1, 0));
                mesh2.material.SetVector("_FillDir", new Vector4(0, 0, -1, 0));
                if (data.shapeType == HolderShape.ShortL || data.shapeType == HolderShape.L) mesh1.material.SetVector("_FillDir", new Vector4(-1, 0, 0, 0));
                if (data.shapeType == HolderShape.ShortL || data.shapeType == HolderShape.L) mesh2.material.SetVector("_FillDir", new Vector4(-1, 0, 0, 0));
                if (data.shapeType == HolderShape.ReverseL) mesh1.material.SetVector("_FillDir", new Vector4(0, 0, 1, 0));
                if (data.shapeType == HolderShape.ReverseL) mesh2.material.SetVector("_FillDir", new Vector4(0, 0, 1, 0));
                break;
            case 270:
                mesh1.material.SetVector("_FillDir", new Vector4(-1, 0, 0, 0));
                mesh2.material.SetVector("_FillDir", new Vector4(-1, 0, 0, 0));
                if (data.shapeType == HolderShape.ShortL || data.shapeType == HolderShape.L) mesh1.material.SetVector("_FillDir", new Vector4(0, 0, 1, 0));
                if (data.shapeType == HolderShape.ShortL || data.shapeType == HolderShape.L) mesh2.material.SetVector("_FillDir", new Vector4(0, 0, 1, 0));
                if (data.shapeType == HolderShape.ReverseL) mesh1.material.SetVector("_FillDir", new Vector4(1, 0, 0, 0));
                if (data.shapeType == HolderShape.ReverseL) mesh2.material.SetVector("_FillDir", new Vector4(1, 0, 0, 0));
                break;
        }
        SetFillAmountToHalf_01Mesh(0);
        SetFillAmountToHalf_02Mesh(0);
    }

    // Chỉnh nước chính 
    public void SetFillAmountToMainMesh(float amount) => mainMesh.material.SetFloat("_FillAmount", amount);
    public float GetFillAmountToMainMesh() => mainMesh.material.GetFloat("_FillAmount");

    // Chỉnh nước phụ
    public void SetFillAmountToLayerMesh(float amount) => layerMesh.material.SetFloat("_FillAmount", amount);
    public float GetFillAmountToLayerMesh() => layerMesh.material.GetFloat("_FillAmount");

    // chỉnh nước half01
    public void SetFillAmountToHalf_01Mesh(float amount) => mesh1.material.SetFloat("_FillAmount", amount);
    public float GetFillAmountToHalf_01Mesh() => mesh1.material.GetFloat("_FillAmount");

    // chỉnh nước half02
    public void SetFillAmountToHalf_02Mesh(float amount) => mesh2.material.SetFloat("_FillAmount", amount);
    public float GetFillAmountToHalf_02Mesh() => mesh2.material.GetFloat("_FillAmount");

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
            if (data.rotation == 180) directionObj[1].SetActive(true);
            if (data.rotation == 270) directionObj[0].SetActive(true);
        }

        if (data.direction == HolderDirection.Vertical)
        {
            if (data.rotation == 0) directionObj[0].SetActive(true);
            if (data.rotation == 90) directionObj[1].SetActive(true);
            if (data.rotation == 180) directionObj[0].SetActive(true);
            if (data.rotation == 270) directionObj[1].SetActive(true);
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
            if (data.rotation == 180) stoneDirection[1].SetActive(true);
            if (data.rotation == 270) stoneDirection[0].SetActive(true);
        }

        if (data.direction == HolderDirection.Vertical)
        {
            if (data.rotation == 0) stoneDirection[0].SetActive(true);
            if (data.rotation == 90) stoneDirection[1].SetActive(true);
            if (data.rotation == 180) stoneDirection[0].SetActive(true);
            if (data.rotation == 270) stoneDirection[1].SetActive(true);
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
