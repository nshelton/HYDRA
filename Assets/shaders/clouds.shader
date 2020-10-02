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

           
            // iq code

            
            float noise( in float3 x )
            {
                float3 p = floor(x);
                float3 f = frac(x);
	            f = f*f*(3.0-2.0*f);
    
	            float2 uv = (p.xy+float2(37.0,239.0)*p.z) + f.xy;
                float2 rg = tex2D(_MainTex,uv+0.5).yx;
           
	            return -1.0 + 2.0 * lerp( rg.x, rg.y, f.z );
            }

            float map5( in float3 p )
            {
	            float3 q = p - float3(0.0,0.1,1.0)*_Time.x;
	            float f;
                f  = 0.50000*noise( q ); q = q*2.02;
                f += 0.25000*noise( q ); q = q*2.03;
                f += 0.12500*noise( q ); q = q*2.01;
                f += 0.06250*noise( q ); q = q*2.02;
                f += 0.03125*noise( q );
	            return clamp( 1.5 - p.y - 2.0 + 1.75*f, 0.0, 1.0 );
            }
            float map4( in float3 p )
            {
	            float3 q = p - float3(0.0,0.1,1.0)*_Time.x;
	            float f;
                f  = 0.50000*noise( q ); q = q*2.02;
                f += 0.25000*noise( q ); q = q*2.03;
                f += 0.12500*noise( q ); q = q*2.01;
                f += 0.06250*noise( q );
	            return clamp( 1.5 - p.y - 2.0 + 1.75*f, 0.0, 1.0 );
            }
            float map3( in float3 p )
            {
	            float3 q = p - float3(0.0,0.1,1.0)*_Time.x;
	            float f;
                f  = 0.50000*noise( q ); q = q*2.02;
                f += 0.25000*noise( q ); q = q*2.03;
                f += 0.12500*noise( q );
	            return clamp( 1.5 - p.y - 2.0 + 1.75*f, 0.0, 1.0 );
            }
            float map2( in float3 p )
            {
	            float3 q = p - float3(0.0,0.1,1.0)*_Time.x;
	            float f;
                f  = 0.50000*noise( q ); q = q*2.02;
                f += 0.25000*noise( q );;
	            return clamp( 1.5 - p.y - 2.0 + 1.75*f, 0.0, 1.0 );
            }

            float3 sundir = normalize( float3(-1.0,0.0,-1.0) );
            
    #define MARCH(STEPS,MAPLOD)\
            for(int i=0; i<STEPS; i++)\
            {\
               float3 pos = ro + t*rd;\
               float den = MAPLOD( pos );\
               if( den>0.01 )\
               {\
                 float dif = clamp((den - MAPLOD(pos+0.3*sundir))/0.6, 0.0, 1.0 );\
                 float3  lin = float3(0.65,0.7,0.75)*1.4 + float3(1.0,0.6,0.3)*dif;\
                 float4  col = float4( lerp( float3(1.0,0.95,0.8), float3(0.25,0.3,0.35), den ), den );\
                 col.xyz *= lin;\
                 col.xyz = lerp( col.xyz, bgcol, 1.0-exp(-0.003*t*t) );\
                 col.w *= 0.4;\
                 \
                 col.rgb *= col.a;\
                 sum += col*(1.0-sum.a);\
               }\
               t += max(0.05,0.02*t);\
            }
            
            float4 raymarch( float3 ro, float3 rd, float3 bgcol)
            {
	            float4 sum = (float4)(0.0);

	            float t = 0.0;//0.05*tex2D( _MainTex, px&255, 0 ).x;

                MARCH(40,map5);
                MARCH(40,map4);
                MARCH(30,map3);
                MARCH(30,map2);

               return clamp( sum, 0.0, 1.0 );
            }


            float4 render( float3 ro, float3 rd)
            {
                // background sky     
	            float sun = clamp( dot(sundir,rd), 0.0, 1.0 );
	            float3 col = float3(0.6,0.71,0.75) - rd.y*0.2*float3(1.0,0.5,1.0) + 0.15*0.5;
	            col += 0.2*float3(1.0,.6,0.1)*pow( sun, 8.0 );

                // clouds    
                float4 res = raymarch( ro, rd, col );
                //col = res.xyz;
                col = col*(1.0-res.w) + res.xyz;
    
                // sun glare    
	             col += 0.2*float3(1.0,0.4,0.2)*pow( sun, 3.0 );

                return float4( col, 1.0 ) ;
            }


            float4 frag (v2f i) : SV_Target
            {
                float3 viewDirection = normalize(i.worldPos - _WorldSpaceCameraPos);
                float4 color = render( _WorldSpaceCameraPos, viewDirection );
                return color;

            }

            ENDCG
        }
    }
}
