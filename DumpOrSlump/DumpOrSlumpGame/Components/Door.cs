using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using AudioEmitter = GameEngine.Components.AudioEmitter;

namespace DumpOrSlumpGame.Components;

/// <summary>
/// Interactable door with open/close animations and sound. Taps toggle the state; plays SFX and updates animation
/// </summary>
public class Door : Component
{
    public enum DoorState
    {
        Closed,
        Open
    }
    
    private Texture2D animation;
    private Rectangle[] animation_opening;
    private Rectangle[] animation_closing;
    private Rectangle[] animation_open;
    private Rectangle[] animation_closed;
    private Rectangle[] _currentAnimation;
    
    DoorState _state = DoorState.Closed;
    
    bool _isOpening = false;
    bool _isClosing = false;
    
    SpriteRenderer _spriteRenderer;

    private AudioEmitter _audioEmitter;

    public Door(Node parent) : base(parent) { }

    // load sprite sheet, set up animations, create background panel, and init audio emitter
    public override void Start(IScene scene)
    {
        animation = Globals.content.Load<Texture2D>("SpriteSheets/DoorSpriteSheet");
        animation_opening = new Rectangle[10];
        animation_opening[0] = new Rectangle(0, 0, 512, 512);
        animation_opening[1] = new Rectangle(512, 0, 512, 512);
        animation_opening[2] = new Rectangle(512 * 2, 0, 512, 512);
        animation_opening[3] = new Rectangle(512 * 3, 0, 512, 512);
        animation_opening[4] = new Rectangle(512 * 4, 0, 512, 512);
        animation_opening[5] = new Rectangle(512 * 5, 0, 512, 512);
        animation_opening[6] = new Rectangle(512 * 5, 0, 512, 512);
        animation_opening[7] = new Rectangle(512 * 5, 0, 512, 512);
        animation_opening[8] = new Rectangle(512 * 5, 0, 512, 512);
        animation_opening[9] = new Rectangle(512 * 5, 0, 512, 512);
        
        animation_closing = new Rectangle[10];
        animation_closing[9] = new Rectangle(0, 0, 512, 512);
        animation_closing[8] = new Rectangle(0, 0, 512, 512);
        animation_closing[7] = new Rectangle(0, 0, 512, 512);
        animation_closing[6] = new Rectangle(0, 0, 512, 512);
        animation_closing[5] = new Rectangle(0, 0, 512, 512);
        animation_closing[4] = new Rectangle(512, 0, 512, 512);
        animation_closing[3] = new Rectangle(512 * 2, 0, 512, 512);
        animation_closing[2] = new Rectangle(512 * 3, 0, 512, 512);
        animation_closing[1] = new Rectangle(512 * 4, 0, 512, 512);
        animation_closing[0] = new Rectangle(512 * 5, 0, 512, 512);
        
        animation_open = new Rectangle[1];
        animation_open[0] = new Rectangle(512 * 5, 0, 512, 512);
        animation_closed = new Rectangle[1];
        animation_closed[0] = new Rectangle(0, 0, 512, 512);

        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        _spriteRenderer.AddAnimation("closed", new AnimationData(animation, animation_closed, 0.2f, false, Render2D: false));
        _spriteRenderer.AddAnimation("opening", new AnimationData(animation, animation_opening, 0.2f, false, Render2D: false));
        _spriteRenderer.AddAnimation("closing", new AnimationData(animation, animation_closing, 0.2f, false, Render2D: false));
        
        _audioEmitter = Parent.GetComponent<AudioEmitter>();
        _audioEmitter.SetSoundEffect(Globals.content.Load<SoundEffect>("SoundEffect/door_open_close"));
        _audioEmitter.CurrentSoundInstance.Volume = 1f;

        var backgroundPosition = Parent.Transform.Position;
        backgroundPosition.Z -= 0.05f;
        var doorBackground = new Node("doorBackground", backgroundPosition, scale: Parent.Transform.Scale);
        var doorBackgroundSpriteRenderer = new SpriteRenderer(doorBackground, Globals._Graphics.GraphicsDevice);
        doorBackgroundSpriteRenderer.AddAnimation("idle", new AnimationData(
            animation,
            new []{new Rectangle(512 * 6, 0, 512, 512)},
            1f,
            false,
            Render2D: false
            ));
        doorBackground.AddComponent(doorBackgroundSpriteRenderer);
        scene.SafeInsert(doorBackground);
    }

    // per‑frame: continue playing open/close until animation ends
    public override void Update(GameTime gameTime, TouchCollection touchCollection)
    {
        if (_isOpening)
        {
            HandleOpenDoor();
        } else if (_isClosing)
        {
            HandleCloseDoor();
        }
    }

    // external trigger: starts opening or closing if idle
    public void SwitchDoorState()
    {
        Logger.Error("Door state called");
        _audioEmitter.PlaySound();
        if (!_isOpening && !_isClosing)
        {
            if (_state == DoorState.Closed)
            {
                _isOpening = true;
                _spriteRenderer.SetAnimation("opening");
            }
            else
            {
                _isClosing = true;
                _spriteRenderer.SetAnimation("closing");
            }
        }
    }

    // handle finishing open animation
    private void HandleOpenDoor()
    {
        if (_spriteRenderer.AnimationEnded())
        {
            _isOpening = false;
            _state = DoorState.Open;
        }
    }

    // handle finishing close animation
    private void HandleCloseDoor()
    {
        if (_spriteRenderer.AnimationEnded())
        {
            _isClosing = false;
            _state = DoorState.Closed;
            Game1.Instance.waitBetweenPanning = false;
        }
    }
}