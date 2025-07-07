using System.Text;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.MainMenu;

/// <summary>
/// Displays the credits screen. Configures fonts, sizes, colors, and positions for the title and body text
/// </summary>
public class CreditScreen : Component
{
    private TextRenderer _mainBodyTextRenderer;
    private TextRenderer _titleTextRenderer;
    
    public CreditScreen(Node parent, bool active = true) : base(parent, active) { }

    // setup fonts, compose credit lines, position title & body
    public override void Start(IScene scene)
    {
        var font = Globals.content.Load<SpriteFont>("Fonts/Press Start 2P");

        var textRenderes = Parent.GetComponents<TextRenderer>();
        _titleTextRenderer = textRenderes[1];
        _titleTextRenderer.SetFont(font);
        _titleTextRenderer.SetColor(Color.Black);
        var titleFontSize = new Vector2(0.75f * Camera.scale, 0.75f * Camera.scale);
        _titleTextRenderer.SetFontSize(titleFontSize);
        
        _mainBodyTextRenderer = textRenderes[0];
        _mainBodyTextRenderer.SetFont(font);
        _mainBodyTextRenderer.SetColor(Color.Black);
        var fontsize = new Vector2(0.4f * Camera.scale, 0.4f * Camera.scale);
        _mainBodyTextRenderer.SetFontSize(fontsize);
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"UI and in-game sound effects: ZapSplat");
        sb.AppendLine($"Background music: Pixabay");
        sb.AppendLine($"Game development and 2D/3D art: Sadge Games");
        string s = sb.ToString();
        _mainBodyTextRenderer.SetText(s);
        _titleTextRenderer.SetText("Credits:");

        var titleMeasuredSize = font.MeasureString("Credits:") * titleFontSize;
        
        var measuredSize = font.MeasureString(s) * fontsize;
        
        var textOffset = new Vector2(-measuredSize.X / 2, 0);
        _mainBodyTextRenderer.SetOffset(textOffset);

        textOffset.Y -= titleMeasuredSize.Y;
        _titleTextRenderer.SetOffset(textOffset);
            
    }
}