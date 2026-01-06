Shader "Sprites/Lit-DropShadow"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [HDR] _ShadowColor ("Shadow Color", Color) = (0,0,0,0.5)
        _ShadowOffset ("Shadow Offset", Vector) = (0.05, -0.05, 0, 0)
        _ShadowBlur ("Shadow Blur", Range(0, 0.1)) = 0.02
        _ShadowSamples ("Shadow Quality", Range(4, 32)) = 16
        
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "RenderPipeline"="UniversalPipeline"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        // Shadow Pass
        Pass
        {
            Name "ShadowPass"
            
            HLSLPROGRAM
            #pragma vertex ShadowVert
            #pragma fragment ShadowFrag
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                half4 _ShadowColor;
                float4 _ShadowOffset;
                float _ShadowBlur;
                float _ShadowSamples;
            CBUFFER_END

            Varyings ShadowVert(Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) + _ShadowOffset.xy;
                o.color = v.color * _ShadowColor;

                #ifdef PIXELSNAP_ON
                o.positionCS = UnityPixelSnap(o.positionCS);
                #endif

                return o;
            }

            half4 ShadowFrag(Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                half4 color = half4(0, 0, 0, 0);
                float samples = _ShadowSamples;
                float angleStep = 6.28318530718 / samples; // 2*PI
                
                // Sample in a circular pattern for blur
                for(float j = 0; j < samples; j++)
                {
                    float angle = angleStep * j;
                    float2 offset = float2(cos(angle), sin(angle)) * _ShadowBlur;
                    color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + offset);
                }
                
                color /= samples;
                color *= i.color;
                color.rgb *= color.a;
                
                return color;
            }
            ENDHLSL
        }

        // Sprite Pass
        Pass
        {
            Name "SpritePass"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_0 USE_SHAPE_LIGHT_TYPE_1 USE_SHAPE_LIGHT_TYPE_2 USE_SHAPE_LIGHT_TYPE_3
            #pragma multi_compile _ DEBUG_DISPLAY

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
                float4 tangent      : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
                float3 positionWS   : TEXCOORD1;
                float4 tangentWS    : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            half4 _MainTex_ST;
            half4 _Color;

            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif
            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif
            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif
            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            Varyings SpriteVert(Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                o.tangentWS = float4(TransformObjectToWorldDir(v.tangent.xyz), v.tangent.w);

                #ifdef PIXELSNAP_ON
                o.positionCS = UnityPixelSnap(o.positionCS);
                #endif

                return o;
            }

            half4 SpriteFrag(Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 color = texColor * i.color;

                // 2D Lighting
                #if defined(USE_SHAPE_LIGHT_TYPE_0) || defined(USE_SHAPE_LIGHT_TYPE_1) || defined(USE_SHAPE_LIGHT_TYPE_2) || defined(USE_SHAPE_LIGHT_TYPE_3)
                    half3 normal = half3(0, 0, 1);
                    half3 lighting = half3(0, 0, 0);

                    #if USE_SHAPE_LIGHT_TYPE_0
                    lighting += ComputeShapeLighting0(i.positionWS, normal);
                    #endif
                    #if USE_SHAPE_LIGHT_TYPE_1
                    lighting += ComputeShapeLighting1(i.positionWS, normal);
                    #endif
                    #if USE_SHAPE_LIGHT_TYPE_2
                    lighting += ComputeShapeLighting2(i.positionWS, normal);
                    #endif
                    #if USE_SHAPE_LIGHT_TYPE_3
                    lighting += ComputeShapeLighting3(i.positionWS, normal);
                    #endif

                    color.rgb *= lighting;
                #endif

                color.rgb *= color.a;
                
                return color;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}