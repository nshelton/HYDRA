Shader "RDSystem/Initialization"
{
    Properties
    {
        _Seed("Seeding", Range(0, 1)) = 0
    }

    CGINCLUDE

    #include "UnityCustomRenderTexture.cginc"

    half _Seed;

    float UVRandom(float2 uv)
    {
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }

    half4 frag(v2f_init_customrendertexture i) : SV_Target
    {
        float rnd = UVRandom(i.texcoord);
        return float4(UVRandom(i.texcoord), UVRandom(i.texcoord+1), UVRandom(i.texcoord+3), 0) - 0.5;

    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            Name "Init"
            CGPROGRAM
            #pragma vertex InitCustomRenderTextureVertexShader
            #pragma fragment frag
            ENDCG
        }
    }
}
