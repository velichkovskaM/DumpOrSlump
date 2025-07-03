using GameEngine.Core;

namespace GameEngine.Components;

/// <summary>
/// Represents an axis-aligned bounding box AABB collider for 2D or 3D collision detection, inheriting from Collider
/// </summary>
public class AABBCollider : Collider
{
    public AABBCollider(Node parent, BoundingBox boundingBox, bool active = true) : base(parent, boundingBox, active) { }
}