// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_URP_VFX_Vignette_Burning_01"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		_FireEmissive("Fire Emissive", Float) = 1
		[Space(33)][Header(LUT)][Space(13)]_LUT("LUT", 2D) = "white" {}
		_LUTRange("LUT Range", Float) = 1
		_LUTOffset("LUT Offset", Float) = 0
		_LUTPan("LUT Pan", Float) = 0
		_EdgeMaskPower("Edge Mask Power", Float) = 1
		_EdgeMaskMultiply("Edge Mask Multiply", Float) = 1
		_EdgeMaskAdd("Edge Mask Add", Float) = 0.3
		_EdgeMaskOffset("Edge Mask Offset", Float) = 0
		[Space(33)][Header(Fire)][Space(13)]_FireTexture("Fire Texture", 2D) = "white" {}
		_FireUVScale("Fire UV Scale", Vector) = (2,0.1,0,0)
		_FireUVPanSpeed("Fire UV Pan Speed", Vector) = (0.1,1,0,0)
		_FireUVSwirl("Fire UV Swirl", Float) = 0.05
		_FireSecondaryUVScale("Fire Secondary UV Scale", Vector) = (4,0.2,0,0)
		_FireSecondaryUVPanSpeed("Fire Secondary UV Pan Speed", Vector) = (0.2,2,0,0)
		_FireSecondaryUVSwirl("Fire Secondary UV Swirl", Float) = 0.05
		_ErosionSmoothness("Erosion Smoothness", Float) = 0.5
		[Space(13)][Header(AR)][Space(13)]_Cull1("Cull", Float) = 2
		_Src1("Src", Float) = 5
		_Dst1("Dst", Float) = 10
		_ZWrite1("ZWrite", Float) = 0
		_ZTest1("ZTest", Float) = 2


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
			float2 _FireUVScale;
			float2 _FireUVPanSpeed;
			float2 _FireSecondaryUVPanSpeed;
			float2 _FireSecondaryUVScale;
			float _Cull1;
			float _LUTRange;
			float _EdgeMaskAdd;
			float _EdgeMaskMultiply;
			float _EdgeMaskPower;
			float _EdgeMaskOffset;
			float _FireSecondaryUVSwirl;
			float _FireUVSwirl;
			float _ErosionSmoothness;
			float _LUTPan;
			float _ZWrite1;
			float _ZTest1;
			float _Dst1;
			float _Src1;
			float _LUTOffset;
			float _FireEmissive;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _LUT;
			sampler2D _FireTexture;


			
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

				float2 temp_cast_0 = (_LUTPan).xx;
				float temp_output_265_0 = ( 1.0 - IN.ase_texcoord3.z );
				float2 temp_output_73_0_g61 = _FireUVPanSpeed;
				float2 texCoord68_g61 = IN.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g61 = ( texCoord68_g61 - float2( 1,1 ) );
				float2 appendResult93_g61 = (float2(frac( ( atan2( (temp_output_86_0_g61).x , (temp_output_86_0_g61).y ) / TWO_PI ) ) , length( temp_output_86_0_g61 )));
				float2 panner59_g61 = ( ( (temp_output_73_0_g61).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g61);
				float2 texCoord103_g61 = IN.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g61 = ( texCoord103_g61 - float2( 1,1 ) );
				float2 appendResult107_g61 = (float2(frac( ( atan2( (temp_output_104_0_g61).x , (temp_output_104_0_g61).y ) / TWO_PI ) ) , length( temp_output_104_0_g61 )));
				float2 panner60_g61 = ( ( _TimeParameters.x * (temp_output_73_0_g61).y ) * float2( 0,1 ) + appendResult93_g61);
				float2 appendResult90_g61 = (float2(( (panner59_g61).x + ( (appendResult107_g61).y * _FireUVSwirl ) ) , (panner60_g61).y));
				float randomUV282 = IN.ase_texcoord4.z;
				float4 tex2DNode205 = tex2D( _FireTexture, ( ( _FireUVScale * appendResult90_g61 ) + randomUV282 ) );
				float2 temp_output_73_0_g62 = _FireSecondaryUVPanSpeed;
				float2 texCoord68_g62 = IN.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g62 = ( texCoord68_g62 - float2( 1,1 ) );
				float2 appendResult93_g62 = (float2(frac( ( atan2( (temp_output_86_0_g62).x , (temp_output_86_0_g62).y ) / TWO_PI ) ) , length( temp_output_86_0_g62 )));
				float2 panner59_g62 = ( ( (temp_output_73_0_g62).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g62);
				float2 texCoord103_g62 = IN.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g62 = ( texCoord103_g62 - float2( 1,1 ) );
				float2 appendResult107_g62 = (float2(frac( ( atan2( (temp_output_104_0_g62).x , (temp_output_104_0_g62).y ) / TWO_PI ) ) , length( temp_output_104_0_g62 )));
				float2 panner60_g62 = ( ( _TimeParameters.x * (temp_output_73_0_g62).y ) * float2( 0,1 ) + appendResult93_g62);
				float2 appendResult90_g62 = (float2(( (panner59_g62).x + ( (appendResult107_g62).y * _FireSecondaryUVSwirl ) ) , (panner60_g62).y));
				float4 tex2DNode248 = tex2D( _FireTexture, ( ( _FireSecondaryUVScale * appendResult90_g62 ) + randomUV282 ) );
				float ifLocalVar255 = 0;
				if( tex2DNode205.g > 0.5 )
				ifLocalVar255 = ( 1.0 - ( ( 1.0 - tex2DNode205.g ) * ( 1.0 - tex2DNode248.g ) ) );
				else if( tex2DNode205.g < 0.5 )
				ifLocalVar255 = ( tex2DNode205.g * tex2DNode248.g );
				float2 texCoord27_g60 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_1 = (1.0).xx;
				float2 temp_output_26_0_g60 = ( texCoord27_g60 - temp_cast_1 );
				float temp_output_13_0_g60 = frac( ( atan2( (temp_output_26_0_g60).x , (temp_output_26_0_g60).y ) / 6.283 ) );
				float2 temp_output_17_0_g60 = ( temp_output_26_0_g60 * temp_output_26_0_g60 );
				float temp_output_12_0_g60 = sqrt( ( (temp_output_17_0_g60).x + (temp_output_17_0_g60).y ) );
				float2 appendResult5_g60 = (float2(temp_output_13_0_g60 , temp_output_12_0_g60));
				float2 texCoord11_g58 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_2 = (0.0).xx;
				float2 appendResult3_g58 = (float2((appendResult5_g60).x , ( ( distance( max( ( abs( ( texCoord11_g58 + -0.5 ) ) - ( float2( 0.15,0.15 ) * 0.5 ) ) , float2( 0,0 ) ) , temp_cast_2 ) + _EdgeMaskOffset ) * 1.5 )));
				float smoothstepResult268 = smoothstep( temp_output_265_0 , ( temp_output_265_0 + _ErosionSmoothness ) , saturate( ( saturate( ifLocalVar255 ) - saturate( ( saturate( ( saturate( pow( ( 1.0 - (appendResult3_g58).y ) , _EdgeMaskPower ) ) * _EdgeMaskMultiply ) ) + _EdgeMaskAdd ) ) ) ));
				float temp_output_270_0 = saturate( smoothstepResult268 );
				float2 temp_cast_3 = (( ( temp_output_270_0 * _LUTRange ) + _LUTOffset )).xx;
				float2 panner242 = ( 1.0 * _Time.y * temp_cast_0 + temp_cast_3);
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( tex2D( _LUT, panner242 ) * ( ( temp_output_270_0 + 1.0 ) * _FireEmissive ) ).rgb;
				float Alpha = temp_output_270_0;
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
			float2 _FireUVScale;
			float2 _FireUVPanSpeed;
			float2 _FireSecondaryUVPanSpeed;
			float2 _FireSecondaryUVScale;
			float _Cull1;
			float _LUTRange;
			float _EdgeMaskAdd;
			float _EdgeMaskMultiply;
			float _EdgeMaskPower;
			float _EdgeMaskOffset;
			float _FireSecondaryUVSwirl;
			float _FireUVSwirl;
			float _ErosionSmoothness;
			float _LUTPan;
			float _ZWrite1;
			float _ZTest1;
			float _Dst1;
			float _Src1;
			float _LUTOffset;
			float _FireEmissive;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _FireTexture;


			
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

				float temp_output_265_0 = ( 1.0 - IN.ase_texcoord2.z );
				float2 temp_output_73_0_g61 = _FireUVPanSpeed;
				float2 texCoord68_g61 = IN.ase_texcoord2.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g61 = ( texCoord68_g61 - float2( 1,1 ) );
				float2 appendResult93_g61 = (float2(frac( ( atan2( (temp_output_86_0_g61).x , (temp_output_86_0_g61).y ) / TWO_PI ) ) , length( temp_output_86_0_g61 )));
				float2 panner59_g61 = ( ( (temp_output_73_0_g61).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g61);
				float2 texCoord103_g61 = IN.ase_texcoord2.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g61 = ( texCoord103_g61 - float2( 1,1 ) );
				float2 appendResult107_g61 = (float2(frac( ( atan2( (temp_output_104_0_g61).x , (temp_output_104_0_g61).y ) / TWO_PI ) ) , length( temp_output_104_0_g61 )));
				float2 panner60_g61 = ( ( _TimeParameters.x * (temp_output_73_0_g61).y ) * float2( 0,1 ) + appendResult93_g61);
				float2 appendResult90_g61 = (float2(( (panner59_g61).x + ( (appendResult107_g61).y * _FireUVSwirl ) ) , (panner60_g61).y));
				float randomUV282 = IN.ase_texcoord3.z;
				float4 tex2DNode205 = tex2D( _FireTexture, ( ( _FireUVScale * appendResult90_g61 ) + randomUV282 ) );
				float2 temp_output_73_0_g62 = _FireSecondaryUVPanSpeed;
				float2 texCoord68_g62 = IN.ase_texcoord2.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g62 = ( texCoord68_g62 - float2( 1,1 ) );
				float2 appendResult93_g62 = (float2(frac( ( atan2( (temp_output_86_0_g62).x , (temp_output_86_0_g62).y ) / TWO_PI ) ) , length( temp_output_86_0_g62 )));
				float2 panner59_g62 = ( ( (temp_output_73_0_g62).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g62);
				float2 texCoord103_g62 = IN.ase_texcoord2.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g62 = ( texCoord103_g62 - float2( 1,1 ) );
				float2 appendResult107_g62 = (float2(frac( ( atan2( (temp_output_104_0_g62).x , (temp_output_104_0_g62).y ) / TWO_PI ) ) , length( temp_output_104_0_g62 )));
				float2 panner60_g62 = ( ( _TimeParameters.x * (temp_output_73_0_g62).y ) * float2( 0,1 ) + appendResult93_g62);
				float2 appendResult90_g62 = (float2(( (panner59_g62).x + ( (appendResult107_g62).y * _FireSecondaryUVSwirl ) ) , (panner60_g62).y));
				float4 tex2DNode248 = tex2D( _FireTexture, ( ( _FireSecondaryUVScale * appendResult90_g62 ) + randomUV282 ) );
				float ifLocalVar255 = 0;
				if( tex2DNode205.g > 0.5 )
				ifLocalVar255 = ( 1.0 - ( ( 1.0 - tex2DNode205.g ) * ( 1.0 - tex2DNode248.g ) ) );
				else if( tex2DNode205.g < 0.5 )
				ifLocalVar255 = ( tex2DNode205.g * tex2DNode248.g );
				float2 texCoord27_g60 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_0 = (1.0).xx;
				float2 temp_output_26_0_g60 = ( texCoord27_g60 - temp_cast_0 );
				float temp_output_13_0_g60 = frac( ( atan2( (temp_output_26_0_g60).x , (temp_output_26_0_g60).y ) / 6.283 ) );
				float2 temp_output_17_0_g60 = ( temp_output_26_0_g60 * temp_output_26_0_g60 );
				float temp_output_12_0_g60 = sqrt( ( (temp_output_17_0_g60).x + (temp_output_17_0_g60).y ) );
				float2 appendResult5_g60 = (float2(temp_output_13_0_g60 , temp_output_12_0_g60));
				float2 texCoord11_g58 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_1 = (0.0).xx;
				float2 appendResult3_g58 = (float2((appendResult5_g60).x , ( ( distance( max( ( abs( ( texCoord11_g58 + -0.5 ) ) - ( float2( 0.15,0.15 ) * 0.5 ) ) , float2( 0,0 ) ) , temp_cast_1 ) + _EdgeMaskOffset ) * 1.5 )));
				float smoothstepResult268 = smoothstep( temp_output_265_0 , ( temp_output_265_0 + _ErosionSmoothness ) , saturate( ( saturate( ifLocalVar255 ) - saturate( ( saturate( ( saturate( pow( ( 1.0 - (appendResult3_g58).y ) , _EdgeMaskPower ) ) * _EdgeMaskMultiply ) ) + _EdgeMaskAdd ) ) ) ));
				float temp_output_270_0 = saturate( smoothstepResult268 );
				

				float Alpha = temp_output_270_0;
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
			float2 _FireUVScale;
			float2 _FireUVPanSpeed;
			float2 _FireSecondaryUVPanSpeed;
			float2 _FireSecondaryUVScale;
			float _Cull1;
			float _LUTRange;
			float _EdgeMaskAdd;
			float _EdgeMaskMultiply;
			float _EdgeMaskPower;
			float _EdgeMaskOffset;
			float _FireSecondaryUVSwirl;
			float _FireUVSwirl;
			float _ErosionSmoothness;
			float _LUTPan;
			float _ZWrite1;
			float _ZTest1;
			float _Dst1;
			float _Src1;
			float _LUTOffset;
			float _FireEmissive;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _FireTexture;


			
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

				float temp_output_265_0 = ( 1.0 - IN.ase_texcoord.z );
				float2 temp_output_73_0_g61 = _FireUVPanSpeed;
				float2 texCoord68_g61 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g61 = ( texCoord68_g61 - float2( 1,1 ) );
				float2 appendResult93_g61 = (float2(frac( ( atan2( (temp_output_86_0_g61).x , (temp_output_86_0_g61).y ) / TWO_PI ) ) , length( temp_output_86_0_g61 )));
				float2 panner59_g61 = ( ( (temp_output_73_0_g61).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g61);
				float2 texCoord103_g61 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g61 = ( texCoord103_g61 - float2( 1,1 ) );
				float2 appendResult107_g61 = (float2(frac( ( atan2( (temp_output_104_0_g61).x , (temp_output_104_0_g61).y ) / TWO_PI ) ) , length( temp_output_104_0_g61 )));
				float2 panner60_g61 = ( ( _TimeParameters.x * (temp_output_73_0_g61).y ) * float2( 0,1 ) + appendResult93_g61);
				float2 appendResult90_g61 = (float2(( (panner59_g61).x + ( (appendResult107_g61).y * _FireUVSwirl ) ) , (panner60_g61).y));
				float randomUV282 = IN.ase_texcoord1.z;
				float4 tex2DNode205 = tex2D( _FireTexture, ( ( _FireUVScale * appendResult90_g61 ) + randomUV282 ) );
				float2 temp_output_73_0_g62 = _FireSecondaryUVPanSpeed;
				float2 texCoord68_g62 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g62 = ( texCoord68_g62 - float2( 1,1 ) );
				float2 appendResult93_g62 = (float2(frac( ( atan2( (temp_output_86_0_g62).x , (temp_output_86_0_g62).y ) / TWO_PI ) ) , length( temp_output_86_0_g62 )));
				float2 panner59_g62 = ( ( (temp_output_73_0_g62).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g62);
				float2 texCoord103_g62 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g62 = ( texCoord103_g62 - float2( 1,1 ) );
				float2 appendResult107_g62 = (float2(frac( ( atan2( (temp_output_104_0_g62).x , (temp_output_104_0_g62).y ) / TWO_PI ) ) , length( temp_output_104_0_g62 )));
				float2 panner60_g62 = ( ( _TimeParameters.x * (temp_output_73_0_g62).y ) * float2( 0,1 ) + appendResult93_g62);
				float2 appendResult90_g62 = (float2(( (panner59_g62).x + ( (appendResult107_g62).y * _FireSecondaryUVSwirl ) ) , (panner60_g62).y));
				float4 tex2DNode248 = tex2D( _FireTexture, ( ( _FireSecondaryUVScale * appendResult90_g62 ) + randomUV282 ) );
				float ifLocalVar255 = 0;
				if( tex2DNode205.g > 0.5 )
				ifLocalVar255 = ( 1.0 - ( ( 1.0 - tex2DNode205.g ) * ( 1.0 - tex2DNode248.g ) ) );
				else if( tex2DNode205.g < 0.5 )
				ifLocalVar255 = ( tex2DNode205.g * tex2DNode248.g );
				float2 texCoord27_g60 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_0 = (1.0).xx;
				float2 temp_output_26_0_g60 = ( texCoord27_g60 - temp_cast_0 );
				float temp_output_13_0_g60 = frac( ( atan2( (temp_output_26_0_g60).x , (temp_output_26_0_g60).y ) / 6.283 ) );
				float2 temp_output_17_0_g60 = ( temp_output_26_0_g60 * temp_output_26_0_g60 );
				float temp_output_12_0_g60 = sqrt( ( (temp_output_17_0_g60).x + (temp_output_17_0_g60).y ) );
				float2 appendResult5_g60 = (float2(temp_output_13_0_g60 , temp_output_12_0_g60));
				float2 texCoord11_g58 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_1 = (0.0).xx;
				float2 appendResult3_g58 = (float2((appendResult5_g60).x , ( ( distance( max( ( abs( ( texCoord11_g58 + -0.5 ) ) - ( float2( 0.15,0.15 ) * 0.5 ) ) , float2( 0,0 ) ) , temp_cast_1 ) + _EdgeMaskOffset ) * 1.5 )));
				float smoothstepResult268 = smoothstep( temp_output_265_0 , ( temp_output_265_0 + _ErosionSmoothness ) , saturate( ( saturate( ifLocalVar255 ) - saturate( ( saturate( ( saturate( pow( ( 1.0 - (appendResult3_g58).y ) , _EdgeMaskPower ) ) * _EdgeMaskMultiply ) ) + _EdgeMaskAdd ) ) ) ));
				float temp_output_270_0 = saturate( smoothstepResult268 );
				

				surfaceDescription.Alpha = temp_output_270_0;
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
			float2 _FireUVScale;
			float2 _FireUVPanSpeed;
			float2 _FireSecondaryUVPanSpeed;
			float2 _FireSecondaryUVScale;
			float _Cull1;
			float _LUTRange;
			float _EdgeMaskAdd;
			float _EdgeMaskMultiply;
			float _EdgeMaskPower;
			float _EdgeMaskOffset;
			float _FireSecondaryUVSwirl;
			float _FireUVSwirl;
			float _ErosionSmoothness;
			float _LUTPan;
			float _ZWrite1;
			float _ZTest1;
			float _Dst1;
			float _Src1;
			float _LUTOffset;
			float _FireEmissive;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _FireTexture;


			
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

				float temp_output_265_0 = ( 1.0 - IN.ase_texcoord.z );
				float2 temp_output_73_0_g61 = _FireUVPanSpeed;
				float2 texCoord68_g61 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g61 = ( texCoord68_g61 - float2( 1,1 ) );
				float2 appendResult93_g61 = (float2(frac( ( atan2( (temp_output_86_0_g61).x , (temp_output_86_0_g61).y ) / TWO_PI ) ) , length( temp_output_86_0_g61 )));
				float2 panner59_g61 = ( ( (temp_output_73_0_g61).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g61);
				float2 texCoord103_g61 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g61 = ( texCoord103_g61 - float2( 1,1 ) );
				float2 appendResult107_g61 = (float2(frac( ( atan2( (temp_output_104_0_g61).x , (temp_output_104_0_g61).y ) / TWO_PI ) ) , length( temp_output_104_0_g61 )));
				float2 panner60_g61 = ( ( _TimeParameters.x * (temp_output_73_0_g61).y ) * float2( 0,1 ) + appendResult93_g61);
				float2 appendResult90_g61 = (float2(( (panner59_g61).x + ( (appendResult107_g61).y * _FireUVSwirl ) ) , (panner60_g61).y));
				float randomUV282 = IN.ase_texcoord1.z;
				float4 tex2DNode205 = tex2D( _FireTexture, ( ( _FireUVScale * appendResult90_g61 ) + randomUV282 ) );
				float2 temp_output_73_0_g62 = _FireSecondaryUVPanSpeed;
				float2 texCoord68_g62 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g62 = ( texCoord68_g62 - float2( 1,1 ) );
				float2 appendResult93_g62 = (float2(frac( ( atan2( (temp_output_86_0_g62).x , (temp_output_86_0_g62).y ) / TWO_PI ) ) , length( temp_output_86_0_g62 )));
				float2 panner59_g62 = ( ( (temp_output_73_0_g62).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g62);
				float2 texCoord103_g62 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g62 = ( texCoord103_g62 - float2( 1,1 ) );
				float2 appendResult107_g62 = (float2(frac( ( atan2( (temp_output_104_0_g62).x , (temp_output_104_0_g62).y ) / TWO_PI ) ) , length( temp_output_104_0_g62 )));
				float2 panner60_g62 = ( ( _TimeParameters.x * (temp_output_73_0_g62).y ) * float2( 0,1 ) + appendResult93_g62);
				float2 appendResult90_g62 = (float2(( (panner59_g62).x + ( (appendResult107_g62).y * _FireSecondaryUVSwirl ) ) , (panner60_g62).y));
				float4 tex2DNode248 = tex2D( _FireTexture, ( ( _FireSecondaryUVScale * appendResult90_g62 ) + randomUV282 ) );
				float ifLocalVar255 = 0;
				if( tex2DNode205.g > 0.5 )
				ifLocalVar255 = ( 1.0 - ( ( 1.0 - tex2DNode205.g ) * ( 1.0 - tex2DNode248.g ) ) );
				else if( tex2DNode205.g < 0.5 )
				ifLocalVar255 = ( tex2DNode205.g * tex2DNode248.g );
				float2 texCoord27_g60 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_0 = (1.0).xx;
				float2 temp_output_26_0_g60 = ( texCoord27_g60 - temp_cast_0 );
				float temp_output_13_0_g60 = frac( ( atan2( (temp_output_26_0_g60).x , (temp_output_26_0_g60).y ) / 6.283 ) );
				float2 temp_output_17_0_g60 = ( temp_output_26_0_g60 * temp_output_26_0_g60 );
				float temp_output_12_0_g60 = sqrt( ( (temp_output_17_0_g60).x + (temp_output_17_0_g60).y ) );
				float2 appendResult5_g60 = (float2(temp_output_13_0_g60 , temp_output_12_0_g60));
				float2 texCoord11_g58 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_1 = (0.0).xx;
				float2 appendResult3_g58 = (float2((appendResult5_g60).x , ( ( distance( max( ( abs( ( texCoord11_g58 + -0.5 ) ) - ( float2( 0.15,0.15 ) * 0.5 ) ) , float2( 0,0 ) ) , temp_cast_1 ) + _EdgeMaskOffset ) * 1.5 )));
				float smoothstepResult268 = smoothstep( temp_output_265_0 , ( temp_output_265_0 + _ErosionSmoothness ) , saturate( ( saturate( ifLocalVar255 ) - saturate( ( saturate( ( saturate( pow( ( 1.0 - (appendResult3_g58).y ) , _EdgeMaskPower ) ) * _EdgeMaskMultiply ) ) + _EdgeMaskAdd ) ) ) ));
				float temp_output_270_0 = saturate( smoothstepResult268 );
				

				surfaceDescription.Alpha = temp_output_270_0;
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
			float2 _FireUVScale;
			float2 _FireUVPanSpeed;
			float2 _FireSecondaryUVPanSpeed;
			float2 _FireSecondaryUVScale;
			float _Cull1;
			float _LUTRange;
			float _EdgeMaskAdd;
			float _EdgeMaskMultiply;
			float _EdgeMaskPower;
			float _EdgeMaskOffset;
			float _FireSecondaryUVSwirl;
			float _FireUVSwirl;
			float _ErosionSmoothness;
			float _LUTPan;
			float _ZWrite1;
			float _ZTest1;
			float _Dst1;
			float _Src1;
			float _LUTOffset;
			float _FireEmissive;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _FireTexture;


			
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

				float temp_output_265_0 = ( 1.0 - IN.ase_texcoord1.z );
				float2 temp_output_73_0_g61 = _FireUVPanSpeed;
				float2 texCoord68_g61 = IN.ase_texcoord1.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g61 = ( texCoord68_g61 - float2( 1,1 ) );
				float2 appendResult93_g61 = (float2(frac( ( atan2( (temp_output_86_0_g61).x , (temp_output_86_0_g61).y ) / TWO_PI ) ) , length( temp_output_86_0_g61 )));
				float2 panner59_g61 = ( ( (temp_output_73_0_g61).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g61);
				float2 texCoord103_g61 = IN.ase_texcoord1.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g61 = ( texCoord103_g61 - float2( 1,1 ) );
				float2 appendResult107_g61 = (float2(frac( ( atan2( (temp_output_104_0_g61).x , (temp_output_104_0_g61).y ) / TWO_PI ) ) , length( temp_output_104_0_g61 )));
				float2 panner60_g61 = ( ( _TimeParameters.x * (temp_output_73_0_g61).y ) * float2( 0,1 ) + appendResult93_g61);
				float2 appendResult90_g61 = (float2(( (panner59_g61).x + ( (appendResult107_g61).y * _FireUVSwirl ) ) , (panner60_g61).y));
				float randomUV282 = IN.ase_texcoord2.z;
				float4 tex2DNode205 = tex2D( _FireTexture, ( ( _FireUVScale * appendResult90_g61 ) + randomUV282 ) );
				float2 temp_output_73_0_g62 = _FireSecondaryUVPanSpeed;
				float2 texCoord68_g62 = IN.ase_texcoord1.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_86_0_g62 = ( texCoord68_g62 - float2( 1,1 ) );
				float2 appendResult93_g62 = (float2(frac( ( atan2( (temp_output_86_0_g62).x , (temp_output_86_0_g62).y ) / TWO_PI ) ) , length( temp_output_86_0_g62 )));
				float2 panner59_g62 = ( ( (temp_output_73_0_g62).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult93_g62);
				float2 texCoord103_g62 = IN.ase_texcoord1.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_104_0_g62 = ( texCoord103_g62 - float2( 1,1 ) );
				float2 appendResult107_g62 = (float2(frac( ( atan2( (temp_output_104_0_g62).x , (temp_output_104_0_g62).y ) / TWO_PI ) ) , length( temp_output_104_0_g62 )));
				float2 panner60_g62 = ( ( _TimeParameters.x * (temp_output_73_0_g62).y ) * float2( 0,1 ) + appendResult93_g62);
				float2 appendResult90_g62 = (float2(( (panner59_g62).x + ( (appendResult107_g62).y * _FireSecondaryUVSwirl ) ) , (panner60_g62).y));
				float4 tex2DNode248 = tex2D( _FireTexture, ( ( _FireSecondaryUVScale * appendResult90_g62 ) + randomUV282 ) );
				float ifLocalVar255 = 0;
				if( tex2DNode205.g > 0.5 )
				ifLocalVar255 = ( 1.0 - ( ( 1.0 - tex2DNode205.g ) * ( 1.0 - tex2DNode248.g ) ) );
				else if( tex2DNode205.g < 0.5 )
				ifLocalVar255 = ( tex2DNode205.g * tex2DNode248.g );
				float2 texCoord27_g60 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_0 = (1.0).xx;
				float2 temp_output_26_0_g60 = ( texCoord27_g60 - temp_cast_0 );
				float temp_output_13_0_g60 = frac( ( atan2( (temp_output_26_0_g60).x , (temp_output_26_0_g60).y ) / 6.283 ) );
				float2 temp_output_17_0_g60 = ( temp_output_26_0_g60 * temp_output_26_0_g60 );
				float temp_output_12_0_g60 = sqrt( ( (temp_output_17_0_g60).x + (temp_output_17_0_g60).y ) );
				float2 appendResult5_g60 = (float2(temp_output_13_0_g60 , temp_output_12_0_g60));
				float2 texCoord11_g58 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_1 = (0.0).xx;
				float2 appendResult3_g58 = (float2((appendResult5_g60).x , ( ( distance( max( ( abs( ( texCoord11_g58 + -0.5 ) ) - ( float2( 0.15,0.15 ) * 0.5 ) ) , float2( 0,0 ) ) , temp_cast_1 ) + _EdgeMaskOffset ) * 1.5 )));
				float smoothstepResult268 = smoothstep( temp_output_265_0 , ( temp_output_265_0 + _ErosionSmoothness ) , saturate( ( saturate( ifLocalVar255 ) - saturate( ( saturate( ( saturate( pow( ( 1.0 - (appendResult3_g58).y ) , _EdgeMaskPower ) ) * _EdgeMaskMultiply ) ) + _EdgeMaskAdd ) ) ) ));
				float temp_output_270_0 = saturate( smoothstepResult268 );
				

				surfaceDescription.Alpha = temp_output_270_0;
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
Node;AmplifyShaderEditor.CommentaryNode;281;-6016,-640;Inherit;False;548;257.3334;Random UV;2;283;282;Random UV;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;130;-3456,1024;Inherit;False;Property;_EdgeMaskOffset;Edge Mask Offset;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;283;-5968,-592;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;126;-3200,1024;Inherit;False;SH_F_Vefects_VFX_SDF_Box_UV;0;;58;7b24bf4a1857f98439a6a786a080fdc7;0;1;7;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;222;-4736,-384;Inherit;False;Property;_FireUVSwirl;Fire UV Swirl;15;0;Create;True;0;0;0;False;0;False;0.05;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;196;-4736,-256;Inherit;False;Property;_FireUVScale;Fire UV Scale;13;0;Create;True;0;0;0;False;0;False;2,0.1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;203;-4736,-128;Inherit;False;Property;_FireUVPanSpeed;Fire UV Pan Speed;14;0;Create;True;0;0;0;False;0;False;0.1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;244;-4736,384;Inherit;False;Property;_FireSecondaryUVSwirl;Fire Secondary UV Swirl;18;0;Create;True;0;0;0;False;0;False;0.05;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;245;-4736,512;Inherit;False;Property;_FireSecondaryUVScale;Fire Secondary UV Scale;16;0;Create;True;0;0;0;False;0;False;4,0.2;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;246;-4736,640;Inherit;False;Property;_FireSecondaryUVPanSpeed;Fire Secondary UV Pan Speed;17;0;Create;True;0;0;0;False;0;False;0.2,2;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;282;-5712,-592;Inherit;False;randomUV;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;133;-2816,1024;Inherit;False;False;True;False;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;247;-4096,384;Inherit;False;SH_F_Vefects_UV_Radial_Distortion;-1;;62;a5b4adfbe5eabfa4f9c38d170d3dddde;0;4;120;FLOAT;0;False;98;FLOAT2;1,1;False;73;FLOAT2;1,1;False;66;FLOAT2;0,0;False;1;FLOAT2;100
Node;AmplifyShaderEditor.FunctionNode;219;-4096,-384;Inherit;False;SH_F_Vefects_UV_Radial_Distortion;-1;;61;a5b4adfbe5eabfa4f9c38d170d3dddde;0;4;120;FLOAT;0;False;98;FLOAT2;1,1;False;73;FLOAT2;1,1;False;66;FLOAT2;0,0;False;1;FLOAT2;100
Node;AmplifyShaderEditor.GetLocalVarNode;285;-3792,-256;Inherit;False;282;randomUV;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;287;-3760,480;Inherit;False;282;randomUV;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;178;-4736,-1024;Inherit;True;Property;_FireTexture;Fire Texture;12;0;Create;True;0;0;0;False;3;Space(33);Header(Fire);Space(13);False;b269f0779cfd3e14eaedadbe43aa8795;b269f0779cfd3e14eaedadbe43aa8795;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.OneMinusNode;258;-2592,1024;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;135;-2816,1152;Inherit;False;Property;_EdgeMaskPower;Edge Mask Power;8;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;284;-3792,-384;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;286;-3760,384;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;205;-4096,-640;Inherit;True;Property;_TextureSample1;Texture Sample 1;26;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;248;-4096,128;Inherit;True;Property;_TextureSample2;Texture Sample 1;26;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;131;-2432,1024;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;250;-3584,128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;249;-3584,-640;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;277;-2304,1024;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;280;-2176,1152;Inherit;False;Property;_EdgeMaskMultiply;Edge Mask Multiply;9;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;251;-3328,-640;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;278;-2176,1024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;252;-3072,-640;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;257;-3328,128;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;276;-2816,-384;Inherit;False;Constant;_Float6;Float 6;23;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;260;-1856,1152;Inherit;False;Property;_EdgeMaskAdd;Edge Mask Add;10;0;Create;True;0;0;0;False;0;False;0.3;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;279;-2048,1024;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;255;-2816,-256;Inherit;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;259;-1664,1024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;110;-1536,-128;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;263;-1792,640;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;262;-2176,512;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;265;-1280,-128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;275;-1536,256;Inherit;False;Property;_ErosionSmoothness;Erosion Smoothness;19;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;261;-1792,512;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;264;-1536,512;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;267;-1280,128;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;49;459.26,31.68577;Inherit;False;1238;166;Auto Register Variables;5;54;53;52;51;50;Lush was here! <3;0.4872068,0.2971698,1,1;0;0
Node;AmplifyShaderEditor.SmoothstepOpNode;268;-1024,0;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;507.2599,79.68589;Inherit;False;Property;_Cull1;Cull;20;0;Create;True;0;0;0;True;3;Space(13);Header(AR);Space(13);False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;763.2599,79.68589;Inherit;False;Property;_Src1;Src;21;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;1019.26,79.68589;Inherit;False;Property;_Dst1;Dst;22;0;Create;True;0;0;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;1531.26,79.68589;Inherit;False;Property;_ZTest1;ZTest;24;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;1275.26,79.68589;Inherit;False;Property;_ZWrite1;ZWrite;23;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;242;-1024,-640;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;243;-1024,-512;Inherit;False;Property;_LUTPan;LUT Pan;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;109;-768,-640;Inherit;True;Property;_LUT;LUT;4;0;Create;True;0;0;0;False;3;Space(33);Header(LUT);Space(13);False;-1;3dbd74d5a41bb74428dcc82a97f5576f;3dbd74d5a41bb74428dcc82a97f5576f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;238;-1280,-640;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;241;-1280,-512;Inherit;False;Property;_LUTOffset;LUT Offset;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;239;-1536,-640;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;240;-1536,-512;Inherit;False;Property;_LUTRange;LUT Range;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;271;-384,-640;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;272;-384,128;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;273;-384,256;Inherit;False;Property;_FireEmissive;Fire Emissive;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;269;-512,128;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;270;-768,0;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;0,0;Float;False;True;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;Vefects/SH_Vefects_URP_VFX_Vignette_Burning_01;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;0;True;_Cull1;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;True;True;2;5;True;_Src1;10;True;_Dst1;1;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;True;_ZWrite1;True;3;True;_ZTest1;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;;0;0;Standard;21;Surface;1;638255503152939265;  Blend;0;0;Two Sided;1;0;Forward Only;0;0;Cast Shadows;0;638500290705344932;  Use Shadow Threshold;0;0;GPU Instancing;1;0;LOD CrossFade;0;0;Built-in Fog;0;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Vertex Position,InvertActionOnDeselection;1;0;0;10;False;True;False;True;False;False;True;True;True;False;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;5;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;6;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;7;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;8;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;9;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
WireConnection;126;7;130;0
WireConnection;282;0;283;3
WireConnection;133;0;126;0
WireConnection;247;120;244;0
WireConnection;247;98;245;0
WireConnection;247;73;246;0
WireConnection;219;120;222;0
WireConnection;219;98;196;0
WireConnection;219;73;203;0
WireConnection;258;0;133;0
WireConnection;284;0;219;100
WireConnection;284;1;285;0
WireConnection;286;0;247;100
WireConnection;286;1;287;0
WireConnection;205;0;178;0
WireConnection;205;1;284;0
WireConnection;248;0;178;0
WireConnection;248;1;286;0
WireConnection;131;0;258;0
WireConnection;131;1;135;0
WireConnection;250;0;248;2
WireConnection;249;0;205;2
WireConnection;277;0;131;0
WireConnection;251;0;249;0
WireConnection;251;1;250;0
WireConnection;278;0;277;0
WireConnection;278;1;280;0
WireConnection;252;0;251;0
WireConnection;257;0;205;2
WireConnection;257;1;248;2
WireConnection;279;0;278;0
WireConnection;255;0;205;2
WireConnection;255;1;276;0
WireConnection;255;2;252;0
WireConnection;255;4;257;0
WireConnection;259;0;279;0
WireConnection;259;1;260;0
WireConnection;263;0;259;0
WireConnection;262;0;255;0
WireConnection;265;0;110;3
WireConnection;261;0;262;0
WireConnection;261;1;263;0
WireConnection;264;0;261;0
WireConnection;267;0;265;0
WireConnection;267;1;275;0
WireConnection;268;0;264;0
WireConnection;268;1;265;0
WireConnection;268;2;267;0
WireConnection;242;0;238;0
WireConnection;242;2;243;0
WireConnection;109;1;242;0
WireConnection;238;0;239;0
WireConnection;238;1;241;0
WireConnection;239;0;270;0
WireConnection;239;1;240;0
WireConnection;271;0;109;0
WireConnection;271;1;272;0
WireConnection;272;0;269;0
WireConnection;272;1;273;0
WireConnection;269;0;270;0
WireConnection;270;0;268;0
WireConnection;1;2;271;0
WireConnection;1;3;270;0
ASEEND*/
//CHKSM=C1F57C41368CD7A9A796960118499BB119AB7401