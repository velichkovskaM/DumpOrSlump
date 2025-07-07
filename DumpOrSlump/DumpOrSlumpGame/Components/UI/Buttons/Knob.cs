using GameEngine;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using BoundingBox = GameEngine.Core.BoundingBox;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace DumpOrSlumpGame.Components.UI.Buttons;

/// <summary>
/// Volume-control knob UI. Draggable handle that maps horizontal position to master volume, updates settings, and persists value
/// </summary>
public class Knob : Component
{
    public SoundBar SoundBar;
    private SpriteRenderer _spriteRenderer;
    public float volume = 0;
    private float parentWidth = 0;
    private int touchId = 0;
    private BoundingBox _boundingBox;
    private float maxX = 0;
    private float minX = 0;
    private Vector3 dragOffset = Vector3.Zero;
    
    // attach to parent & bind soundbar
    public Knob(Node parent, SoundBar soundBar, bool active = true) : base(parent, active)
    {
        SoundBar = soundBar;
    }

    // load sprite, compute bounds, position knob
    public override void Start(IScene scene)
    {
        volume = GameEngine.SaveAPI.settings.volume;
        
        var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
        var knob = new []{new Rectangle(256 * 6, 256 * 6, 256, 256)};

        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        _spriteRenderer.AddAnimation("idle", new AnimationData(
            asset,
            knob,
            1,
            false,
            isUI: true
        ));

        maxX = SoundBar._boundingBox.maximum.X;
        minX = SoundBar._boundingBox.minimum.X;
        
        parentWidth = maxX - minX;
        Parent.Transform.Position += new Vector3(parentWidth * volume - parentWidth / 2, 0, 0);
        var (width, height) = _spriteRenderer.GetDimensions();
        _boundingBox = new BoundingBox(new Vector2(-(width / 2), -(height / 4)), new Vector2(width / 2, height / 4)) * Parent.Transform.Scale + Parent.Transform.Position;
    }

    // remember touch id & initial drag offset
    public void SetTouchIdAndOffset(int id, Vector2 offset)
    {
        touchId = id;
        dragOffset = Parent.Transform.Position.Subtract(offset);
    }

    // drag handling, volume mapping, save & apply
    public override void Update(GameTime gameTime, TouchCollection touches)
    {
        foreach (var touch in touches)
        {
            if (touch.State == TouchLocationState.Moved && touchId == touch.Id)
            {
                Parent.Transform.Position = new Vector3(MathHelper.Clamp(touch.Position.X + dragOffset.X, minX, maxX), Parent.Transform.Position.Y, Parent.Transform.Position.Z);
                volume = (Parent.Transform.Position.X - minX) / (maxX - minX);
                GameEngine.SaveAPI.settings.volume = volume >= 0 ? volume : 0;
                MusicController.SetVolume(SaveAPI.settings.volume);
                SoundEffect.MasterVolume = SaveAPI.settings.sound_on_off ? SaveAPI.settings.volume : 0;
            }
            
            if (touch.State == TouchLocationState.Released && touchId == touch.Id)
            {
                var (width, height) = _spriteRenderer.GetDimensions();
                _boundingBox =
                    new BoundingBox(new Vector2(-(width / 2), -(height / 4)), new Vector2(width / 2, height / 4)) *
                    Parent.Transform.Scale + Parent.Transform.Position;
                GameEngine.SaveAPI.SaveSettingsFile();
                MusicController.SetVolume(SaveAPI.settings.volume);
                SoundEffect.MasterVolume = SaveAPI.settings.sound_on_off ? SaveAPI.settings.volume : 0;
            }
        }
    }
}