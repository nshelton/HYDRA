Shader "Unlit/clouds2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        Density("density", float ) = 1
        ShadowDensity("ShadowDensity", float ) = 1
        Scroll("Scroll", float ) = 1
        [HDR]_Color("_Color", Color ) = (1,1,1,1)
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        //ZWrite Off 
        //ZTest Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "noise.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(v.vertex, unity_ObjectToWorld);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            
            float Density;
            float ShadowDensity;
            float Scroll;
            fixed4 _Color;

            float sdBox( float3 p, float3 b )
            {
              float3 q = abs(p) - b;
              return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
            }

            float opDisplace( in float3 p )
            {
                float d1 = sdBox(p, float3(10, 0.1, 10 ));
               // float d2 = sin(2*p.x)*sin(2*p.y)*sin(2*p.z);
                float d2 = snoise(p) * 0.3;
                d2 += snoise(p * 2) * 0.15;
                return d1+d2;
            }

            float DE (float3 p)
            {
                return opDisplace(p);
            }

            float density(float3 p) {

                float3 scr = float3(0, 0, _Time.x * Scroll);

                float v = (snoise(p + scr) * 0.5 + 0.5 ) *  smoothstep(0, -4, p.y);
                v +=      (snoise(p*2 + scr) * 0.5 + 0.5 ) * 0.5 *  smoothstep(0, -4, p.y);
                v +=      (snoise(p*4 + scr) * 0.5 + 0.5 ) * 0.25 *  smoothstep(0, -4, p.y);
                v +=      (snoise(p*8 + scr) * 0.5 + 0.5 ) * 0.125 *  smoothstep(0, -4, p.y);
                return v;
            }

            float4 raymarch(float3 ro, float3 rd)
            {
                float lightDir  = float3(0,1,0);

                float StepSize = 0.05;
                float pxNoise = snoise(rd.xyz * 1000 + _Time.a)*StepSize * 0.1;
                StepSize += pxNoise;

                float3 pos = ro + rd  + pxNoise;

                float curdensity = 0;
                float transmittance = 1;
                
                float3 lightenergy = 0;

                for (int i = 0; i < 64; i++)
                {
                    float d = density(pos);

                    if ( d > 0.01){
                        float3 lpos = pos;
                        float shadowdist = 0;

                        for (int j = 0; j < 16; j++) {
                            lpos += lightDir * StepSize;
                            shadowdist += density(lpos);
                        }

                        curdensity = saturate(d * Density * StepSize);
                        float shadowterm = exp(-shadowdist * ShadowDensity * StepSize);
                        float3 absorbedlight = shadowterm * curdensity;
                        lightenergy += absorbedlight * transmittance * _Color;
                        transmittance *= 1-curdensity;

                    }

                    pos += StepSize * rd;
                }

                return float4( lightenergy , transmittance);

            }

            float TWO_PI = 3.141592  ;

            float4 frag (v2f i) : SV_Target
            {
                
                float3 worldPosition = i.worldPos;
                float3 viewDirection = normalize(i.worldPos - _WorldSpaceCameraPos);
                  return raymarch (_WorldSpaceCameraPos, viewDirection);

              
                //col *= uv.y/5;

                // apply fog
           //      col.rg = abs(i.uv );
                //

            }
            ENDCG
        }
    }
}
