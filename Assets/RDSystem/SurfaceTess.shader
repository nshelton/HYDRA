﻿ Shader "Tessellation Surface" {
        Properties {
            _Tess ("Tessellation", Range(1,38)) = 4
            _Displacement ("Displacement", Range(0, 10.0)) = 0.3
            _Twist ("Twist", Range(0, 10.0)) =1
        _MainTex("RD Texture", 2D) = "white" {}
        [Space]
        _Color0("Color 0", Color) = (1,1,1,1)
        _Color1("Color 1", Color) = (1,1,1,1)
        [Space]
        _Smoothness0("Smoothness 0", Range(0, 1)) = 0.5
        _Smoothness1("Smoothness 1", Range(0, 1)) = 0.5
        [Space]
        _Metallic0("Metallic 0", Range(0, 1)) = 0.0
        _Metallic1("Metallic 1", Range(0, 1)) = 0.0
        [Space]
        _Threshold("Threshold", float) = 0.1
        _Fading("Edge Smoothing", float) = 0.2
        _NormalStrength("Normal Strength", Range(0, 1)) = 0.9
        _Skybox ("Cubemap", Cube) = "" {}

        _ColorParam("colorParam", Vector) = (1,1,1,1)
        _Palette("_Palette", float) = 1
        _reflection("_reflection", float) = 1

        }
        SubShader {
            Tags { "RenderType"="Opaque" }
            Cull Off
            LOD 300
            
            CGPROGRAM

            #pragma surface surf Standard addshadow fullforwardshadows vertex:disp tessellate:tessDistance nolightmap
            #pragma target 4.6
            #include "Tessellation.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
                
            };

            float _Tess;

            float4 tessDistance (appdata v0, appdata v1, appdata v2) {
         
                return  _Tess;
            }

            float _Displacement;
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Twist;
            float _Palette;
            float4 _ColorParam;
            samplerCUBE _Skybox;

            float Twist(float2 uv) {
                float offs = sin(uv.x* 7 + sin(uv.y * 18) + _Time.x);
                return  _Twist * offs;
            }
            
float3 pal(in float t, in float3 a, in float3 b, in float3 c, in float3 d)
{
    return a + b * cos(6.28318 * (c * t + d));
}

float3 palette(float t)
{
    t = (t * _ColorParam.x) + _ColorParam.y;
    float3 color = pal(t, float3(0.5, 0.5, 0.5), float3(0.5, 0.5, 0.5), float3(1.0, 1.0, 1.0), float3(0.0, 0.33, 0.67));
    if (_Palette > (1.0)) color = pal(t, float3(0.5, 0.5, 0.5), float3(0.5, 0.5, 0.5), float3(1.0, 1.0, 1.0), float3(0.0, 0.10, 0.20));
    if (_Palette > (2.0)) color = pal(t, float3(0.5, 0.5, 0.5), float3(0.5, 0.5, 0.5), float3(1.0, 1.0, 1.0), float3(0.3, 0.20, 0.20));
    if (_Palette > (3.0)) color = pal(t, float3(0.5, 0.5, 0.5), float3(0.5, 0.5, 0.5), float3(1.0, 1.0, 0.5), float3(0.8, 0.90, 0.30));
    if (_Palette > (4.0)) color = pal(t, float3(0.5, 0.5, 0.5), float3(0.5, 0.5, 0.5), float3(1.0, 0.7, 0.4), float3(0.0, 0.15, 0.20));
    if (_Palette > (5.0)) color = pal(t, float3(0.5, 0.5, 0.5), float3(0.5, 0.5, 0.5), float3(2.0, 1.0, 0.0), float3(0.5, 0.20, 0.25));
    if (_Palette > (5.0)) color = pal(t, float3(0.5, 0.5, 0.5), float3(0.5, 0.5, 0.5), float3(2.0, 1.0, 0.0), float3(0.5, 0.20, 0.25));
    if (_Palette > (6.0))
        color = (float3) 1.0;

    return color;
}

            float FBM(float2 uv)
            {
                float lod = 1;
             float val = normalize(tex2Dlod(_MainTex, float4(uv, lod,0)).xyz).z * 1;
            //    val += normalize(tex2Dlod(_MainTex, float4(uv/4, lod, 0)).xyz).z * 4;
            //    val += normalize(tex2Dlod(_MainTex, float4(uv/2, lod, 0)).xyz).z * 2;
            //    val += normalize(tex2Dlod(_MainTex, float4(uv*2, lod, 0)).xyz).z * 0.5;
                return   val;

               /*

                float val = tex2Dlod(_MainTex, float4(uv, lod,0)).z * 1;
                val += tex2Dlod(_MainTex, float4(uv/4, lod, 0)).z * 4;
                val += tex2Dlod(_MainTex, float4(uv/2, lod, 0)).z * 2;
                val += tex2Dlod(_MainTex, float4(uv*2, lod, 0)).z * 0.5;
                return   val /7;

                float4 val = 0;
                val += tex2Dlod(_MainTex, float4(uv, 0, 0)) * 0.125; 
                val += tex2Dlod(_MainTex, float4(uv/2, 0, 0)) * 0.25;
                val += tex2Dlod(_MainTex, float4(uv/4, 0, 0)) * 0.5; 
                val += tex2Dlod(_MainTex, float4(uv/8, 0, 0)); 
                val += tex2Dlod(_MainTex, float4(uv/16, 0, 0)) * 2; 

                float3 norm = normalize(val.xyz);
                return 0.5 + norm.z;
              */

            }

            float3 getTwistNormal(float2 uv) {

                float2 duv = float2(0.01, 0);
                float twistx = Twist(uv.xy + duv.xy);
                float twistx2 = Twist(uv.xy - duv.xy);
                
                float twisty = Twist(uv.xy + duv.yx);
                float twisty2 = Twist(uv.xy - duv.yx);

                return normalize(float3(twistx - twistx2 , 1, twisty - twisty2));
            }

            void disp (inout appdata  v)
            {
                float twist = Twist(v.texcoord.xy);
                float d = FBM(v.texcoord.xy) * _Displacement;

               v.normal = getTwistNormal(v.texcoord.xy);

                v.vertex.xyz += float3(0, twist, 0)  + v.normal * d;
            }

            struct Input {
                float2 uv_MainTex;
                float3 viewDir;
                float3 worldNormal;
                float3 worldRefl;
                INTERNAL_DATA 
            };

            fixed4 _Color0, _Color1;
            half _Smoothness0, _Smoothness1;
            half _Metallic0, _Metallic1;
            half _Threshold, _Fading;
            half _NormalStrength;
            half _reflection;

            //void surf (Input IN, inout SurfaceOutput o) {
            void surf(Input IN, inout SurfaceOutputStandard o) {

                float3 duv = float3(_MainTex_TexelSize.xy, 0);
                float2 uv = IN.uv_MainTex;
                half v0 = FBM(uv);
                half v1 = FBM( uv - duv.xz);
                half v2 = FBM( uv + duv.xz);
                half v3 = FBM( uv - duv.zy);
                half v4 = FBM( uv + duv.zy);

                float p = smoothstep(_Threshold - _Fading, _Threshold + _Fading, v0 );
                o.Albedo =  palette(v0);
                o.Smoothness = lerp(_Smoothness0, _Smoothness1, p);
                o.Metallic = lerp(_Metallic0, _Metallic1, p);
                o.Normal = normalize(_Displacement * float3(v1 - v2,  v3 - v4, 1 - _NormalStrength));
                o.Emission = _reflection * texCUBE (_Skybox, reflect(IN.viewDir, o.Normal));

               // o.Normal +=     getTwistNormal(uv);
               // o.Normal = normalize(o.Normal);
                 
            }
            ENDCG
        }
        FallBack "Diffuse"
    }