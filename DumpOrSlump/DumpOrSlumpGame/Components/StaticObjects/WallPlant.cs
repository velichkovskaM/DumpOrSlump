using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.StaticObjects;

/// <summary>
/// Decorative wall plant sprite. Loads a single frame and registers it with the scene’s SpriteRenderer
/// </summary>
public class WallPlant : Component
{
    private Texture2D animation;
    private Rectangle[] wallPlant;
    SpriteRenderer _spriteRenderer;
    
    public WallPlant(Node parent) : base(parent) { }

    // load sprite & add static animation
    public override void Start(IScene scene)
    {
        animation = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
        wallPlant = new Rectangle[1];
        wallPlant[0] = new Rectangle(256 * 12, 256 * 2, 256, 256);

        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        _spriteRenderer.AddAnimation("static", new AnimationData(animation, wallPlant, 0.2f, false, Render2D: false));
    }
}