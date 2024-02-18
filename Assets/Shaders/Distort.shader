Shader "Unlit/Distort"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistortGain("Distort Gain", Float) = 1
        _DistortTimeGain("Distort Time Gain", Integer) = 10
        _DistortFrequency("Distort Frequency", Range(0, 1)) = 0.1
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
            float4 _MainTex_ST;
            sampler2D _Tex;

            float _DistortGain;
            int _DistortTimeGain;
            float _DistortFrequency;

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
                i.uv.x += sin(i.uv.y * 10 - _Time.y * 30) * _DistortGain * step(1 - _DistortFrequency, frac(_Time.y));
                i.uv.x = frac(i.uv.x);
                fixed4 col = tex2D(_MainTex, i.uv);
                
                return col;
            }
            ENDCG
        }
    }
}
