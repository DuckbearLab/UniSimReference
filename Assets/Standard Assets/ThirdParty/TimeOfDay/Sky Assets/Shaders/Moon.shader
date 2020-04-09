// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Time of Day/Moon"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D)    = "white" {}
        _Phase   ("Moon Phase", float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent-430"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Fog
        {
            Mode Off
        }

        Pass
        {
            Cull Back
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Phase;

            struct v2f {
                float4 position   : POSITION;
                fixed4 color      : COLOR;
                float2 uv_MainTex : TEXCOORD0;
                float3 normal     : TEXCOORD1;
                float3 viewdir    : TEXCOORD2;
            };

            v2f vert(appdata_base v) {
                v2f o;

                float phaseabs = abs(_Phase);
                float3 offset = 10 * float3(_Phase, -phaseabs, -phaseabs);

                float3 normal  = v.normal;
                float3 viewdir = normalize(ObjSpaceViewDir(v.vertex));

                o.position   = UnityObjectToClipPos(v.vertex);
                o.color.rgb  = 1 - phaseabs;
                o.color.a    = 1;
                o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.normal     = v.normal;
                o.viewdir    = normalize(viewdir + offset);

                return o;
            }

            fixed4 frag(v2f i) : COLOR {
                fixed4 color = i.color;

                fixed shading = max(0, dot(i.normal, i.viewdir));
                color.rgb *= pow(shading, 0.5);

                // Moon texture
                fixed3 moontex = tex2D(_MainTex, i.uv_MainTex);
                color.rgb *= moontex.rgb;

                return color;
            }

            ENDCG
        }
    }

    Fallback "VertexLit"
}
