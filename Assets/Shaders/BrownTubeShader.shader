// éQçlÅFhttps://sayachang-bot.hateblo.jp/entry/2019/12/11/231351
Shader "Custom/BrownTubeShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;

			float2 barrel(float2 uv) {
				float s1 = .99, s2 = .04;
				float2 centre = 2. * uv - 1.;
				float barrel = min(1. - length(centre) * s1, 1.0) * s2;
				return uv - centre * barrel;
			}

			float2 CRT(float2 uv)
			{
				float2 nu = uv * 2. - 1.;
				float2 offset = abs(nu.yx) / float2(12., 8.);
				nu += nu * offset * offset;
				return nu;
			}

			fixed4 frag(v2f_img i) : SV_Target
			{
				// barrel distortion
				float2 p = barrel(i.uv);
				fixed4 col = tex2D(_MainTex, p);
				
				// crt monitor
				float2 crt = CRT(i.uv);
				crt = abs(crt);
				crt = pow(crt, 20.);
				col.rgb = lerp(col.rgb, (.0).xxx, crt.x + crt.y);

				return col;
			}
			ENDCG
		}
	}
}