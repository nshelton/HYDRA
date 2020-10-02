   Shader "Tessellation Kinect" {
        Properties {
            _MainTex ("Texture", 2D) = "white" {}

            _EdgeLength ("Edge length", Range(2,50)) = 15
            _Color ("Color", color) = (1,1,1,0)
            _Specular ("_Specular", Range(0,1)) = 0.5
            _Gloss ("_Gloss", Range(0,1)) = 0.5

            _NormalDist ("_NormalDist", Range(1,10)) = 1
            _CullDist ("_CullDist", Range(1,10)) = 1
            _CullThresh ("_CullThresh", Range(0,1)) = 1

            _noiseAmp("noise amp ", Range(0,1)) = 1
            _noiseFreq("noise freq ", Range(0,40)) = 1
            _NoiseDir("noise dir ", Vector) = (0,1,0)

            _EmissionColor ("EmissionColor", color) = (1,1,1,0)
            _EmissionAmount("EmissionAmount", Range(0,10)) = 1
            _EmissionPct("_EmissionPct", Range(0,1)) = 1
        }
        SubShader {
            Tags { "RenderType"="Transparent" }
            LOD 300

            CGPROGRAM
            #pragma surface surf BlinnPhong addshadow fullforwardshadows vertex:vert tessellate:tessEdge nolightmap 
            #pragma target 4.6

            #include "Tessellation.cginc"
            #include "resources/noise.cginc"

            // pseudo random number generator
            float nrand(float2 uv, float salt)
            {
                uv += float2(salt, 0);
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }
        
            struct appdata {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            struct Input {
                float2 uv_MainTex;
                float3 normal : NORMAL;
            };
            
            sampler2D _MainTex;
            sampler2D _ColorTex;
            sampler2D _PositionTex;
            float _EdgeLength;
            fixed4 _Color;
            fixed4 _EmissionColor;
            fixed _EmissionAmount;
            fixed _EmissionPct;
            
            fixed _Specular;
            fixed _Gloss;

            float _NormalDist;
            float _CullDist;
            float _CullThresh;
            float3 _NoiseDir;

            #define WIDTH  640.0
            #define HEIGHT 576.0

            float4 tessEdge (appdata v0, appdata v1, appdata v2)
            {
                return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
            }

            float _noiseAmp;
            float _noiseFreq;

            void vert (inout appdata v) 
            {

                float2 uv = v.texcoord.xy;
                //uv.y = 1.0 - uv.y;
                float3 pos = tex2Dlod(_PositionTex, float4(uv,0,0));
                 float3 _scroll= float3(0, 0, _Time.x);
                float clampnoise = snoise(pos * _noiseFreq + _scroll);
                clampnoise = smoothstep(0.5, 1.0, clampnoise)  * _noiseAmp;
                pos += _NoiseDir * clampnoise;
                v.vertex.xyz = pos;
            }

            float3 CalcNormal(float2 uv)
            {
                float2 dx = float2(_NormalDist/WIDTH, 0);
                float2 dy = float2(0, _NormalDist/HEIGHT);

                float3 pos = tex2Dlod(_PositionTex, float4(uv,0,0));
                float3 p0 = tex2Dlod(_PositionTex, float4(uv + dx,0,0));
                float3 p1 = tex2Dlod(_PositionTex, float4(uv + dy,0,0));

                p0 -= pos;
                p1 -= pos;

                return cross(normalize(p0), normalize(p1));

            }

            float GetCull(float2 uv) 
            {
                float2 dx = float2(_CullDist/WIDTH, 0);
                float2 dy = float2(0, _CullDist/HEIGHT);
                float thresh = _CullThresh;

                
                float4 pos = tex2D(_PositionTex, uv);
                if ( pos.w ==0)
                    return -1;

                if(length(tex2D (_PositionTex, uv + dx).xyz - pos.xyz) > thresh ||
                   length(tex2D (_PositionTex, uv-dx).xyz - pos.xyz) > thresh ||
                   length(tex2D (_PositionTex, uv+dy).xyz - pos.xyz) > thresh ||
                   length(tex2D (_PositionTex, uv-dy).xyz - pos.xyz) > thresh )

                    return -1;
                 else
                    return 1;

            }

    
            float4 GetEmission(float2 uv)
            {
                float numlines = 150;
                float l = floor((uv.y) * numlines);
                float linewidth = frac((uv.y) * numlines);

                float n = nrand(l, 1);
              //  n = step(n, _EmissionPct );

                //float offs = nrand(l, 13);
                float speed = nrand(l, 3) * 0.5 + 0.5;
                float offs = frac(uv.x + n - _Time.y * speed);

                float alpha = smoothstep(0.1, 0.3, linewidth) * smoothstep(0.9, 0.7, linewidth) ;
                float3 col =  pow( offs, 5) * _EmissionColor.rgb *  _EmissionAmount;
                clip(alpha- 0.5);

                return float4(col, alpha);
            }


            void surf (Input IN, inout SurfaceOutput o) 
            {

                half4 c = tex2D (_ColorTex, IN.uv_MainTex) ;
                

                clip(GetCull(IN.uv_MainTex));

                o.Albedo = c.rgb;
                o.Specular = _Specular;
                
                if ( _EmissionAmount > 0){
                    float4 slices = GetEmission(IN.uv_MainTex);
                    o.Emission = slices.rgb;
                }

                o.Gloss = _Gloss;
                o.Normal = CalcNormal(IN.uv_MainTex);

            }
            ENDCG
        }
        FallBack "Diffuse"
    }