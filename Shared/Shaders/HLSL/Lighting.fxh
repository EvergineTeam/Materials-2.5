//-----------------------------------------------------------------------------
// Lighting.fxh
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// Directional Light Phong 
// Material SkyLightPhong(float3 eyeVector, float3 worldNormal)
// {
    // Material result;
	
	// // Diffuse
	// float diffuseIntensity = saturate( dot(SkyLight.Direction, worldNormal));
	// result.Diffuse = SkyLight.DiffuseColor * diffuseIntensity;

	// // Specular
	// float3 reflectionVector = normalize(reflect(-SkyLight.Direction, worldNormal));
	// float3 RdotV = saturate(dot(reflectionVector, eyeVector));
	
	// float3 specularIntensity = pow(RdotV, SpecularPower);
	// result.Specular = SkyLight.SpecularColor * specularIntensity ;
	
    // return result;
// }


// // Directional Light Blinn-Phong 
// Material SkyLightBlinn(float3 eyeVector, float3 worldNormal)
// {
	// Material result;

	// // Diffuse
	// float diffuseIntensity = saturate( dot(SkyLight.Direction, worldNormal));
	// result.Diffuse = SkyLight.DiffuseColor * diffuseIntensity;
		
	// // Specular
	// float3 halfVector = normalize(eyeVector + SkyLight.Direction);
	// float3 HdotN = saturate(dot(halfVector, worldNormal));
								
	// float3 specularIntensity = pow(HdotN, SpecularPower);  // 4.2
	// result.Specular = SkyLight.SpecularColor * specularIntensity ;
	
	// return result;
// }

// // Point Light
// Material PointLightBlinn(float3 eyeVector, float3 worldPosition, float3 worldNormal)
// {
	// Material result = (Material)0;
	
	// float3 lightPosition = PLights[0].Position;
	// float3 lightDirection = normalize(lightPosition - worldPosition);
	
	// float d = distance(lightPosition, worldPosition);
	
	// // att = 1 - (d / a)^f
	// float att = 1 - pow(saturate(d / PLights[0].Attenuation), PLights[0].Falloff);
	
	// // Diffuse
	// float diffuseIntensity = saturate(dot(lightDirection, worldNormal));
	// result.Diffuse += PLights[0].DiffuseColor * diffuseIntensity * att;
	
	// // Specular
	// // float3 halfVector = normalize(eyeVector + lightDirection);
	// // float3 HdotN = saturate(dot(halfVector, worldNormal));
								
	// // float3 specularIntensity = pow(HdotN, SpecularPower);
	// // result.Specular += PLights[0].SpecularColor * specularIntensity * att;
		
	// return result;
// }

// // Multiple Point Lights
// Material MultiplePointLights(float3 eyeVector,  float3 worldPosition, float3 worldNormal)
// {
	// Material result = (Material)0;

	// [unroll]
	// for (int i = 0; i < NumPointLights; i++)
    // {
		// float3 lightPosition = PLights[i].Position;
		// float3 lightDirection = normalize(lightPosition - worldPosition);
		
		// float d = distance(lightPosition, worldPosition);
		
		// // att = 1 - (d / a)^f
		// float att = 1 - pow(saturate(d / PLights[i].Attenuation), PLights[i].Falloff);
		
		// // Diffuse
		// float diffuseIntensity = saturate(dot(lightDirection, worldNormal));
		// result.Diffuse += PLights[i].DiffuseColor * diffuseIntensity * att;
	// }
	
	// return result;
// }

// Material ComputeLights(float3 eyeVector, float3 worldPosition, float3 worldNormal)
// {
	// Material result = (Material)0;

	// // Diffuse
	// float diffuseIntensity = saturate(dot(SkyLight.Direction, worldNormal));
	// result.Diffuse = SkyLight.DiffuseColor * diffuseIntensity;
		
	// // Specular
	// float3 halfVector = normalize(eyeVector + SkyLight.Direction);
	// float3 HdotN = saturate(dot(halfVector, worldNormal));
								
	// float3 specularIntensity = pow(HdotN, SpecularPower); // 4.2
	// result.Specular = SkyLight.SpecularColor * specularIntensity ;
	
	// // [unroll]
	// // for (int i = 0; i < NumPointLights; i++)
    // // {
		// // float3 lightPosition = PLights[i].Position;
		// // float3 lightDirection = normalize(lightPosition - worldPosition);
		
		// // float d = distance(lightPosition, worldPosition);
		
		// // // att = 1 - (d / a)^f
		// // float att = 1 - pow(saturate(d / PLights[i].Attenuation), PLights[i].Falloff);
		
		// // // Diffuse
		// // float PdiffuseIntensity = saturate(dot(lightDirection, worldNormal));
		// // result.Diffuse += PLights[i].DiffuseColor * PdiffuseIntensity * att;
	// // }
	
	// return result;
// }

Material OneDirectionalLight(float3 eyeVector, float3 worldNormal)
{
	Material result;

	// Diffuse
	float diffuseIntensity = saturate(dot(Light.Direction, worldNormal));
	result.Diffuse = Light.Color * diffuseIntensity;
		
	// Specular
	float3 halfVector = normalize(eyeVector + Light.Direction);
	float3 HdotN = saturate(dot(halfVector, worldNormal));
								
	float3 specularIntensity = pow(HdotN, SpecularPower);
	result.Specular = Light.Color * specularIntensity ;
	
	return result;
}


