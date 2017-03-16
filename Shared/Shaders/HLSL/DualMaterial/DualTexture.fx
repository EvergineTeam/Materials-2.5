//-----------------------------------------------------------------------------
// DualTexture.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// MUL		=	Multiplicative
// ADD		=	Additive
// MSK		=	Mask
// LIT		=	Lighting
// FIRST	=	First texture only
// SECON	=	Second texture only

#include "..\Helpers.fxh"

cbuffer Matrices : register(b0)
{
	float4x4	WorldViewProj			: packoffset(c0);
	float4x4	World					: packoffset(c4);
	float4x4	WorldInverseTranspose	: packoffset(c8);
};

cbuffer Parameters : register(b1)
{
	float4		DiffuseColor			: packoffset(c0.x);
	float3		AmbientColor			: packoffset(c1.x);
	float2		TextureOffset1			: packoffset(c2.x);
	float2		TextureOffset2			: packoffset(c2.z);
}

Texture2D		Texture1 				: register(t0);
Texture2D		Texture2				: register(t1);

SamplerState	TextureSampler1 		: register(s0);
SamplerState	TextureSampler2 		: register(s1);

Texture2D LightingTexture 				: register(t2);
sampler LightingTextureSampler =
sampler_state
{
	Texture = <DiffuseTexture>;
	MipFilter = POINT;
	MinFilter = POINT;
	MagFilter = POINT;
};

// STRUCTS
struct VS_IN
{
	float4 Position		: POSITION;
	float2 TexCoord1	: TEXCOORD0;
	float2 TexCoord2	: TEXCOORD1;
};

struct VS_OUT
{
	float4 Position 	: SV_POSITION;
	float4 PositionCS	: TEXCOORD0;
	float2 TexCoord1	: TEXCOORD1;
	float2 TexCoord2	: TEXCOORD2;
};

// FUNCTIONS
VS_OUT vsDualTexture(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	output.Position = mul(input.Position, WorldViewProj);
	output.PositionCS = output.Position;
	output.TexCoord1 = input.TexCoord1 + TextureOffset1;
	output.TexCoord2 = input.TexCoord2 + TextureOffset2;

	return output;
}

float4 psDualTexture(VS_OUT input) : SV_Target0
{
	float2 screenPosition = ComputeScreenPosition(input.PositionCS);
	float3 diffuseIntensity = float3(1, 1, 1);
	float specularIntensity = 0;
	float4 color = DiffuseColor;
	float3 intensity;
	float4 diffuse1;
	float4 diffuse2;

#if LIT
	float4 lighting = LightingTexture.Sample(LightingTextureSampler, screenPosition);
	DecodeLightDiffuseSpecular(lighting, diffuseIntensity, specularIntensity);
#endif

	intensity = diffuseIntensity + (specularIntensity.xxx);

#if FIRST
	diffuse1 = Texture1.Sample(TextureSampler1, input.TexCoord1);
	color = diffuse1;
#endif

#if SECON
	diffuse2 = Texture2.Sample(TextureSampler2, input.TexCoord2);
	color = diffuse2;
#endif

#if FIRST && SECON
	color = DiffuseColor;
	#if LMAP	
		color.rgb *= diffuse1.rgb * 2;
		color *= diffuse2;
	#endif

	#if MUL
		color.rgb *= diffuse1.rgb;
		color *= diffuse2;
	#endif

	#if ADD
		diffuse1.rgb += diffuse2.rgb;
		color *= clamp(diffuse1, 0, 1);
	#endif

	#if MSK
		color.rgb *= lerp(diffuse1.rgb, diffuse2.rgb, diffuse2.a);
	#endif
#endif

	color.rgb = AmbientColor + color.rgb * intensity;

	return color;
}
