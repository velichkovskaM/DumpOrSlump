using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.Buttons;

/// <summary>
/// Main‑menu Start button. Supplies sprite frames, click SFX, and requests level change on release
/// </summary>
public class StartButton : ButtonComponent
{
    public StartButton(Node parent) : base(parent) { }

    // load textures, animations & click SFX
    public override (Texture2D asset, AnimationData nonClicked, AnimationData clicked, SoundEffectInstance soundEffect) GetButtonData()
    {
        var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
        var _soundEffect = Globals.content.Load<SoundEffect>("SoundEffect/Click").CreateInstance();
        _soundEffect.Volume = 1.0f;
        
        var clickedAnimation = new AnimationData(
            asset,
            new Rectangle[]{new Rectangle(256 * 2, 256 * 4, 256, 256)},
            0.2f,
            false,
            isUI: true
        );
        
        var normalAnimation = new AnimationData(
            asset,
            new Rectangle[]{new Rectangle(256 * 2, 256 * 3, 256, 256)},
            0.2f,
            false,
            isUI: true
        );
       
        return (asset, normalAnimation, clickedAnimation, _soundEffect);
    }

    // request level change & play sound
    public override void OnRelease()
    {
        Game1.Instance.RequestChangeLevel(Game1.Instance.requestedLevelToChangeTo);
        _soundEffect.Play();
    }
}