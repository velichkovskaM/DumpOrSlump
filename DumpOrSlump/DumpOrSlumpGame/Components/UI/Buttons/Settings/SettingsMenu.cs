using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.Buttons.Settings;

/// <summary>
/// Settings‑screen background sprite. Draws a static panel behind the settings UI
/// </summary>
public class SettingsMenu : Component
{
    SpriteRenderer _spriteRenderer;
    
    public SettingsMenu(Node parent) : base(parent) { }

    // load background sprite & register animation
    public override void Start(IScene scene)
    {
        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        
        var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
        var background = new Rectangle[]{new Rectangle(256 * 7, 256 * 2, 256 * 3, 256)};
        
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