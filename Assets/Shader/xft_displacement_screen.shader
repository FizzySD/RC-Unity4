Shader "Xffect/displacement/screen" {
Properties {
 _DispMap ("Displacement Map (RG)", 2D) = "white" {}
 _MaskTex ("Mask (R)", 2D) = "white" {}
 _DispScrollSpeedX ("Map Scroll Speed X", Float) = 0
 _DispScrollSpeedY ("Map Scroll Speed Y", Float) = 0
 _StrengthX ("Displacement Strength X", Float) = 1
 _StrengthY ("Displacement Strength Y", Float) = -1
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}