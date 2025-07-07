using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace DumpOrSlumpGame.Components.UI.Buttons;

/// <summary>
/// Button that unequips the player's currently held item. Provides sprite frames, click SFX, and triggers detach logic on release
/// </summary>
public class DeEquip : ButtonComponent
{
    private SpriteRenderer _spriteRenderer;
    
    private BoundingBox _boundingBox;
    
    private bool clicked = false;

    private int touchId = 0;
    
    public DeEquip(Node parent) : base(parent) { }
    
    // load textures, animations & click SFX
    public override (Texture2D asset, AnimationData nonClicked, AnimationData clicked, SoundEffectInstance soundEffect) GetButtonData()
    {
        var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
        var _soundEffect = Globals.content.Load<SoundEffect>("SoundEffect/Click").CreateInstance();
        _soundEffect.Volume = 1.0f;
        var clickedAnimation = new AnimationData(
            asset,
            new Rectangle[]{new Rectangle(256 * 3, 256 * 7, 256, 256)},
            0.2f,
            false,
            isUI: true
        );
        var normalAnimation = new AnimationData(
            asset,
            new Rectangle[]{new Rectangle(256 * 2, 256 * 7, 256, 256)},
            0.2f,
            false,
            isUI: true
        );
            
        return (asset, normalAnimation, clickedAnimation, _soundEffect);
    }

    // detach equipment from player
    public override void OnRelease()
    {
        Game1.GetScene().FindNodeByName("Player").GetComponent<Player>().detachEquipment();
    }
}