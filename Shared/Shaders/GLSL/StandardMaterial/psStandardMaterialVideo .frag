//-----------------------------------------------------------------------------
// psStandardMaterialVideo.frag
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#extension GL_OES_EGL_image_external : require

#ifdef GL_ES
precision mediump float;
#endif

uniform float Alpha;
uniform vec3 DiffuseColor;
uniform samplerExternalOES Diffuse;

varying vec2 outTexCoord;

void main()
{
	vec4 diffuse = texture2D(Diffuse, outTexCoord);
	gl_FragColor = diffuse * vec4(DiffuseColor, Alpha);
}
