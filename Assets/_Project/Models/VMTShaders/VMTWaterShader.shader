Shader "Custom/MeshFillLiquid"
{
    Properties
    {
        _Color ("Color", Vector) = (1,1,1,1)
        _Fill ("Fill Amount", Range(0,1)) = 1

        _MinZ ("Min Z", Float) = 0
        _Depth ("Depth", Float) = 1

        _Speed01 ("Speed 01", Range(0, 1)) = 1
        _Speed02 ("Speed 02", Range(0, 1)) = 1
        [IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0

        _FoamColor ("Foam Color", Color) = (1,1,1,1)
        _FoamThickness ("Foam Thickness", Range(0.001, 0.2)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Standard clipthreshold:0.1
        #pragma target 3.0

        float4 _Color;
        float  _Fill;
        float  _MinZ;
        float  _Depth;

        float _Speed01;
        float _Speed02;

        float4 _FoamColor;
        float  _FoamThickness;

        struct Input
        {
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Convert worldPos → local Z
            float localZ = mul(unity_WorldToObject, float4(IN.worldPos, 1)).z;
            float baseLevel = (localZ - _MinZ) / _Depth;

            float waveFill = _Fill;

            // Only calculate waves if 0 < Fill < 1
            bool hasWave = (_Fill > 0.0 && _Fill < 1.0);

            float frontWave = 0;
            float backWave = 0;

            if (hasWave)
            {
                // Foreground wave
                frontWave =
                    sin(IN.worldPos.x * 10 + _Time.y * (_Speed01 * 4)) * 0.02 +
                    cos(IN.worldPos.z * 8  + _Time.y * (_Speed02 * 5)) * 0.015;

                // Background wave (reverse direction)
                backWave =
                    sin(IN.worldPos.x * 6 - _Time.y * (_Speed01 * 2)) * 0.015 +
                    cos(IN.worldPos.z * 4 - _Time.y * (_Speed02 * 2)) * 0.010;

                waveFill = _Fill + frontWave + backWave;
            }

            // Clip water surface
            if (baseLevel > waveFill)
                clip(-1);

            // Foam only when waves exist
            float foamMask = 0;

            if (hasWave)
            {
                foamMask = saturate(1 - abs(baseLevel - waveFill) / _FoamThickness);
            }

            float3 finalColor = lerp(_Color.rgb, _FoamColor.rgb, foamMask);

            o.Albedo = finalColor;
            o.Alpha  = _Color.a;
        }

        ENDCG
    }
}
