Shader "Custom/URP/ComputerCircuit"
{
    Properties
    {
        _TraceColor ("Trace Color", Color) = (0.1, 0.8, 0.3, 1)
        _NeonColor ("Neon Color", Color) = (0.5, 1.0, 0.2, 1)
        _PulseSpeed ("Pulse Speed", Float) = 2.0
        _GlowWidth ("Glow Width", Range(0.01,0.5)) = 0.08
        _EmissionStrength ("Emission", Range(0,20)) = 8.0
        _TraceOpacity ("Opacity", Range(0,1)) = 1.0
        _Distance ("Distance", Float) = 0
        _NumTraces ("Num Traces", Range(1,8)) = 3
        _TraceThickness ("Trace Thickness", Range(0.01,0.4)) = 0.06
        _TileScale ("Tile Scale", Float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _TraceColor;
                float4 _NeonColor;
                float _PulseSpeed;
                float _GlowWidth;
                float _EmissionStrength;
                float _TraceOpacity;
                float _Distance;
                float _NumTraces;
                float _TraceThickness;
                float _TileScale;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv  = IN.uv;
                OUT.uv2 = IN.uv2;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                float worldX = _Distance * _TileScale;
                float scroll = worldX - _Time.y * _PulseSpeed * 0.5;

                int numTraces = (int)_NumTraces;
                float traceMask = 0.0;
                float glowMask  = 0.0;

                [unroll(8)]
                for (int i = 0; i < numTraces; i++)
                {
                    float t = (float)i / (float)(numTraces - 1 + 0.0001);
                    float traceY = lerp(0.1, 0.9, t);

                    float distY = abs(uv.y - traceY);

                    float traceLine = smoothstep(_TraceThickness, _TraceThickness * 0.3, distY);
                    float glow      = smoothstep(_TraceThickness * 3.0, 0.0, distY) * 0.4;

                    float phaseOffset = (float)i * 0.33;

                    float tileX   = frac((uv.x * worldX * 0.1) + phaseOffset - _Time.y * _PulseSpeed * 0.05);
                    float pulseGlow = smoothstep(_GlowWidth, 0.0, abs(tileX - 0.5));

                    float segmentFreq = 5.0 * _TileScale;
                    float segment = step(0.35, frac(uv.x * segmentFreq + (float)i * 0.5));

                    traceMask += traceLine * segment;
                    glowMask  += (traceLine * pulseGlow + glow * pulseGlow);
                }

                traceMask = saturate(traceMask);
                glowMask  = saturate(glowMask);

                float edgeFadeX = smoothstep(0.0, 0.03, uv.x) * smoothstep(1.0, 0.97, uv.x);

                float3 baseCol  = _TraceColor.rgb * traceMask * 0.5;
                float3 emission = _NeonColor.rgb * glowMask * _EmissionStrength * edgeFadeX;

                float alpha = saturate(traceMask + glowMask * 0.6) * _TraceOpacity * edgeFadeX;

                return half4(baseCol + emission, alpha);
            }
            ENDHLSL
        }
    }
}