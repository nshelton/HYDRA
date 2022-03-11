Shader "RDSystem/Update2"
{
    Properties
    {
        _InputTex ("Texture", 2D) = "black" {}
    }

    CGINCLUDE

    #include "UnityCustomRenderTexture.cginc"


    float _RotationPower, _Amplification, _Diagonal, _Curl, _Laplacian;
    float _LaplDivScale;
    float _DivScale;
    float _CenterWeight;
    float _EdgeWeight;
    float _CornerWeight;
    float _dx;
    float _dy;

    float _InputWeight;

    sampler2D _InputTex;


    half4 frag(v2f_customrendertexture i) : SV_Target
    {
    
        const float _K0 = _CenterWeight * -20.0/6.0; // center weight
        const float _K1 = _EdgeWeight *  4.0/6.0; // edge-neighbors
        const float _K2 = _CornerWeight * 1.0/6.0; // vertex-neighbors
        const float cs = _Curl ; // curl scale
        const float ls = _Laplacian; // laplacian scale
        const float ps = -0.06 * _LaplDivScale; // laplacian of divergence scale
        const float ds = -0.08 * _DivScale;  // divergence scale
        const float pwr =_RotationPower; // power when deriving rotation angle from curl
        const float amp = _Amplification + 0.5; // self-amplification
        const float sq2 = _Diagonal; // diagonal weight
    
        /* original parameters
        const float _K0 = -20.0/6.0; // center weight
        const float _K1 = 4.0/6.0; // edge-neighbors
        const float _K2 = 1.0/6.0; // vertex-neighbors
        const float cs = 0.25; // curl scale
        const float ls = 0.24; // laplacian scale
        const float ps = -0.06; // laplacian of divergence scale
        const float ds = -0.08; // divergence scale
        const float pwr = 0.2; // power when deriving rotation angle from curl
        const float amp = 1.0; // self-amplification
        const float sq2 = 0.7; // diagonal weight
        */
        float2 zoom = normalize(i.globalTexcoord - 0.5) * -0.002 ;
        float2 vUv = i.globalTexcoord + float2(_dx, _dy) + zoom;
        float2 texel = 1. / float2(_CustomRenderTextureWidth, _CustomRenderTextureHeight);

        // 3x3 neighborhood coordinates
        float step_x = texel.x;
        float step_y = texel.y;
        float2 n  = float2(0.0, step_y);
        float2 ne = float2(step_x, step_y);
        float2 e  = float2(step_x, 0.0);
        float2 se = float2(step_x, -step_y);
        float2 s  = float2(0.0, -step_y);
        float2 sw = float2(-step_x, -step_y);
        float2 w  = float2(-step_x, 0.0);
        float2 nw = float2(-step_x, step_y);

        float3 uv =    tex2D(_SelfTexture2D, vUv).xyz;
        float3 uv_n =  tex2D(_SelfTexture2D, vUv+n).xyz;
        float3 uv_e =  tex2D(_SelfTexture2D, vUv+e).xyz;
        float3 uv_s =  tex2D(_SelfTexture2D, vUv+s).xyz;
        float3 uv_w =  tex2D(_SelfTexture2D, vUv+w).xyz;
        float3 uv_nw = tex2D(_SelfTexture2D, vUv+nw).xyz;
        float3 uv_sw = tex2D(_SelfTexture2D, vUv+sw).xyz;
        float3 uv_ne = tex2D(_SelfTexture2D, vUv+ne).xyz;
        float3 uv_se = tex2D(_SelfTexture2D, vUv+se).xyz;
    
        // uv.x and uv.y are our x and y components, uv.z is divergence 

        // laplacian of all components
        float3 lapl  = _K0*uv + _K1*(uv_n + uv_e + uv_w + uv_s) + _K2*(uv_nw + uv_sw + uv_ne + uv_se);
        float sp = ps * lapl.z;
    
        // calculate curl
        // vectors point clockwise about the center point
        float curl = uv_n.x - uv_s.x - uv_e.y + uv_w.y + sq2 * (uv_nw.x + uv_nw.y + uv_ne.x - uv_ne.y + uv_sw.y - uv_sw.x - uv_se.y - uv_se.x);
    
        // compute angle of rotation from curl
        float sc = cs * sign(curl) * pow(abs(curl), pwr);
    
        // calculate divergence
        // vectors point inwards towards the center point
        float div  = uv_s.y - uv_n.y - uv_e.x + uv_w.x + sq2 * (uv_nw.x - uv_nw.y - uv_ne.x - uv_ne.y + uv_sw.x + uv_sw.y + uv_se.y - uv_se.x);
        float sd = ds * div;

        float2 norm = normalize(uv.xy);
    
        // temp values for the update rule
        float ta = amp * uv.x + ls * lapl.x + norm.x * sp + uv.x * sd;
        float tb = amp * uv.y + ls * lapl.y + norm.y * sp + uv.y * sd;

        // rotate
        float a = ta * cos(sc) - tb * sin(sc);
        float b = ta * sin(sc) + tb * cos(sc);
    
         //div *= 0.5;
        // b *= 0.99;
        //b += 0.0001;
    
        //if (frac(_Time.x * 3 ) < 0.1) {
      /* float2 p = float2(sin(_Time.y * 2), cos(_Time.y * 2.5)) * 0.2 + 0.5;
        if ( length(vUv - p) < 0.02){
            a = 0;//vUv.x - 0.5;
            b = 0;//vUv.y-0.5;
            div = 0;

        }*/
   
        // //}
        // fixed4 c = tex2D (_InputTex, vUv)  ;
        //     float we = saturate(_InputWeight );
        //      a  += c.r * we;//vUv.x - 0.5;
        //     b  += c.r * we;//vUv.y-0.5;
        //     div  += c.r * we;
    
        return clamp(float4(a,b,div,1), -1., 1.);

    }

    ENDCG
        /*



    half4 frag(v2f_customrendertexture i) : SV_Target
    {
        return 0;
    }

    EDNCG
        */

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
