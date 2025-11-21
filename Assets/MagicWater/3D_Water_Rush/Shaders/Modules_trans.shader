Shader "Shader Graphs/Modules_trans_Glass" {
    Properties {
        _Color ("Color", Color) = (1, 0.4, 0.6, 0.4)
        _Alpha ("Alpha", Range(0,1)) = 0.4
        _Gloss ("Glossiness", Range(0,1)) = 0.6
        _Highlight ("Edge Highlight", Range(0,1)) = 0.4
        _InnerDarkness ("Inner Darkness", Range(0,1)) = 0.35
    }

    SubShader{

        Tags { 
            "Queue"="Transparent"
            "RenderType"="Transparent" 
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0

        fixed4 _Color;
        float _Alpha;
        float _Gloss;
        float _Highlight;
        float _InnerDarkness;

        struct Input {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_MainTex;

            // -------------------------------
            // 1. INNER DARK (tối ở giữa)
            // -------------------------------
            float2 fromCenter = abs(uv - 0.5);
            float innerMask = 1.0 - saturate((fromCenter.x + fromCenter.y) * 1.3);
            float3 innerColor = _Color.rgb * (1.0 - _InnerDarkness * (1 - innerMask));

            // -------------------------------
            // 2. EDGE HIGHLIGHT (viền sáng)
            // -------------------------------
            float edge = smoothstep(0.5, 0.25, distance(uv, float2(0.5, 0.5)));
            float3 highlight = _Highlight * edge * float3(1,1,1);

            // -------------------------------
            // 3. FINAL COLOR
            // -------------------------------
            o.Albedo = innerColor + highlight;

            // Glossiness = tạo bóng kính
            o.Smoothness = _Gloss;
            o.Metallic = 0;

            // Alpha trong suốt
            o.Alpha = _Color.a * _Alpha;
        }
        ENDCG
    }

    FallBack "Transparent"
}
