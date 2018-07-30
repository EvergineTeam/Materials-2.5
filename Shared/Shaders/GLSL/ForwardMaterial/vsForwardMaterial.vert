//-----------------------------------------------------------------------------
// vsForwardMaterial.vert
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// AMBI			=	Ambient
// VTEX			=	Vertex Texture
// VCOLOR		=	Vertex Color
// VLIT			=	Lighting

// Matrices
uniform mat4	WorldViewProj;
uniform mat4	World;
uniform mat4	WorldInverseTranspose;
uniform vec2	TextureOffset;

// Input
attribute vec4 Position0;
attribute vec3 Normal0;
attribute vec2 TextureCoordinate0;
attribute vec4 Color0;


// Output
varying vec3 outPositionWS;
varying vec3 outNormalWS;
varying vec2 outTexCoord;
varying vec4 outColor;

void main(void)
{
  gl_Position = WorldViewProj * Position0;

#ifdef VLIT
  outPositionWS = (World * Position0).xyz;
#endif

#if defined(AMBI) || defined(VLIT)
  outNormalWS = mat3(WorldInverseTranspose) * Normal0;
#endif

#ifdef VTEX
	outTexCoord = TextureCoordinate0 + TextureOffset;
#else
	outTexCoord = vec2(0.0);
#endif

#ifdef VCOLOR
	outColor = (Color0 / 255.0);
#endif  
}
