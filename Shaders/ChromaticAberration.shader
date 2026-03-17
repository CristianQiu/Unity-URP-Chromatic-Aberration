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
            float _Intensity;

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;
                float2 offset = uv - float2(0.5, 0.5);
                offset = min(abs(offset), float2(0.1, 0.1)) * sign(offset);

                float2 uvR = uv;
                float2 uvG = uv - offset * _Displacement; 
                float2 uvB = uv - offset * (2.0 * _Displacement);

                float4 center = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uvR);
                float g = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uvG).g;
                float b = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uvB).b;

                float r = center.r;
                g = lerp(center.g, g, _Intensity);
                b = lerp(center.b, b, _Intensity);
                float a = center.a;

                return float4(r, g, b, a);
            }

            ENDHLSL
        }
    }

    Fallback Off
}