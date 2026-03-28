Shader "Hidden/ChromaticAberration"
{
    SubShader
    {
        Tags
        { 
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "ChromaticAberration"

            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            #pragma vertex Vert
            #pragma fragment Frag

            float _Displacement;
            half _Intensity;

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;
                float2 offset = uv - float2(0.5, 0.5);
                offset = clamp(offset, -0.1, 0.1);

                float2 uvR = uv;
                float2 uvG = uv - offset * _Displacement; 
                float2 uvB = uv - offset * (2.0 * _Displacement);

                half4 center = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uvR);
                half g = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uvG).g;
                half b = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uvB).b;

                half2 gb = lerp(center.gb, half2(g, b), _Intensity);

                return half4(center.r, gb, center.a);
            }

            ENDHLSL
        }
    }

    Fallback Off
}