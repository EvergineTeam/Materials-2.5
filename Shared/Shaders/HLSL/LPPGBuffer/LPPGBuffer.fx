//-----------------------------------------------------------------------------
// Gbuffer.fx
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// NORMAL  	=	Normal
// DEPTH	=	Depth texture
// MRT		=	Multi Render Target

#include "..\Helpers.fxh"

cbuffer Matrices : register(b0)
{
	float4x4	WorldViewProj						: packoffset(c0);
	float4x4	World								: packoffset(c4);
	float4x4	WorldInverseTranspose				: packoffset(c8);
};

cbuffer Parameters : register(b1)
{
	float		SpecularPower		: packoffset(c0.x);
	float2		TextureOffset		: packoffset(c0.y);
}

#if NORMAL
Texture2D NormalTexture : register(t0);
SamplerState NormalTextureSampler : register(s0);
#endif

// STRUCTS
struct VS_IN
{
	float4 Position		: POSITION;
	float3 Normal		: NORMAL0;

#if NORMAL
	float3 Tangent		: TANGENT0;
	float3 Binormal		: BINORMAL0;
	float2 TexCoord		: TEXCOORD0;
#endif
};

struct VS_OUT
{
	float4 Position 	: SV_POSITION;
	float4 PositionCS	: TEXCOORD0;
	float3 NormalWS		: TEXCOORD1;

#if NORMAL
	float3 TangentWS	: TEXCOORD2;
	float3 BinormalWS	: TEXCOORD3;
	float2 TexCoord		: TEXCOORD4;
#endif
};

struct PS_OUT
{
	float4 normal		: COLOR0;
#if MRT
	float4 depth		: COLOR1;
#endif
};

// VERTEX SHADER
VS_OUT vsGBuffer(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;

	output.Position = mul(input.Position, WorldViewProj);
	output.PositionCS = output.Position;
	output.NormalWS = mul(input.Normal, (float3x3)WorldInverseTranspose);

#if NORMAL
	output.TangentWS = mul(input.Tangent, (float3x3)WorldInverseTranspose);
	output.BinormalWS = mul(input.Binormal, (float3x3)WorldInverseTranspose);
	output.TexCoord = input.TexCoord + TextureOffset;
#endif

	return output;
}

// PIXEL SHADER
PS_OUT psGBuffer(VS_OUT input) : SV_Target0
{
	float3 normalWS;

#if NORMAL
	// Normal Texture available
	// Normalize the tangent frame after interpolation
	float3x3 tangentFrameWS = float3x3(normalize(input.TangentWS),
	normalize(input.BinormalWS),
	normalize(input.NormalWS));

	// Sample the tangent-sapce normal map and descompress
	float3 normalTS = NormalTexture.Sample(NormalTextureSampler, input.TexCoord).rgb;
	normalTS = normalize(normalTS * 2.0 - 1.0);

	// Convert to world space
	normalWS = mul(normalTS, tangentFrameWS);

#else
	// No Normal texture defined
	normalWS = normalize(input.NormalWS);

#endif 

	PS_OUT output = (PS_OUT)0;

#ifndef DEPTH
	// If we can't read the depth buffer, store Depth
	float depth = input.PositionCS.z / input.PositionCS.w;

	#if MRT		
		// If MRT is available, store Depth in a second RT
		// RT0: | Nx | Ny | Nz | Sp |
		// RT1: | Da | Db | Dz | -- |
		output.normal = EncodeNormalGlossiness(normalWS, SpecularPower);
		output.depth = EncodeFloatRGBA(depth);
		//output.normal = float4(normalWS, 1);

	#else
		// Encode Normal and Depth in a single RT (Specular power is missed)		
		// | Nx | Ny | D1 | D2 |
		output.normal = EncodeDepthNormal(depth, normalWS);
	#endif
#else
	// Encode Normal and Specular power in a single RT (depth from Depth Buffer)
	// RT0: | Nx | Ny | Nz | Sp |
	output.normal = EncodeNormalGlossiness(normalWS, SpecularPower);	
#endif

	return output;
}