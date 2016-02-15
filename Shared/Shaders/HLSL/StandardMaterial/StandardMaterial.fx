//-----------------------------------------------------------------------------
// BasicMaterial.fx
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// AMBI		=	Ambient
// EMIS		=	Emissive
// SPEC		=	Specular
// DIFF		=	Diffuse
// ATEST	=	Alpha Test
// VCOLOR	=	Vertex Color
// LIT		=	Lighting

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
	float		ReferenceAlpha : packoffset(c0.w);
	float3		DiffuseColor			: packoffset(c1.x);
	float		Alpha : packoffset(c1.w);
	float3		AmbientColor			: packoffset(c2.x);
	float3		EmissiveColor			: packoffset(c3.x);
	float2		TextureOffset			: packoffset(c4.x);
}

Texture2D		Diffuse 				: register(t0);
Texture2D		Emissive 				: register(t1);
Texture2D		Specular 				: register(t2);
TextureCube		CubeTexture				: register(t3);

SamplerState	TextureSampler 			: register(s0);
sampler			TextureCubeSampler		: register(s1);


Texture2D LightingTexture 				: register(t4);
sampler LightingTextureSampler =
sampler_state
{
	Texture = <DiffuseTexture>;
	MipFilter = POINT;
	MinFilter = POINT;
	MagFilter = POINT;
};

Texture2D Gbuffer : register(t5);

sampler GbufferSampler =
sampler_state
{
	Texture = <GBuffer>;
	MipFilter = POINT;
	MinFilter = POINT;
	MagFilter = POINT;
};

// STRUCTS
struct VS_IN
{
	float4 Position		: POSITION;
#if VTEX
	float2 TexCoord		: TEXCOORD0;
#endif

#if VCOLOR
	float3 Color		: COLOR0;
#endif
};

struct VS_OUT
{
	float4 Position 	: SV_POSITION;
	float4 PositionCS	: TEXCOORD0;
	float2 TexCoord		: TEXCOORD1;

#if VCOLOR
	float3 Color		: COLOR0;
#endif
};

// FUNCTIONS
VS_OUT vsMaterial(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	output.Position = mul(input.Position, WorldViewProj);
	output.PositionCS = output.Position;

#if VTEX
	output.TexCoord = input.TexCoord + TextureOffset;
#endif

#if VCOLOR
	output.Color = input.Color;
#endif
	return output;
}

float4 psMaterial(VS_OUT input) : SV_Target0
{
	float2 screenPosition = ComputeScreenPosition(input.PositionCS);
	float3 diffuseIntensity = float3(0, 0, 0);
	float specularIntensity = 0;
	float3 color = float3(0, 0, 0);
	float3 intensity;
	float alphaMask = 1;

#if LIT
	float4 lighting = LightingTexture.Sample(LightingTextureSampler, screenPosition);
	DecodeLightDiffuseSpecular(lighting, diffuseIntensity, specularIntensity);
#else
	diffuseIntensity = float3(1, 1, 1);
#endif

#if AMBI
	float glossiness;
	float3 normal;
	float4 gbuffer = Gbuffer.Sample(GbufferSampler, screenPosition);
	DecodeNormalGlossiness(gbuffer, normal, glossiness);
	float3 ambient = CubeTexture.Sample(TextureCubeSampler, normal).xyz;

	#if LIT
		diffuseIntensity += ambient;
	#else
		diffuseIntensity *= ambient;
	#endif
#endif

diffuseIntensity += AmbientColor;

#if EMIS
	float3 emissive = Emissive.Sample(TextureSampler, input.TexCoord).xyz;
	color += emissive * EmissiveColor;
#endif

#if SPEC
	float specular = Specular.Sample(TextureSampler, input.TexCoord).x;
	specular *= specularIntensity;
	intensity = diffuseIntensity + (specular.xxx);
#else
	intensity = diffuseIntensity + (specularIntensity.xxx);
#endif

#if DIFF
	float4 albedo = Diffuse.Sample(TextureSampler, input.TexCoord);
	alphaMask = albedo.a;

	#if VCOLOR
		color += intensity * albedo.xyz * input.Color;
	#else
		color += intensity * albedo.xyz * DiffuseColor;
	#endif
#else
	#if VCOLOR
		color += intensity * DiffuseColor * input.Color;
	#else
		color += intensity * DiffuseColor;
	#endif
#endif

#if ATEST
	if (alphaMask < ReferenceAlpha)
	{
		discard;
	}
#endif

	float4 finalColor = float4(color, alphaMask);
	finalColor *= Alpha;
	return finalColor;
}
