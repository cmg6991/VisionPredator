// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_URP_VFX_Vignette_Speed_01"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		_OpacityMultiply("Opacity Multiply", Float) = 1
		[Space(33)][Header(Lines)][Space(13)]_LinesTexture("Lines Texture", 2D) = "white" {}
		[Space(33)][Header(Distortion)][Space(13)]_DistortionTexture("Distortion Texture", 2D) = "white" {}
		_DistortionUVScale("Distortion UV Scale", Vector) = (2,0.1,0,0)
		_DistortionUVPanSpeed("Distortion UV Pan Speed", Vector) = (0.1,1,0,0)
		_DistortionUVSwirl("Distortion UV Swirl", Float) = 0
		_Lines01UVScale("Lines 01 UV Scale", Vector) = (1,1,0,0)
		_Lines02UVScale("Lines 02 UV Scale", Vector) = (1,1,0,0)
		_Lines02UVPanSpeed("Lines 02 UV Pan Speed", Vector) = (0.1,-2,0,0)
		_Lines01UVPanSpeed("Lines 01 UV Pan Speed", Vector) = (0,0,0,0)
		_Lines01UVSwirl("Lines 01 UV Swirl", Float) = 0
		_Lines02UVSwirl("Lines 02 UV Swirl", Float) = 0
		_DistortionMultiply("Distortion Multiply", Float) = 0
		[Space(13)][Header(AR)][Space(13)]_Cull1("Cull", Float) = 2
		_Src1("Src", Float) = 5
		_Dst1("Dst", Float) = 10
		_ZWrite1("ZWrite", Float) = 0
		_Color("Color", Color) = (1,1,1,0)
		_Vector4("Vector 4", Vector) = (0,0.5,0,0)
		_ZTest1("ZTest", Float) = 2
		_MaskPower("Mask Power", Float) = 3
		_LinesMultiply("Lines Multiply", Float) = 20


		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

		[HideInInspector][ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Unlit" }

		Cull [_Cull1]
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 3.5
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForwardOnly" }

			Blend [_Src1] [_Dst1], One OneMinusSrcAlpha
			ZWrite [_ZWrite1]
			ZTest [_ZTest1]
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 120111


			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ DEBUG_DISPLAY

            #pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_UNLIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"

			

			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
					float fogFactor : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float2 _Vector4;
			float2 _Lines02UVPanSpeed;
			float2 _Lines02UVScale;
			float2 _Lines01UVScale;
			float2 _Lines01UVPanSpeed;
			float2 _DistortionUVScale;
			float2 _DistortionUVPanSpeed;
			float _MaskPower;
			float _Lines02UVSwirl;
			float _DistortionMultiply;
			float _DistortionUVSwirl;
			float _Cull1;
			float _Lines01UVSwirl;
			float _ZWrite1;
			float _ZTest1;
			float _Dst1;
			float _Src1;
			float _LinesMultiply;
			float _OpacityMultiply;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _LinesTexture;
			sampler2D _DistortionTexture;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord3 = v.ase_texcoord;
				o.ase_texcoord4 = v.ase_texcoord1;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.positionWS = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				#ifdef ASE_FOG
					o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif

				o.positionCS = positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 temp_output_73_0_g55 = _Lines01UVPanSpeed;
				float2 texCoord68_g55 = IN.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g55 = ( texCoord68_g55 - float2( 1,1 ) );
				float2 appendResult93_g55 = (float2(frac( ( atan2( (temp_output_86_0_g55).x , (temp_output_86_0_g55).y ) / TWO_PI ) ) , length( temp_output_86_0_g55 )));
				float2 panner59_g55 = ( ( (temp_output_73_0_g55).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g55);
				float2 texCoord103_g55 = IN.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g55 = ( texCoord103_g55 - float2( 1,1 ) );
				float2 appendResult107_g55 = (float2(frac( ( atan2( (temp_output_104_0_g55).x , (temp_output_104_0_g55).y ) / TWO_PI ) ) , length( temp_output_104_0_g55 )));
				float2 panner60_g55 = ( ( _TimeParameters.x * (temp_output_73_0_g55).y ) * float2( 0,1 ) + appendResult93_g55);
				float2 appendResult90_g55 = (float2(( (panner59_g55).x + ( (appendResult107_g55).y * _Lines01UVSwirl ) ) , (panner60_g55).y));
				float2 temp_output_220_100 = ( _Lines01UVScale * appendResult90_g55 );
				float2 temp_output_234_0 = ( ( 1.0 - temp_output_220_100 ) + _Vector4 );
				float2 temp_output_73_0_g54 = _DistortionUVPanSpeed;
				float2 texCoord68_g54 = IN.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g54 = ( texCoord68_g54 - float2( 1,1 ) );
				float2 appendResult93_g54 = (float2(frac( ( atan2( (temp_output_86_0_g54).x , (temp_output_86_0_g54).y ) / TWO_PI ) ) , length( temp_output_86_0_g54 )));
				float2 panner59_g54 = ( ( (temp_output_73_0_g54).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g54);
				float2 texCoord103_g54 = IN.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g54 = ( texCoord103_g54 - float2( 1,1 ) );
				float2 appendResult107_g54 = (float2(frac( ( atan2( (temp_output_104_0_g54).x , (temp_output_104_0_g54).y ) / TWO_PI ) ) , length( temp_output_104_0_g54 )));
				float2 panner60_g54 = ( ( _TimeParameters.x * (temp_output_73_0_g54).y ) * float2( 0,1 ) + appendResult93_g54);
				float2 appendResult90_g54 = (float2(( (panner59_g54).x + ( (appendResult107_g54).y * _DistortionUVSwirl ) ) , (panner60_g54).y));
				float randomUV288 = IN.ase_texcoord4.z;
				float4 temp_output_208_0 = ( tex2D( _DistortionTexture, ( ( _DistortionUVScale * appendResult90_g54 ) + randomUV288 ) ) * _DistortionMultiply );
				float2 temp_output_73_0_g56 = _Lines02UVPanSpeed;
				float2 texCoord68_g56 = IN.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g56 = ( texCoord68_g56 - float2( 1,1 ) );
				float2 appendResult93_g56 = (float2(frac( ( atan2( (temp_output_86_0_g56).x , (temp_output_86_0_g56).y ) / TWO_PI ) ) , length( temp_output_86_0_g56 )));
				float2 panner59_g56 = ( ( (temp_output_73_0_g56).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g56);
				float2 texCoord103_g56 = IN.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g56 = ( texCoord103_g56 - float2( 1,1 ) );
				float2 appendResult107_g56 = (float2(frac( ( atan2( (temp_output_104_0_g56).x , (temp_output_104_0_g56).y ) / TWO_PI ) ) , length( temp_output_104_0_g56 )));
				float2 panner60_g56 = ( ( _TimeParameters.x * (temp_output_73_0_g56).y ) * float2( 0,1 ) + appendResult93_g56);
				float2 appendResult90_g56 = (float2(( (panner59_g56).x + ( (appendResult107_g56).y * _Lines02UVSwirl ) ) , (panner60_g56).y));
				float temp_output_275_0 = (( temp_output_220_100 + -0.45 )).y;
				float temp_output_280_0 = saturate( ( ( ( tex2D( _LinesTexture, ( float4( temp_output_234_0, 0.0 , 0.0 ) + temp_output_208_0 ).rg ).g * tex2D( _LinesTexture, ( ( float4( ( 1.0 - ( _Lines02UVScale * appendResult90_g56 ) ), 0.0 , 0.0 ) + temp_output_208_0 ) + randomUV288 ).rg ).g ) * pow( temp_output_275_0 , _MaskPower ) ) * _LinesMultiply ) );
				float temp_output_118_0 = ( ( saturate( IN.ase_texcoord3.z ) * temp_output_280_0 ) * _OpacityMultiply );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = _Color.rgb;
				float Alpha = saturate( temp_output_118_0 );
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#if defined(_DBUFFER)
					ApplyDecalToBaseColor(IN.positionCS, Color);
				#endif

				#if defined(_ALPHAPREMULTIPLY_ON)
				Color *= Alpha;
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.positionCS.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				return half4( Color, Alpha );
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

            #pragma multi_compile_instancing
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ASE_SRP_VERSION 120111


            #pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			

			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 positionWS : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float2 _Vector4;
			float2 _Lines02UVPanSpeed;
			float2 _Lines02UVScale;
			float2 _Lines01UVScale;
			float2 _Lines01UVPanSpeed;
			float2 _DistortionUVScale;
			float2 _DistortionUVPanSpeed;
			float _MaskPower;
			float _Lines02UVSwirl;
			float _DistortionMultiply;
			float _DistortionUVSwirl;
			float _Cull1;
			float _Lines01UVSwirl;
			float _ZWrite1;
			float _ZTest1;
			float _Dst1;
			float _Src1;
			float _LinesMultiply;
			float _OpacityMultiply;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _LinesTexture;
			sampler2D _DistortionTexture;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord2 = v.ase_texcoord;
				o.ase_texcoord3 = v.ase_texcoord1;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.positionWS = positionWS;
				#endif

				o.positionCS = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 temp_output_73_0_g55 = _Lines01UVPanSpeed;
				float2 texCoord68_g55 = IN.ase_texcoord2.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g55 = ( texCoord68_g55 - float2( 1,1 ) );
				float2 appendResult93_g55 = (float2(frac( ( atan2( (temp_output_86_0_g55).x , (temp_output_86_0_g55).y ) / TWO_PI ) ) , length( temp_output_86_0_g55 )));
				float2 panner59_g55 = ( ( (temp_output_73_0_g55).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g55);
				float2 texCoord103_g55 = IN.ase_texcoord2.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g55 = ( texCoord103_g55 - float2( 1,1 ) );
				float2 appendResult107_g55 = (float2(frac( ( atan2( (temp_output_104_0_g55).x , (temp_output_104_0_g55).y ) / TWO_PI ) ) , length( temp_output_104_0_g55 )));
				float2 panner60_g55 = ( ( _TimeParameters.x * (temp_output_73_0_g55).y ) * float2( 0,1 ) + appendResult93_g55);
				float2 appendResult90_g55 = (float2(( (panner59_g55).x + ( (appendResult107_g55).y * _Lines01UVSwirl ) ) , (panner60_g55).y));
				float2 temp_output_220_100 = ( _Lines01UVScale * appendResult90_g55 );
				float2 temp_output_234_0 = ( ( 1.0 - temp_output_220_100 ) + _Vector4 );
				float2 temp_output_73_0_g54 = _DistortionUVPanSpeed;
				float2 texCoord68_g54 = IN.ase_texcoord2.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g54 = ( texCoord68_g54 - float2( 1,1 ) );
				float2 appendResult93_g54 = (float2(frac( ( atan2( (temp_output_86_0_g54).x , (temp_output_86_0_g54).y ) / TWO_PI ) ) , length( temp_output_86_0_g54 )));
				float2 panner59_g54 = ( ( (temp_output_73_0_g54).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g54);
				float2 texCoord103_g54 = IN.ase_texcoord2.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g54 = ( texCoord103_g54 - float2( 1,1 ) );
				float2 appendResult107_g54 = (float2(frac( ( atan2( (temp_output_104_0_g54).x , (temp_output_104_0_g54).y ) / TWO_PI ) ) , length( temp_output_104_0_g54 )));
				float2 panner60_g54 = ( ( _TimeParameters.x * (temp_output_73_0_g54).y ) * float2( 0,1 ) + appendResult93_g54);
				float2 appendResult90_g54 = (float2(( (panner59_g54).x + ( (appendResult107_g54).y * _DistortionUVSwirl ) ) , (panner60_g54).y));
				float randomUV288 = IN.ase_texcoord3.z;
				float4 temp_output_208_0 = ( tex2D( _DistortionTexture, ( ( _DistortionUVScale * appendResult90_g54 ) + randomUV288 ) ) * _DistortionMultiply );
				float2 temp_output_73_0_g56 = _Lines02UVPanSpeed;
				float2 texCoord68_g56 = IN.ase_texcoord2.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g56 = ( texCoord68_g56 - float2( 1,1 ) );
				float2 appendResult93_g56 = (float2(frac( ( atan2( (temp_output_86_0_g56).x , (temp_output_86_0_g56).y ) / TWO_PI ) ) , length( temp_output_86_0_g56 )));
				float2 panner59_g56 = ( ( (temp_output_73_0_g56).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g56);
				float2 texCoord103_g56 = IN.ase_texcoord2.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g56 = ( texCoord103_g56 - float2( 1,1 ) );
				float2 appendResult107_g56 = (float2(frac( ( atan2( (temp_output_104_0_g56).x , (temp_output_104_0_g56).y ) / TWO_PI ) ) , length( temp_output_104_0_g56 )));
				float2 panner60_g56 = ( ( _TimeParameters.x * (temp_output_73_0_g56).y ) * float2( 0,1 ) + appendResult93_g56);
				float2 appendResult90_g56 = (float2(( (panner59_g56).x + ( (appendResult107_g56).y * _Lines02UVSwirl ) ) , (panner60_g56).y));
				float temp_output_275_0 = (( temp_output_220_100 + -0.45 )).y;
				float temp_output_280_0 = saturate( ( ( ( tex2D( _LinesTexture, ( float4( temp_output_234_0, 0.0 , 0.0 ) + temp_output_208_0 ).rg ).g * tex2D( _LinesTexture, ( ( float4( ( 1.0 - ( _Lines02UVScale * appendResult90_g56 ) ), 0.0 , 0.0 ) + temp_output_208_0 ) + randomUV288 ).rg ).g ) * pow( temp_output_275_0 , _MaskPower ) ) * _LinesMultiply ) );
				float temp_output_118_0 = ( ( saturate( IN.ase_texcoord2.z ) * temp_output_280_0 ) * _OpacityMultiply );
				

				float Alpha = saturate( temp_output_118_0 );
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.positionCS.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off
			AlphaToMask Off

			HLSLPROGRAM

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ASE_SRP_VERSION 120111


            #pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			

			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float2 _Vector4;
			float2 _Lines02UVPanSpeed;
			float2 _Lines02UVScale;
			float2 _Lines01UVScale;
			float2 _Lines01UVPanSpeed;
			float2 _DistortionUVScale;
			float2 _DistortionUVPanSpeed;
			float _MaskPower;
			float _Lines02UVSwirl;
			float _DistortionMultiply;
			float _DistortionUVSwirl;
			float _Cull1;
			float _Lines01UVSwirl;
			float _ZWrite1;
			float _ZTest1;
			float _Dst1;
			float _Src1;
			float _LinesMultiply;
			float _OpacityMultiply;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _LinesTexture;
			sampler2D _DistortionTexture;


			
			int _ObjectId;
			int _PassValue;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );

				o.positionCS = TransformWorldToHClip(positionWS);

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 temp_output_73_0_g55 = _Lines01UVPanSpeed;
				float2 texCoord68_g55 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g55 = ( texCoord68_g55 - float2( 1,1 ) );
				float2 appendResult93_g55 = (float2(frac( ( atan2( (temp_output_86_0_g55).x , (temp_output_86_0_g55).y ) / TWO_PI ) ) , length( temp_output_86_0_g55 )));
				float2 panner59_g55 = ( ( (temp_output_73_0_g55).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g55);
				float2 texCoord103_g55 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g55 = ( texCoord103_g55 - float2( 1,1 ) );
				float2 appendResult107_g55 = (float2(frac( ( atan2( (temp_output_104_0_g55).x , (temp_output_104_0_g55).y ) / TWO_PI ) ) , length( temp_output_104_0_g55 )));
				float2 panner60_g55 = ( ( _TimeParameters.x * (temp_output_73_0_g55).y ) * float2( 0,1 ) + appendResult93_g55);
				float2 appendResult90_g55 = (float2(( (panner59_g55).x + ( (appendResult107_g55).y * _Lines01UVSwirl ) ) , (panner60_g55).y));
				float2 temp_output_220_100 = ( _Lines01UVScale * appendResult90_g55 );
				float2 temp_output_234_0 = ( ( 1.0 - temp_output_220_100 ) + _Vector4 );
				float2 temp_output_73_0_g54 = _DistortionUVPanSpeed;
				float2 texCoord68_g54 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g54 = ( texCoord68_g54 - float2( 1,1 ) );
				float2 appendResult93_g54 = (float2(frac( ( atan2( (temp_output_86_0_g54).x , (temp_output_86_0_g54).y ) / TWO_PI ) ) , length( temp_output_86_0_g54 )));
				float2 panner59_g54 = ( ( (temp_output_73_0_g54).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g54);
				float2 texCoord103_g54 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g54 = ( texCoord103_g54 - float2( 1,1 ) );
				float2 appendResult107_g54 = (float2(frac( ( atan2( (temp_output_104_0_g54).x , (temp_output_104_0_g54).y ) / TWO_PI ) ) , length( temp_output_104_0_g54 )));
				float2 panner60_g54 = ( ( _TimeParameters.x * (temp_output_73_0_g54).y ) * float2( 0,1 ) + appendResult93_g54);
				float2 appendResult90_g54 = (float2(( (panner59_g54).x + ( (appendResult107_g54).y * _DistortionUVSwirl ) ) , (panner60_g54).y));
				float randomUV288 = IN.ase_texcoord1.z;
				float4 temp_output_208_0 = ( tex2D( _DistortionTexture, ( ( _DistortionUVScale * appendResult90_g54 ) + randomUV288 ) ) * _DistortionMultiply );
				float2 temp_output_73_0_g56 = _Lines02UVPanSpeed;
				float2 texCoord68_g56 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g56 = ( texCoord68_g56 - float2( 1,1 ) );
				float2 appendResult93_g56 = (float2(frac( ( atan2( (temp_output_86_0_g56).x , (temp_output_86_0_g56).y ) / TWO_PI ) ) , length( temp_output_86_0_g56 )));
				float2 panner59_g56 = ( ( (temp_output_73_0_g56).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g56);
				float2 texCoord103_g56 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g56 = ( texCoord103_g56 - float2( 1,1 ) );
				float2 appendResult107_g56 = (float2(frac( ( atan2( (temp_output_104_0_g56).x , (temp_output_104_0_g56).y ) / TWO_PI ) ) , length( temp_output_104_0_g56 )));
				float2 panner60_g56 = ( ( _TimeParameters.x * (temp_output_73_0_g56).y ) * float2( 0,1 ) + appendResult93_g56);
				float2 appendResult90_g56 = (float2(( (panner59_g56).x + ( (appendResult107_g56).y * _Lines02UVSwirl ) ) , (panner60_g56).y));
				float temp_output_275_0 = (( temp_output_220_100 + -0.45 )).y;
				float temp_output_280_0 = saturate( ( ( ( tex2D( _LinesTexture, ( float4( temp_output_234_0, 0.0 , 0.0 ) + temp_output_208_0 ).rg ).g * tex2D( _LinesTexture, ( ( float4( ( 1.0 - ( _Lines02UVScale * appendResult90_g56 ) ), 0.0 , 0.0 ) + temp_output_208_0 ) + randomUV288 ).rg ).g ) * pow( temp_output_275_0 , _MaskPower ) ) * _LinesMultiply ) );
				float temp_output_118_0 = ( ( saturate( IN.ase_texcoord.z ) * temp_output_280_0 ) * _OpacityMultiply );
				

				surfaceDescription.Alpha = saturate( temp_output_118_0 );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ScenePickingPass"
			Tags { "LightMode"="Picking" }

			AlphaToMask Off

			HLSLPROGRAM

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ASE_SRP_VERSION 120111


            #pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT

			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			

			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float2 _Vector4;
			float2 _Lines02UVPanSpeed;
			float2 _Lines02UVScale;
			float2 _Lines01UVScale;
			float2 _Lines01UVPanSpeed;
			float2 _DistortionUVScale;
			float2 _DistortionUVPanSpeed;
			float _MaskPower;
			float _Lines02UVSwirl;
			float _DistortionMultiply;
			float _DistortionUVSwirl;
			float _Cull1;
			float _Lines01UVSwirl;
			float _ZWrite1;
			float _ZTest1;
			float _Dst1;
			float _Src1;
			float _LinesMultiply;
			float _OpacityMultiply;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _LinesTexture;
			sampler2D _DistortionTexture;


			
			float4 _SelectionID;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );
				o.positionCS = TransformWorldToHClip(positionWS);
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 temp_output_73_0_g55 = _Lines01UVPanSpeed;
				float2 texCoord68_g55 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g55 = ( texCoord68_g55 - float2( 1,1 ) );
				float2 appendResult93_g55 = (float2(frac( ( atan2( (temp_output_86_0_g55).x , (temp_output_86_0_g55).y ) / TWO_PI ) ) , length( temp_output_86_0_g55 )));
				float2 panner59_g55 = ( ( (temp_output_73_0_g55).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g55);
				float2 texCoord103_g55 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g55 = ( texCoord103_g55 - float2( 1,1 ) );
				float2 appendResult107_g55 = (float2(frac( ( atan2( (temp_output_104_0_g55).x , (temp_output_104_0_g55).y ) / TWO_PI ) ) , length( temp_output_104_0_g55 )));
				float2 panner60_g55 = ( ( _TimeParameters.x * (temp_output_73_0_g55).y ) * float2( 0,1 ) + appendResult93_g55);
				float2 appendResult90_g55 = (float2(( (panner59_g55).x + ( (appendResult107_g55).y * _Lines01UVSwirl ) ) , (panner60_g55).y));
				float2 temp_output_220_100 = ( _Lines01UVScale * appendResult90_g55 );
				float2 temp_output_234_0 = ( ( 1.0 - temp_output_220_100 ) + _Vector4 );
				float2 temp_output_73_0_g54 = _DistortionUVPanSpeed;
				float2 texCoord68_g54 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g54 = ( texCoord68_g54 - float2( 1,1 ) );
				float2 appendResult93_g54 = (float2(frac( ( atan2( (temp_output_86_0_g54).x , (temp_output_86_0_g54).y ) / TWO_PI ) ) , length( temp_output_86_0_g54 )));
				float2 panner59_g54 = ( ( (temp_output_73_0_g54).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g54);
				float2 texCoord103_g54 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g54 = ( texCoord103_g54 - float2( 1,1 ) );
				float2 appendResult107_g54 = (float2(frac( ( atan2( (temp_output_104_0_g54).x , (temp_output_104_0_g54).y ) / TWO_PI ) ) , length( temp_output_104_0_g54 )));
				float2 panner60_g54 = ( ( _TimeParameters.x * (temp_output_73_0_g54).y ) * float2( 0,1 ) + appendResult93_g54);
				float2 appendResult90_g54 = (float2(( (panner59_g54).x + ( (appendResult107_g54).y * _DistortionUVSwirl ) ) , (panner60_g54).y));
				float randomUV288 = IN.ase_texcoord1.z;
				float4 temp_output_208_0 = ( tex2D( _DistortionTexture, ( ( _DistortionUVScale * appendResult90_g54 ) + randomUV288 ) ) * _DistortionMultiply );
				float2 temp_output_73_0_g56 = _Lines02UVPanSpeed;
				float2 texCoord68_g56 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g56 = ( texCoord68_g56 - float2( 1,1 ) );
				float2 appendResult93_g56 = (float2(frac( ( atan2( (temp_output_86_0_g56).x , (temp_output_86_0_g56).y ) / TWO_PI ) ) , length( temp_output_86_0_g56 )));
				float2 panner59_g56 = ( ( (temp_output_73_0_g56).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g56);
				float2 texCoord103_g56 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g56 = ( texCoord103_g56 - float2( 1,1 ) );
				float2 appendResult107_g56 = (float2(frac( ( atan2( (temp_output_104_0_g56).x , (temp_output_104_0_g56).y ) / TWO_PI ) ) , length( temp_output_104_0_g56 )));
				float2 panner60_g56 = ( ( _TimeParameters.x * (temp_output_73_0_g56).y ) * float2( 0,1 ) + appendResult93_g56);
				float2 appendResult90_g56 = (float2(( (panner59_g56).x + ( (appendResult107_g56).y * _Lines02UVSwirl ) ) , (panner60_g56).y));
				float temp_output_275_0 = (( temp_output_220_100 + -0.45 )).y;
				float temp_output_280_0 = saturate( ( ( ( tex2D( _LinesTexture, ( float4( temp_output_234_0, 0.0 , 0.0 ) + temp_output_208_0 ).rg ).g * tex2D( _LinesTexture, ( ( float4( ( 1.0 - ( _Lines02UVScale * appendResult90_g56 ) ), 0.0 , 0.0 ) + temp_output_208_0 ) + randomUV288 ).rg ).g ) * pow( temp_output_275_0 , _MaskPower ) ) * _LinesMultiply ) );
				float temp_output_118_0 = ( ( saturate( IN.ase_texcoord.z ) * temp_output_280_0 ) * _OpacityMultiply );
				

				surfaceDescription.Alpha = saturate( temp_output_118_0 );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;
				outColor = _SelectionID;

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormalsOnly" }

			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

            #pragma multi_compile_instancing
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ASE_SRP_VERSION 120111


            #pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define VARYINGS_NEED_NORMAL_WS

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			

			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float3 normalWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float2 _Vector4;
			float2 _Lines02UVPanSpeed;
			float2 _Lines02UVScale;
			float2 _Lines01UVScale;
			float2 _Lines01UVPanSpeed;
			float2 _DistortionUVScale;
			float2 _DistortionUVPanSpeed;
			float _MaskPower;
			float _Lines02UVSwirl;
			float _DistortionMultiply;
			float _DistortionUVSwirl;
			float _Cull1;
			float _Lines01UVSwirl;
			float _ZWrite1;
			float _ZTest1;
			float _Dst1;
			float _Src1;
			float _LinesMultiply;
			float _OpacityMultiply;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _LinesTexture;
			sampler2D _DistortionTexture;


			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord1 = v.ase_texcoord;
				o.ase_texcoord2 = v.ase_texcoord1;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );
				float3 normalWS = TransformObjectToWorldNormal(v.normalOS);

				o.positionCS = TransformWorldToHClip(positionWS);
				o.normalWS.xyz =  normalWS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 temp_output_73_0_g55 = _Lines01UVPanSpeed;
				float2 texCoord68_g55 = IN.ase_texcoord1.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g55 = ( texCoord68_g55 - float2( 1,1 ) );
				float2 appendResult93_g55 = (float2(frac( ( atan2( (temp_output_86_0_g55).x , (temp_output_86_0_g55).y ) / TWO_PI ) ) , length( temp_output_86_0_g55 )));
				float2 panner59_g55 = ( ( (temp_output_73_0_g55).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g55);
				float2 texCoord103_g55 = IN.ase_texcoord1.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g55 = ( texCoord103_g55 - float2( 1,1 ) );
				float2 appendResult107_g55 = (float2(frac( ( atan2( (temp_output_104_0_g55).x , (temp_output_104_0_g55).y ) / TWO_PI ) ) , length( temp_output_104_0_g55 )));
				float2 panner60_g55 = ( ( _TimeParameters.x * (temp_output_73_0_g55).y ) * float2( 0,1 ) + appendResult93_g55);
				float2 appendResult90_g55 = (float2(( (panner59_g55).x + ( (appendResult107_g55).y * _Lines01UVSwirl ) ) , (panner60_g55).y));
				float2 temp_output_220_100 = ( _Lines01UVScale * appendResult90_g55 );
				float2 temp_output_234_0 = ( ( 1.0 - temp_output_220_100 ) + _Vector4 );
				float2 temp_output_73_0_g54 = _DistortionUVPanSpeed;
				float2 texCoord68_g54 = IN.ase_texcoord1.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g54 = ( texCoord68_g54 - float2( 1,1 ) );
				float2 appendResult93_g54 = (float2(frac( ( atan2( (temp_output_86_0_g54).x , (temp_output_86_0_g54).y ) / TWO_PI ) ) , length( temp_output_86_0_g54 )));
				float2 panner59_g54 = ( ( (temp_output_73_0_g54).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g54);
				float2 texCoord103_g54 = IN.ase_texcoord1.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g54 = ( texCoord103_g54 - float2( 1,1 ) );
				float2 appendResult107_g54 = (float2(frac( ( atan2( (temp_output_104_0_g54).x , (temp_output_104_0_g54).y ) / TWO_PI ) ) , length( temp_output_104_0_g54 )));
				float2 panner60_g54 = ( ( _TimeParameters.x * (temp_output_73_0_g54).y ) * float2( 0,1 ) + appendResult93_g54);
				float2 appendResult90_g54 = (float2(( (panner59_g54).x + ( (appendResult107_g54).y * _DistortionUVSwirl ) ) , (panner60_g54).y));
				float randomUV288 = IN.ase_texcoord2.z;
				float4 temp_output_208_0 = ( tex2D( _DistortionTexture, ( ( _DistortionUVScale * appendResult90_g54 ) + randomUV288 ) ) * _DistortionMultiply );
				float2 temp_output_73_0_g56 = _Lines02UVPanSpeed;
				float2 texCoord68_g56 = IN.ase_texcoord1.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g56 = ( texCoord68_g56 - float2( 1,1 ) );
				float2 appendResult93_g56 = (float2(frac( ( atan2( (temp_output_86_0_g56).x , (temp_output_86_0_g56).y ) / TWO_PI ) ) , length( temp_output_86_0_g56 )));
				float2 panner59_g56 = ( ( (temp_output_73_0_g56).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g56);
				float2 texCoord103_g56 = IN.ase_texcoord1.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g56 = ( texCoord103_g56 - float2( 1,1 ) );
				float2 appendResult107_g56 = (float2(frac( ( atan2( (temp_output_104_0_g56).x , (temp_output_104_0_g56).y ) / TWO_PI ) ) , length( temp_output_104_0_g56 )));
				float2 panner60_g56 = ( ( _TimeParameters.x * (temp_output_73_0_g56).y ) * float2( 0,1 ) + appendResult93_g56);
				float2 appendResult90_g56 = (float2(( (panner59_g56).x + ( (appendResult107_g56).y * _Lines02UVSwirl ) ) , (panner60_g56).y));
				float temp_output_275_0 = (( temp_output_220_100 + -0.45 )).y;
				float temp_output_280_0 = saturate( ( ( ( tex2D( _LinesTexture, ( float4( temp_output_234_0, 0.0 , 0.0 ) + temp_output_208_0 ).rg ).g * tex2D( _LinesTexture, ( ( float4( ( 1.0 - ( _Lines02UVScale * appendResult90_g56 ) ), 0.0 , 0.0 ) + temp_output_208_0 ) + randomUV288 ).rg ).g ) * pow( temp_output_275_0 , _MaskPower ) ) * _LinesMultiply ) );
				float temp_output_118_0 = ( ( saturate( IN.ase_texcoord1.z ) * temp_output_280_0 ) * _OpacityMultiply );
				

				surfaceDescription.Alpha = saturate( temp_output_118_0 );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.positionCS.xyz, unity_LODFade.x );
				#endif

				float3 normalWS = IN.normalWS;

				return half4(NormalizeNormalPerPixel(normalWS), 0.0);
			}

			ENDHLSL
		}

	
	}
	
	CustomEditor "UnityEditor.ShaderGraphUnlitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.CommentaryNode;286;-7648.9,431.9934;Inherit;False;548;257.3334;Random UV;2;288;287;Random UV;0,0,0,1;0;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;287;-7598.9,481.9934;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;196;-5120,-256;Inherit;False;Property;_DistortionUVScale;Distortion UV Scale;9;0;Create;True;0;0;0;False;0;False;2,0.1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;203;-5120,-128;Inherit;False;Property;_DistortionUVPanSpeed;Distortion UV Pan Speed;10;0;Create;True;0;0;0;False;0;False;0.1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;222;-5120,-384;Inherit;False;Property;_DistortionUVSwirl;Distortion UV Swirl;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;288;-7342.9,481.9934;Inherit;False;randomUV;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;219;-4480,-384;Inherit;False;SH_F_Vefects_UV_Radial_Distortion;-1;;54;a5b4adfbe5eabfa4f9c38d170d3dddde;0;4;120;FLOAT;0;False;98;FLOAT2;1,1;False;73;FLOAT2;1,1;False;66;FLOAT2;0,0;False;1;FLOAT2;100
Node;AmplifyShaderEditor.GetLocalVarNode;290;-4144.645,-260.6795;Inherit;False;288;randomUV;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;221;-5888,256;Inherit;False;Property;_Lines01UVSwirl;Lines 01 UV Swirl;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;201;-5888,384;Inherit;False;Property;_Lines01UVScale;Lines 01 UV Scale;13;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;200;-5888,512;Inherit;False;Property;_Lines01UVPanSpeed;Lines 01 UV Pan Speed;16;0;Create;True;0;0;0;False;0;False;0,0;0.2,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;178;-5120,-640;Inherit;True;Property;_DistortionTexture;Distortion Texture;8;0;Create;True;0;0;0;False;3;Space(33);Header(Distortion);Space(13);False;96dcce1503826c54480ca5fa7f3190a2;96dcce1503826c54480ca5fa7f3190a2;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.Vector2Node;229;-5888,1024;Inherit;False;Property;_Lines02UVPanSpeed;Lines 02 UV Pan Speed;15;0;Create;True;0;0;0;False;0;False;0.1,-2;0.2,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;230;-5888,768;Inherit;False;Property;_Lines02UVSwirl;Lines 02 UV Swirl;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;228;-5888,896;Inherit;False;Property;_Lines02UVScale;Lines 02 UV Scale;14;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;289;-4160,-384;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;220;-5504,256;Inherit;False;SH_F_Vefects_UV_Radial_Distortion;-1;;55;a5b4adfbe5eabfa4f9c38d170d3dddde;0;4;120;FLOAT;0;False;98;FLOAT2;1,1;False;73;FLOAT2;1,1;False;66;FLOAT2;0,0;False;1;FLOAT2;100
Node;AmplifyShaderEditor.SamplerNode;205;-4480,-640;Inherit;True;Property;_TextureSample1;Texture Sample 1;26;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;202;-3968,-768;Inherit;False;Property;_DistortionMultiply;Distortion Multiply;19;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;231;-5504,768;Inherit;False;SH_F_Vefects_UV_Radial_Distortion;-1;;56;a5b4adfbe5eabfa4f9c38d170d3dddde;0;4;120;FLOAT;0;False;98;FLOAT2;1,1;False;73;FLOAT2;1,1;False;66;FLOAT2;0,0;False;1;FLOAT2;100
Node;AmplifyShaderEditor.Vector2Node;239;-4960,96;Inherit;False;Property;_Vector4;Vector 4;25;0;Create;True;0;0;0;False;0;False;0,0.5;0,-2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;281;-4608,128;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;-3968,-640;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;282;-4864,768;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;234;-4736,256;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;285;-4960,608;Inherit;False;Constant;_Float8;Float 8;30;0;Create;True;0;0;0;False;0;False;-0.45;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;232;-4224,768;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;292;-4019.856,873.1793;Inherit;False;288;randomUV;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;207;-3968,256;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;225;-3712,0;Inherit;True;Property;_LinesTexture;Lines Texture;7;0;Create;True;0;0;0;False;3;Space(33);Header(Lines);Space(13);False;f12e661041b56834f941d3029c784063;f12e661041b56834f941d3029c784063;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleAddOpNode;283;-4784,560;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;291;-4016,768;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;227;-3712,768;Inherit;True;Property;_TextureSample2;Texture Sample 0;21;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;226;-3712,256;Inherit;True;Property;_TextureSample0;Texture Sample 0;21;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;275;-4608,560;Inherit;True;False;True;True;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;242;-3328,640;Inherit;False;Property;_MaskPower;Mask Power;27;0;Create;True;0;0;0;False;0;False;3;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;233;-3328,256;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;241;-3072,512;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;243;-2560,256;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;277;-2176,640;Inherit;False;Property;_LinesMultiply;Lines Multiply;28;0;Create;True;0;0;0;False;0;False;20;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;110;-2048,-128;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;276;-2176,256;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;279;-1792,128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;280;-1920,256;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;119;-1152,384;Inherit;False;Property;_OpacityMultiply;Opacity Multiply;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;278;-1536,256;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;-1152,256;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;49;459.26,31.68577;Inherit;False;1238;166;Auto Register Variables;5;54;53;52;51;50;Lush was here! <3;0.4872068,0.2971698,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;130;-1520,1024;Inherit;False;Constant;_Float6;Float 6;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;267;-1264,1024;Inherit;False;SH_F_Vefects_VFX_SDF_Box_UV;0;;93;7b24bf4a1857f98439a6a786a080fdc7;0;1;7;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;135;-624,1152;Inherit;False;Property;_EdgeMaskPower;Edge Mask Power;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;133;-880,1024;Inherit;False;False;True;False;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;131;-624,1024;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;136;-368,1152;Inherit;False;Property;_EdgeMaskMultiply;Edge Mask Multiply;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;-368,1024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;113;-1536,-128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-1280,-256;Inherit;False;Property;_OpacitySSSmoothness;Opacity SS Smoothness;6;0;Create;True;0;0;0;False;0;False;0.25;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;125;-96,1024;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;122;-1280,-384;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;115;-1280,-512;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;121;-896,-512;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;507.2599,79.68589;Inherit;False;Property;_Cull1;Cull;20;0;Create;True;0;0;0;True;3;Space(13);Header(AR);Space(13);False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;763.2599,79.68589;Inherit;False;Property;_Src1;Src;21;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;1019.26,79.68589;Inherit;False;Property;_Dst1;Dst;22;0;Create;True;0;0;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;1531.26,79.68589;Inherit;False;Property;_ZTest1;ZTest;26;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;1275.26,79.68589;Inherit;False;Property;_ZWrite1;ZWrite;23;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;117;-384,256;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;224;-1024,-1152;Inherit;False;Property;_Color;Color;24;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;245;-7856,1840;Inherit;False;Property;_Lines01EdgeOffset;Lines 01 Edge Offset;12;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;250;-6960,1968;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;249;-6960,1840;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;251;-6816,2048;Inherit;False;Constant;_Float7;Float 7;28;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;244;-6576,1840;Inherit;False;SH_F_Vefects_VFX_UVControls;-1;;96;646903c10e8abfe49bda8ebf1a207339;0;6;1;FLOAT2;0,0;False;4;FLOAT;0;False;7;FLOAT;0;False;9;FLOAT;0;False;11;FLOAT;0;False;16;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;246;-7216,1840;Inherit;False;True;False;False;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;248;-7216,1968;Inherit;True;False;True;False;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;268;-7600,1840;Inherit;False;SH_F_Vefects_VFX_SDF_Box_UV;0;;97;7b24bf4a1857f98439a6a786a080fdc7;0;1;7;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;114;-768,560;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;-656,16;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;120;-912,16;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;237;-4320,320;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;236;-4496,320;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;235;-4608,304;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.FractNode;240;-3200,512;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;0,0;Float;False;True;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;Vefects/SH_Vefects_URP_VFX_Vignette_Speed_01;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;0;True;_Cull1;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;True;True;2;5;True;_Src1;10;True;_Dst1;1;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;True;_ZWrite1;True;3;True;_ZTest1;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;;0;0;Standard;21;Surface;1;638255503152939265;  Blend;0;0;Two Sided;1;0;Forward Only;0;0;Cast Shadows;0;638500290705344932;  Use Shadow Threshold;0;0;GPU Instancing;1;0;LOD CrossFade;0;0;Built-in Fog;0;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Vertex Position,InvertActionOnDeselection;1;0;0;10;False;True;False;True;False;False;True;True;True;False;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;5;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;6;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;7;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;8;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;9;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
WireConnection;288;0;287;3
WireConnection;219;120;222;0
WireConnection;219;98;196;0
WireConnection;219;73;203;0
WireConnection;289;0;219;100
WireConnection;289;1;290;0
WireConnection;220;120;221;0
WireConnection;220;98;201;0
WireConnection;220;73;200;0
WireConnection;205;0;178;0
WireConnection;205;1;289;0
WireConnection;231;120;230;0
WireConnection;231;98;228;0
WireConnection;231;73;229;0
WireConnection;281;0;220;100
WireConnection;208;0;205;0
WireConnection;208;1;202;0
WireConnection;282;0;231;100
WireConnection;234;0;281;0
WireConnection;234;1;239;0
WireConnection;232;0;282;0
WireConnection;232;1;208;0
WireConnection;207;0;234;0
WireConnection;207;1;208;0
WireConnection;283;0;220;100
WireConnection;283;1;285;0
WireConnection;291;0;232;0
WireConnection;291;1;292;0
WireConnection;227;0;225;0
WireConnection;227;1;291;0
WireConnection;226;0;225;0
WireConnection;226;1;207;0
WireConnection;275;0;283;0
WireConnection;233;0;226;2
WireConnection;233;1;227;2
WireConnection;241;0;275;0
WireConnection;241;1;242;0
WireConnection;243;0;233;0
WireConnection;243;1;241;0
WireConnection;276;0;243;0
WireConnection;276;1;277;0
WireConnection;279;0;110;3
WireConnection;280;0;276;0
WireConnection;278;0;279;0
WireConnection;278;1;280;0
WireConnection;118;0;278;0
WireConnection;118;1;119;0
WireConnection;267;7;130;0
WireConnection;133;0;267;0
WireConnection;131;0;133;0
WireConnection;131;1;135;0
WireConnection;132;0;131;0
WireConnection;132;1;136;0
WireConnection;113;0;110;3
WireConnection;125;0;280;0
WireConnection;125;1;132;0
WireConnection;122;0;113;0
WireConnection;122;1;140;0
WireConnection;115;0;114;0
WireConnection;115;1;113;0
WireConnection;115;2;122;0
WireConnection;121;0;115;0
WireConnection;117;0;118;0
WireConnection;250;0;248;0
WireConnection;249;0;246;0
WireConnection;249;1;250;0
WireConnection;244;1;249;0
WireConnection;244;4;251;0
WireConnection;244;7;251;0
WireConnection;246;0;268;0
WireConnection;248;0;268;0
WireConnection;268;7;245;0
WireConnection;114;0;125;0
WireConnection;116;0;121;0
WireConnection;116;1;120;0
WireConnection;120;0;118;0
WireConnection;237;0;235;0
WireConnection;237;1;236;0
WireConnection;236;0;235;1
WireConnection;235;0;234;0
WireConnection;240;0;275;0
WireConnection;1;2;224;0
WireConnection;1;3;117;0
ASEEND*/
//CHKSM=F54820EA066BD18836BEDF331E9AAF61FE573A11