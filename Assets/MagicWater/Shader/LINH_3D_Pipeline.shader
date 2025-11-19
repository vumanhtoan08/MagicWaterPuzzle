Shader "LINH/UI/GlassEdgeLine"
{
    Properties
    {
        _EdgeColor("Edge Color", Color) = (1,1,1,0.8)
        _InnerAlpha("Inner Transparency", Range(0,1)) = 0.1
        _EdgeWidth("Edge Thickness", Range(0.001, 0.5)) = 0.15
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _EdgeColor;
            float _InnerAlpha;
            float _EdgeWidth;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // khoảng cách từ mép trái/phải
                float dist = min(i.uv.x, 1 - i.uv.x);

                // phần viền (gần mép)
                float edge = saturate(1 - dist / _EdgeWidth);

                // alpha viền (trắng)
                float edgeAlpha = _EdgeColor.a * edge;

                // alpha phần giữa (rất trong suốt)
                float innerAlpha = _InnerAlpha * (1 - edge);

                float finalAlpha = edgeAlpha + innerAlpha;

                float3 finalColor = lerp(0, _EdgeColor.rgb, edge);

                return float4(finalColor, finalAlpha);
            }
            ENDCG
        }
    }
}
