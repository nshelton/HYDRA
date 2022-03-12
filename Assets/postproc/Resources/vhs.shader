Shader "Hidden/Custom/vhs"
{
    HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float _Scanline;
    float _RGBFuzz;
    float _RGBScroll;
    float _Height;
    float _Width;

    float2 _Drift;
    float2 _Jitter;
    float2 _Jump;
    float _Shake;
    
        // pseudo random number generator
    float Hash(float2 uv, float salt)
    {
        uv += float2(salt, 0);
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }
        

    float4 Frag(VaryingsDefault i) : SV_Target
    {

        float2 vhs = float2(1.0 - i.texcoord.x, i.texcoord.y);

        float2 uv = i.texcoord;
       
        int py = ( _Height ) * i.texcoord.y;
        int px = ( _Width ) * i.texcoord.x;
 
        uv.y = lerp(uv.y, frac(uv.y + _Jump.x), _Jump.y);

          // Jitter
          float t = floor(_Time.z *10);
          float blockSize = pow(Hash(t,t), 4)* 50;
          float yy = floor(py / blockSize );
        float jitter = Hash(float2(yy, yy), 0) * 2 - 1;
        uv.x += jitter * (_Jitter.x < abs(jitter)) * _Jitter.y;

        float4 a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

        float powScroll = pow(_RGBScroll, 2);
        float2 gOffset = float2( _RGBFuzz * 0.01 * sin((float)py/60.0 + 100 * _Time.a * powScroll ) , 0);
        float2 bOffset = float2( _RGBFuzz * 0.02 * sin((float)py/92.0 + 100 * _Time.a * powScroll) , 0);
        float4 b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + gOffset);
        float4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + bOffset);

        float4 scanMask = (float4) 0.0;

        // px /= 3;
        // py /= 3;

        int cline = py% 3;
 
        if (cline == 0)
            scanMask.r = 1;
        else if (cline == 1)
            scanMask.g = 1;
        else if (cline ==2)
            scanMask.b = 1;

        scanMask = lerp(1, scanMask, _Scanline);


        return  scanMask * float4(b.r, a.g, c.b, 1.0);
    }

        ENDHLSL

        SubShader
    {
        Cull Off ZWrite Off ZTest Always

            Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}