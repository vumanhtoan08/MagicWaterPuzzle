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
    ZWrite On
    LOD 200

    CGPROGRAM
    #pragma surface surf Standard alpha:fade
    #pragma target 3.0

    fixed4 _Color;
    float _Alpha;
    float _Gloss;
    float _Highlight;
    float _InnerDarkness;

    sampler2D _MatcapTex;

    struct Input {
        float2 uv_MainTex;
        float3 worldNormal;
        float3 viewDir;
    };

    void surf(Input IN, inout SurfaceOutputStandard o)
    {
        float2 uv = IN.uv_MainTex;

        // ======== 1. INNER DARK ========
        float2 c = abs(uv - 0.5);
        float innerMask = 1.0 - saturate((c.x + c.y) * 1.3);
        float3 innerColor = _Color.rgb * (1.0 - _InnerDarkness * (1 - innerMask));

        // ======== 2. EDGE HIGHLIGHT ========
        float edge = smoothstep(0.5, 0.25, distance(uv, float2(0.5, 0.5)));
        float3 highlight = _Highlight * edge;

        // ======== 3. MATCAP REFLECTION ========
        float3 n = normalize(IN.worldNormal);
        float2 matCapUV = n.xy * 0.5 + 0.5;
        float3 matcap = tex2D(_MatcapTex, matCapUV).rgb;

        // ======== 4. FRESNEL RIM LIGHT ========
        float3 v = normalize(IN.viewDir);
        float fresnel = pow(1 - saturate(dot(n, v)), 2.0);
        float3 rim = fresnel * 0.45;

        // ======== 5. FINAL COLOR ========
        float3 finalCol = innerColor;
        finalCol += matcap * 0.25;    // thêm độ bóng
        finalCol += highlight;
        finalCol += rim;              // thêm trong suốt

        o.Albedo = finalCol;
        o.Metallic = 0.05;
        o.Smoothness = _Gloss;

        o.Alpha = _Color.a * _Alpha;
    }
    ENDCG
}


    FallBack "Transparent"
}
