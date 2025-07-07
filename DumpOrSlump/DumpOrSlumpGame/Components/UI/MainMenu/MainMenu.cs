using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.MainMenu;

/// <summary>
/// Main‑menu background screen. Renders a full‑screen static image behind the UI buttons
/// </summary>
public class MainMenu : Component
{
    private SpriteRenderer _spriteRenderer;
    
    public MainMenu(Node parent, bool active = true) : base(parent, active) { }

    // load background texture & register animation
    public override void Start(IScene scene)
    {
        var asset = Globals.content.Load<Texture2D>("SpriteSheets/MainPage");
        
        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        
        _spriteRenderer.AddAnimation("idle", new AnimationData(
            asset,
            new []{new Rectangle(0, 0, 2208, 1242)},
            1,
            false,
            isUI: true
            ));
    }
}