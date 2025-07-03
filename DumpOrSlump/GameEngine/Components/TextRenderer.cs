using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameEngine.Components;

/// <summary>
/// Renders text at the parent node's position, using a specified SpriteFont
/// Supports offset, custom font size scaling, and color
/// </summary>
public class TextRenderer : Component
{
    public string Text { get; set; } = "";
    public SpriteFont Font { get; set; }
    public Vector2 Offset { get; set; } = new Vector2(0, 0);
    public Vector2 FontSize { get; set; } = Vector2.One;
    public Color color = Color.White;
    public TextRenderer(Node parent, bool active = true) : base(parent, active) { }

    public void SetText(string text) => Text = text;
    public void SetFont(SpriteFont font) => Font = font;
    public void SetOffset(Vector2 offset) => Offset = offset;
    public void SetColor(Color color) => this.color = color;
    
    public void SetFontSize(Vector2 fontSize)
    {
        FontSize = fontSize;
    }

    public override void Draw(Camera camera, SpriteBatch spriteBatch)
    {
        if (Font != null) spriteBatch.DrawString(
            Font,
            Text,
            new Vector2(Parent.Transform.Position.X + Offset.X, Parent.Transform.Position.Z + Offset.Y),
            color,
            0.0f,
            Vector2.Zero,
            FontSize,
            SpriteEffects.None,
            0
        );
    }
}