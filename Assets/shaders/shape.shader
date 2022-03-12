Shader "Unlit/primitive"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("color", Color) = (1,1,1,1)
        _Size("size", float) = 0.5
        _Size2("size2", float) = 0.5
        _Num("num", float) = 0.5
        _Offset("offset", float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Size;
            float _Size2;
            float _Count;
            int _Num;
            int _NumY;
            float _Offset;
            float _Type;
            float _Rotation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float Shape(float2 uv, float2 center) {
                if ( _Type < 0.5) {
                    return length(uv - center);
                } else if ( _Type < 1) {
                    return abs(uv.x - center.x) + abs(uv.y - center.y)  ;

                }
                return length(uv - center);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = float4(0,0,0,0);
                // apply fog

                float2 center = float2(0.5, 0.5);

                for( float jj = 0; jj < _NumY; jj ++) {
                    for( float ii = 0; ii < _Num; ii ++) {
                        float2 center =  float2((ii + 0.5)/_Num,(jj + 0.5)/_NumY) - float2(0.5, 0.5);
                        center *= _Offset;
                        center += float2(0.5, 0.5);
                        float r = Shape(i.uv, center);
                        col += (1 - step(_Size*0.1, r)) * _Color;
                    }
                }
                if ( col.x %2 == 0) {
                    col = float4(0,0,0,0);

                }
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
