//-----------------------------------------------------------------------------
// psForwardMaterial.frag
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// AMBI			=	Ambient
// DIFF			=	Diffuse
// POINT		=	Point Light
// SPOT			=	Spot Light
// DIRECTIONAL	=	Directional
// ATEST		=	Alpha Test
// VCOLOR		=	Vertex Color
// LIT			=	Lighting

// Matrices
uniform mat4	WorldViewProj;
uniform mat4	World;
uniform mat4	WorldInverseTranspose;

// Parameters
uniform vec3	CameraPosition;
uniform float	ReferenceAlpha;
uniform vec3	DiffuseColor;
uniform float	Alpha;
uniform vec3	AmbientColor;
uniform float	SpecularPower;
uniform float	SpecularIntensity;

// Light Parameters
uniform vec3	LightPosition;
uniform float	LightConeAngle;
uniform vec3	LightColor;
uniform float	LightLightRange;
uniform vec3	LightDirection;
uniform float	LightIntensity;

// Textures
uniform sampler2D Diffuse;
uniform samplerCube Ambient;

// Input	
varying vec3 outPositionWS;
varying vec3 outNormalWS;
varying vec2 outTexCoord;
varying vec4 outColor;

void main(void)
{
	vec3  diffuseIntensity = vec3(0.0);
	float specular = 0.0;
	vec3 color = vec3(0.0);
	vec3 intensity;
	float alphaMask = 1.0;

#ifdef LIT

	vec3 eyeVector = normalize(CameraPosition - outPositionWS);
	vec3 normal = normalize(outNormalWS);
	
	vec3 L = vec3(0.0);
	float attenuation = 1.0;

	#ifndef DIRECTIONAL
		
		L = LightPosition - outPositionWS;
		float dist = length(L);
		attenuation = (1.0 - clamp(dist / LightLightRange, 0.0, 1.0));

		L /= dist;

	#else
		
		L = -LightDirection;

	#endif

	#ifdef SPOT

	  vec3 L2 = LightDirection;
	  float rho = dot(-L, L2);
	  attenuation *= clamp((rho - LightConeAngle) / (1.0 - LightConeAngle), 0.0, 1.0);

	#endif

	float NdotL = clamp(dot(L, normal), 0.0, 1.0);

	diffuseIntensity = vec3(NdotL) * attenuation * LightColor * LightIntensity;

	vec3 halfVector = normalize(eyeVector + L);
	float HdotN = clamp(dot(halfVector, normal), 0.0, 1.0);
	specular = pow(HdotN, SpecularPower) * SpecularIntensity * diffuseIntensity.x;

	#ifdef AMBI
		vec3 ambient = textureCube(Ambient, normal).xyz;
		diffuseIntensity += ambient;
	#else
		diffuseIntensity += AmbientColor;
	#endif

	intensity = diffuseIntensity + vec3(specular);

#else
	diffuseIntensity = vec3(1.0);

	#ifdef AMBI
		vec3 normal = normalize(outNormalWS);
		vec3 ambient = textureCube(Ambient, normal).xyz;
		diffuseIntensity *= ambient;
	#endif

	intensity = diffuseIntensity + AmbientColor;

#endif
	
#ifdef DIFF
	vec4 albedo = texture2D(Diffuse, outTexCoord);
	alphaMask = albedo.a;
		
	#ifdef VCOLOR
		color += intensity * albedo.xyz * outColor.xyz;
		alphaMask *= outColor.a;
	#else
		color += intensity * albedo.xyz * DiffuseColor;
	#endif
#else
	#ifdef VCOLOR
		color += intensity * DiffuseColor * outColor.xyz;
		alphaMask *= outColor.a;
	#else
		color += intensity * DiffuseColor;
	#endif
#endif

#ifdef ATEST
  if (alphaMask < ReferenceAlpha)
	{
		discard;
	}
#endif

  vec4 finalColor = vec4(color, alphaMask);
  finalColor *= Alpha;
  
  gl_FragColor = finalColor;
}
