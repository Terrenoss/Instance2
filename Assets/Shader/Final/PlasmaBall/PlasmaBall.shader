Shader "Custom/PlasmaBall"
{
    Properties
    {
        _CoreColor  ("Core Color",  Color)          = (0.4, 0.1, 1.0, 1)
        _ArcColor   ("Arc Color",   Color)          = (0.8, 0.3, 1.0, 1)
        _BgColor    ("Background",  Color)          = (0.02, 0.0, 0.08, 1)
        _Speed      ("Arc Speed",   Float)          = 1.2
        _ArcCount   ("Arc Count",   Float)          = 6.0
        _Turbulence ("Turbulence",  Float)          = 3.5
        _GlowRadius ("Core Glow",   Range(0,1))     = 0.22
        _ArcWidth   ("Arc Width",   Range(0.001,0.04)) = 0.015
        _Intensity  ("Intensity",   Float)          = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct v2f
            {
                float4 clip      : SV_POSITION;
                float3 localPos  : TEXCOORD0;  
                float3 localNorm : TEXCOORD1;   
            };

            float4 _CoreColor, _ArcColor, _BgColor;
            float  _Speed, _ArcCount, _Turbulence, _GlowRadius, _ArcWidth, _Intensity;

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }
            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(hash(i),          hash(i+float2(1,0)), u.x),
                            lerp(hash(i+float2(0,1)), hash(i+float2(1,1)), u.x), u.y);
            }
            float fbm(float2 p)
            {
                float v=0., a=0.5;
                for(int k=0;k<5;k++){ v+=a*noise(p); p=p*2.1+float2(1.7,9.2); a*=0.5; }
                return v;
            }

            float3 rotY(float3 p, float a)
            {
                float c=cos(a), s=sin(a);
                return float3(c*p.x+s*p.z, p.y, -s*p.x+c*p.z);
            }
            float3 rotZ(float3 p, float a)
            {
                float c=cos(a), s=sin(a);
                return float3(c*p.x-s*p.y, s*p.x+c*p.y, p.z);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.clip      = UnityObjectToClipPos(v.vertex);
                o.localPos  = v.vertex.xyz;       
                o.localNorm = v.normal;
                return o;
            }

            float distToSeg3D(float3 p, float3 a, float3 b)
            {
                float3 ab = b - a, ap = p - a;
                float  t  = saturate(dot(ap, ab) / dot(ab, ab));
                return length(ap - t * ab);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 pos = i.localPos;          
                float  t   = _Time.y;

                float distC = length(pos);
                float core  = exp(-distC * distC / (_GlowRadius * _GlowRadius));

                float arcs = 0.0;

                for(int k = 0; k < 12; k++)
                {
                    if((float)k >= _ArcCount) break;

                    float fk = (float)k;

                    float phi   = fk / _ArcCount * UNITY_PI * 2.0
                                + t * _Speed * (0.15 + hash(float2(fk, 0.0)) * 0.2);
                    float theta = hash(float2(fk, 1.0)) * UNITY_PI;

                    float3 dir = normalize(float3(
                        sin(theta) * cos(phi),
                        cos(theta),
                        sin(theta) * sin(phi)
                    ));
                    float3 endPt = dir * 0.5;

                    float3 prev = float3(0, 0, 0);
                    float  arcGlow = 0.0;
                    int    steps = 6;

                    for(int s = 1; s <= 6; s++)
                    {
                        float  frac01 = (float)s / 6.0;
                        float3 straight = endPt * frac01;

                        float2 noiseIn  = float2(frac01 * 3.0 + fk * 5.7,
                                                  t * _Speed * 0.8 + fk * 2.3);
                        float  nx = (fbm(noiseIn)           - 0.5) * _Turbulence * 0.12;
                        float  ny = (fbm(noiseIn + 17.3)    - 0.5) * _Turbulence * 0.12;
                        float  nz = (fbm(noiseIn + 31.9)    - 0.5) * _Turbulence * 0.12;

                        float env = sin(frac01 * UNITY_PI);
                        float3 curPt = straight + float3(nx, ny, nz) * env;

                        float d = distToSeg3D(pos, prev, curPt);
                        arcGlow = max(arcGlow, 1.0 - smoothstep(0.0, _ArcWidth, d));

                        prev = curPt;
                    }

                    float depthFade = 0.5 + 0.5 * dot(normalize(endPt), float3(0,0,-1));
                    arcs = max(arcs, arcGlow * (0.6 + 0.4 * depthFade));
                }

                float3 n       = normalize(i.localNorm);
                float3 viewDir = normalize(ObjSpaceViewDir(float4(pos, 1)));
                float  fresnel = pow(1.0 - saturate(dot(n, viewDir)), 3.0) * 0.5;

                float2 nebUV = float2(atan2(pos.z, pos.x) / (2.0*UNITY_PI),
                                      acos(clamp(pos.y/0.5,-1,1)) / UNITY_PI);
                float  nebula = fbm(nebUV * 2.5 + float2(t*0.03, 0.0)) * 0.2;

                float3 col = _BgColor.rgb;
                col += _CoreColor.rgb * core   * _Intensity;
                col += _ArcColor.rgb  * arcs   * _Intensity;
                col += _CoreColor.rgb * fresnel * _Intensity;
                col += _ArcColor.rgb  * nebula;

                col  = col / (col + 0.5);
                col  = pow(max(col, 0.0), 0.4545);

                return fixed4(col, 1.0);
            }
            ENDCG
        }
    }
}