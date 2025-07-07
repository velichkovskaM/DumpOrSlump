// Transformation matrices
float4x4 World;
float4x4 ViewProjection;
float4x4 WorldInverseTranspose; // For transforming normals

// Lighting parameters
float3 LightPositions[8];  // Array of positions for 8 point lights in world space
float3 LightColors[8];     // Array of colors/intensities for the lights
float LightRanges[8];      // Array of maximum ranges for the lights
float3 AmbientColor;       // Ambient light color

// Base color and texture toggle
float4 ObjectColor;
bool UseTexture;

// Texture and sampler
texture ObjectTexture;
sampler TextureSampler = sampler_state
{
    Texture = <ObjectTexture>;
};

// Vertex shader input structure
struct VertexIn
{
    float4 Position : POSITION0;  // Position as float4 (x, y, z, w)
    float3 Normal   : NORMAL0;    // Vertex normal for lighting
    float2 TexCoord : TEXCOORD0;  // Texture coordinates (UV)
};

// Vertex shader output structure
struct VertexOut
{
    float4 Position    : POSITION;   // Transformed position
    float3 WorldPos    : TEXCOORD0;  // Position in world space for lighting
    float3 WorldNormal : TEXCOORD1;  // Normal in world space
    float2 TexCoord    : TEXCOORD2;  // Pass-through texture coordinates
};

// Vertex Shader
VertexOut VS(VertexIn input)
{
    VertexOut output;
    // Transform position from object space to world space
    float4 worldPos = mul(input.Position, World);
    output.WorldPos = worldPos.xyz;
    // Transform to clip space
    output.Position = mul(worldPos, ViewProjection);
    // Transform normal to world space and normalize
    float3 worldNormal = mul(input.Normal, (float3x3)WorldInverseTranspose);
    output.WorldNormal = normalize(worldNormal);
    // Pass texture coordinates
    output.TexCoord = input.TexCoord;
    return output;
}

// Pixel Shader
float4 PS(VertexOut input) : COLOR
{
    // Base color from texture or solid color using a ternary operator
    float4 baseColor = UseTexture ? tex2D(TextureSampler, input.TexCoord) : ObjectColor;

    // Normalize the interpolated normal
    float3 normal = normalize(input.WorldNormal);

    // Initialize total diffuse lighting contribution
    float3 totalDiffuse = 0.0;

    // Loop over all 8 lights to accumulate diffuse lighting
    for (int i = 0; i < 8; i++)
    {
        // Calculate light direction and distance
        float3 lightDir = LightPositions[i] - input.WorldPos;
        float distance = length(lightDir);
        lightDir = normalize(lightDir);

        // Diffuse lighting (Lambertian)
        float diffuseFactor = max(dot(normal, lightDir), 0.0);

        // Linear attenuation based on distance
        float attenuation = 1.0 - saturate(distance / LightRanges[i]);

        // Accumulate this light's contribution
        totalDiffuse += LightColors[i] * diffuseFactor * attenuation;
    }

    // Combine ambient and diffuse lighting with base color
    float3 finalColor = AmbientColor + baseColor.rgb * totalDiffuse;

    return float4(finalColor, baseColor.a); // Preserve alpha from base color
}

technique SimpleTechnique
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS();
    }
}