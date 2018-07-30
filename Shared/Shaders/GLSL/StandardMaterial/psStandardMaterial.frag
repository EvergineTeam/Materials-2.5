//-----------------------------------------------------------------------------
// psStandardMaterial.frag
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

//#define DIFF
//#define EMIS
//#define SPEC
//#define DUAL
//#define ENV
//#define IBL
//#define LIT

#ifdef DIFF
#define TEXTURE
#endif

#ifdef EMIS
#define TEXTURE
#endif

#ifdef SPEC
#define TEXTURE
#endif

#ifdef DUAL
#define TEXTURE
#endif

#ifdef ENV
#define NORMAL
#endif

#ifdef IBL
#define NORMAL
#endif

#ifndef LIT
  #ifdef NORMAL
		#define NOTLITNORMAL
	#endif
#endif

// Matrices
uniform mat4	WorldViewProj;
uniform mat4	World;
uniform mat4	WorldInverseTranspose;
uniform float   TexCoordFix;

// Parameters
uniform vec3	CameraPosition;
uniform float	ReferenceAlpha;
uniform vec3	DiffuseColor;
uniform float	Alpha;
uniform vec3	AmbientColor;
uniform vec3	EmissiveColor;
uniform float	FresnelFactor;
uniform float	IBLFactor;
uniform float	EnvironmentAmount;

// Textures
uniform sampler2D Diffuse1;
uniform sampler2D Diffuse2;
uniform sampler2D Emissive;
uniform sampler2D Specular;
uniform samplerCube IBLTexture;
uniform samplerCube ENVTexture;
uniform sampler2D LightingTexture;
uniform sampler2D GBufferTexture;

// Input	
#ifdef LIT
	varying vec4 outPositionCS;
#endif

#ifdef TEXTURE
	varying vec2 outTexCoord1;
#endif

#ifdef DUAL
	varying vec2 outTexCoord2;
#endif

#ifdef ENV
	varying vec3 outCameraVector;
#endif

#ifdef NOTLITNORMAL
	varying vec3 outNormalWS;
#endif

#ifdef VCOLOR
	varying vec4 outColor;
#endif

// HELPERS
vec2 ComputeScreenPosition(vec4 pos, float screenFix)
{
	vec2 screenPos = pos.xy / pos.w;
	return (0.5 * (vec2(screenPos.x, screenFix * screenPos.y) + 1.0));
}

void DecodeLightDiffuseSpecular(vec4 enc, out vec3 diffuse, out float specular)
{
	diffuse = enc.xyz;
	specular = enc.w;
	
	diffuse = diffuse * 2.0;	
}

void DecodeNormalGlossiness(vec4 enc, out vec3 normal, out float glossiness)
{
	normal = normalize(enc.xyz * 2.0 - 1.0);
	glossiness = enc.w * 255.0;
}

void main(void)
{
	vec3  diffuseIntensity = vec3(1.0);
	float specularIntensity = 0.0;
	vec3 basecolor = DiffuseColor;
	vec3 intensity;
	float alphaMask = 1.0;

#ifdef VCOLOR
	basecolor *= outColor.xyz;
	alphaMask *= outColor.a;
#endif

#ifdef LIT
	vec2 screenPosition = ComputeScreenPosition(outPositionCS, TexCoordFix);
	vec4 lighting = texture2D(LightingTexture, screenPosition);
	DecodeLightDiffuseSpecular(lighting, diffuseIntensity, specularIntensity);	
#endif
	
#ifdef DIFF
	vec4 albedo = texture2D(Diffuse1, outTexCoord1);
	alphaMask = albedo.a;
	basecolor *= albedo.rgb;
#endif

#ifdef DUAL
	vec4 diffuse2 = texture2D(Diffuse2, outTexCoord2);
	#ifdef LMAP
		basecolor *= 2.0 * diffuse2.rgb;
		alphaMask = diffuse2.a;
	#endif

	#ifdef MUL
		basecolor *= diffuse2.rgb;
		alphaMask = diffuse2.a;
	#endif

	#ifdef ADD
		vec3 add = basecolor + diffuse2.rgb;
		basecolor = DiffuseColor * clamp(add, 0.0, 1.0);
		alphaMask += diffuse2.a;
	#endif

	#ifdef MSK
		basecolor = DiffuseColor * mix(basecolor, diffuse2.rgb, diffuse2.a);
	#endif
#endif

#ifdef SPEC
	float specular = texture2D(Specular, outTexCoord1).x;
	specularIntensity *= specular;
#endif

#ifdef NORMAL
	#ifdef LIT
		float glossiness;
		vec3 normal;
		vec4 gbuffer = texture2D(GBufferTexture, screenPosition);
		DecodeNormalGlossiness(gbuffer, normal, glossiness);
	#else
		vec3 normal = normalize(outNormalWS);
	#endif
#endif

#ifdef IBL
	vec3 ibl = textureCube(IBLTexture, normal).xyz;

	#ifdef LIT
		diffuseIntensity += ibl * IBLFactor;
	#else
		diffuseIntensity *= ibl * IBLFactor;
	#endif
#endif

	vec3 diffuseContribution = basecolor * diffuseIntensity;

#ifdef ENV
	vec3 cameraVector = normalize(outCameraVector);
	float envAmount = EnvironmentAmount;

	#ifdef FRES
		float fresnelTerm = abs(dot(cameraVector, normal));
		envAmount = pow(max(1.0 - fresnelTerm, 0.0), FresnelFactor) * EnvironmentAmount;
	#endif

	vec3 envCoord = reflect(cameraVector, normal);
	vec3 envColor = textureCube(ENVTexture, envCoord).xyz;

	#ifdef SPEC
		envAmount *= specular;
	#endif

	diffuseContribution = mix(diffuseContribution, envColor, envAmount);
#endif

	// Compute final color
	vec3 color = diffuseContribution + specularIntensity + AmbientColor;

#ifdef EMIS
	vec3 emissive = texture2D(Emissive, outTexCoord1).rgb;
	color += emissive * EmissiveColor;
#endif

#ifdef ATEST
	if (alphaMask < ReferenceAlpha)
	{
		discard;
	}
#endif

  vec4 finalColor = vec4(color.xyz, alphaMask);
  finalColor *= Alpha;
  
  gl_FragColor = finalColor;
}
