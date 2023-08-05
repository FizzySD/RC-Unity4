Shader "Unlit/Premultiplied Colored (SoftClip)" {
    Properties {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Lambert alpha
        #pragma target 3.0

        sampler2D _MainTex;
        
        // This defines the soft clipping threshold
        #define CLIP_THRESHOLD 0.01

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = smoothstep(CLIP_THRESHOLD, 1.0, c.a);
        }
        ENDCG
    }
    FallBack "Transparent/VertexLit"
}