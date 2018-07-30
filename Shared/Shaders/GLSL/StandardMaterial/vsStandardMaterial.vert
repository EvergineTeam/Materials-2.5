//-----------------------------------------------------------------------------
// vsStandardMaterial.vert
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

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

// Parameters
uniform vec3	CameraPosition;
uniform vec2	TextureOffset1;
uniform vec2	TextureOffset2;

// Input
	attribute vec4 Position0;

#ifdef TEXTURE
	attribute vec2 TextureCoordinate0;
#endif

#ifdef DUAL
	attribute vec2 TextureCoordinate1;
#endif

#ifdef NOTLITNORMAL
	attribute vec3 Normal0;
#endif

#ifdef VCOLOR
	attribute vec4 Color0;
#endif


// Output
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


void main(void)
{
#ifdef LIT
	outPositionCS = WorldViewProj * Position0;
#else
	#ifdef NORMAL
		outNormalWS = normalize(mat3(WorldInverseTranspose) * Normal0);
	#endif
#endif

#ifdef TEXTURE
	outTexCoord1 = TextureCoordinate0 + TextureOffset1;
#endif

#ifdef DUAL
	outTexCoord2 = TextureCoordinate1 + TextureOffset2;
#endif

#ifdef ENV
	vec3 positionWS = (World * Position0).xyz;
	outCameraVector = positionWS - CameraPosition;
#endif

#ifdef VCOLOR
	outColor = (Color0 / 255.0);
#endif  

  gl_Position = WorldViewProj * Position0;
}
