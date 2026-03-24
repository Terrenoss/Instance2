Shader "Custom/ComputerVirus"
{
    Properties
    {
        _InfectionTex ("Infection Map (R)", 2D)               = "black" {}
        _VirusTint    ("Virus Tint",        Color)            = (0, 1, 0.2, 1)
        _GlitchColor  ("Glitch Color",      Color)            = (1, 0.1, 0.3, 1)
        _Speed        ("Scroll Speed",      Float)            = 1.5
        _Columns      ("Grid Columns",      Float)            = 8.0
        _Rows         ("Grid Rows",         Float)            = 5.0
        _GlitchAmount ("Glitch Amount",     Range(0,1))       = 0.3
        _CellGap      ("Cell Gap",          Range(0,0.05))    = 0.01
        _CharThick    ("Char Thickness",    Range(0.02,0.15)) = 0.07
        _ScrollX      ("Vitesse scroll X",  Float)            = 1.0
        _ScrollY      ("Vitesse scroll Y",  Float)            = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f     { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            sampler2D _InfectionTex;
            float4 _VirusTint, _GlitchColor;
            float  _Speed, _Columns, _Rows, _GlitchAmount, _CellGap, _CharThick;
            float  _ScrollX, _ScrollY;

            float hash21(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }
            float hash11(float x)
            {
                return frac(sin(x * 4375.34) * 23451.1);
            }

            float getInfection(float col, float row)
            {
                float2 uv = float2((col + 0.5) / _Columns, (row + 0.5) / _Rows);
                return step(0.5, tex2D(_InfectionTex, uv).r);
            }

            float seg(float2 uv, float2 a, float2 b, float thick)
            {
                float2 ab = b - a;
                float2 ap = uv - a;
                float t = saturate(dot(ap, ab) / dot(ab, ab));
                return 1.0 - smoothstep(0.0, thick, length(ap - t * ab));
            }

            float drawZero(float2 uv, float thick)
            {
                float L = 0.2, R = 0.8, Top = 0.88, Bot = 0.12;
                return max(
                    max(seg(uv, float2(L,Top), float2(R,Top), thick),
                        seg(uv, float2(L,Bot), float2(R,Bot), thick)),
                    max(seg(uv, float2(L,Top), float2(L,Bot), thick),
                        seg(uv, float2(R,Top), float2(R,Bot), thick))
                );
            }

            float drawOne(float2 uv, float thick)
            {
                float MX = 0.5, Top = 0.88, Bot = 0.12;
                return max(
                    seg(uv, float2(MX,Top), float2(MX,Bot), thick),
                    max(seg(uv, float2(0.3,Top), float2(MX,Top), thick),
                        seg(uv, float2(0.25,Bot), float2(0.75,Bot), thick))
                );
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float glitchRow  = floor(uv.y * _Rows);
                float glitchGate = step(1.0 - _GlitchAmount * 0.25,
                                        hash21(float2(glitchRow, floor(_Time.y * 7.0))));
                float glitchShift = glitchGate
                                  * (hash11(glitchRow + floor(_Time.y * 11.0)) - 0.5)
                                  * 0.035;
                float2 guv = uv + float2(glitchShift, 0.0);

                float2 scrollOffset = float2(
                    _Time.y * _ScrollX / _Columns,
                    _Time.y * _ScrollY / _Rows
                );
                float2 scrolledUV = guv + scrollOffset;

                float col = floor(scrolledUV.x * _Columns);
                float row = floor(scrolledUV.y * _Rows);
                float2 cellUV = frac(scrolledUV * float2(_Columns, _Rows));

                float inCell = step(_CellGap, cellUV.x)
                             * step(_CellGap, cellUV.y)
                             * step(cellUV.x, 1.0 - _CellGap)
                             * step(cellUV.y, 1.0 - _CellGap);

                float2 innerUV = saturate((cellUV - _CellGap) / (1.0 - 2.0 * _CellGap));

                float colFixed = floor(guv.x * _Columns);
                float rowFixed = floor(guv.y * _Rows);
                float isInfected = getInfection(colFixed, rowFixed);

                float timeStep = floor(_Time.y * _Speed);
                float finalBit = step(0.5, hash21(float2(col + timeStep * 0.317,
                                                          row + timeStep * 0.731)));

                float glitchFrame = floor(_Time.y * _Speed * 6.0
                                        + colFixed * 1.3 + rowFixed * 2.1);
                float glitchBit   = step(0.5, hash21(float2(colFixed,
                                                            rowFixed + glitchFrame)));
                finalBit = lerp(finalBit, glitchBit, isInfected * 0.8);

                float alpha = (finalBit < 0.5)
                            ? drawZero(innerUV, _CharThick)
                            : drawOne (innerUV, _CharThick);

                float3 deadColor = _VirusTint.rgb * 0.12;
                float3 liveColor = _VirusTint.rgb;

                float glitchFlash = step(0.96,
                    hash21(float2(colFixed,
                                  rowFixed + floor(_Time.y * 18.0)))) * _GlitchAmount;
                liveColor = lerp(liveColor, _GlitchColor.rgb, glitchFlash);

                float3 finalColor = lerp(deadColor, liveColor, isInfected) * inCell;

                return fixed4(finalColor, alpha * inCell);
            }
            ENDCG
        }
    }
}