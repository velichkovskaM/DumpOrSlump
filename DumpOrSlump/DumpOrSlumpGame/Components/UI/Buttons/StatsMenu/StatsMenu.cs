using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.Buttons.HelpMenu;

/// <summary>
/// Stats display panel. Shows stored run statistics or fallback text, with optional scrolling background sprite
/// </summary>
public class StatsMenu : Component
{
    private SpriteRenderer _spriteRenderer;
    private TextRenderer _mainBodyTextRenderer;
    private TextRenderer _titleTextRenderer;
    
    public StatsMenu(Node parent, bool active = true) : base(parent, active) { }

    // load background sprite and prepare text renderers
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
            )
        );
    }
}