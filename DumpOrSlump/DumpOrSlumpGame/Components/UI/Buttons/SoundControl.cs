using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework.Audio;

namespace DumpOrSlumpGame.Components.UI.Buttons
{
    /// <summary>
    /// Toggle button for enabling/disabling all sound. Maintains on/off state, updates settings, and adjusts master volume
    /// </summary>
    public class SoundControl : StateButtonComponent
    {
        public SoundControl(Node parent) : base(parent) { }
        
        // load textures, animations & initial state
        public override (Texture2D asset, AnimationData nonClicked, AnimationData clicked, bool initialState) GetButtonData()
        {
            var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
            var clickedAnimation = new AnimationData(
                asset,
                new Rectangle[]{new Rectangle(256 * 8, 256 * 6, 256, 256)},
                0.2f,
                false,
                isUI: true
            );
            var normalAnimation = new AnimationData(
                asset,
                new Rectangle[]{new Rectangle(256 * 9, 256 * 6, 256, 256)},
                0.2f,
                false,
                isUI: true
            );
            state = GameEngine.SaveAPI.settings.sound_on_off;
            
            return (asset, normalAnimation, clickedAnimation, state);
        }

        // toggle visual state & call On/Off handlers
        public override void OnClick()
        {
            Logger.Error($"Sound control clicked with state: {state}");
            
            if (state == false)
            {
                _spriteRenderer.SetAnimation("on");
                state = true;
                On();
            }
            else
            {
                _spriteRenderer.SetAnimation("off");
                state = false;
                Off();
            }
        }

        // enable audio & persist setting
        public override void On()
        {
            GameEngine.SaveAPI.settings.sound_on_off = true;
            MusicController.setIsDeaf(false);
            SoundEffect.MasterVolume = SaveAPI.settings.sound_on_off ? SaveAPI.settings.volume : 0;
            GameEngine.SaveAPI.SaveSettingsFile();
        }

        // disable audio & persist setting
        public override void Off()
        {
            GameEngine.SaveAPI.settings.sound_on_off = false;
            MusicController.setIsDeaf(true);
            SoundEffect.MasterVolume = SaveAPI.settings.sound_on_off ? SaveAPI.settings.volume : 0;
            GameEngine.SaveAPI.SaveSettingsFile();
        }
    }
}
