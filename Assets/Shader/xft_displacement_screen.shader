Shader "Xffect/displacement/screen" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DispMap ("Displacement Map (RG)", 2D) = "white" {}
        _MaskTex ("Mask (R)", 2D) = "white" {}
        _DispScrollSpeedX ("Map Scroll Speed X", Float) = 0
        _DispScrollSpeedY ("Map Scroll Speed Y", Float) = 0
        _StrengthX ("Displacement Strength X", Float) = 1
        _StrengthY ("Displacement Strength Y", Float) = -1
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex, _DispMap, _MaskTex;
            float _DispScrollSpeedX, _DispScrollSpeedY, _StrengthX, _StrengthY;

            v2f vert (float4 vertex : POSITION, float2 uv : TEXCOORD0) {
                v2f o;
                o.pos = vertex;
                o.uv = uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 disp = tex2D(_DispMap, i.uv + _Time.yy * float2(_DispScrollSpeedX, _DispScrollSpeedY)).rg;
                float mask = tex2D(_MaskTex, i.uv).r;
                float2 uv = i.uv + mask * (disp - 0.5) * float2(_StrengthX, _StrengthY);
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}