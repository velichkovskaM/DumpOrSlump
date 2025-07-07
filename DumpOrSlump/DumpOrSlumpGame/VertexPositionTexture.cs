using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame;

/// <summary>
/// Compact vertex layout combining a 3D position and a 2D texture coordinate. Used by custom geometry renderers in Dump Or Slump
/// </summary>
public struct VertexPositionTexture : IVertexType
{
    public Vector3 Position;
    public Vector2 TextureCoordinate;

    // vertex layout declaration
    public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
    );

    // return layout to GPU
    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

    // ctor: assign fields
    public VertexPositionTexture(Vector3 position, Vector2 textureCoordinate)
    {
        Position = position;
        TextureCoordinate = textureCoordinate;
    }
}