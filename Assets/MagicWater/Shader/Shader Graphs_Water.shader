Shader "Custom/MeshFillPro"
{
    Properties
    {
        _WaterColor ("Water Color", Color) = (1,0,0,1)
        _FillAmount("Fill Amount", Range(0,1)) = 0.5
        _FillDir ("Fill Direction", Vector) = (0,1,0,0)
        _MinValue ("Min", Float) = -1
        _MaxValue ("Max", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            stencil
            {
                ref 1
                comp equal
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _WaterColor;
            float _FillAmount;
            float4 _FillDir;
            float _MinValue;
            float _MaxValue;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 objPos : TEXCOORD0;   // object space position
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.objPos = v.vertex.xyz; // dùng object space y
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Lấy giá trị fill theo hướng FillDir trong object space
                float dirValue = dot(i.objPos, normalize(_FillDir.xyz));

                // Chuẩn hóa về 0–1 trong khoảng Min/Max
                float normalized = saturate( (dirValue - _MinValue) / (_MaxValue - _MinValue) );

                // Clip như stencil → cắt mesh
                if (normalized > _FillAmount)
                    clip(-1);   // bỏ pixel

                return _WaterColor;
            }

            ENDCG
        }
    }
}
