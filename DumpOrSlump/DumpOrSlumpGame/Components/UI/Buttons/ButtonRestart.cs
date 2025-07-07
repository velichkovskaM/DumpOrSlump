using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.Buttons
{
    /// <summary>
    /// Restart button. Supplies sprite frames, restart SFX, and restarts the level when released
    /// </summary>
    public class ButtonRestart : ButtonComponent
    {
        public ButtonRestart(Node parent) : base(parent) { }
        
        // load textures, animations & restart SFX
        public override (Texture2D asset, AnimationData nonClicked, AnimationData clicked, SoundEffectInstance soundEffect) GetButtonData()
        {
            var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
            var _soundEffect = Globals.content.Load<SoundEffect>("SoundEffect/Restart").CreateInstance();
            _soundEffect.Volume = 1.0f;
            var clickedAnimation = new AnimationData(
                asset,
                new Rectangle[]{new Rectangle(256 * 1, 256 * 6, 256, 256)},
                0.2f,
                false,
                isUI: true
            );
            var normalAnimation = new AnimationData(
                asset,
                new Rectangle[]{new Rectangle(256 * 0, 256 * 6, 256, 256)},
                0.2f,
                false,
                isUI: true
            );
            
            return (asset, normalAnimation, clickedAnimation, _soundEffect);
        }

        // request restart & play sound
        public override void OnRelease()
        {
            Game1.Instance.RequestRestart();
            _soundEffect.Play();
        }
    }
}