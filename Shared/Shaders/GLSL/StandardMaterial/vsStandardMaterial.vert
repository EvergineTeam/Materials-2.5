//-----------------------------------------------------------------------------
// vsStandardMaterial.vert
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// VCOLOR	=	Vertex Color

// Matrices
uniform mat4	WorldViewProj;

// Input
attribute vec4 Position0;
#ifdef VTEX
attribute vec2 TextureCoordinate0;
#endif
#ifdef VCOLOR
attribute vec3 Color0;
#endif


// Output
varying vec2 outTexCoord;
varying vec4 outPositionCS;
#ifdef VCOLOR
varying vec3 outColor;
#endif

void main(void)
{
  outPositionCS = WorldViewProj * Position0;
#ifdef VTEX
	outTexCoord = TextureCoordinate0;
#else
	outTexCoord = vec2(0.0);
#endif
#ifdef VCOLOR
	outColor = (Color0 / 255.0);
#endif  

  gl_Position = WorldViewProj * Position0;
}
