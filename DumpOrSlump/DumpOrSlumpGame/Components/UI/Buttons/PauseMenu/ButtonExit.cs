using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.Buttons.PauseMenu
{
    /// <summary>
    /// Pause‑menu exit button. Supplies sprite frames, click SFX, hides the pause UI, and resumes gameplay when released
    /// </summary>
    public class ButtonExit : ButtonComponent
    {
        // attach to parent
        public ButtonExit(Node parent): base(parent) { }
        
        // Load textures, animations & click SFX
        public override (Texture2D asset, AnimationData nonClicked, AnimationData clicked, SoundEffectInstance soundEffect) GetButtonData()
        {
            var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
            var _soundEffect = Globals.content.Load<SoundEffect>("SoundEffect/Click").CreateInstance();
            _soundEffect.Volume = 1.0f;
            var clickedAnimation = new AnimationData(
                asset,
                new Rectangle[]{new Rectangle(256 * 1, 256 * 7, 256, 256)},
                0.2f,
                false,
                isUI: true
            );
            var normalAnimation = new AnimationData(
                asset,
                new Rectangle[]{new Rectangle(256 * 0, 256 * 7, 256, 256)},
                0.2f,
                false,
                isUI: true
            );

            return (asset, normalAnimation, clickedAnimation, _soundEffect);
        }

        // Resume gameplay, hide pause menu & play sound
        public override void OnRelease()
        {
            Game1.pause = false;
            var scene = Game1.GetScene();
            scene.FindNodeByName("PauseMenuBackground").SetActive(false);
            scene.FindNodeByName("DeEquipButton").SetActive(true);
            scene.FindNodeByName("Controller").SetActive(true);
            scene.FindNodeByName("Timer").SetActive(true);
            scene.FindNodeByName("PauseButton").SetActive(true);
            scene.FindNodeByName("StaminaController").SetActive(true);
            _soundEffect.Play();
        }
    }
}