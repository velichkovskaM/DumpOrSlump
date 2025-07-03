using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace GameEngine.Components;

/// <summary>
/// Represents a clickable button component with an on/off state. Switches between two animations depending on the state
/// </summary>
public class StateButtonComponent : Component
{
    protected bool state = false;
    protected SpriteRenderer _spriteRenderer;
    protected BoundingBox _boundingBox;

    public StateButtonComponent(Node parent) : base(parent) { }

    // Tuple: (texture, onAnimation, offAnimation, initialState)
    public virtual (Texture2D asset, AnimationData nonClicked, AnimationData clicked, bool initialState) GetButtonData()
    {
        return (null, null, null, false);
    }

    // Initializes the button by assigning animations and computing the bounding box
    public override void Start(IScene scene)
    {
        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        
        var (asset, on, off, initialState) = GetButtonData();
        _spriteRenderer.AddAnimation("on", on);
        _spriteRenderer.AddAnimation("off", off);

        if (!initialState) _spriteRenderer.SetAnimation("off");

        var (width, height) = _spriteRenderer.GetDimensions();
        _boundingBox = new BoundingBox(new Vector2(-(width / 2), -(height / 4)), new Vector2(width / 2, height / 4)) * Parent.Transform.Scale + Parent.Transform.Position;
    }

    public override void Update(GameTime gameTime, TouchCollection touchCollection) { }
    public virtual void OnClick() { }
    public virtual void On() { }
    public virtual void Off() { }
}