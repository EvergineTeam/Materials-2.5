// #version 120

// SkyboxEffectvsSkybox.vert
//
// Copyright 2012 Weekend Game Studios. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// Matrices
uniform mat4	WorldViewProj;


// Input
attribute vec3 Position0;

// Output
varying vec3 outEnvCoord;

void main(void)
{
	gl_Position = WorldViewProj * vec4(Position0, 1);
	outEnvCoord = Position0;
}