Shader "Custom/MeshFillSurface"
{
    Properties
    {
        _Color ("Color", Vector) = (1,1,1,1)
        _Fill ("Fill Amount", Range(0,1)) = 1
        _MinZ ("Mesh MinZ", Float) = 0
        _Depth ("Mesh Depth", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Standard clipthreshold:0.1

        float4 _Color;
        float  _Fill;
        float  _MinZ;
        float  _Depth;

        struct Input {
            float3 worldPos;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float localZ = mul(unity_WorldToObject, float4(IN.worldPos,1)).z;
            float normalizedZ = (localZ - _MinZ) / _Depth;

            if (normalizedZ > _Fill)
                clip(-1);

            o.Albedo = _Color.rgb;
            o.Alpha  = _Color.a;
        }
        ENDCG
    }
}
