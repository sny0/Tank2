Shader "Unlit/AfterImageBuffer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AfterImageTex ("AfterImageTexture", 2D) = "white" {}
        _AfterImageTime("AfterImageFlame", Range(0, 10)) = 5
        _AfterEffectRed("After Effect Red", Range(0, 1)) = 1
        _AfterEffectGreen("After Effect Green", Range(0, 1)) = 0
        _AfterEffectBlue("After Effect Blue", Range(0, 1)) = 0

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
            sampler2D _AfterImageTex;
            float _AfterImageTime;

            float _AfterEffectRed;
            float _AfterEffectGreen;
            float _AfterEffectBlue;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = float4(0, 0, 0, 0);
                fixed4 col_main = tex2D(_MainTex, i.uv);
                fixed4 col_buffer = tex2D(_AfterImageTex, i.uv);

                col_buffer.a = (_AfterImageTime * col_buffer.a - unity_DeltaTime.x) / _AfterImageTime;
                col_buffer.a = max(col_buffer.a, 0);

                col_main.a = step(0.9, col_main.a);
                
                col.r = _AfterEffectRed * max(col_main.a, col_buffer.a);
                col.g = _AfterEffectGreen * max(col_main.a, col_buffer.a);
                col.b = _AfterEffectBlue * max(col_main.a, col_buffer.a);

                col.a = max(col_main.a, col_buffer.a);

                return col;
            }
            ENDCG
        }
    }
}
