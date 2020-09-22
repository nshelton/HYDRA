Shader "Hidden/Custom/Mirror"
{
    HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float _Amount;
    float _Height;

    float4 Frag(VaryingsDefault i) : SV_Target
    {

        float2 mirror = float2(1.0 - i.texcoord.x, i.texcoord.y);
        float4 L = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        float4 R = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, mirror);
        
        int px = _Height * i.texcoord.y;


        if (_Amount >= 0.9) {
            L = i.texcoord.x > 0.5 ? L : R;
        }
        else {

            if (px % 2 == 0)
                L = lerp(L, R, _Amount);
            else
                R = lerp(R, L, _Amount);
        }
    
        float4 result = lerp(L, R, _Amount);

        if (_Amount > 0.75){
            float2 uvMirror = float2(
                i.texcoord.x < 0.5 ? i.texcoord.x : 1- i.texcoord.x , 
                i.texcoord.y);

            float4 m = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvMirror);
            result = lerp(result, m,( _Amount - 0.75 ) * 4);
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