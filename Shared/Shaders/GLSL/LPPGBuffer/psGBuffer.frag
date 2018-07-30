//-----------------------------------------------------------------------------
// psGBuffer.frag
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision highp float;
#endif

// NORMAL   = Normal
// DEPTH  = Depth texture
// MRT  	= Multi Render Target

// Matrices
uniform mat4	WorldViewProj;
uniform mat4	World;
uniform mat4	WorldInverseTranspose;

// Parameters
uniform float  SpecularPower;

#ifdef NORMAL
// Texture
uniform sampler2D Normal;
#endif

// Input	
varying vec4 outPositionCS;
varying vec3 outNormalWS;
#ifdef NORMAL
	varying vec3 outTangentWS;
	varying vec3 outBinormalWS;
	varying vec2 outTexCoord;
#endif

vec4 EncodeNormalGlossiness(vec3 normal, float glossiness)
{
	vec4 enc;
	enc.xyz = vec3(normal.xyz*0.5 + 0.5);
	enc.w = glossiness / 255.0;
	return enc;
}

vec4 EncodeFloatRGBA(float v)
{
  const vec4 bitSh = vec4(256.0*256.0*256.0, 256.0*256.0, 256.0, 1.0);
  const vec4 bitMsk = vec4(0.0, 1.0/256.0, 1.0/256.0, 1.0/256.0);
  vec4 res = fract(v * bitSh);
  res -= res.xxyz * bitMsk;
  return res;
}

void main(void)
{
	vec3 normalWS;
  
#ifdef NORMAL

	// Normal Texture available
	// Normalize the tangent frame after interpolation
	mat3 tangentFrameWS = mat3( normalize(outTangentWS),
                              normalize(outBinormalWS),
                              normalize(outNormalWS));

	// Sample the tangent-sapce normal map and descompress
	vec3  normalTS = texture2D(Normal, outTexCoord).xyz;
	normalTS = normalize(normalTS * 2.0 - 1.0);

	// Convert to world space
	normalWS = tangentFrameWS * normalTS;		

#else

	// No Normal texture defined
	normalWS = normalize(outNormalWS);

#endif

	vec4 fragColor;

#ifndef DEPTH
	// If we can't read the depth buffer, store Depth
	float depth = outPositionCS.z / outPositionCS.w;
  
	#ifdef MRT
	    // If MRT is available, store Depth in a second RT
		// RT0: | Nx | Ny | Nz | Sp |
		// RT1: | Da | Db | Dz | -- |
		gl_FragData[0] = EncodeNormalGlossiness(normalWS, SpecularPower);

		vec4 encodedDepth = EncodeFloatRGBA(depth);
		gl_FragData[1] = encodedDepth;
	#else
    
    // Encode Normal and Specular power in a single RT (depth from Depth Buffer)
  	// RT0: | Nx | Ny | Nz | Sp |
  	gl_FragColor = vec4(1,1,1,1); //EncodeNormalGlossiness(normalWS, SpecularPower);
    
  #endif
  
#else

	// Encode Normal and Specular power in a single RT (depth from Depth Buffer)
	// RT0: | Nx | Ny | Nz | Sp |
	gl_FragColor = EncodeNormalGlossiness(normalWS , SpecularPower);

#endif
}
