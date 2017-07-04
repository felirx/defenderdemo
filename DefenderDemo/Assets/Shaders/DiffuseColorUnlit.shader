Shader "Custom/Diffuse color unlit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Intensity ("Intensity", Range(1.0, 3.0) ) = 1.0
		_Color ("Tint Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	
	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"		
		}

		Pass
		{
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
		
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
			};

			struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float _Intensity;
			float4 _Color;

			v2f vert(appdata i)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				
				o.uv0 = TRANSFORM_TEX(i.texcoord, _MainTex);
				
				return o;
			}
			
			half4 frag(v2f i) : COLOR
			{
				half4 main_color = tex2D(_MainTex, i.uv0);
				return clamp(main_color * _Intensity * _Color, 0.0f, 1.0f);
			}
			ENDCG
		}
	}
}