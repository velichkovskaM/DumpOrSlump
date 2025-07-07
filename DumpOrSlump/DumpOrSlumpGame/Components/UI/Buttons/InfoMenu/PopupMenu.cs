using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.Buttons.HelpMenu;

/// <summary>
/// Popup message window. Renders a static background sprite and centered text string supplied at construction time
/// </summary>
public class PopupMenu : Component
{
    private SpriteRenderer _spriteRenderer;
    private TextRenderer textRenderer;
    private string text;

    // attach to parent, store message text
    public PopupMenu(Node parent, bool active = true, string text = "") : base(parent, active)
    {
        this.text = text;
    }

    // load background sprite & call text setup
    public override void Start(IScene scene)
    {
        var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
        var background = new [] {new Rectangle(256 * 11, 0, 256 * 3, 256)};

        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        _spriteRenderer.AddAnimation("idle", new AnimationData(
            asset,
            background,
            1,
            false,
            isUI: true
            ));
        SetupTextDisplay();
    }

    // configure font, color, size, offset & set text
    public void SetupTextDisplay()
    {
        var font = Globals.content.Load<SpriteFont>("Fonts/Press Start 2P");

        textRenderer = Parent.GetComponent<TextRenderer>();

        textRenderer.SetFont(font);
        textRenderer.SetColor(Color.Black);
        var fontsize = new Vector2(0.5f, 0.5f);
        textRenderer.SetFontSize(fontsize);
        var distance = font.MeasureString(text) / 2;
        textRenderer.SetOffset(new Vector2(-distance.X * textRenderer.FontSize.X, -distance.Y * textRenderer.FontSize.Y));
        
        textRenderer.SetText(text);
    }
}