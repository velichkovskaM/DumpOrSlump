using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework.Audio;

namespace DumpOrSlumpGame.Components.UI.Buttons.HelpMenu
{
    /// <summary>
    /// Stats‑screen exit button. Supplies sprite frames, click SFX, and hides the stats menu when released
    /// </summary>
    public class ButtonStatsExit(Node parent) : ButtonComponent(parent)
    {
        // load textures, animations & click SFX
        public override (Texture2D asset, AnimationData nonClicked, AnimationData clicked, SoundEffectInstance soundEffect) GetButtonData()
        {
            var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
            var _soundEffect = Globals.content.Load<SoundEffect>("SoundEffect/Click").CreateInstance();
            _soundEffect.Volume = 1.0f;
            var clickedAnimation = new AnimationData(
                asset,
                [new Rectangle(256 * 1, 256 * 7, 256, 256)],
                0.2f,
                false,
                isUI: true
            );
            var normalAnimation = new AnimationData(
                asset,
                [new Rectangle(256 * 0, 256 * 7, 256, 256)],
                0.2f,
                false,
                isUI: true
            );
            
            return (asset, normalAnimation, clickedAnimation, _soundEffect);
        }
        
        // hide stats menu & play sound
        public override void OnRelease()
        {
            var scene = Game1.GetScene();
            scene.FindNodeByName("statsMenuBackground").SetActive(false);
            _soundEffect.Play();
        }
    }
}