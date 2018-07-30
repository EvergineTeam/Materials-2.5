//-----------------------------------------------------------------------------
// vsStandardMaterialVideo.vert
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// Matrices
uniform mat4	WorldViewProj;
uniform mat4	TextCoordTransform;

// Input
attribute vec4 Position0;
attribute vec4 TextureCoordinate0;

// Output	
varying vec2 outTexCoord;

void main()
{
	gl_Position = WorldViewProj * Position0;
	outTexCoord = (TextCoordTransform * TextureCoordinate0).xy;
}
