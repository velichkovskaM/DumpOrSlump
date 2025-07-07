using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.Buttons.Settings
{
    /// <summary>
    /// Settings‑menu exit button. Supplies sprite frames, click SFX, and closes the settings menu when released
    /// </summary>
    public class ButtonSettingsExit : ButtonComponent
    {
        public ButtonSettingsExit(Node parent) : base(parent) { }

        // load textures, animations & click SFX
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

        // hide settings menu, restore main buttons & play sound
        public override void OnRelease()
        {
            var scene = Game1.GetScene();
            scene.FindNodeByName("settingsMenuBackground")?.SetActive(false);
            scene.FindNodeByName("HelpButton")?.SetActive(true);
            scene.FindNodeByName("SettingsButton")?.SetActive(true);
            scene.FindNodeByName("StartButton")?.SetActive(true);
            _soundEffect.Play();
        }
    }
}