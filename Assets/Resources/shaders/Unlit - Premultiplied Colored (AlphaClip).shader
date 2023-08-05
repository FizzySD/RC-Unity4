Shader "Unlit/Premultiplied Colored (AlphaClip)" {
    Properties {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType" = "AlphaTest" "Queue" = "Transparent" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Lambert alpha
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            clip(c.a - 0.01);  // Change this value as needed
        }
        ENDCG
    }
    FallBack "Transparent/VertexLit"
}