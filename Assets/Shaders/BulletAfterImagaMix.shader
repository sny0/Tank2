Shader "Unlit/BulletAfterImagaMix"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BulletAfterImageTex ("Bullet After Image Texture", 2D) = "white" {}
        _MixRate("Mix Rate", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

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
            sampler2D _BulletAfterImageTex;
            float _MixedRate;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col_main = tex2D(_MainTex, i.uv);
                fixed4 col_baf = tex2D(_BulletAfterImageTex, i.uv);

                fixed4 col = float4(0, 0, 0, 1);
                //col.rgb = lerp(col_main.rgb, col_baf.rgb, col_baf.r * _MixedRate);
                //col = col_main;
                //fixed4 col = tex2D(_BulletAfterImageTex, i.uv);
                
                float d = step(0.995, col_baf.a);
                d = 1 - d;
                col.rgb = col_main.rgb * (1. - col_baf.a * d) + col_baf.rgb * col_baf.a * d;

                //col.rgb = col_baf.rgb;
                return col;
            }
            ENDCG
        }
    }
}
