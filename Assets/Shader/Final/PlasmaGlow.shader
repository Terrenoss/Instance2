Shader "Custom/PlasmaGlow"
{
    Properties
    {
        _CoreColor   ("Core Color",    Color)       = (0.0, 0.9, 1.0, 1)
        _GlowColor   ("Glow Color",    Color)       = (0.0, 0.7, 1.0, 1)
        _CoreSize    ("Core Size",     Range(0,1))  = 0.35
        _GlowSize    ("Glow Size",     Range(0,2))  = 1.2
        _GlowPower   ("Glow Power",    Float)       = 2.5
        _Intensity   ("Intensity",     Float)       = 2.0
        _PulseSpeed  ("Pulse Speed",   Float)       = 1.5
        _PulseAmt    ("Pulse Amount",  Range(0,1))  = 0.15
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        Blend One One          
        ZWrite Off
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct v2f
            {
                float4 clip     : SV_POSITION;
                float3 localPos : TEXCOORD0;
                float3 norm     : TEXCOORD1;
            };

            float4 _CoreColor, _GlowColor;
            float  _CoreSize, _GlowSize, _GlowPower, _Intensity;
            float  _PulseSpeed, _PulseAmt;

            v2f vert(appdata v)
            {
                v2f o;
                o.clip     = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz;
                o.norm     = v.normal;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 n       = normalize(i.norm);
                float3 viewDir = normalize(ObjSpaceViewDir(float4(i.localPos, 1)));
                float  NdotV   = saturate(dot(n, viewDir));

                float fresnel = pow(1.0 - NdotV, _GlowPower);

                float inner = pow(NdotV, _GlowSize) * (1.0 - pow(NdotV, _CoreSize * 10.0 + 0.01));

                float core = pow(NdotV, 1.0 / (_CoreSize + 0.01));

                float pulse = 1.0 + _PulseAmt * sin(_Time.y * _PulseSpeed * 6.2832);

                float3 col = _GlowColor.rgb  * fresnel * _Intensity
                           + _GlowColor.rgb  * inner   * _Intensity * 0.5
                           + _CoreColor.rgb  * core    * _Intensity * 0.3;

                col *= pulse;

                return fixed4(col, 1.0);
            }
            ENDCG
        }
    }
}