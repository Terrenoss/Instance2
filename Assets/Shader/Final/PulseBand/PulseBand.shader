Shader "Custom/PulseBand"
{
    Properties
    {
        _ColorEdge   ("Couleur des extrémités", Color)           = (0.6, 0.0, 0.0, 1)
        _ColorCenter ("Couleur du centre",      Color)           = (0.0, 0.8, 0.0, 1)
        _BandWidth   ("Largeur de la bande",    Range(0.0, 100.0)) = 0.4
        _Softness    ("Douceur de transition",  Range(0.0, 0.5)) = 0.1
        _BandCenter  ("Décalage du centre",     Range(-0.5, 0.5)) = 0.0
        _Axis        ("Axe (0=X 1=Y 2=Z)",      Range(0, 2))     = 2
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
                float4 vertex    : SV_POSITION;
                float3 objectPos : TEXCOORD0;  
                float3 objNormal : TEXCOORD1;  
            };

            float4 _ColorEdge;
            float4 _ColorCenter;
            float  _BandWidth;
            float  _Softness;
            float  _BandCenter;
            float  _Axis;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex    = UnityObjectToClipPos(v.vertex);
                o.objectPos = v.vertex.xyz; 
                o.objNormal = v.normal;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float coord;
                float normalOnAxis;

                if (_Axis < 0.5)       
                {
                    coord        = i.objectPos.x;
                    normalOnAxis = abs(i.objNormal.x);
                }
                else if (_Axis < 1.5)  
                {
                    coord        = i.objectPos.y;
                    normalOnAxis = abs(i.objNormal.y);
                }
                else                  
                {
                    coord        = i.objectPos.z;
                    normalOnAxis = abs(i.objNormal.z);
                }

                float t = coord + 0.5 + _BandCenter; 

                float dist     = abs(t - 0.5);
                float halfBand = _BandWidth * 0.5;

                float band = smoothstep(halfBand, halfBand - _Softness, dist);

                float faceMask = 1.0 - smoothstep(0.6, 0.9, normalOnAxis);
                band *= faceMask;

                float3 col = lerp(_ColorEdge.rgb, _ColorCenter.rgb, band);
                return fixed4(col, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}