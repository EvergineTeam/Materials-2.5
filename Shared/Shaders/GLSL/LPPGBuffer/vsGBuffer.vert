//-----------------------------------------------------------------------------
// vsGBuffer.vert
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// NORMAL  	=	Normal mapping

// Matrices
uniform mat4	WorldViewProj;
uniform mat4	WorldInverseTranspose;

// Input
attribute vec4 Position0;
attribute vec3 Normal0;
#ifdef NORMAL
	attribute vec3 Tangent0;
	attribute vec3 Binormal0;
	attribute vec2 TextureCoordinate0;
#endif

// Output	
varying vec4 outPositionCS;
varying vec3 outNormalWS;
#ifdef NORMAL
	varying vec3 outTangentWS;
	varying vec3 outBinormalWS;
	varying vec2 outTexCoord;
#endif

void main(void)
{
  outPositionCS = WorldViewProj * Position0;
  outNormalWS = mat3(WorldInverseTranspose) * Normal0;
  
#ifdef NORMAL
	outTexCoord = TextureCoordinate0;
	outTangentWS = mat3(WorldInverseTranspose) * Tangent0;
	
	outBinormalWS = mat3(WorldInverseTranspose) * Binormal0;	
#endif  

  gl_Position = outPositionCS;
}

