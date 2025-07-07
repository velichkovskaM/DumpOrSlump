using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI;

/// <summary>
/// Animated decorative dog sprite for the MainMenu. Loads a 3‑frame sleep animation and registers it with the SpriteRenderer
/// </summary>
public class UIDog : Component
{
    public UIDog(Node parent, bool active = true) : base(parent, active)
    {
        var texture = Globals.content.Load<Texture2D>("SpriteSheets/DoggoSpriteSheet");
        var animation_sleep = new Rectangle[3];
        animation_sleep[0] = new Rectangle(256 * 5, 256 , 256, 256);
        animation_sleep[1] = new Rectangle(256 * 6, 256, 256, 256);
        animation_sleep[2] = new Rectangle(256 * 7, 256, 256, 256);
        
        var spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        
        spriteRenderer.AddAnimation("sleep", new AnimationData(
            texture,
            animation_sleep,
            0.2,
            isUI: true
        ));
    }
}