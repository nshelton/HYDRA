Shader "Unlit/primitive"
{
    Properties
    {
        _Width ("width", Range(0, 0.5)) = 0.2
        _Type ("type", Range(0, 5)) = 0.2
        _Rotate ("rotate", Range(0, 5)) = 0
        _Margin ("margin", Range(0, 1)) = 0
        _Repeat ("repeat", Range(0, 100)) = 1
    }

    CGINCLUDE

    #include "UnityCustomRenderTexture.cginc"


    float _Width;
    float _Type;
    float _Rotate;
    float _Margin;
    float _Repeat;
    fixed _Amount;
    fixed _Scroll;
    
    
    half4 frag(v2f_customrendertexture i) : SV_Target
    {
        float2x2 fMatrix = { cos(_Rotate), sin(_Rotate), -sin(_Rotate), cos(_Rotate)};

        fixed4 col = 0;
        float2 uv =  i.globalTexcoord;
        uv = mul(uv, fMatrix);

         uv.y += _Scroll;

        uv = frac(uv * _Repeat);
        float w = _Width;// * (1 +  sin(uv.y));

        if ( _Type < 1) {

            if (length(uv - 0.5) < w)
                col = 1;

        } else if (_Type < 2) {

            if ( abs(uv.x-0.5) < w || abs(uv.y-0.5) < w )
                col = 1;

        } else if (_Type < 3) {
                    
            if ( abs(uv.x-0.5) < w && abs(uv.y-0.5) < w )
                col = 1;

        } else if (_Type < 4) {

            if ( abs(uv.x-0.5) > w || abs(uv.y-0.5) > w )
                col = 1;

        } else if (_Type < 5) {

            if ( abs(uv.x-0.5) > w && abs(uv.y-0.5) > w )
                col = 1;

        }

        if (uv.x < _Margin || uv.y < _Margin || uv.y > 1 - _Margin || uv.x > 1 - _Margin)
            col = 0;

        return col * _Amount;
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            Name "Update"
            CGPROGRAM
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            ENDCG
        }
    }
}
