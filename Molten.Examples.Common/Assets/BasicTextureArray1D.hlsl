﻿struct VS_IN
{
    float4 pos : POSITION;
    float3 uv : TEXCOORD;
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float3 uv : TEXCOORD;
};

cbuffer Common : register(b0)
{
    float4x4 view : packoffset(c0);
    float4x4 projection : packoffset(c4);
    float4x4 viewProjection : packoffset(c8);
    float4x4 invViewProjection : packoffset(c12);
}

cbuffer Object : register(b1)
{
    float4x4 wvp : packoffset(c0);
    float4x4 world : packoffset(c4);
}

Texture1DArray mapAlbedo;
SamplerState texSampler;

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN) 0;
    output.pos = mul(input.pos, wvp);
    output.uv = input.uv;
    return output;
}

float4 PS(PS_IN input) : SV_Target
{
    return mapAlbedo.Sample(texSampler, input.uv.xz);
}