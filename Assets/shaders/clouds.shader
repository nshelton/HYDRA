Shader "Unlit/clouds"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        Density("density", float ) = 1
        ShadowDensity("ShadowDensity", float ) = 1
        Scroll("Scroll", float ) = 1
        [HDR] _Color("_Color", Color ) = (1,1,1,1)
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        //    Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        //ZWrite Off 
        //ZTest Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
                int2 screenPos : TEXCOORD2;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(v.vertex, unity_ObjectToWorld);
                o.screenPos = ComputeScreenPos(o.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 _Color;


            float2x2 rot(in float a){float c = cos(a), s = sin(a);return float2x2(c,s,-s,c);}
            const float3x3 m3 = float3x3(0.33338, 0.56034, -0.71817, -0.87887, 0.32651, -0.15323, 0.15162, 0.69596, 0.61339)*1.93;
            float mag2(float2 p){return dot(p,p);}
            float linstep(in float mn, in float mx, in float x){ return clamp((x - mn)/(mx - mn), 0., 1.); }
            float prm1 = 0.;
            float2 bsMo = float2(0,0);

            float2 disp(float t){ return float2(sin(t*0.22)*1., cos(t*0.175)*1.)*2.; }

            float2 map(float3 p)
            {
                float3 p2 = p;
                p2.xy -= disp(p.z).xy;
                p.xy = mul(p.xy, rot(sin(p.z+_Time.x)*(0.1 + prm1*0.05) + _Time.x*0.09));
                float cl = mag2(p2.xy);
                float d = 0.;
                p *= .61;
                float z = 1.;
                float trk = 1.;
                float dspAmp = 0.1 + prm1*0.2;
                for(int i = 0; i < 5; i++)
                {
                    p += sin(p.zxy*0.75*trk + _Time.x*trk*.8)*dspAmp;
                    d -= abs(dot(cos(p), sin(p.yzx))*z);
                    z *= 0.57;
                    trk *= 1.4;
                    p = mul(p,m3);
                }
                d = abs(d + prm1*3.)+ prm1*.3 - 2.5 + bsMo.y;
                return float2(d + cl*.2 + 0.25, cl);
            }

            float4 render( in float3 ro, in float3 rd, float time )
            {
                float4 rez = (float4)0;
                const float ldst = 8.;
                float3 lpos = float3(disp(time + ldst)*0.5, time + ldst);
                float t = 1.5;
                float fogT = 0.;
                for(int i=0; i<10; i++)
                {
                    if(rez.a > 0.99)break;

                    float3 pos = ro + t*rd;
                    float2 mpv = map(pos);
                    float den = clamp(mpv.x-0.3,0.,1.)*1.12;
                    float dn = clamp((mpv.x + 2.),0.,3.);
                    
                    float4 col = (float4)0;
                    if (mpv.x > 0.6)
                    {
                        
                        col = float4(sin(float3(5.,0.4,0.2) + mpv.y*0.1 +sin(pos.z*0.4)*0.5 + 1.8)*0.5 + 0.5,0.08);
                        col *= den*den*den;
                        col.rgb *= linstep(4.,-2.5, mpv.x)*2.3;
                        float dif =  clamp((den - map(pos+.8).x)/9., 0.001, 1. );
                        dif += clamp((den - map(pos+.35).x)/2.5, 0.001, 1. );
                        col.xyz *= den*(float3(0.005,.045,.075) + 1.5*float3(0.033,0.07,0.03)*dif);
                    }
                    
                    float fogC = exp(t*0.2 - 2.2);
                    col.rgba += float4(0.06,0.11,0.11, 0.1)*clamp(fogC-fogT, 0., 1.);
                    fogT = fogC;
                    rez = rez + col*(1. - rez.a);
                    t += clamp(0.5 - dn*dn*.05, 0.09, 0.3) * 10;
                }
                return clamp(rez, 0.0, 1.0);
            }

            float getsat(float3 c)
            {
                float mi = min(min(c.x, c.y), c.z);
                float ma = max(max(c.x, c.y), c.z);
                return (ma - mi)/(ma+ 1e-7);
            }

            //from my "Will it blend" shader (https://www.shadertoy.com/view/lsdGzN)
            float3 iLerp(in float3 a, in float3 b, in float x)
            {
                float3 ic = lerp(a, b, x) + float3(1e-6,0.,0.);
                float sd = abs(getsat(ic) - lerp(getsat(a), getsat(b), x));
                float3 dir = normalize(float3(2.*ic.x - ic.y - ic.z, 2.*ic.y - ic.x - ic.z, 2.*ic.z - ic.y - ic.x));
                float lgt = dot(float3(1,1,1), ic);
                float ff = dot(dir, normalize(ic));
                ic += 1.5*dir*sd*ff*lgt;
                return clamp(ic,0.,1.);
            }


            float4 frag (v2f i) : SV_Target
            {
                float3 ro = _WorldSpaceCameraPos;
                
                ro += float3(sin(_Time.x)*0.5,sin(_Time.x*1.)*0.,0);
                
                float dspAmp = .85;
                ro.xy += disp(ro.z)*dspAmp;
                float tgtDst = 3.5;
                
                float3 rd = normalize(i.worldPos - _WorldSpaceCameraPos);

                prm1 = smoothstep(-0.4, 0.4,sin(_Time.x*0.3));
                float4 scn = render(ro, rd, _Time.x);
                
                float3 col = scn.rgb;
                col = iLerp(col.bgr, col.rgb, clamp(1.-prm1,0.05,1.));
                
                col = pow(col, float3(.55,0.65,0.6))*float3(1.,.97,.9);

                return float4( col, 1.0 );

            }

            ENDCG
        }
    }
}
