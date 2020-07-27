Shader "Hidden/Kino/PostProcess/Chroma"
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

        float4 g = SampleInput(positionSS);
        float4 r = SampleInput(positionSS + int2(_Intensity, 0));
        float4 r2 = SampleInput(positionSS + int2(_Intensity* 2, 0));
        float4 b = SampleInput(positionSS - int2(_Intensity, 0));
        float4 b2 = SampleInput(positionSS - int2(_Intensity* 2, 0));

        return float4(
            0.5 * g.r + r.r + 2 * r2.r,
            2 * g.g + 0.75 * b.g + 0.75 * r.g,
            0.5 * g.b + b.b + 2 * b2.b,
            3.5) / 3.5;
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
