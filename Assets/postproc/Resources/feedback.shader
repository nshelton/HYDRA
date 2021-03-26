Shader "Hidden/Custom/feedback"
{
    HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_FeedbackTex, sampler_FeedbackTex);

    TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
	half4 _CameraDepthTexture_ST;

    float _Blend;
    float _Scale;

    float4 Frag(VaryingsDefault i) : SV_Target
    {

        float2 uv = i.texcoord;
       
        float4 a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

        float scale = lerp(0.9, 1.1, _Scale);
        float2 uvf = ((uv - 0.5) * scale)+ 0.5;
        float4 b = SAMPLE_TEXTURE2D(_FeedbackTex, sampler_FeedbackTex, uvf);
        float4 result = lerp(a, b,  pow(_Blend, 0.5));
        
        float depth =  Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv));
        result.a = depth;

        if(depth < b.a)
        {
            result = float4(a.rgb, depth);
        }

        return result;
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