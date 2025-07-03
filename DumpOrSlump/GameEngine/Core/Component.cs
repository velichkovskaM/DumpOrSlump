using GameEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameEngine.Core;

/// <summary>
/// Base class for game components that add behavior, logic or rendering to a node in the scene graph
/// </summary>
public class Component
{
    public Node Parent;
    public bool Active { get; set; }

    public Component(Node parent, bool active = true)
    {
        this.Parent = parent;
        this.Active = active;
    }

    public virtual void Update(GameTime gameTime, TouchCollection touches) { }
    public virtual void Draw(Camera camera, SpriteBatch spriteBatch) { }
    public virtual void Start(IScene scene) { }
}