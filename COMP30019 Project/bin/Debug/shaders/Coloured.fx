// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

struct VS_COLOURED_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
	float4 nrm : NORMAL;
};

struct PS_COLOURED_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
	float4 nrm : NORMAL;
};

//This variable will be put in constant buffer
//float4x4 worldViewProj;

cbuffer shaderGlobals
{
	float4x4 worldViewProj;
	float4 eyePos4;
	float4 lightAmbCol;
	float4 lightPntPos;
	float4 lightPntCol;
}

PS_COLOURED_IN VS_COLOURED( VS_COLOURED_IN input )
{
	/*PS_COLOURED_IN output = (PS_COLOURED_IN)0;
	
	output.pos = mul(float4(input.pos, 1.0f), worldViewProj);
	output.col = input.col;*/
	
	PS_COLOURED_IN output = (PS_COLOURED_IN)0;
	

	// Calculate ambient RGB intensities
	float Ka = 1;
	float3 amb = input.col.rgb*lightAmbCol.rgb*Ka;

	// Calculate diffuse RBG reflections
	float fAtt = 1;
	float Kd = 1;
	float4 L = normalize(lightPntPos - input.pos);
	float3 dif = fAtt*lightPntCol.rgb*Kd*input.col.rgb*saturate(dot(normalize(input.nrm),L));

	// Calculate specular reflections
	float Ks = 1;
	float specN = 1;
	float4 V = normalize(eyePos4 - input.pos);
	float4 R = float4(0,0,0,0);
	float3 spe = fAtt*lightPntCol.rgb*Ks*pow(saturate(dot(V,R)),specN);

	// Combine reflection components
	output.col.rgb = amb.rgb+dif.rgb+spe.rgb;
	output.col.a = input.col.a;

	// Convert Vertex position into eye coords
	output.pos = mul(input.pos, worldViewProj);

	return output;
}

float4 PS_COLOURED( PS_COLOURED_IN input ) : SV_Target
{
	return input.col;
}
