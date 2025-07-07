using System;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace DumpOrSlumpGame.Components.UI
{
    /// <summary>
    /// Countdown timer UI component. Displays remaining time, updates each frame, and triggers game‑over on expiry
    /// </summary>
    public class Timer : Component
    {
        
        public double maxTime { get; private set; }
        public double timer { get; private set;  }
        private double total_time;
        private double current_time;

        private SpriteFont font;

        private SpriteRenderer _spriteRenderer;
        private TextRenderer _textRenderer;


        // cache duration, load assets, init text
        public Timer(Node parent, double total_time) : base(parent)
        {
            // All periods:
            var loader = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
            // Animation:
            maxTime = total_time;
            this.total_time = total_time;
            current_time = total_time;
            
            font = Globals.content.Load<SpriteFont>("Fonts/Press Start 2P");

            var timerSprite = new Rectangle[1];
            timerSprite[0] = new Rectangle(256 * 7, 256 * 7, 256, 256);
            
            _spriteRenderer = parent.GetComponent<SpriteRenderer>();
            _spriteRenderer.AddAnimation("idle", new AnimationData(
                loader, timerSprite, 0.2, false, isUI: true
                ));
            
            _textRenderer = parent.GetComponent<TextRenderer>();
            _textRenderer.SetText(TimeSpan.FromSeconds(current_time).ToString(@"mm\:ss"));
            _textRenderer.SetFont(font);
            _textRenderer.SetFontSize(new Vector2(0.7f * Camera.scale, 0.7f * Camera.scale));
            _textRenderer.SetOffset(new Vector2(-75 * Camera.scale, -35 * Camera.scale));
        }
        
        // per‑frame: decrement timer, update UI, handle timeout
        public override void Update(GameTime gameTime, TouchCollection touches)
        {

            if (current_time > 0)
            {
                current_time -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                if (!Game1.isGameWon)
                {
                    current_time = 0;
                    Game1.Instance.SetGameLost("Time ran out");
                }

            }

            timer += gameTime.ElapsedGameTime.TotalSeconds;

            if (!Game1.isGameWon)
            {
                _textRenderer.SetText(TimeSpan.FromSeconds(current_time).ToString(@"mm\:ss"));
            }
        }
    }
}
