using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace DumpOrSlumpGame.Components.UI.Buttons;

/// <summary>
/// Base class for UI button components that handle touch interaction, sprite animations, bounding box calculation, and click sound effects
/// </summary>
public class ButtonComponent : Component
{
    protected int touchId = -1;
    
    protected SpriteRenderer _spriteRenderer;
    protected BoundingBox _boundingBox;
    protected SoundEffectInstance _soundEffect;

    // Creates a new button component attached to the given parent node
    public ButtonComponent(Node parent) : base(parent) { }

    // A tuple containing the texture, normal animation, clicked animation, and sound effect
    public virtual (Texture2D asset, AnimationData nonClicked, AnimationData clicked, SoundEffectInstance soundEffect) GetButtonData()
    {
        return (null, null, null, null);
    }

    public override void Start(IScene scene)
    {
        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        
        var (asset, rawNormal, rawClicked, soundEffect) = GetButtonData();

        if (rawNormal.frames.Length == 0 || rawClicked.frames.Length == 0)
        {
            Logger.Error($"Frame lengths are for both animation data: {rawNormal.frames.Length}, and {rawClicked.frames.Length}.");
        }
        
        AnimationData normal = rawNormal with { frames = new[] { GetRealBounds(asset, (rawNormal.frames[0].X, rawNormal.frames[0].Y), (rawNormal.frames[0].Width, rawNormal.frames[0].Height)) } };
        AnimationData clicked = rawNormal with { frames = new[] { GetRealBounds(asset, (rawClicked.frames[0].X, rawClicked.frames[0].Y), (rawClicked.frames[0].Width, rawClicked.frames[0].Height)) } };
        
        _spriteRenderer.AddAnimation("normal", normal);
        _spriteRenderer.AddAnimation("clicked", clicked);

        var (width, height) = _spriteRenderer.GetDimensions();
        _boundingBox = new BoundingBox(new Vector2(-(width / 2), -(height / 4)), new Vector2(width / 2, height / 4)) * Parent.Transform.Scale + Parent.Transform.Position;

        _soundEffect = soundEffect;
    }

    // Updates the button state each frame. Resets the animation to normal when the touch is released
    public override void Update(GameTime gameTime, TouchCollection touchCollection)
    {

        foreach (var touch in touchCollection)
        {
            if (touchId == touch.Id)
            {
                if (touch.State == TouchLocationState.Released)
                {
                    _spriteRenderer.SetAnimation("normal");
                    OnRelease();
                }
            }
        }
    }

    // Called when the button is clicked. Plays the clicked animation and stores the touch ID
    public virtual void OnClick(TouchLocation touch)
    {
        _spriteRenderer.SetAnimation("clicked");
        touchId = touch.Id;
    }
    
    public virtual void OnRelease() { }
    
    // Calculates the real bounds of a sprite by trimming transparent edges
    public Rectangle GetRealBounds(Texture2D texture, (int x, int y) position, (int width, int height) size)
    {
        Color[] data = new Color[size.width * size.height];
        texture.GetData(0, new Rectangle(position.x, position.y, 256, 256), data, 0, size.width * size.height);
        
        int left = 0;
        int right = 0;
        int top = 0;
        int bottom = 0;
        
        int middleRowStart = size.height / 2 * size.width;
        int middleColumnIndex = size.width / 2;
        
        for (int x = 0; x < size.width; x++)
        {
            if (data[middleRowStart + x].A == 0)
            {
                left++;
            }
            else
            {
                break;
            }
        }
        
        for (int x = size.width - 1; x >= 0; x--)
        {
            if (data[middleRowStart + x].A == 0)
            {
                right++;
            }
            else
            {
                break;
            }
        }
        
        for (int y = 0; y < size.height; y++)
        {
            int index = y * size.width + middleColumnIndex;
            if (data[index].A == 0)
            {
                top++;
            }
            else
            {
                break;
            }
        }
        
        for (int y = size.height - 1; y >= 0; y--)
        {
            int index = y * size.width + middleColumnIndex;
            if (data[index].A == 0)
            {
                bottom++;
            }
            else
            {
                break;
            }
        }
        
        return new Rectangle(position.x + left, position.y + top, size.width - (right+left), size.height - (top+bottom));
    }
}