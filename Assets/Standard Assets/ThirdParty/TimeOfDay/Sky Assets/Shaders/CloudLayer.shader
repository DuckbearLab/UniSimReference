// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Time of Day/Cloud Layer"
{
    Properties
    {
        // Parameters
        _OneOverGamma   ("Gamma",               float) = 1
        _CloudTone      ("Cloud Tone",          float) = 1
        _CloudDensity   ("Cloud Density",       float) = 1
        _CloudSharpness ("Cloud Sharpness",     float) = 1
        _CloudShading   ("Cloud Shading",       float) = 1
        _CloudSpeed1    ("Cloud Plane 1 Speed", float) = 1
        _CloudSpeed2    ("Cloud Plane 2 Speed", float) = 1.5
        _CloudScale1    ("Cloud Plane 1 Scale", float) = 12
        _CloudScale2    ("Cloud Plane 2 Scale", float) = 13

        // Colors
        _LightColor ("Light Color", Color) = (0, 0, 0, 0)

        // Textures
        _NoiseTexture ("Noise Texture", 2D) = "white" {}
        _NoiseNormals ("Noise Normals", 2D) = "bump" {}

        // Vectors
        _LightDirection ("Light Direction", Vector) = (0, 0, 0, 0)
        _WindDirection  ("Wind Direction",  Vector) = (2, 1, 2, 1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent-400"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Fog
        {
            Mode Off
        }

        Pass
        {
            Cull Front
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _OneOverGamma;
            float _CloudTone;
            float _CloudDensity;
            float _CloudSharpness;
            float _CloudShading;
            float _CloudSpeed1;
            float _CloudSpeed2;
            float _CloudScale1;
            float _CloudScale2;

            float4 _LightColor;
            float3 _LightDirection;

            float4 _WindDirection;

            sampler2D _NoiseTexture;
            sampler2D _NoiseNormals;

            struct v2f {
                float4 position : POSITION;
                fixed4 color    : COLOR;
                float2 cloudUV1 : TEXCOORD0;
                float2 cloudUV2 : TEXCOORD1;
                float3 sundir   : TEXCOORD2;
            };

            v2f vert(appdata_tan v) {
                v2f o;

                // Relative position
                float3 vertnorm = normalize(v.vertex.xyz);

                // Cloud setup
                float2 cloudUV = float2(vertnorm.z, vertnorm.x) / max(0.05, vertnorm.y);
                float cloudTime1 = _Time.x * _CloudSpeed1;
                float cloudTime2 = _Time.x * _CloudSpeed2;

                // Write results
                o.position  = UnityObjectToClipPos(v.vertex);
                o.cloudUV1  = (cloudUV + cloudTime1 * normalize(_WindDirection.xy)) / _CloudScale1;
                o.cloudUV2  = (cloudUV + cloudTime2 * normalize(_WindDirection.zw)) / _CloudScale2;
                o.color.rgb = _LightColor.rgb * _CloudTone;
                o.color.a   = _LightColor.a * pow(vertnorm.y, 0.5);

                // Adjust output color according to gamma value
                o.color = pow(o.color, _OneOverGamma);

                // Transform sun direction into tangent space
                TANGENT_SPACE_ROTATION;
                o.sundir = mul(rotation, -_LightDirection);

                return o;
            }

            fixed4 frag(v2f i) : COLOR {
                fixed4 color = i.color;

                // Cloud texture
                fixed4 noiseTexture1 = tex2D(_NoiseTexture, i.cloudUV1);
                fixed4 noiseTexture2 = tex2D(_NoiseTexture, i.cloudUV2);
                fixed cloud_alpha = noiseTexture1.a * noiseTexture2.a;

                // Cloud normals
                fixed4 noiseNormal1 = tex2D(_NoiseNormals, i.cloudUV1);
                fixed4 noiseNormal2 = tex2D(_NoiseNormals, i.cloudUV2);
                fixed3 cloud_normal = UnpackNormal( 0.5 * (noiseNormal1 + noiseNormal2) );
                fixed cloud_shading = max(0, dot(cloud_normal, i.sundir));

                // Write results
                color.a *= pow(cloud_alpha, _CloudSharpness) * _CloudDensity;
                color.rgb *= lerp(1, cloud_shading, _CloudShading);

                return color;
            }

            ENDCG
        }
    }

    FallBack "None"
}
