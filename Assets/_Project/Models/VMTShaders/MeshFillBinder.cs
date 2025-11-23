using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(Renderer))]
public class MeshFillBinder : MonoBehaviour
{
    private Material material;

    void Start()
    {
        material = GetComponent<Renderer>().material;

        var mesh = GetComponent<MeshFilter>().sharedMesh;
        var bounds = mesh.bounds;

        float minZ = bounds.min.z;
        float depth = bounds.size.z;

        material.SetFloat("_MinZ", minZ);
        material.SetFloat("_Depth", depth);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.U))
        {
            Debug.Log("Update");
            SetMaterialsFill(0.1f);
        }
        if (Input.GetKey(KeyCode.I))
        {
            SetMaterialsFill(-0.1f);
        }
    }

    private void SetMaterialsFill(float value)
    {
        float currentAmount = material.GetFloat("_Fill");
        currentAmount += value;
        material.SetFloat("_Fill", Mathf.Clamp01(currentAmount));
    }
}