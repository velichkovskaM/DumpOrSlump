using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.Buttons
{
    /// <summary>
    /// Button that requests a transition to the specified next level. Provides sprite frames and click SFX
    /// </summary>
    public class ButtonNextLevel : ButtonComponent
    {
        private LevelEnum nextLevel;

        // attach to parent, store target level
        public ButtonNextLevel(Node parent, LevelEnum nextLevel) : base(parent)
        {
            this.nextLevel = nextLevel;
        }
        
        // load textures, animations & click SFX
        public override (Texture2D asset, AnimationData nonClicked, AnimationData clicked, SoundEffectInstance soundEffect) GetButtonData()
        {
            var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
            var _soundEffect = Globals.content.Load<SoundEffect>("SoundEffect/Restart").CreateInstance();
            _soundEffect.Volume = 1.0f;
            var clickedAnimation = new AnimationData(
                asset,
                new Rectangle[]{new Rectangle(256 * 12, 256 * 8, 256, 256)},
                0.2f,
                false,
                isUI: true
            );
            var normalAnimation = new AnimationData(
                asset,
                new Rectangle[]{new Rectangle(256 * 11, 256 * 8, 256, 256)},
                0.2f,
                false,
                isUI: true
            );
            
            return (asset, normalAnimation, clickedAnimation, _soundEffect);
        }

        // request level change & play sound
        public override void OnRelease()
        {
            Game1.Instance.RequestChangeLevel(nextLevel);
            _soundEffect.Play();
        }
    }
}