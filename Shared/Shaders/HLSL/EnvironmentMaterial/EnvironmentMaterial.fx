//-----------------------------------------------------------------------------
// EnvironmentMaterial.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// DIFF		=	Diffuse Map
// LIT		=	Lighting
// FRES		=	Fresnel
// ENV		=	Environment Map
// NORMAL	=	Normals

#include "..\Helpers.fxh"

cbuffer Matrices : register(b0)
{
	float4x4	WorldViewProj			: packoffset(c0);
	float4x4	World					: packoffset(c4);
	float4x4	WorldInverseTranspose	: packoffset(c8);
};

cbuffer Parameters : register(b1)
{
	float3		CameraPosition			: packoffset(c0.x);
	float		FresnelFactor			: packoffset(c0.w);
	float3		DiffuseColor			: packoffset(c1.x);
	float		EnvironmentAmount		: packoffset(c1.w);
	float3		AmbientColor			: packoffset(c2.x);
}

Texture2D		Diffuse 				: register(t0);
TextureCube		CubeTexture				: register(t1);

SamplerState	TextureSampler 			: register(s0);
sampler			TextureCubeSampler		: register(s1);

Texture2D LightingTexture 				: register(t2);
sampler LightingTextureSampler =
sampler_state
{
	Texture = <DiffuseTexture>;
	MipFilter = POINT;
	MinFilter = POINT;
	MagFilter = POINT;
};

Texture2D Gbuffer : register(t3);

sampler GbufferSampler =
sampler_state
{
	Texture = <GBuffer>;
	MipFilter = POINT;
	MinFilter = POINT;
	MagFilter = POINT;
};

#if NORMAL
Texture2D NormalTexture : register(t4);
#endif

// STRUCTS
struct VS_IN
{
	float4 Position		: POSITION;

#if DIFF || NORMAL
	float2 TexCoord		: TEXCOORD0;
#endif

#if !LIT
	float3 Normal		: NORMAL0;
#endif

#if NORMAL
	float3 Tangent		: TANGENT0;
	float3 Binormal		: BINORMAL0;
#endif

};

struct VS_OUT
{
	float4 Position 	: SV_POSITION;
	float4 PositionCS	: TEXCOORD0;
	float3 CameraVector	: TEXCOORD1;

#if DIFF || NORMAL
	float2 TexCoord		: TEXCOORD2;
#endif

#if !LIT
	float3 NormalWS		: TEXCOORD3;
#endif

#if NORMAL
	float3 TangentWS	: TEXCOORD4;
	float3 BinormalWS	: TEXCOORD5;
#endif
};

// FUNCTIONS
VS_OUT vsEnvironmentMaterial(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	output.Position = mul(input.Position, WorldViewProj);
	output.PositionCS = output.Position;
	float3 positionWS = mul(input.Position, World).xyz;
	output.CameraVector = positionWS - CameraPosition;

#if !LIT
	output.NormalWS = mul(input.Normal, (float3x3)WorldInverseTranspose);
#endif

#if NORMAL
	output.TangentWS = mul(input.Tangent, (float3x3)WorldInverseTranspose);
	output.BinormalWS = mul(input.Binormal, (float3x3)WorldInverseTranspose);
#endif

#if DIFF || NORMAL
	output.TexCoord = input.TexCoord;
#endif

	return output;
}

float4 psEnvironmentMaterial(VS_OUT input) : SV_Target0
{
	float2 screenPosition = ComputeScreenPosition(input.PositionCS);
	float3 diffuseIntensity = float3(0, 0, 0);
	float specularIntensity = 0;
	float3 color;
	float3 intensity;
	float alphaMask = 1;
	float envAmount = EnvironmentAmount;
	float glossiness;
	float3 normal;

#if LIT
	float4 lighting = LightingTexture.Sample(LightingTextureSampler, screenPosition);
	DecodeLightDiffuseSpecular(lighting, diffuseIntensity, specularIntensity);
	float4 gbuffer = Gbuffer.Sample(GbufferSampler, screenPosition);
	DecodeNormalGlossiness(gbuffer, normal, glossiness);
#else
	diffuseIntensity = float3(1, 1, 1);

	#if NORMAL
		// Normalize the tangent frame after interpolation
		float3x3 tangentFrameWS = float3x3(	normalize(input.TangentWS),
											normalize(input.BinormalWS),
											normalize(input.NormalWS));

		// Sample the tangent-sapce normal map and descompress
		float3 normalTS = NormalTexture.Sample(TextureSampler, input.TexCoord).rgb;
		normalTS = normalize(normalTS * 2.0 - 1.0);

		// Convert to world space
		normal = mul(normalTS, tangentFrameWS);
	#else
		normal = normalize(input.NormalWS);
	#endif
#endif

	float3 nomalizedCameraVector = normalize(input.CameraVector);
	intensity = diffuseIntensity + (specularIntensity.xxx);

#if FRES
	float fresnelTerm = abs(dot(nomalizedCameraVector, normal));
	envAmount = pow(max(1 - fresnelTerm, 0), FresnelFactor) * EnvironmentAmount;
#endif

#if DIFF
	float4 albedo = Diffuse.Sample(TextureSampler, input.TexCoord);
	alphaMask = albedo.a;
	
	color = albedo.xyz * DiffuseColor;
#else
	color = DiffuseColor;
#endif

#if ENV
	float3 envCoord = reflect(nomalizedCameraVector, normal);
	float3 enviromentMap = CubeTexture.Sample(TextureCubeSampler, envCoord).xyz;
	color = lerp(color, enviromentMap, envAmount);
#endif

	color = AmbientColor + color * intensity;

	return float4(color, alphaMask);
}
