// PixelationEffect.fx

texture SceneTexture;
sampler2D SceneSampler = sampler_state
{
    Texture = <SceneTexture>;
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

float PixelSize = 0.1;
float2 Resolution;

struct VertexOut
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

VertexOut VS_PassThrough(float4 Position : POSITION, float2 TexCoord : TEXCOORD0)
{
    VertexOut output;
    output.Position = Position;
    output.TexCoord = TexCoord;
    return output;
}

float4 PS_Grayscale(VertexOut input) : COLOR
{
    float4 color = tex2D(SceneSampler, input.TexCoord);
    float gray = dot(color.rgb, float3(0.299, 0.587, 0.114));
    return float4(gray, gray, gray, color.a);
}

float4 PS_Pixelate(VertexOut input) : COLOR
{
    if (Resolution.x <= 0 || Resolution.y <= 0)
    {
        return float4(1, 0, 0, 1); // Red if Resolution is invalid
    }

    if (PixelSize <= 0)
    {
        return float4(0, 1, 0, 1); // Green if PixelSize is invalid
    }

    float2 pixelSize = PixelSize / Resolution;
    float2 snappedCoord = floor(input.TexCoord / pixelSize) * pixelSize;
    snappedCoord = clamp(snappedCoord, 0, 1);

    float4 color = tex2D(SceneSampler, snappedCoord);

    //if (color.r == 0 && color.g == 0 && color.b == 0)
    //{
    //    return float4(0, 0, 1, 1); // Blue if the sampled color is black
    //}

    return color;
}

technique SimpleEffect
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS_PassThrough();
        PixelShader  = compile ps_3_0 PS_Grayscale();
    }
}

technique PixelationTechnique
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS_PassThrough();
        PixelShader  = compile ps_3_0 PS_Pixelate();
    }
}