using System.Text;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.UI.MainMenu;

/// <summary>
/// Stats screen presented on the main menu. Displays previous run statistics or fallback text when none are available
/// Configures fonts, text sizes, colors, and positions dynamically
/// </summary>
public class StatsScreen : Component
{
    private TextRenderer _mainBodyTextRenderer;
    private TextRenderer _titleTextRenderer;
    
    public StatsScreen(Node parent, bool active = true) : base(parent, active) { }

    // load font, set up text renderers, populate stats
    public override void Start(IScene scene)
    {
        var font = Globals.content.Load<SpriteFont>("Fonts/Press Start 2P");

        var textRenderes = Parent.GetComponents<TextRenderer>();
        _titleTextRenderer = textRenderes[1];
        _titleTextRenderer.SetFont(font);
        _titleTextRenderer.SetColor(Color.Black);
        var titleFontSize = new Vector2(0.75f * Camera.scale, 0.75f * Camera.scale);
        _titleTextRenderer.SetFontSize(titleFontSize);
        
        _mainBodyTextRenderer = textRenderes[0];
        _mainBodyTextRenderer.SetFont(font);
        _mainBodyTextRenderer.SetColor(Color.Black);
        var fontsize = new Vector2(0.4f * Camera.scale, 0.4f * Camera.scale);
        _mainBodyTextRenderer.SetFontSize(fontsize);

        var statsAvailable = GameEngine.SaveAPI.settings.have_played;
        
        if (statsAvailable)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Clutter picked up last run: {GameEngine.SaveAPI.settings.clothes_picked_up}");
            sb.AppendLine($"Dust picked up last run: {GameEngine.SaveAPI.settings.dust_picked_up}");
            sb.AppendLine($"Clothes picked up last run: {GameEngine.SaveAPI.settings.clothes_picked_up}");
            sb.AppendLine($"Clutter missed last run: {GameEngine.SaveAPI.settings.clutter_missed}");
            sb.AppendLine($"Dust missed last run: {GameEngine.SaveAPI.settings.dust_missed}");
            sb.AppendLine($"Clothes missed last run: {GameEngine.SaveAPI.settings.clutter_missed}");
            sb.AppendLine($"Died last run: {(GameEngine.SaveAPI.settings.died ? "Yes" : "No")}");
            sb.AppendLine($"Cause of death: {GameEngine.SaveAPI.settings.reason}");
            sb.AppendLine($"Time left on the timer last run: {GameEngine.SaveAPI.settings.time_left}s");
            string s = sb.ToString();
            _mainBodyTextRenderer.SetText(s);
            _titleTextRenderer.SetText("Stats for previous run:");

            var titleMeasuredSize = font.MeasureString("Stats for previous run:") * titleFontSize;
            
            var measuredSize = font.MeasureString(s) * fontsize;
            
            
            var textOffset = new Vector2(-measuredSize.X / 2, 0);
            _mainBodyTextRenderer.SetOffset(textOffset);

            textOffset.Y -= titleMeasuredSize.Y;
            _titleTextRenderer.SetOffset(textOffset);
        }
        else
        {
            var measuredSize = font.MeasureString("No stats to recover.") * fontsize;
            _mainBodyTextRenderer.SetText("No stats to recover.");
            Parent.Transform.Position = new Vector3(10 * Camera.scale, 0, Globals._Graphics.GraphicsDevice.Viewport.Height-measuredSize.Y);
        }
    }
}