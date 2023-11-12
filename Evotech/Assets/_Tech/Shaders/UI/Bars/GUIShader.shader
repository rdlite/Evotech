Shader "Shader Graphs/BarNoise"
{
    Properties
    {
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
		_ColorTint ("Tint", Color) = (1,1,1,1)
        _FloatingSpeed("FloatingSpeed", Float) = 0.1
        _Intensity("Intensity", Float) = 1
        [NoScaleOffset]_NoiseTex("NoiseTex", 2D) = "white" {}
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        
        _StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalSpriteUnlitSubTarget"
        }
        
        Stencil{
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]
        
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest [unity_GUIZTestMode]
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _FloatingSpeed;
        float _Intensity;
        float4 _NoiseTex_TexelSize;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_NoiseTex);
        SAMPLER(sampler_NoiseTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Saturate_float4(float4 In, out float4 Out)
        {
            Out = saturate(In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D.tex, _Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D.samplerstate, _Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_R_4_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.r;
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_G_5_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.g;
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_B_6_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.b;
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_A_7_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.a;
            float _Split_ac12968945964feba37d29effbd705b8_R_1_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[0];
            float _Split_ac12968945964feba37d29effbd705b8_G_2_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[1];
            float _Split_ac12968945964feba37d29effbd705b8_B_3_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[2];
            float _Split_ac12968945964feba37d29effbd705b8_A_4_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[3];
            float4 _Combine_76b69bf85ff44bd2a1d20b302e8a996e_RGBA_4_Vector4;
            float3 _Combine_76b69bf85ff44bd2a1d20b302e8a996e_RGB_5_Vector3;
            float2 _Combine_76b69bf85ff44bd2a1d20b302e8a996e_RG_6_Vector2;
            Unity_Combine_float(_Split_ac12968945964feba37d29effbd705b8_R_1_Float, _Split_ac12968945964feba37d29effbd705b8_G_2_Float, _Split_ac12968945964feba37d29effbd705b8_B_3_Float, 0, _Combine_76b69bf85ff44bd2a1d20b302e8a996e_RGBA_4_Vector4, _Combine_76b69bf85ff44bd2a1d20b302e8a996e_RGB_5_Vector3, _Combine_76b69bf85ff44bd2a1d20b302e8a996e_RG_6_Vector2);
            float _Property_083a5691667e41f68e79e914226f873a_Out_0_Float = _Intensity;
            float _Float_a50b904a11b147c8b0821d1aefa2f152_Out_0_Float = 1;
            UnityTexture2D _Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_NoiseTex);
            float2 _Vector2_640816d5fcdb427f9f7b04da020497b4_Out_0_Vector2 = float2(0, IN.TimeParameters.x);
            float2 _Multiply_7d024a40c5fe43eca5bde32e6eff0017_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Vector2_640816d5fcdb427f9f7b04da020497b4_Out_0_Vector2, float2(-1, -1), _Multiply_7d024a40c5fe43eca5bde32e6eff0017_Out_2_Vector2);
            float _Property_6438b2c7900d4928a64a2c4ba41b8282_Out_0_Float = _FloatingSpeed;
            float2 _Multiply_0f54c2e9b4bf4a7796f872c8811c81f9_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_7d024a40c5fe43eca5bde32e6eff0017_Out_2_Vector2, (_Property_6438b2c7900d4928a64a2c4ba41b8282_Out_0_Float.xx), _Multiply_0f54c2e9b4bf4a7796f872c8811c81f9_Out_2_Vector2);
            float2 _TilingAndOffset_e09f9570686f4d8faec07670845bcc0f_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_0f54c2e9b4bf4a7796f872c8811c81f9_Out_2_Vector2, _TilingAndOffset_e09f9570686f4d8faec07670845bcc0f_Out_3_Vector2);
            float4 _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D.tex, _Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D.samplerstate, _Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_e09f9570686f4d8faec07670845bcc0f_Out_3_Vector2) );
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_R_4_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.r;
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_G_5_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.g;
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_B_6_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.b;
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_A_7_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.a;
            float4 _Multiply_4d730ded93e04c9887b1f8033ae34e2a_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Float_a50b904a11b147c8b0821d1aefa2f152_Out_0_Float.xxxx), _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4, _Multiply_4d730ded93e04c9887b1f8033ae34e2a_Out_2_Vector4);
            float _Float_8185e4b379014b02b8a11d5f676b769b_Out_0_Float = 1;
            UnityTexture2D _Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_NoiseTex);
            float2 _Multiply_29d868f14385453d849f0ec68979ff81_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Vector2_640816d5fcdb427f9f7b04da020497b4_Out_0_Vector2, float2(-1, 0.1), _Multiply_29d868f14385453d849f0ec68979ff81_Out_2_Vector2);
            float _Property_b105eb0cc8fb497aa0c5cede7ba30e61_Out_0_Float = _FloatingSpeed;
            float2 _Multiply_a6819037527d4f88bc74795ac626da43_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_29d868f14385453d849f0ec68979ff81_Out_2_Vector2, (_Property_b105eb0cc8fb497aa0c5cede7ba30e61_Out_0_Float.xx), _Multiply_a6819037527d4f88bc74795ac626da43_Out_2_Vector2);
            float2 _TilingAndOffset_91e1863b78934b2ca434b466fb82f61e_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_a6819037527d4f88bc74795ac626da43_Out_2_Vector2, _TilingAndOffset_91e1863b78934b2ca434b466fb82f61e_Out_3_Vector2);
            float4 _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D.tex, _Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D.samplerstate, _Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_91e1863b78934b2ca434b466fb82f61e_Out_3_Vector2) );
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_R_4_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.r;
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_G_5_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.g;
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_B_6_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.b;
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_A_7_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.a;
            float4 _Multiply_e6aa9cdd4a964656b59c28ae8a567247_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Float_8185e4b379014b02b8a11d5f676b769b_Out_0_Float.xxxx), _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4, _Multiply_e6aa9cdd4a964656b59c28ae8a567247_Out_2_Vector4);
            float _Float_72ad0de96209471f9bea3ae38ef6408e_Out_0_Float = 0.4;
            float4 _Subtract_2b3020b7bf174bf3859893901b838513_Out_2_Vector4;
            Unity_Subtract_float4(_Multiply_e6aa9cdd4a964656b59c28ae8a567247_Out_2_Vector4, (_Float_72ad0de96209471f9bea3ae38ef6408e_Out_0_Float.xxxx), _Subtract_2b3020b7bf174bf3859893901b838513_Out_2_Vector4);
            float4 _Add_1b272b47f23d4f29a46347472cbda9b1_Out_2_Vector4;
            Unity_Add_float4(_Multiply_4d730ded93e04c9887b1f8033ae34e2a_Out_2_Vector4, _Subtract_2b3020b7bf174bf3859893901b838513_Out_2_Vector4, _Add_1b272b47f23d4f29a46347472cbda9b1_Out_2_Vector4);
            float4 _Saturate_f212bdd3dd9e457a8d49495a333f4a53_Out_1_Vector4;
            Unity_Saturate_float4(_Add_1b272b47f23d4f29a46347472cbda9b1_Out_2_Vector4, _Saturate_f212bdd3dd9e457a8d49495a333f4a53_Out_1_Vector4);
            float4 _Multiply_6dff8091e62340ff996280aeea9cd490_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Split_ac12968945964feba37d29effbd705b8_A_4_Float.xxxx), _Saturate_f212bdd3dd9e457a8d49495a333f4a53_Out_1_Vector4, _Multiply_6dff8091e62340ff996280aeea9cd490_Out_2_Vector4);
            float4 _Multiply_02e4e5af9fc1434bac2a4f1d338924c4_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Property_083a5691667e41f68e79e914226f873a_Out_0_Float.xxxx), _Multiply_6dff8091e62340ff996280aeea9cd490_Out_2_Vector4, _Multiply_02e4e5af9fc1434bac2a4f1d338924c4_Out_2_Vector4);
            surface.BaseColor = _Combine_76b69bf85ff44bd2a1d20b302e8a996e_RGB_5_Vector3;
            surface.Alpha = (_Multiply_02e4e5af9fc1434bac2a4f1d338924c4_Out_2_Vector4).x;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _FloatingSpeed;
        float _Intensity;
        float4 _NoiseTex_TexelSize;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_NoiseTex);
        SAMPLER(sampler_NoiseTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Saturate_float4(float4 In, out float4 Out)
        {
            Out = saturate(In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_083a5691667e41f68e79e914226f873a_Out_0_Float = _Intensity;
            UnityTexture2D _Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D.tex, _Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D.samplerstate, _Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_R_4_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.r;
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_G_5_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.g;
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_B_6_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.b;
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_A_7_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.a;
            float _Split_ac12968945964feba37d29effbd705b8_R_1_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[0];
            float _Split_ac12968945964feba37d29effbd705b8_G_2_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[1];
            float _Split_ac12968945964feba37d29effbd705b8_B_3_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[2];
            float _Split_ac12968945964feba37d29effbd705b8_A_4_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[3];
            float _Float_a50b904a11b147c8b0821d1aefa2f152_Out_0_Float = 1;
            UnityTexture2D _Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_NoiseTex);
            float2 _Vector2_640816d5fcdb427f9f7b04da020497b4_Out_0_Vector2 = float2(0, IN.TimeParameters.x);
            float2 _Multiply_7d024a40c5fe43eca5bde32e6eff0017_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Vector2_640816d5fcdb427f9f7b04da020497b4_Out_0_Vector2, float2(-1, -1), _Multiply_7d024a40c5fe43eca5bde32e6eff0017_Out_2_Vector2);
            float _Property_6438b2c7900d4928a64a2c4ba41b8282_Out_0_Float = _FloatingSpeed;
            float2 _Multiply_0f54c2e9b4bf4a7796f872c8811c81f9_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_7d024a40c5fe43eca5bde32e6eff0017_Out_2_Vector2, (_Property_6438b2c7900d4928a64a2c4ba41b8282_Out_0_Float.xx), _Multiply_0f54c2e9b4bf4a7796f872c8811c81f9_Out_2_Vector2);
            float2 _TilingAndOffset_e09f9570686f4d8faec07670845bcc0f_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_0f54c2e9b4bf4a7796f872c8811c81f9_Out_2_Vector2, _TilingAndOffset_e09f9570686f4d8faec07670845bcc0f_Out_3_Vector2);
            float4 _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D.tex, _Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D.samplerstate, _Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_e09f9570686f4d8faec07670845bcc0f_Out_3_Vector2) );
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_R_4_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.r;
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_G_5_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.g;
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_B_6_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.b;
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_A_7_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.a;
            float4 _Multiply_4d730ded93e04c9887b1f8033ae34e2a_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Float_a50b904a11b147c8b0821d1aefa2f152_Out_0_Float.xxxx), _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4, _Multiply_4d730ded93e04c9887b1f8033ae34e2a_Out_2_Vector4);
            float _Float_8185e4b379014b02b8a11d5f676b769b_Out_0_Float = 1;
            UnityTexture2D _Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_NoiseTex);
            float2 _Multiply_29d868f14385453d849f0ec68979ff81_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Vector2_640816d5fcdb427f9f7b04da020497b4_Out_0_Vector2, float2(-1, 0.1), _Multiply_29d868f14385453d849f0ec68979ff81_Out_2_Vector2);
            float _Property_b105eb0cc8fb497aa0c5cede7ba30e61_Out_0_Float = _FloatingSpeed;
            float2 _Multiply_a6819037527d4f88bc74795ac626da43_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_29d868f14385453d849f0ec68979ff81_Out_2_Vector2, (_Property_b105eb0cc8fb497aa0c5cede7ba30e61_Out_0_Float.xx), _Multiply_a6819037527d4f88bc74795ac626da43_Out_2_Vector2);
            float2 _TilingAndOffset_91e1863b78934b2ca434b466fb82f61e_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_a6819037527d4f88bc74795ac626da43_Out_2_Vector2, _TilingAndOffset_91e1863b78934b2ca434b466fb82f61e_Out_3_Vector2);
            float4 _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D.tex, _Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D.samplerstate, _Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_91e1863b78934b2ca434b466fb82f61e_Out_3_Vector2) );
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_R_4_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.r;
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_G_5_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.g;
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_B_6_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.b;
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_A_7_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.a;
            float4 _Multiply_e6aa9cdd4a964656b59c28ae8a567247_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Float_8185e4b379014b02b8a11d5f676b769b_Out_0_Float.xxxx), _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4, _Multiply_e6aa9cdd4a964656b59c28ae8a567247_Out_2_Vector4);
            float _Float_72ad0de96209471f9bea3ae38ef6408e_Out_0_Float = 0.4;
            float4 _Subtract_2b3020b7bf174bf3859893901b838513_Out_2_Vector4;
            Unity_Subtract_float4(_Multiply_e6aa9cdd4a964656b59c28ae8a567247_Out_2_Vector4, (_Float_72ad0de96209471f9bea3ae38ef6408e_Out_0_Float.xxxx), _Subtract_2b3020b7bf174bf3859893901b838513_Out_2_Vector4);
            float4 _Add_1b272b47f23d4f29a46347472cbda9b1_Out_2_Vector4;
            Unity_Add_float4(_Multiply_4d730ded93e04c9887b1f8033ae34e2a_Out_2_Vector4, _Subtract_2b3020b7bf174bf3859893901b838513_Out_2_Vector4, _Add_1b272b47f23d4f29a46347472cbda9b1_Out_2_Vector4);
            float4 _Saturate_f212bdd3dd9e457a8d49495a333f4a53_Out_1_Vector4;
            Unity_Saturate_float4(_Add_1b272b47f23d4f29a46347472cbda9b1_Out_2_Vector4, _Saturate_f212bdd3dd9e457a8d49495a333f4a53_Out_1_Vector4);
            float4 _Multiply_6dff8091e62340ff996280aeea9cd490_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Split_ac12968945964feba37d29effbd705b8_A_4_Float.xxxx), _Saturate_f212bdd3dd9e457a8d49495a333f4a53_Out_1_Vector4, _Multiply_6dff8091e62340ff996280aeea9cd490_Out_2_Vector4);
            float4 _Multiply_02e4e5af9fc1434bac2a4f1d338924c4_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Property_083a5691667e41f68e79e914226f873a_Out_0_Float.xxxx), _Multiply_6dff8091e62340ff996280aeea9cd490_Out_2_Vector4, _Multiply_02e4e5af9fc1434bac2a4f1d338924c4_Out_2_Vector4);
            surface.Alpha = (_Multiply_02e4e5af9fc1434bac2a4f1d338924c4_Out_2_Vector4).x;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _FloatingSpeed;
        float _Intensity;
        float4 _NoiseTex_TexelSize;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_NoiseTex);
        SAMPLER(sampler_NoiseTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Saturate_float4(float4 In, out float4 Out)
        {
            Out = saturate(In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_083a5691667e41f68e79e914226f873a_Out_0_Float = _Intensity;
            UnityTexture2D _Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D.tex, _Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D.samplerstate, _Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_R_4_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.r;
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_G_5_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.g;
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_B_6_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.b;
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_A_7_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.a;
            float _Split_ac12968945964feba37d29effbd705b8_R_1_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[0];
            float _Split_ac12968945964feba37d29effbd705b8_G_2_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[1];
            float _Split_ac12968945964feba37d29effbd705b8_B_3_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[2];
            float _Split_ac12968945964feba37d29effbd705b8_A_4_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[3];
            float _Float_a50b904a11b147c8b0821d1aefa2f152_Out_0_Float = 1;
            UnityTexture2D _Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_NoiseTex);
            float2 _Vector2_640816d5fcdb427f9f7b04da020497b4_Out_0_Vector2 = float2(0, IN.TimeParameters.x);
            float2 _Multiply_7d024a40c5fe43eca5bde32e6eff0017_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Vector2_640816d5fcdb427f9f7b04da020497b4_Out_0_Vector2, float2(-1, -1), _Multiply_7d024a40c5fe43eca5bde32e6eff0017_Out_2_Vector2);
            float _Property_6438b2c7900d4928a64a2c4ba41b8282_Out_0_Float = _FloatingSpeed;
            float2 _Multiply_0f54c2e9b4bf4a7796f872c8811c81f9_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_7d024a40c5fe43eca5bde32e6eff0017_Out_2_Vector2, (_Property_6438b2c7900d4928a64a2c4ba41b8282_Out_0_Float.xx), _Multiply_0f54c2e9b4bf4a7796f872c8811c81f9_Out_2_Vector2);
            float2 _TilingAndOffset_e09f9570686f4d8faec07670845bcc0f_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_0f54c2e9b4bf4a7796f872c8811c81f9_Out_2_Vector2, _TilingAndOffset_e09f9570686f4d8faec07670845bcc0f_Out_3_Vector2);
            float4 _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D.tex, _Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D.samplerstate, _Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_e09f9570686f4d8faec07670845bcc0f_Out_3_Vector2) );
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_R_4_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.r;
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_G_5_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.g;
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_B_6_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.b;
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_A_7_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.a;
            float4 _Multiply_4d730ded93e04c9887b1f8033ae34e2a_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Float_a50b904a11b147c8b0821d1aefa2f152_Out_0_Float.xxxx), _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4, _Multiply_4d730ded93e04c9887b1f8033ae34e2a_Out_2_Vector4);
            float _Float_8185e4b379014b02b8a11d5f676b769b_Out_0_Float = 1;
            UnityTexture2D _Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_NoiseTex);
            float2 _Multiply_29d868f14385453d849f0ec68979ff81_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Vector2_640816d5fcdb427f9f7b04da020497b4_Out_0_Vector2, float2(-1, 0.1), _Multiply_29d868f14385453d849f0ec68979ff81_Out_2_Vector2);
            float _Property_b105eb0cc8fb497aa0c5cede7ba30e61_Out_0_Float = _FloatingSpeed;
            float2 _Multiply_a6819037527d4f88bc74795ac626da43_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_29d868f14385453d849f0ec68979ff81_Out_2_Vector2, (_Property_b105eb0cc8fb497aa0c5cede7ba30e61_Out_0_Float.xx), _Multiply_a6819037527d4f88bc74795ac626da43_Out_2_Vector2);
            float2 _TilingAndOffset_91e1863b78934b2ca434b466fb82f61e_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_a6819037527d4f88bc74795ac626da43_Out_2_Vector2, _TilingAndOffset_91e1863b78934b2ca434b466fb82f61e_Out_3_Vector2);
            float4 _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D.tex, _Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D.samplerstate, _Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_91e1863b78934b2ca434b466fb82f61e_Out_3_Vector2) );
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_R_4_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.r;
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_G_5_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.g;
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_B_6_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.b;
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_A_7_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.a;
            float4 _Multiply_e6aa9cdd4a964656b59c28ae8a567247_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Float_8185e4b379014b02b8a11d5f676b769b_Out_0_Float.xxxx), _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4, _Multiply_e6aa9cdd4a964656b59c28ae8a567247_Out_2_Vector4);
            float _Float_72ad0de96209471f9bea3ae38ef6408e_Out_0_Float = 0.4;
            float4 _Subtract_2b3020b7bf174bf3859893901b838513_Out_2_Vector4;
            Unity_Subtract_float4(_Multiply_e6aa9cdd4a964656b59c28ae8a567247_Out_2_Vector4, (_Float_72ad0de96209471f9bea3ae38ef6408e_Out_0_Float.xxxx), _Subtract_2b3020b7bf174bf3859893901b838513_Out_2_Vector4);
            float4 _Add_1b272b47f23d4f29a46347472cbda9b1_Out_2_Vector4;
            Unity_Add_float4(_Multiply_4d730ded93e04c9887b1f8033ae34e2a_Out_2_Vector4, _Subtract_2b3020b7bf174bf3859893901b838513_Out_2_Vector4, _Add_1b272b47f23d4f29a46347472cbda9b1_Out_2_Vector4);
            float4 _Saturate_f212bdd3dd9e457a8d49495a333f4a53_Out_1_Vector4;
            Unity_Saturate_float4(_Add_1b272b47f23d4f29a46347472cbda9b1_Out_2_Vector4, _Saturate_f212bdd3dd9e457a8d49495a333f4a53_Out_1_Vector4);
            float4 _Multiply_6dff8091e62340ff996280aeea9cd490_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Split_ac12968945964feba37d29effbd705b8_A_4_Float.xxxx), _Saturate_f212bdd3dd9e457a8d49495a333f4a53_Out_1_Vector4, _Multiply_6dff8091e62340ff996280aeea9cd490_Out_2_Vector4);
            float4 _Multiply_02e4e5af9fc1434bac2a4f1d338924c4_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Property_083a5691667e41f68e79e914226f873a_Out_0_Float.xxxx), _Multiply_6dff8091e62340ff996280aeea9cd490_Out_2_Vector4, _Multiply_02e4e5af9fc1434bac2a4f1d338924c4_Out_2_Vector4);
            surface.Alpha = (_Multiply_02e4e5af9fc1434bac2a4f1d338924c4_Out_2_Vector4).x;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEFORWARD
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _FloatingSpeed;
        float _Intensity;
        float4 _NoiseTex_TexelSize;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_NoiseTex);
        SAMPLER(sampler_NoiseTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Saturate_float4(float4 In, out float4 Out)
        {
            Out = saturate(In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D.tex, _Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D.samplerstate, _Property_f221278b1e74462891ae520238b150cd_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_R_4_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.r;
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_G_5_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.g;
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_B_6_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.b;
            float _SampleTexture2D_2f154ef5036b43f498c59b952d225986_A_7_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4.a;
            float _Split_ac12968945964feba37d29effbd705b8_R_1_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[0];
            float _Split_ac12968945964feba37d29effbd705b8_G_2_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[1];
            float _Split_ac12968945964feba37d29effbd705b8_B_3_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[2];
            float _Split_ac12968945964feba37d29effbd705b8_A_4_Float = _SampleTexture2D_2f154ef5036b43f498c59b952d225986_RGBA_0_Vector4[3];
            float4 _Combine_76b69bf85ff44bd2a1d20b302e8a996e_RGBA_4_Vector4;
            float3 _Combine_76b69bf85ff44bd2a1d20b302e8a996e_RGB_5_Vector3;
            float2 _Combine_76b69bf85ff44bd2a1d20b302e8a996e_RG_6_Vector2;
            Unity_Combine_float(_Split_ac12968945964feba37d29effbd705b8_R_1_Float, _Split_ac12968945964feba37d29effbd705b8_G_2_Float, _Split_ac12968945964feba37d29effbd705b8_B_3_Float, 0, _Combine_76b69bf85ff44bd2a1d20b302e8a996e_RGBA_4_Vector4, _Combine_76b69bf85ff44bd2a1d20b302e8a996e_RGB_5_Vector3, _Combine_76b69bf85ff44bd2a1d20b302e8a996e_RG_6_Vector2);
            float _Property_083a5691667e41f68e79e914226f873a_Out_0_Float = _Intensity;
            float _Float_a50b904a11b147c8b0821d1aefa2f152_Out_0_Float = 1;
            UnityTexture2D _Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_NoiseTex);
            float2 _Vector2_640816d5fcdb427f9f7b04da020497b4_Out_0_Vector2 = float2(0, IN.TimeParameters.x);
            float2 _Multiply_7d024a40c5fe43eca5bde32e6eff0017_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Vector2_640816d5fcdb427f9f7b04da020497b4_Out_0_Vector2, float2(-1, -1), _Multiply_7d024a40c5fe43eca5bde32e6eff0017_Out_2_Vector2);
            float _Property_6438b2c7900d4928a64a2c4ba41b8282_Out_0_Float = _FloatingSpeed;
            float2 _Multiply_0f54c2e9b4bf4a7796f872c8811c81f9_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_7d024a40c5fe43eca5bde32e6eff0017_Out_2_Vector2, (_Property_6438b2c7900d4928a64a2c4ba41b8282_Out_0_Float.xx), _Multiply_0f54c2e9b4bf4a7796f872c8811c81f9_Out_2_Vector2);
            float2 _TilingAndOffset_e09f9570686f4d8faec07670845bcc0f_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_0f54c2e9b4bf4a7796f872c8811c81f9_Out_2_Vector2, _TilingAndOffset_e09f9570686f4d8faec07670845bcc0f_Out_3_Vector2);
            float4 _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D.tex, _Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D.samplerstate, _Property_e335bb81c31746ce9c8a0d5b25fb5788_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_e09f9570686f4d8faec07670845bcc0f_Out_3_Vector2) );
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_R_4_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.r;
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_G_5_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.g;
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_B_6_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.b;
            float _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_A_7_Float = _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4.a;
            float4 _Multiply_4d730ded93e04c9887b1f8033ae34e2a_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Float_a50b904a11b147c8b0821d1aefa2f152_Out_0_Float.xxxx), _SampleTexture2D_6f524d40e1d34bb3b25ba4cefb0ce4bb_RGBA_0_Vector4, _Multiply_4d730ded93e04c9887b1f8033ae34e2a_Out_2_Vector4);
            float _Float_8185e4b379014b02b8a11d5f676b769b_Out_0_Float = 1;
            UnityTexture2D _Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_NoiseTex);
            float2 _Multiply_29d868f14385453d849f0ec68979ff81_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Vector2_640816d5fcdb427f9f7b04da020497b4_Out_0_Vector2, float2(-1, 0.1), _Multiply_29d868f14385453d849f0ec68979ff81_Out_2_Vector2);
            float _Property_b105eb0cc8fb497aa0c5cede7ba30e61_Out_0_Float = _FloatingSpeed;
            float2 _Multiply_a6819037527d4f88bc74795ac626da43_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_29d868f14385453d849f0ec68979ff81_Out_2_Vector2, (_Property_b105eb0cc8fb497aa0c5cede7ba30e61_Out_0_Float.xx), _Multiply_a6819037527d4f88bc74795ac626da43_Out_2_Vector2);
            float2 _TilingAndOffset_91e1863b78934b2ca434b466fb82f61e_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_a6819037527d4f88bc74795ac626da43_Out_2_Vector2, _TilingAndOffset_91e1863b78934b2ca434b466fb82f61e_Out_3_Vector2);
            float4 _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D.tex, _Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D.samplerstate, _Property_ba34903d1e0344d4944d28737fd5b1a3_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_91e1863b78934b2ca434b466fb82f61e_Out_3_Vector2) );
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_R_4_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.r;
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_G_5_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.g;
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_B_6_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.b;
            float _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_A_7_Float = _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4.a;
            float4 _Multiply_e6aa9cdd4a964656b59c28ae8a567247_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Float_8185e4b379014b02b8a11d5f676b769b_Out_0_Float.xxxx), _SampleTexture2D_5bd9bfbc60224ebd9359b659717ea7b7_RGBA_0_Vector4, _Multiply_e6aa9cdd4a964656b59c28ae8a567247_Out_2_Vector4);
            float _Float_72ad0de96209471f9bea3ae38ef6408e_Out_0_Float = 0.4;
            float4 _Subtract_2b3020b7bf174bf3859893901b838513_Out_2_Vector4;
            Unity_Subtract_float4(_Multiply_e6aa9cdd4a964656b59c28ae8a567247_Out_2_Vector4, (_Float_72ad0de96209471f9bea3ae38ef6408e_Out_0_Float.xxxx), _Subtract_2b3020b7bf174bf3859893901b838513_Out_2_Vector4);
            float4 _Add_1b272b47f23d4f29a46347472cbda9b1_Out_2_Vector4;
            Unity_Add_float4(_Multiply_4d730ded93e04c9887b1f8033ae34e2a_Out_2_Vector4, _Subtract_2b3020b7bf174bf3859893901b838513_Out_2_Vector4, _Add_1b272b47f23d4f29a46347472cbda9b1_Out_2_Vector4);
            float4 _Saturate_f212bdd3dd9e457a8d49495a333f4a53_Out_1_Vector4;
            Unity_Saturate_float4(_Add_1b272b47f23d4f29a46347472cbda9b1_Out_2_Vector4, _Saturate_f212bdd3dd9e457a8d49495a333f4a53_Out_1_Vector4);
            float4 _Multiply_6dff8091e62340ff996280aeea9cd490_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Split_ac12968945964feba37d29effbd705b8_A_4_Float.xxxx), _Saturate_f212bdd3dd9e457a8d49495a333f4a53_Out_1_Vector4, _Multiply_6dff8091e62340ff996280aeea9cd490_Out_2_Vector4);
            float4 _Multiply_02e4e5af9fc1434bac2a4f1d338924c4_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Property_083a5691667e41f68e79e914226f873a_Out_0_Float.xxxx), _Multiply_6dff8091e62340ff996280aeea9cd490_Out_2_Vector4, _Multiply_02e4e5af9fc1434bac2a4f1d338924c4_Out_2_Vector4);
            surface.BaseColor = _Combine_76b69bf85ff44bd2a1d20b302e8a996e_RGB_5_Vector3;
            surface.Alpha = (_Multiply_02e4e5af9fc1434bac2a4f1d338924c4_Out_2_Vector4).x;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}