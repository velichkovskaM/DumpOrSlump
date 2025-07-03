using System;
using GameEngine.Components;
using Microsoft.Xna.Framework;

namespace GameEngine.Physics.Collisions;

/// <summary>
/// Provides SAT-based (Separating Axis Theorem) collision detection and resolution for convex polygons
/// Uses the Separating Axis Theorem to test and resolve overlaps
/// 1. Compute edge normals (axes)
/// 2. Project both shapes onto each axis
/// 3. Look for a gap between the projections
/// 4. If there is any gap, they do not collide
/// 5. If there is no gap on any axis, they collide
/// </summary>
public static class ConvexCollisionHandler
{
    // Resolves a detected collision by pushing the collider out along the minimum penetration axis
    public static void ResolveCollision(Collider collider, float penetrationDepth, Vector2 axis)
    {
        var adjustment = penetrationDepth * Vector2.Normalize(axis);
        collider.Parent.Transform.Position += new Vector3(adjustment.X, 0, adjustment.Y);
    }

    // Runs SAT collision detection for two colliders
    // Returns whether they collide, plus the minimum penetration depth and the best axis
    public static (bool collides, float? penetrationDepth, Vector2? axis) CollisionCheck(Collider collider, Collider other)
    {
        var colliderVertices = collider.GetVertices();
        var otherVertices = other.GetVertices();
        var axes = GetNormals(colliderVertices, otherVertices);

        var penetrationDepthMin = float.MaxValue;
        var bestAxis = Vector2.Zero;
        
        foreach (var axis in axes)
        {
            (float minCollider, float maxCollider) = ProjectShape(colliderVertices, axis);
            (float minOther, float maxOther) = ProjectShape(otherVertices, axis);

            // If a separating axis is found, no collision
            if (maxCollider + float.Epsilon < minOther || maxOther + float.Epsilon < minCollider)
            {
                return (false, null, null);
            }
            
            // Keep the smallest penetration axis for resolution
            var penetrationDepth = Math.Min(maxCollider - minOther, maxOther - minCollider);
            if (penetrationDepth < penetrationDepthMin)
            {
                penetrationDepthMin = penetrationDepth;
                bestAxis = axis;
            }
        }

        // Ensure axis points away from the other shape
        if (Vector2.Dot(bestAxis, (collider.GetCenter() - other.GetCenter())) < 0)
        {
            bestAxis = -bestAxis;
        }

        return (true, penetrationDepthMin, bestAxis);
    }

    // Projects a shape’s vertices onto an axis and returns the min and max scalar projection
    public static (float min, float max) ProjectShape(Vector2[] vertices, Vector2 axis)
    {
        var max  = float.MinValue;
        var min = float.MaxValue;
        foreach (var vertex in vertices)
        {
            var temp = Vector2.Dot(vertex, axis);
            if (temp > max) max = temp;
            if (temp < min) min = temp;
        }
        
        return (min, max);
    }

    // Computes edge normals for all edges of both polygons
    // Each edge normal is perpendicular to an edge
    public static Vector2[] GetNormals(Vector2[] colliderVertices, Vector2[] otherVertices)
    {
        var normals = new Vector2[colliderVertices.Length + otherVertices.Length];

        for (var i = 0; i < colliderVertices.Length; i++)
        {
            var nextIndex = (i + 1) % colliderVertices.Length;
            var edge = colliderVertices[nextIndex] - colliderVertices[i];
            
            normals[i] = Vector2.Normalize(new Vector2(-edge.Y, edge.X));
        }
        
        for (var i = 0; i < otherVertices.Length; i++)
        {
            var nextIndex = (i + 1) % otherVertices.Length;
            var edge = otherVertices[nextIndex] - otherVertices[i];
            
            normals[colliderVertices.Length + i] = Vector2.Normalize(new Vector2(-edge.Y, edge.X));
        }

        return normals;
    }
}