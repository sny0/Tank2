Shader "Unlit/TVNoise"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HorizontalPixels("Horizontal Pixels", Integer) = 100
        _VerticalPixels("Vertical Pixels", Integer) = 100

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
            int _HorizontalPixels;
            int _VerticalPixels;

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
                float x = floor(i.uv.x * _HorizontalPixels);
                float y = floor(i.uv.y * _VerticalPixels);

                float value = sin(dot(float2(x, y) * _Time.y, float2(12.9898, 78.233))) * 43758.5453;
                value = value - floor(value);
                
                float n = value;
                
                float4 col = float4(n, n, n, 1.);
                return col;
            }
            ENDCG
        }
    }
}
