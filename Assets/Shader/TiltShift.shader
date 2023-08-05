Shader "Hidden/TiltShift" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Blurred ("Blurred", 2D) = "white" {}
        _Coc ("Coc", 2D) = "white" {}
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _Blurred, _Coc;
            
            v2f vert (appdata_base v) {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 blurred = tex2D(_Blurred, i.uv);
                fixed4 coc = tex2D(_Coc, i.uv);
                fixed4 col = tex2D(_MainTex, i.uv);
                return lerp(col, blurred, coc.a);
            }
            ENDCG
        }
    }
}