//-----------------------------------------------------------------------------
// EnvironmentMaterial.fx
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// DIFF		=	Diffuse Map
// LIT		=	Lighting
// FRES		=	Fresnel
// ENV		=	Environment Map
// NORMAL	=	Normals
// TEXC		=	DIFF || NORMAL

#ifdef DIFF
#define TEXC
#endif

#ifdef NORMAL
#define TEXC
#endif

#ifdef GL_ES
precision mediump float;
#endif

uniform sampler2D Diffuse;
uniform samplerCube Environment;
uniform sampler2D LightingTexture;
uniform sampler2D GBufferTexture;
uniform sampler2D Normal;

// Parameters
uniform vec3	CameraPosition;
uniform float	FresnelFactor;
uniform vec3	DiffuseColor;
uniform float	EnvironmentAmount;
uniform vec3	AmbientColor;
uniform float	TexCoordFix;

// Output	
varying vec4 outPositionCS;
varying vec3 outCameraVector;

#ifdef TEXC
	varying vec2 outTexCoord;
#endif

#ifndef LIT
	varying vec3 outNormalWS;
#endif

#ifdef NORMAL
	varying vec3 outTangentWS;
	varying vec3 outBinormalWS;
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
	vec2 screenPosition = ComputeScreenPosition(outPositionCS, TexCoordFix);
	vec3 diffuseIntensity = vec3(.0);
	float specularIntensity = 0.0;
	vec3 color;
	vec3 intensity;
	float alphaMask = 1.0;
	float envAmount = EnvironmentAmount;
	float glossiness;
	vec3 normal;

#ifdef LIT
	vec4 lighting = texture2D(LightingTexture, screenPosition);
	DecodeLightDiffuseSpecular(lighting, diffuseIntensity, specularIntensity);
	vec4 gbuffer = texture2D(GBufferTexture, screenPosition);
	DecodeNormalGlossiness(gbuffer, normal, glossiness);
#else
	diffuseIntensity = vec3(1.0);

	#ifdef NORMAL
		// Normalize the tangent frame after interpolation
		mat3 tangentFrameWS = mat3( normalize(outTangentWS),
									normalize(outBinormalWS),
									normalize(outNormalWS));

		// Sample the tangent-sapce normal map and descompress
		vec3  normalTS = texture2D(Normal, outTexCoord).xyz;
		normalTS = normalize(normalTS * 2.0 - 1.0);

		// Convert to world space
		normal = tangentFrameWS * normalTS;		
	#else
		normal = normalize(outNormalWS);
	#endif
#endif

	vec3 nomalizedCameraVector = normalize(outCameraVector);
	intensity = diffuseIntensity + vec3(specularIntensity);

#ifdef FRES
	float fresnelTerm = abs(dot(nomalizedCameraVector, normal));
	envAmount = pow(max(1.0 - fresnelTerm, 0.0), FresnelFactor) * EnvironmentAmount;
#endif

#ifdef DIFF
	vec4 albedo = texture2D(Diffuse, outTexCoord);
	alphaMask = albedo.a;
	
	color = albedo.xyz * DiffuseColor;
#else
	color = DiffuseColor;
#endif

#ifdef ENV
	vec3 envCoord = reflect(nomalizedCameraVector, normal);
	vec3 enviromentMap = textureCube(Environment, envCoord).xyz;
	color = mix(color, enviromentMap, envAmount);
#endif

	color = AmbientColor + color * intensity;

	gl_FragColor = vec4(color, alphaMask);
}
