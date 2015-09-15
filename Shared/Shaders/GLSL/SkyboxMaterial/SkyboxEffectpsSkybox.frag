// SkyboxEffectpsSkybox.frag
//
// Copyright 2012 Weekend Game Studios. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

uniform samplerCube EnvironmentMap;

varying vec3 outEnvCoord;

void main(void)
{
	gl_FragColor = textureCube(EnvironmentMap, outEnvCoord);
}
