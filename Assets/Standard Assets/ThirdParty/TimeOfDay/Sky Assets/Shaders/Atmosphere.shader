// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Time of Day/Atmosphere"
{
    Properties
    {
        // Parameters
        _OneOverGamma  ("Gamma",          float) = 1
        _DayBrightness ("Day Brightness", float) = 1
        _DayHaziness   ("Day Haziness",   float) = 0.1
        _NightHaziness ("Night Haziness", float) = 0.1

        // Colors
        _SunColor       ("Sun Color",        Color) = (0, 0, 0, 0)
        _DayColor       ("Day Color",        Color) = (0, 0, 0, 0)
        _NightColor     ("Night Color",      Color) = (0, 0, 0, 0)
        _NightHazeColor ("Night Haze Color", Color) = (0, 0, 0, 0)

        // Vectors
        _SunDirection      ("Sun Direction",       Vector) = (0, 0, 0, 0)
        _BetaRayleigh      ("Beta Rayleigh",       Vector) = (0, 0, 0, 0)
        _BetaRayleighTheta ("Beta Rayleigh Theta", Vector) = (0, 0, 0, 0)
        _BetaMie           ("Beta Mie",            Vector) = (0, 0, 0, 0)
        _BetaMieTheta      ("Beta Mie Theta",      Vector) = (0, 0, 0, 0)
        _HenyeyGreenstein  ("Henyey Greenstein",   Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent-420"
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
            float _DayBrightness;
            float _DayHaziness;
            float _NightHaziness;

            float4 _SunColor;
            float3 _DayColor;
            float3 _NightColor;
            float3 _NightHazeColor;

            float3 _SunDirection;
            float3 _BetaRayleigh;
            float3 _BetaRayleighTheta;
            float3 _BetaMie;
            float3 _BetaMieTheta;
            float3 _HenyeyGreenstein;

            struct v2f {
                float4 position : POSITION;
                fixed4 color    : COLOR;
            };

            v2f vert(appdata_base v) {
                v2f o;

                // Position and direction calculations
                float3 vertnorm = normalize(v.vertex.xyz);
                float cosTheta  = dot(-v.normal, _SunDirection);

                // Optical depth - parameter value
                // See [2] page 70 equation (5.7)
                float ny     = max(0.001, v.normal.y);
                float fDay   = pow(ny, _DayHaziness);
                float fNight = pow(ny, _NightHaziness);

                // Optical depth - simplified as far as possible
                // See [2] page 71 equation (5.8)
                // See [2] page 71 equation (5.10)
                // See [2] page 76 equation (6.1)
                float sr = 190000 - fDay*(360000 - fDay*190000);
                float sm = 190000 - fDay*(372000 - fDay*190000);

                // Rayleigh scattering factor
                // See [2] page 64 equation (5.2)
                float3 betaRayleighTheta = _BetaRayleighTheta * (2.0 + 0.5*cosTheta*cosTheta);

                // Mie scattering factor
                // See [2] page 65 equation (5.3)
                float3 betaMieTheta = _BetaMieTheta * _HenyeyGreenstein.x
                                    / pow(_HenyeyGreenstein.y - _HenyeyGreenstein.z*cosTheta, 1.5);

                // Sunlight scattering solution
                // See [2] page 63 equation (5.1)
                float3 T = exp(-(sr*_BetaRayleigh + sm*_BetaMie));
                float3 L0 = _DayColor;
                float3 L1 = _DayBrightness * (betaRayleighTheta + betaMieTheta) / (_BetaRayleigh + _BetaMie);

                // Resulting sky color at day and night
                float3 dayColor   = _SunColor.rgb * lerp(L1, L0, T);
                float3 nightColor = _NightColor + (1-fNight) * _NightHazeColor;

                // Write results
                o.position  = UnityObjectToClipPos(v.vertex);
                o.color.rgb = lerp(nightColor, dayColor, _SunColor.a);
                o.color.a   = (1 - (1-_SunColor.a) * fNight);

                // Adjust output color according to gamma value
                o.color = pow(o.color, _OneOverGamma);

                return o;
            }

            fixed4 frag(v2f i) : COLOR {
                return i.color;
            }

            ENDCG
        }
    }

    FallBack "None"
}
