// Shader "Linh/Glass" {
// 	Properties {
// 		[Header(Shader Properties)] [NoScaleOffset] _MatcapTex ("Matcap Texture", 2D) = "white" {}
// 		[NoScaleOffset] _TextureAlpha ("TextureAlpha", 2D) = "white" {}
// 		_Texture ("Texture", 2D) = "white" {}
// 		_Brightness ("Brightness", Float) = 1
// 	}
// 	DummyShaderTextExporter
// 	SubShader{
// 		Tags { "RenderType" = "Opaque" }
// 		LOD 200
// 		CGPROGRAM
// #pragma surface surf Standard
// #pragma target 3.0

// 		struct Input
// 		{
// 			float2 uv_MainTex;
// 		};

// 		void surf(Input IN, inout SurfaceOutputStandard o)
// 		{
// 			o.Albedo = 1;
// 		}
// 		ENDCG
// 	}
// }

Shader "Linh/Glass"
{
    Properties
    {
        [Header(Shader Properties)]
        [NoScaleOffset] _MatcapTex ("Matcap Texture", 2D) = "white" {}
        [NoScaleOffset] _TextureAlpha ("TextureAlpha", 2D) = "white" {}
        _Texture ("Texture", 2D) = "white" {}

        _Brightness ("Brightness", Float) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0

        sampler2D _MatcapTex;
        sampler2D _TextureAlpha;
        sampler2D _Texture;

        float _Brightness;

        struct Input
        {
            float2 uv_Texture;
            float3 viewDir;
            float3 worldNormal;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Base color từ texture người dùng
            float4 baseCol = tex2D(_Texture, IN.uv_Texture);

            // Kênh alpha để điều khiển độ mờ trong
            float alphaMask = tex2D(_TextureAlpha, IN.uv_Texture).r;

            // Mattecap highlight (cho hiệu ứng kính bóng sáng)
            float3 n = normalize(IN.worldNormal);
            float3 v = normalize(IN.viewDir);

            float2 matcapUV = n.xy * 0.5 + 0.5;
            float3 matcap = tex2D(_MatcapTex, matcapUV).rgb;

            // Trộn highlight nhẹ giống game mẫu
            float3 glassLight = lerp(baseCol.rgb, matcap, 0.4);

            // Hiệu ứng sáng viền trong suốt (rim light)
            float rim = 1 - saturate(dot(n, v));
            float3 rimCol = rim * 0.3 * float3(1,1,1);

            // Final albedo
            o.Albedo = (glassLight + rimCol) * _Brightness;

            // Kính mờ → Metallic thấp, Smoothness cao
            o.Metallic = 0.05;
            o.Smoothness = 0.85;

            // Alpha trong suốt
            o.Alpha = alphaMask * baseCol.a;
        }
        ENDCG
    }
}
