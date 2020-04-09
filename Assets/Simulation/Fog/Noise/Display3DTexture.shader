﻿Shader "Hidden/Display3dTexture"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Display ("Texture", 3D) = "white" {}
		Layer ("float", Range(0, 1)) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler3D _Display;
			float Layer;

			fixed4 frag (v2f i) : SV_Target
			{				
				return tex3D(_Display, float3(i.uv.x, i.uv.y, Layer));
			}
			ENDCG
		}
	}
}
