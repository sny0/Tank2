Shader "Unlit/ScanlinesAndStripes"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NumVerticalPixels("Num Vertical Pixels", Integer) = 150
        _NumHorizontalPixels("Num Horizontal Pixels", Integer) = 200
        _ScanSpeed ("Scan Speed", Float) = 1.0
        _StripBlack("Strip Black", Range(0, 1)) = 0.9
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            #define PI 3.14159265359

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _NumVerticalPixels;
            int _NumHorizontalPixels;
            float _ScanSpeed;
            float _StripBlack;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // scanning line
                float dy = 1. / _NumVerticalPixels;

                float y = (_Time.y * _ScanSpeed) % _NumVerticalPixels;

                float d_sl = i.uv.y - (1. - y);
                d_sl = floor(d_sl / dy);
                d_sl += _NumVerticalPixels;
                d_sl %= _NumVerticalPixels;

                d_sl = d_sl / (_NumVerticalPixels * 1.);
                d_sl = lerp(0.8, 1., 1. - d_sl);

                col.rgb = col.rgb * d_sl;
                
                
                // horizontal stripes
                float d_hs = i.uv.y * _NumHorizontalPixels * PI;
                d_hs = abs(sin(d_hs));
                d_hs = step(0.95, d_hs);
                d_hs *= 1. - _StripBlack;

                col.rgb *= 1. - d_hs;

                // vertical stripes
                float d_vs = i.uv.x * _NumVerticalPixels * PI;
                d_vs = abs(sin(d_vs));
                d_vs = step(0.95, d_vs);
                d_vs *= 1. - _StripBlack;

                col.rgb *= 1. - d_vs;
                

                return col;
            }
            ENDCG
        }
    }
}
