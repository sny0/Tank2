Shader "Unlit/AwakedEnemy"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _HorizontalPixels("Horizontal Pixels", Integer) = 10
        _VerticalPixels("Vertical Pixels", Integer) = 10
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            LOD 200

            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

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
                int _HorizontalPixels;
                int _VerticalPixels;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    float x = floor(i.uv.x * _HorizontalPixels);
                    float y = floor(i.uv.y * _VerticalPixels);

                    float value = sin(dot(float2(x, y) * _Time.y, float2(12.9898, 78.233))) * 43758.5453;
                    value = value - floor(value);

                    value = lerp(0.1, 1., value);

                    col.rgb *= value;

                    return col;
                }
                ENDCG
            }
        }
}