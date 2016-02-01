// DualTexturepsDualTexture.frag
//
// Copyright 2012 Weekend Game Studios. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// MUL  	=	Multiplicative
// ADD		=	Additive
// MSK		=	Mask
// LIT		=	Lighting
// FIRST	=	First texture only
// SECON	=	Second texture only

#ifdef GL_ES
precision mediump float;
#endif

uniform sampler2D Texture1;
uniform sampler2D Texture2;
uniform sampler2D LightingTexture;

// Parameters
uniform vec4 DiffuseColor;
uniform vec3 AmbientColor;
uniform vec2 OffsetTexture1;
uniform vec2 OffsetTexture2;
uniform float TexCoordFix;

// Output
varying vec4 outPositionCS;
varying vec2 outTexCoord1;
varying vec2 outTexCoord2;

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

void main(void)
{
	vec2 screenPosition = ComputeScreenPosition(outPositionCS, TexCoordFix);
	vec3 diffuseIntensity = vec3(1.0);
	float specularIntensity = 0.0;
	vec4 color = DiffuseColor;
	vec3 intensity;
	vec4 diffuse1;
	vec4 diffuse2;

#ifdef LIT
	vec4 lighting = texture2D(LightingTexture, screenPosition);
	DecodeLightDiffuseSpecular(lighting, diffuseIntensity, specularIntensity);
#endif

	intensity = diffuseIntensity + vec3(specularIntensity);

#ifdef FIRST
	diffuse1 = texture2D(Texture1, outTexCoord1);
	color = diffuse1;
#endif

#ifdef SECON
	diffuse2 = texture2D(Texture2, outTexCoord2);
	color = diffuse2;
#endif

#ifdef FIRST 
#ifdef SECON
	color = DiffuseColor;
	#ifdef LMAP
		color.rgb *= diffuse1.rgb * 2.0;
		color *= diffuse2;
	#endif

	#ifdef MUL
		color.rgb *= diffuse1.rgb;
		color *= diffuse2;
	#endif

	#ifdef ADD
		diffuse1.rgb += diffuse2.rgb;
		color *= clamp(diffuse1, 0.0, 1.0);
	#endif

	#ifdef MSK
		color.rgb = mix(diffuse1.rgb, diffuse2.rgb, diffuse2.a);
	#endif
#endif
#endif

	color.rgb = AmbientColor + color.rgb * intensity;

	gl_FragColor = color;
}
