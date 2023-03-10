Shader "Hidden/Custom/Anaglyph3D"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float4 finalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
		finalColor += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + 0.006f) * float4(1, 0, 0, 1);
		finalColor += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord - 0.006f) * float4(0, 1, 1, 1);
        return finalColor / 2;
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