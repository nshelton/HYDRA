Shader "Hidden/Kino/PostProcess/Mirror"
{
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vertex(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    TEXTURE2D_X(_InputTexture);
    float _Intensity;

    float4 SampleInput(int2 coord)
    {
        coord = min(max(0, coord), _ScreenSize.xy - 1);
        return LOAD_TEXTURE2D_X(_InputTexture, coord);
    }

    float4 Fragment(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        int2 positionSS = input.texcoord * _ScreenSize.xy;

        int2 mirrorSS = int2(_ScreenSize.x - positionSS.x, positionSS.y);

        float4 L = SampleInput(positionSS);
        float4 R = SampleInput(mirrorSS);

        if (_Intensity >= 0.9) {
            L = input.texcoord.x > 0.5 ? L : R;
        }
        else {

            if (positionSS.y % 2 == 0)
                L = lerp(L, R, _Intensity);
            else
                R = lerp(R, L, _Intensity);
        }

        return lerp(L, R, _Intensity);

    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Cull Off ZWrite Off ZTest Always
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDHLSL
        }
    }
    Fallback Off
}
