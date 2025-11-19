Shader "Linh/Glass" {
	Properties {
		[Header(Shader Properties)] [NoScaleOffset] _MatcapTex ("Matcap Texture", 2D) = "white" {}
		[NoScaleOffset] _TextureAlpha ("TextureAlpha", 2D) = "white" {}
		_Texture ("Texture", 2D) = "white" {}
		_Brightness ("Brightness", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
}