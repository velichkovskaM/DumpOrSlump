using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace GameEngine.Components;

/// <summary>
/// Represents a convex polygon collider with an arbitrary list of vertices
/// Extends the base Collider with custom shape support
/// </summary>
public class ConvexCollider : Collider
{
    public List<Vector2> vertices;
    
    // Creates a new convex polygon collider with the given parent node and vertices
    // Automatically calculates its axis-aligned bounding box AABB
    public ConvexCollider(Node parent, List<Vector2> vertices, bool active = true) : base(parent, GetAABBCollider(parent, vertices), active)
    {
        this.vertices = vertices;
    }

    // Calculates the axis-aligned bounding box (AABB) for a given list of vertices, scaling and translating it to world space
    public static BoundingBox GetAABBCollider(Node parent, List<Vector2> vertices)
    {
        if (vertices == null || vertices.Count == 0)
            throw new ArgumentException("Vertices list cannot be null or empty");
        
        Vector2 maxValues = new Vector2(
            vertices.Max(v => v.X),
            vertices.Max(v => v.Y)
        );
        
        Vector2 minValues = new Vector2(
            vertices.Min(v => v.X),
            vertices.Min(v => v.Y)
        );
        
        return new BoundingBox(minValues, maxValues) * parent.Transform.Scale + parent.Transform.Position;
    }
    
    // Gets the current axis-aligned bounding box AABB for this convex collider, taking the parent transform's scale and position into account
    public override BoundingBox GetAABBCollider()
    {
        return GetAABBCollider(Parent, vertices);
    }

    // Gets the transformed vertices of the convex shape in world space. Applies the parent transform's scale and position offset
    public override Vector2[] GetVertices()
    {
        Vector2[] result = new Vector2[vertices.Count];
        
        for (int i = 0; i < vertices.Count; i++)
        {
            result[i] = (vertices[i] * new Vector2(Parent.Transform.Scale.X, Parent.Transform.Scale.Z)).Add(Parent.Transform.Position);
        }

        return result;
    }
}