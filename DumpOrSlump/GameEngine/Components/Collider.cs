using GameEngine.Core;
using GameEngine.Physics.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace GameEngine.Components;

/// <summary>
/// Base class for collider components. Provides AABB collision data, visualization helpers, and core geometry operations
/// </summary>
public class Collider : Component
{
    protected BoundingBox AABBCollider;
    
    // Creates a new collider attached to the given parent node
    // Automatically registers the collider for collision detection
    public Collider(Node parent, BoundingBox boundingBox, bool active = true) : base(parent, active)
    {
        AABBCollider = boundingBox;
        CollisionDetection.addColliderSubscription(this);
    }

    public virtual BoundingBox GetAABBCollider()
    {
        return new BoundingBox(
            AABBCollider.minimum + new Vector2(Parent.Transform.Position.X, Parent.Transform.Position.Z),
            AABBCollider.maximum + new Vector2(Parent.Transform.Position.X, Parent.Transform.Position.Z)
            );
    }

    // Gets the world-space center of the colliders AABB
    public Vector2 GetCenter()
    {
        var aabb = GetAABBCollider();
        var center = (aabb.maximum + aabb.minimum) / 2;
        return center;
    }
    
    // Gets the collider's vertices in world space
    // By default returns the corners of the AABB
    public virtual Vector2[] GetVertices()
    {
        return GetAABBCollider().ToVertices();
    }
}