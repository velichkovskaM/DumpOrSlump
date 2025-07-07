using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.Buttons
{
    /// <summary>
    /// Pause menu button that returns the player to the main menu from gameplay. Plays a sound and flags level change
    /// </summary>
    public class ButtonMenu : ButtonComponent
    {

        public ButtonMenu(Node parent) : base(parent) { }
        
        // load textures, animations & click SFX
        public override (Texture2D asset, AnimationData nonClicked, AnimationData clicked, SoundEffectInstance soundEffect) GetButtonData()
        {
            var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
            var _soundEffect = Globals.content.Load<SoundEffect>("SoundEffect/Restart").CreateInstance();
            _soundEffect.Volume = 1.0f;
            var clickedAnimation = new AnimationData(
                asset,
                new Rectangle[]{new Rectangle(256 * 7, 256 * 5, 256, 256)},
                0.2f,
                false,
                isUI: true
            );
            var normalAnimation = new AnimationData(
                asset,
                new Rectangle[]{new Rectangle(256 * 6, 256 * 5, 256, 256)},
                0.2f,
                false,
                isUI: true
            );
            
            return (asset, normalAnimation, clickedAnimation, _soundEffect);
        }

        // set last run data, request main‑menu level & play sound
        public override void OnRelease()
        {
            Game1.Instance.SetLastRunData(true, "Went to main menu");
            Game1.Instance.RequestChangeLevel(LevelEnum.MainMenu);
            _soundEffect.Play();
        }
    }
}