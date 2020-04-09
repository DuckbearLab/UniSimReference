Shader "Custom/BillboardShaderSlow" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ScaleX ("Scale X", Float) = 1.0
		_ScaleY ("Scale Y", Float) = 1.0
		_MovementX("X Movement Range", Float) = 0.15
		_MovementY ("Y Movement Range", Float) = 0.025
		_MovementXSlowRatio("Movement X Slow Ratio", Float) = 0.35
		_MovementYSlowRatio("Movement Y Slow Ratio", Float) = 0.2
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	

	SubShader {
		Tags {
			"Queue"="Transparent"
			"RenderType"="Transparent"
			"IgnoreProjector"="True"
		}
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM

		#pragma surface surf Lambert vertex:vert

		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform float _ScaleX;
		uniform float _ScaleY;
		uniform float _MovementX;
		uniform float _MovementY;
		uniform float _MovementXSlowRatio;
		uniform float _MovementYSlowRatio;
		fixed _Cutoff;

		struct Input {
			float2 uv_MainTex;
		};

		
		void vert (inout appdata_full v) {
			float3 dirX = normalize(mul(UNITY_MATRIX_IT_MV, float4(1,0,0,1)).xyz);
			float3 dirY = normalize(mul(UNITY_MATRIX_IT_MV, float4(0,1,0,1)).xyz);

			float3 dx = dirX * (v.texcoord.x - 0.5) * _ScaleX
					 + dirX * sin((_Time.z + v.vertex.x * 0.3 + v.vertex.z * 0.6) * _MovementXSlowRatio) * v.texcoord.y * _MovementX;
			float3 dy = dirY * v.texcoord.y * _ScaleY
					 + dirY * sin((_Time.z + v.vertex.x * 0.3 + v.vertex.z * 0.6) * _MovementYSlowRatio) * v.texcoord.y * _MovementY;

			v.vertex.xyz += dx + dy;

			v.normal = float3(0,1,0);
		}

		
		void surf (Input IN, inout SurfaceOutput o) {

			float4 col = tex2D (_MainTex, IN.uv_MainTex);

			clip(col.a - _Cutoff);

			o.Albedo = col.rgb;
		}

		ENDCG

	}
}
