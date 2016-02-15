//-----------------------------------------------------------------------------
// SkyboxEffect.fx
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Matrices : register(b0)
{
    float4x4	WorldViewProj						: packoffset(c0);
};

TextureCube EnvironmentMap 		: register(t0);
sampler EnvironmentMapSampler 	: register(s0);

VS_OUT_ENVIRONMENT vsSkybox( VS_IN input )
{
    VS_OUT_ENVIRONMENT output = (VS_OUT_ENVIRONMENT)0;

    output.Position = mul(input.Position, WorldViewProj);
    output.EnvCoord = input.Position;
    
    return output;
}

float4 psSkybox( VS_OUT_ENVIRONMENT input ) : SV_Target0
{
    float4 envmap = EnvironmentMap.Sample(EnvironmentMapSampler, input.EnvCoord);
    
    return envmap;
}
