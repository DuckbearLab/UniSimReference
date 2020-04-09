Shader "Hidden/Fog"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		FogStartDistance ("Fog Start Distance", Range(0, 500)) = 0
		FogMaxDistance ("Fog Maximum", Range(100, 1500)) = 500
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
			
			float FogStartDistance, FogMaxDistance;
			float CurveExponent;
			float4 _ScreenSize; //x is width, y is height, z is 1/width, w is 1/height
			sampler2D _MainTex;
			sampler2D _LastCameraDepthTexture; //automatically set by unity to be the last depth texture rendered by any camera. 
			sampler2D FogGradient;

			float InverseLerp(float min, float max, float value)
			{
				return clamp((value-min)/(max-min), 0, 1);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv); //the color rendered originally
				/*
				float totalDepth = 0; //find the average depth over the 7x7 pixel area
				for (int dX = -3; dX < 4; dX++)
				{
					for (int dY = -3; dY < 4; dY++)
					{
						float2 newUV = clamp(i.uv + float2(dX*_ScreenSize.z, dY*_ScreenSize.w), 0, 1);
						float depth = tex2D(_LastCameraDepthTexture, newUV).r; //get the distance to the object at this coord (depth)
						depth = Linear01Depth(depth) * _ProjectionParams.z; //transform depth from 0-1 to unity units
						totalDepth += depth; 
					}
				}
				totalDepth /= 49; 
				float fogAmount = InverseLerp(FogStartDistance, FogMaxDistance, totalDepth); //invLerp from start to max fog distance at value depth
				*/

				float depth = tex2D(_LastCameraDepthTexture, i.uv).r; //get the distance to the object at this coord (depth)
				depth = Linear01Depth(depth) * _ProjectionParams.z; //transform depth from 0-1 to unity units			
				float fogAmount = InverseLerp(FogStartDistance, FogMaxDistance, depth); //invLerp from start to max fog distance at value depth
																						//returns value 0 at depth = min, 1 at depth=max
				float lerpAmount = pow(fogAmount, CurveExponent);	//delinearize value by raising it to a positive power. 
																	//see y=sqrt(x) for values < 1, and y=x^2 for values > 1

				float4 fogColor = tex2D(FogGradient, float2(fogAmount, 0.5f)); //sample the gradient texture for the fog color at this distance
				
				float fogVisibilityFactor = fogColor.a;
				fogColor.a = 1;
				
				return lerp(col, fogColor, lerpAmount * fogVisibilityFactor); //multiply the lerp amount by the alpha
			}
			ENDCG
		}
	}
}
