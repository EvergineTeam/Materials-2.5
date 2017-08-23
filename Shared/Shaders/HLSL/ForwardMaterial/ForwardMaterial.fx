//-----------------------------------------------------------------------------
// ForwardMaterial.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// AMBI			=	Ambient
// DIFF			=	Diffuse
// POINT		=	Point Light
// SPOT			=	Spot Light
// DIRECTIONAL	=	Directional
// ATEST		=	Alpha Test
// VCOLOR		=	Vertex Color
// LIT			=	Lighting

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
	float		ReferenceAlpha			: packoffset(c0.w);
	float3		DiffuseColor			: packoffset(c1.x);
	float		Alpha					: packoffset(c1.w);
	float3		AmbientColor			: packoffset(c2.x);
	float		SpecularPower			: packoffset(c2.w);
	float2		TextureOffset			: packoffset(c3.x);
	float		SpecularIntensity		: packoffset(c3.z);
	LightStruct Light					: packoffset(c4);
}

Texture2D		Diffuse 				: register(t0);
TextureCube		CubeTexture				: register(t1);

SamplerState	TextureSampler 			: register(s0);
sampler			TextureCubeSampler		: register(s1);

// STRUCTS
struct VS_IN
{
	min16float4 Position		: POSITION;

#if VLIT || AMBI
	min16float3 Normal			: NORMAL0;
#endif

#if VTEX
	min16float2 TexCoord		: TEXCOORD0;
#endif

#if VCOLOR
	min16float4 Color			: COLOR0;
#endif
};

struct VS_OUT
{
	min16float4 Position 	: SV_POSITION;
	min16float3 PositionWS	: TEXCOORD0;
	min16float3 NormalWS	: TEXCOORD1;
	min16float2 TexCoord	: TEXCOORD2;
	min16float4 Color		: COLOR0;
};

// FUNCTIONS
VS_OUT vs(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	output.Position = mul(input.Position, WorldViewProj);

#if VLIT
	output.PositionWS = mul(input.Position, World).xyz;
	output.NormalWS = mul(input.Normal, (min16float3x3)WorldInverseTranspose);
#else
	#if AMBI
	output.NormalWS = mul(input.Normal, (min16float3x3)WorldInverseTranspose);
	#endif
#endif

#if VTEX
	output.TexCoord = input.TexCoord + TextureOffset;
#endif

#if VCOLOR
	output.Color = input.Color;
#endif

	return output;
}

float4 ps(VS_OUT input) : SV_Target0
{
	min16float3 diffuseIntensity = min16float3(0, 0, 0);
	min16float specular = 0;
	min16float3 color = min16float3(0, 0, 0);
	min16float3 intensity;
	min16float alphaMask = 1;

#if LIT

	min16float3 eyeVector = normalize(CameraPosition - input.PositionWS);
	min16float3 normal = normalize(input.NormalWS);

	min16float3 L = min16float3(0, 0, 0);
	min16float attenuation = 1;

	#if POINT || SPOT
		
		L = Light.Position - input.PositionWS;
		min16float dist = length(L);
		attenuation = (1 - saturate(dist / Light.LightRange));

		L /= dist;

	#elif DIRECTIONAL
		
		L = -Light.Direction;

	#endif

	#if SPOT

		min16float3 L2 = Light.Direction;
		min16float rho = dot(-L, L2);
		attenuation *= saturate((rho - Light.ConeAngle) / (1 - Light.ConeAngle));

	#endif
	
	min16float NdotL = saturate(dot(L, normal));

	diffuseIntensity = NdotL.xxx * attenuation * Light.Color * Light.Intensity;

	min16float3 halfVector = normalize(eyeVector + L);
	min16float HdotN = saturate(dot(halfVector, normal));
	specular = pow(HdotN, SpecularPower) * SpecularIntensity * diffuseIntensity.x;

	#if AMBI
		min16float3 ambient = CubeTexture.Sample(TextureCubeSampler, normal).xyz;
		diffuseIntensity += ambient;
	#else
		diffuseIntensity += AmbientColor;
	#endif

	intensity = diffuseIntensity + specular.xxx;

#else

	diffuseIntensity = min16float3(1, 1, 1);

	#if AMBI
		min16float3 normal = normalize(input.NormalWS);
		min16float3 ambient = CubeTexture.Sample(TextureCubeSampler, normal).xyz;
		diffuseIntensity *= ambient;
	#endif

	intensity = diffuseIntensity + AmbientColor;

#endif

#if DIFF
	min16float4 albedo = Diffuse.Sample(TextureSampler, input.TexCoord);
	alphaMask = albedo.a;

	#if VCOLOR
		color += intensity * albedo.xyz * input.Color.xyz;
		alphaMask *= input.Color.a;
	#else
		color += intensity * albedo.xyz * DiffuseColor;
	#endif
#else
	#if VCOLOR
		color += intensity * DiffuseColor * input.Color.xyz;
		alphaMask *= input.Color.a;
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

	min16float4 finalColor = min16float4(color, alphaMask);
	finalColor *= Alpha;
	return finalColor;
}
