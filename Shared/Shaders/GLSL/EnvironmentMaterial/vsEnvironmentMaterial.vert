//-----------------------------------------------------------------------------
// EnvironmentMaterial.fx
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// DIFF  	=	Diffuse
// LIT		=	Lighting
// FRES		=	Fresnel
// NORMAL	=	Normals
// TEXC		=	DIFF || NORMAL

#ifdef DIFF
#define TEXC
#endif

#ifdef NORMAL
#define TEXC
#endif


// Matrices
uniform mat4	WorldViewProj;
uniform mat4	World;
uniform mat4	WorldInverseTranspose;

// Parameters
uniform vec3				CameraPosition;
uniform float				FresnelFactor;
uniform vec3				DiffuseColor;
uniform float				EnvironmentAmount;
uniform vec3				AmbientColor;

// Input
attribute vec4 Position0;

#ifdef TEXC
	attribute vec2 TextureCoordinate0;
#endif

#ifndef LIT
	attribute vec3 Normal0;
#endif

#ifdef NORMAL
	attribute vec3 Tangent0;
	attribute vec3 Binormal0;
#endif

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

void main(void)
{
	outPositionCS = WorldViewProj * Position0;
	gl_Position = outPositionCS;
	vec3 positionWS = (World * Position0).xyz;
	outCameraVector = positionWS - CameraPosition;

#ifndef LIT
	outNormalWS = normalize(mat3(WorldInverseTranspose) * Normal0);
#endif

#ifdef NORMAL
	outTangentWS = mat3(WorldInverseTranspose) * Tangent0;
	outBinormalWS = mat3(WorldInverseTranspose) * Binormal0;	
#endif  

#ifdef TEXC
	outTexCoord = TextureCoordinate0;
#endif
}
