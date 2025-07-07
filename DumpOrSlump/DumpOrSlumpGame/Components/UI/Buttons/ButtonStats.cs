using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.Buttons
{
    /// <summary>
    /// Stats-menu button. Supplies sprite frames, click SFX, and opens the stats menu when released
    /// </summary>
    public class ButtonStats : ButtonComponent
    {
        public ButtonStats(Node parent) : base(parent) { }
        
        // load textures, animations & click SFX
        public override (Texture2D asset, AnimationData nonClicked, AnimationData clicked, SoundEffectInstance soundEffect) GetButtonData()
        {
            var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
            var _soundEffect = Globals.content.Load<SoundEffect>("SoundEffect/Click").CreateInstance();
            _soundEffect.Volume = 1.0f;
            var clickedAnimation = new AnimationData(
                asset,
                new Rectangle[]{new Rectangle(256 * 12, 256 * 6, 256, 256)},
                0.2f,
                false,
                isUI: true
            );
            var normalAnimation = new AnimationData(
                asset,
                new Rectangle[]{new Rectangle(256 * 11, 256 * 6, 256, 256)},
                0.2f,
                false,
                isUI: true
            );
            
            return (asset, normalAnimation, clickedAnimation, _soundEffect);
        }
        
        // show stats menu & play sound
        public override void OnRelease()
        {
            var scene = Game1.GetScene();
            scene.FindNodeByName("statsMenuBackground").SetActive(true);
            _soundEffect.Play();
        }
    }
}