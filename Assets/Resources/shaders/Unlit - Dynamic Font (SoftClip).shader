Shader "Unlit/Dynamic Font (SoftClip)" {
    Properties {
        _MainTex ("Alpha (A)", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
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
            // Soft alpha clipping
            o.Alpha = smoothstep(0.45, 0.55, c.a);
        }
        ENDCG
    }
    FallBack "Transparent/VertexLit"
}