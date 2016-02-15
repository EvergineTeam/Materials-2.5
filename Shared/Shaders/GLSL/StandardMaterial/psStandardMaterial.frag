//-----------------------------------------------------------------------------
// psStandardMaterial.frag
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// AMBI    =  Ambient
// EMIS  	=	Emissive
// SPEC		=	Specular
// DIFF		=	Diffuse
// ATEST	=	Alpha Test
// VCOLOR	=	Vertex Color
// LIT		=	Lighting

// Matrices
uniform mat4	WorldViewProj;
uniform mat4	World;
uniform mat4	WorldInverseTranspose;
uniform float   TexCoordFix;

// Parameters
uniform vec3	CameraPosition;
#ifdef ATEST
uniform float	ReferenceAlpha;
#endif
uniform vec3	DiffuseColor;
uniform float	Alpha;
uniform vec3	AmbientColor;
uniform vec3	EmissiveColor;

// Textures
uniform sampler2D Diffuse;
uniform sampler2D Emissive;
uniform sampler2D Specular;
uniform samplerCube Ambient;
uniform sampler2D LightingTexture;
uniform sampler2D GBufferTexture;

// Input	
varying vec2 outTexCoord;
varying vec4 outPositionCS;
#ifdef VCOLOR
varying vec3 outColor;
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
	vec3  diffuseIntensity = vec3(0.0);
	float specularIntensity = 0.0;
	vec3 color = vec3(0.0);
	vec3 intensity;
	float alphaMask = 1.0;

#ifdef LIT
	vec4 lighting = texture2D(LightingTexture, screenPosition);
	DecodeLightDiffuseSpecular(lighting, diffuseIntensity, specularIntensity);	
#else
	diffuseIntensity = vec3(1.0);
#endif
	
#ifdef AMBI
	float glossiness;
	vec3 normal;
	vec4 gbuffer = texture2D(GBufferTexture, screenPosition);
	DecodeNormalGlossiness(gbuffer, normal, glossiness);
	vec3 ambient = textureCube(Ambient, normal).xyz;

	#ifdef LIT
		diffuseIntensity += ambient;
	#else
		diffuseIntensity *= ambient;
	#endif
#endif

diffuseIntensity += AmbientColor;

#ifdef EMIS
	vec3 emissive = texture2D(Emissive, outTexCoord).xyz;
	color += emissive * EmissiveColor;
#endif

#ifdef SPEC
	float specular = texture2D(Specular, outTexCoord).x;
	specular *= specularIntensity;
	intensity = diffuseIntensity + vec3(specular);
#else
	intensity = diffuseIntensity + vec3(specularIntensity);
#endif

#ifdef DIFF
	vec4 albedo = texture2D(Diffuse, outTexCoord);
	alphaMask = albedo.a;
		
	#ifdef VCOLOR
		color += intensity * albedo.xyz * outColor;
	#else
		color += intensity * albedo.xyz * DiffuseColor;
	#endif
#else
	#ifdef VCOLOR
		color += intensity * DiffuseColor * outColor;
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

  vec4 finalColor = vec4 (color.xyz, alphaMask);
  finalColor *= Alpha;
  
  gl_FragColor = finalColor;
}
