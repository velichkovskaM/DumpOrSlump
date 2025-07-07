using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.Buttons.PauseMenu;

/// <summary>
/// Pause‑menu backdrop sprite. Renders a static background behind pause UI elements
/// </summary>
public class PauseMenuBackground : Component
{
    private SpriteRenderer _spriteRenderer;
    
    public PauseMenuBackground(Node parent) : base(parent) { }
    
    // load background sprite & register animation
    public override void Start(IScene scene)
    {
        var assets = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
        
        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        _spriteRenderer.AddAnimation("idle", new AnimationData(
            assets,
            new Rectangle[]{new Rectangle(256 * 6, 256 * 3, 256 * 4, 256 * 2)},
            1,
            false,
            isUI: true
            )
        );
    }
}