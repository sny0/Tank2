Shader "Unlit/Mix"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MixedSideTex ("MixedSideTexture", 2D) = "white" {}
        _MixRate ("MixRate", Range(0.0, 1.0)) = 0.0 
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
            sampler2D _MixedSideTex;
            float4 _MainTex_ST;

            float _MixRate;

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
                fixed4 col_mixed = tex2D(_MixedSideTex, i.uv);
                
                //float rate = min(1., _Time.y / _NoiseTime);
                
                float4 col = float4(0, 0, 0, 1);
                col.rbg = lerp(col_main.rgb, col_mixed.rgb, _MixRate);

                return col;
            }
            ENDCG
        }
    }
}
