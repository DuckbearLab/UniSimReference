// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Time of Day/Space"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent-450"
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

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct v2f {
                float4 position : POSITION;
                float2 starUV   : TEXCOORD0;
            };

            v2f vert(appdata_base v) {
                v2f o;

                float3 vertnorm = normalize(v.vertex.xyz);

                o.position = UnityObjectToClipPos(v.vertex);
                o.starUV = MultiplyUV(UNITY_MATRIX_TEXTURE0, float2(vertnorm.z, vertnorm.x) / max(0.05, vertnorm.y));

                return o;
            }

            fixed4 frag(v2f i) : COLOR {
                return tex2D(_MainTex, i.starUV.xy);
            }

            ENDCG
        }
    }

    FallBack "None"
}
