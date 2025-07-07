using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI;

/// <summary>
/// Individual stamina heart icon. Provides full/half/empty states and switches animation on demand
/// </summary>
public class StaminaHeart : Component
{
    private SpriteRenderer _spriteRenderer;
    
    public StaminaHeart(Node parent, bool active = true) : base(parent, active) { }

    // load heart sprites & register animations
    public override void Start(IScene scene)
    {
        var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        
        _spriteRenderer.AddAnimation("full", new AnimationData(
            asset,
            new []{new Rectangle(256 * 6, 256 * 7, 256, 256)},
            1,
            false,
            isUI: true
            ));
        
        _spriteRenderer.AddAnimation("half", new AnimationData(
            asset,
            new []{new Rectangle(256 * 5, 256 * 7, 256, 256)},
            1,
            false,
            isUI: true
        ));
        
        _spriteRenderer.AddAnimation("empty", new AnimationData(
            asset,
            new []{new Rectangle(256 * 4, 256 * 7, 256, 256)},
            1,
            false,
            isUI: true
        ));
    }

    // switch to full heart
    public void SetFull()
    {
        _spriteRenderer.SetAnimation("full");
    }

    // switch to half heart
    public void SetHalf()
    {
        _spriteRenderer.SetAnimation("half");
    }

    // switch to empty heart
    public void SetEmpty()
    {
        _spriteRenderer.SetAnimation("empty");
    }
}