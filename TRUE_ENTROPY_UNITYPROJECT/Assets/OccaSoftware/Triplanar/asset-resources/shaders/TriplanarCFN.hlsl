#ifndef OS_TRIPLANAR_MAPPING_INCLUDED
#define OS_TRIPLANAR_MAPPING_INCLUDED

half DoBlend(half3 Blend, half x, half y, half z)
{
	return x * Blend.x + y * Blend.y + z * Blend.z;
}

half3 DoBlend(half3 Blend, half3 x, half3 y, half3 z)
{
	return x * Blend.x + y * Blend.y + z * Blend.z;
}

half3 BlendNormals(half3 Blend, half3 normalWS, half3 x, half3 y, half3 z)
{
	x = half3(x.xy + normalWS.zy, abs(x.z) * normalWS.x);
	y = half3(y.xy + normalWS.xz, abs(y.z) * normalWS.y);
	z = half3(z.xy + normalWS.xy, abs(z.z) * normalWS.z);
	return half3(normalize(x.zyx * Blend.x + y.xzy * Blend.y + z.xyz * Blend.z));
}

float DoBlend(float3 Blend, float x, float y, float z)
{
	return x * Blend.x + y * Blend.y + z * Blend.z;
}

float3 DoBlend(float3 Blend, float3 x, float3 y, float3 z)
{
	return x * Blend.x + y * Blend.y + z * Blend.z;
}

float3 BlendNormals(float3 Blend, float3 normalWS, float3 x, float3 y, float3 z)
{
	x = float3(x.xy + normalWS.zy, abs(x.z) * normalWS.x);
	y = float3(y.xy + normalWS.xz, abs(y.z) * normalWS.y);
	z = float3(z.xy + normalWS.xy, abs(z.z) * normalWS.z);
	return float3(normalize(x.zyx * Blend.x + y.xzy * Blend.y + z.xyz * Blend.z));
}

void BlendDefault_float(float3 Blend, float3 InputX, float3 InputY, float3 InputZ, out float3 Result)
{
	Result = DoBlend(Blend, InputX, InputY, InputZ);
}

void BlendNormals_float(float3 Blend, float3 normalWS, float3 normalTSX, float3 normalTSY, float3 normalTSZ, out float3 Result)
{
	Result = BlendNormals(Blend, normalWS, normalTSX, normalTSY, normalTSZ);
}

void BlendDefault_half(half3 Blend, half3 InputX, half3 InputY, half3 InputZ, out half3 Result)
{
	Result = DoBlend(Blend, InputX, InputY, InputZ);
}

void BlendNormals_half(half3 Blend, half3 normalWS, half3 normalTSX, half3 normalTSY, half3 normalTSZ, out half3 Result)
{
	Result = BlendNormals(Blend, normalWS, normalTSX, normalTSY, normalTSZ);
}

void BlendAll_float(
float3 Blend,
float3 NormalWS,
float3 AlbedoX, float3 AlbedoY, float3 AlbedoZ, 
float SmoothnessX, float SmoothnessY, float SmoothnessZ, 
float3 EmissionX, float3 EmissionY, float3 EmissionZ,
float OcclusionX, float OcclusionY, float OcclusionZ,
float MetalnessX, float MetalnessY, float MetalnessZ,
float3 NormalX, float3 NormalY, float3 NormalZ,
out float3 Albedo,
out float Smoothness,
out float3 Emission,
out float Occlusion,
out float Metalness,
out float3 Normals
)
{
	Albedo = DoBlend(Blend, AlbedoX, AlbedoY, AlbedoZ);
	Smoothness = DoBlend(Blend, SmoothnessX, SmoothnessY, SmoothnessZ);
	Emission = DoBlend(Blend, EmissionX, EmissionY, EmissionZ);
	Occlusion = DoBlend(Blend, OcclusionX, OcclusionY, OcclusionZ);
	Metalness = DoBlend(Blend, MetalnessX, MetalnessY, MetalnessZ);
	Normals = BlendNormals(Blend, NormalWS, NormalX, NormalY, NormalZ);
}

void BlendAll_half(
half3 Blend,
half3 NormalWS,
half3 AlbedoX, half3 AlbedoY, half3 AlbedoZ, 
half SmoothnessX, half SmoothnessY, half SmoothnessZ, 
half3 EmissionX, half3 EmissionY, half3 EmissionZ,
half OcclusionX, half OcclusionY, half OcclusionZ,
half MetalnessX, half MetalnessY, half MetalnessZ,
half3 NormalX, half3 NormalY, half3 NormalZ,
out half3 Albedo,
out half Smoothness,
out half3 Emission,
out half Occlusion,
out half Metalness,
out half3 Normals
)
{
	Albedo = DoBlend(Blend, AlbedoX, AlbedoY, AlbedoZ);
	Smoothness = DoBlend(Blend, SmoothnessX, SmoothnessY, SmoothnessZ);
	Emission = DoBlend(Blend, EmissionX, EmissionY, EmissionZ);
	Occlusion = DoBlend(Blend, OcclusionX, OcclusionY, OcclusionZ);
	Metalness = DoBlend(Blend, MetalnessX, MetalnessY, MetalnessZ);
	Normals = BlendNormals(Blend, NormalWS, NormalX, NormalY, NormalZ);
}
#endif