Shader "LINH/3D/Water" {
	Properties {
		_WaterEdge ("Water", 2D) = "white" {}
		_Color ("Color", Vector) = (1,1,1,1)
		_Fill ("Fill", Range(0, 1)) = 1
		_Speed01 ("Speed 01", Range(0, 1)) = 0.3
		_Speed02 ("Speed 02", Range(0, 1)) = 0.3
		[IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
}