#include "gbuffer_common.hlsl"

struct VS_IN
{
	float4 pos			: POSITION0;
	float3 normal		: NORMAL0;
	float3 tangent		: TANGENT0;
	float3 binormal		: BINORMAL0;
	float2 uv			: TEXCOORD0;
};

cbuffer cbShadowGenVS : register(b0)
{
	float4x4 ShadowMat : packoffset(c0);
}

// VERTEX SHADER
VS_OUT VS(VS_IN input)
{
  VS_OUT o;

  // NOTE ********* WVP should be calculated once per object on the CPU. NOT per vertex.
  float4 worldPos = mul(input.pos, world);
  float4 viewPos = mul(worldPos, view);
  o.pos = mul(viewPos, projection);

  o.normal = mul(input.normal, (float3x3)world);
  o.normal = normalize(o.normal);
  o.uv = input.uv;

  // Calculate the tangent vector against the world matrix only and then normalize the final value.
  o.tangent = mul(input.tangent, (float3x3)world);
  o.tangent = normalize(o.tangent);

  // Calculate the binormal vector against the world matrix only and then normalize the final value.
  o.binormal = mul(input.binormal, (float3x3)world);
  o.binormal = normalize(o.binormal);

  return o;
}

PS_OUT PS(VS_OUT input)
{
  PS_OUT o;
  o.diffuse = mapDiffuse.Sample(texSampler, input.uv);            //output Color

  // Calculate normals
  float3 nMap = mapNormal.Sample(texSampler, input.uv).rgb;
  float3 glow = mapGlow.Sample(texSampler, input.uv).rgb;

  // Expand the range of the normal value from (0, +1) to (-1, +1).
  nMap = (nMap * 2.0f) - 1.0f;

  float3 normal = (nMap.x * input.tangent) + (nMap.y * input.binormal) + (nMap.z * input.normal);
	  normal = normalize(normal);

  o.normal.rgb = 0.5 * (normal + 1.0);
  o.emissive.rgb = glow * emissivePower;

  // UNUSED
  // colorData.a
  // emissive.a
  o.emissive.a = 1;
  o.diffuse.a = 1;

  return o;
}

PS_OUT PS_Basic(VS_OUT input)
{
	PS_OUT o;
	o.diffuse = mapDiffuse.Sample(texSampler, input.uv);
	o.emissive = mapGlow.Sample(texSampler, input.uv);
	o.normal.rgb = 0.5 * (input.normal + 1.0);

	// UNUSED
	// colorData.a
	// emissive.a
	// there is no normal.a because of 11,11,10 bit channels.
	o.emissive.a = 1;
	o.diffuse.a = 1;

	return o;
}