using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using AudioEmitter = GameEngine.Components.AudioEmitter;

namespace DumpOrSlumpGame.Components.UI;

/// <summary>
/// Displays win/lose text and plays corresponding audio. Stops all active emitters and the player's sounds before showing a message
/// </summary>
public class WinLoseText : Component
{
    TextRenderer _renderer;
    SpriteFont _font;
    SoundEffectInstance _soundEffectWin;
    SoundEffectInstance _soundEffectLost;
    
    public WinLoseText(Node parent, bool active = true) : base(parent, active) { }

    // load font, configure renderer, preload SFX
    public override void Start(IScene scene)
    {
        _font = Globals.content.Load<SpriteFont>("Fonts/Press Start 2P");
        _renderer = Parent.GetComponent<TextRenderer>();
        _renderer.SetFont(_font);
        
        _renderer.SetFontSize(new Vector2(1.4f, 1.4f));
        
        var distance = _font.MeasureString("GAME OVER") / 2;
        _renderer.SetText("GAME OVER");
        _renderer.SetOffset(new Vector2(-distance.X * _renderer.FontSize.X, -distance.Y * _renderer.FontSize.Y));
        
        _soundEffectWin = Globals.content.Load<SoundEffect>("SoundEffect/WinOutro").CreateInstance();
        _soundEffectWin.Volume = 1.0f;
        
        _soundEffectLost = Globals.content.Load<SoundEffect>("SoundEffect/LostOutro").CreateInstance();
        _soundEffectLost.Volume = 1.0f;
    }

    // show win text, stop all sounds, play win SFX
    public void SetWinText()
    {
        var emitters = Game1.Instance.Scene.FindAllComponents<AudioEmitter>();
        foreach (var emitter in emitters)
            emitter.StopSound();
        
        var player = Game1.Instance.Scene.FindNodeByName("Player");
        if (player != null) player.GetComponent<Player>()._soundEffect.Stop();
        
        var distance = _font.MeasureString("YOU WIN") / 2;
        _soundEffectWin.Play();
        _renderer.SetText("YOU WIN");
        _renderer.SetOffset(new Vector2(-distance.X * _renderer.FontSize.X, -distance.Y * _renderer.FontSize.Y));
    }

    // show lose text, stop all sounds, play lose SFX
    public void SetLoseText()
    {
        var emitters = Game1.Instance.Scene.FindAllComponents<AudioEmitter>();
        foreach (var emitter in emitters)
            emitter.StopSound();
        
        var player = Game1.Instance.Scene.FindNodeByName("Player");
        if (player != null) player.GetComponent<Player>()._soundEffect.Stop();
        
        var distance = _font.MeasureString("GAME OVER") / 2;
        _soundEffectLost.Play();
        _renderer.SetText("GAME OVER");
        _renderer.SetOffset(new Vector2(-distance.X * _renderer.FontSize.X, -distance.Y * _renderer.FontSize.Y));
    }
}
