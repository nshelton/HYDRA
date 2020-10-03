Shader "Hidden/Custom/feedback"
{
    HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_FeedbackTex, sampler_FeedbackTex);
       
    float _Blend;

    float4 Frag(VaryingsDefault i) : SV_Target
    {

        float2 uv = i.texcoord;
       
        float4 a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

        float2 uvf = ((uv - 0.5) * 0.99)+ 0.5;
        float4 b = SAMPLE_TEXTURE2D(_FeedbackTex, sampler_FeedbackTex, uvf);

        return  lerp(a, b,  pow(_Blend, 0.5));
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