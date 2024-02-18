Shader "Unlit/Sonar"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Transparency("Transparency", Range(0.0, 1.0)) = 0.5
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
                float _Transparency;

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
                    fixed4 col = fixed4(0., 0., 0., 0.);

                    float d = distance(float2(0.5, 0.5), i.uv);
                    d /= distance(float2(0.5, 0.5), float2(0.5, 1));
                    d = min(d, 1.);

                    float d2 = d * 3.14;
                    d2 = abs(sin(d2 - _Time.y * 5));
                    d2 = step(0.99, d2);

                    col.r = 0.95 * d2;


                    col.a = d2 * _Transparency * lerp(0., 1, 1 - d);
                    return col;
                }
                ENDCG
            }
        }
}