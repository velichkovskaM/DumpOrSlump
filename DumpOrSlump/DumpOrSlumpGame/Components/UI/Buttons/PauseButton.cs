using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.Buttons.MenuButtons
{
    /// <summary>
    /// Pause button displayed during gameplay. Opens the pause menu, disables gameplay UI, and pauses the game when released
    /// </summary>
    public class PauseButton : ButtonComponent
    {
        public PauseButton(Node parent) : base(parent) { }
        
        // load textures, animations & click SFX
        public override (Texture2D asset, AnimationData nonClicked, AnimationData clicked, SoundEffectInstance soundEffect) GetButtonData()
        {
            var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
            var _soundEffect = Globals.content.Load<SoundEffect>("SoundEffect/Click").CreateInstance();
            _soundEffect.Volume = 1.0f;
            var clickedAnimation = new AnimationData(
                asset,
                new Rectangle[]{new Rectangle(256 * 9, 256 * 5, 256, 256)},
                0.2f,
                false,
                isUI: true
            );
            var normalAnimation = new AnimationData(
                asset,
                new Rectangle[]{new Rectangle(256 * 8, 256 * 5, 256, 256)},
                0.2f,
                false,
                isUI: true
            );
            
            return (asset, normalAnimation, clickedAnimation, _soundEffect);
        }

        // toggle pause UI & play sound
        public override void OnRelease()
        {
            if (!Game1.pause)
            {
                Game1.pause = true;
                var scene = Game1.GetScene();
                scene.FindNodeByName("PauseMenuBackground").SetActive(true);
                scene.FindNodeByName("DeEquipButton").SetActive(false);
                scene.FindNodeByName("Controller").SetActive(false);
                scene.FindNodeByName("Timer").SetActive(false);
                scene.FindNodeByName("PauseButton").SetActive(false);
                scene.FindNodeByName("StaminaController").SetActive(false);
                _soundEffect.Play();
            }
        }
    }
}