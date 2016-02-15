//-----------------------------------------------------------------------------
// DualTexture.fx
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// MUL		=	Multiplicative
// ADD		=	Additive
// MSK		=	Mask
// LIT		=	Lighting
// FIRST	=	First texture only
// SECON	=	Second texture only

// Matrices
uniform mat4 WorldViewProj;

// Parameters
uniform vec4 DiffuseColor;
uniform vec3 AmbientColor;
uniform vec2 OffsetTexture1;
uniform vec2 OffsetTexture2;

// Input
attribute vec4 Position0;
attribute vec2 TextureCoordinate0;
attribute vec2 TextureCoordinate1;

// Output
varying vec4 outPositionCS;
varying vec2 outTexCoord1;
varying vec2 outTexCoord2;

void main(void)
{
	outPositionCS = WorldViewProj * Position0;
    gl_Position = outPositionCS;

    outTexCoord1 = TextureCoordinate0 + OffsetTexture1;
    outTexCoord2 = TextureCoordinate1 + OffsetTexture2;
}
