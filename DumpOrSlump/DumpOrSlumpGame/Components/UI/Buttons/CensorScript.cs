using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.Buttons;

/// <summary>
/// Censor bar overlay sprite. Renders a sprite to put UI more in focus
/// </summary>
public class CensorScript : Component
{
    private SpriteRenderer _spriteRenderer;
    
    public CensorScript(Node parent, bool active = true) : base(parent, active) { }


    // load sprite & add static animation
    public override void Start(IScene scene)
    {
        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        
        var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
        var staticAnimation = new AnimationData(
            asset,
            new Rectangle[]{new Rectangle(256 * 8, 256 * 7, 256, 256)},
            0.2f,
            false,
            isUI: true
        );
        
        _spriteRenderer.AddAnimation("idle", staticAnimation);
    }
}