Shader "Hidden/Custom/Chroma"
{
    HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float _Amount;

    float4 Frag(VaryingsDefault i) : SV_Target
    {

        float2 delta = float2(_Amount, 0);
        float4 g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        float4 r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + delta);
        float4 r2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + delta*2);
        float4 b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord - delta);
        float4 b2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord - delta*2);

        return float4(
             0.5 * g.r + r.r +  2 * r2.r  ,
            2 * g.g + 0.75 * b.g + 0.75 * r.g, 
            0.5 * g.b + b.b + 2 * b2.b,
            3.5) /3.5 ;
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