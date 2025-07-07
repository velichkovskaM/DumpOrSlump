using System;
using System.Collections.Generic;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace GameEngine.Physics.Collisions;

/// <summary>
/// Handles collision checks and resolutions for all colliders in the scene
/// Supports AABB and Convex collider types
/// </summary>
public class CollisionDetection
{
    public static List<Collider> collideables = new List<Collider>();
    public static List<Collider> collidingObjects = new List<Collider>(100);
    public static int totalChecksMade = 0;
    public static int totalPossibleChecks = 0;
    
    // Registers a collider so it participates in collision checks
    public static void addColliderSubscription(Collider col)
    {
        collideables.Add(col);
    }

    // Removes a collider from collision checks
    public static void removeColliderSubscription(Collider col)
    {
        collideables.Remove(col);
    }
    
    // Checks for collisions for the given node's collider
    // Resolves AABB and Convex collisions
    public static void HandleMovement(Node node, Camera camera)
    {
        if (node.GetComponent<Collider>() is Collider collider)
        {
            
            var aabbCollider = collider.GetAABBCollider();

            // Compare against all other colliders
            collidingObjects.AddRange(collideables);
            
            totalChecksMade += collidingObjects.Count;
            totalPossibleChecks += collideables.Count;
            foreach (var other in collidingObjects)
            {
                // Skip self-collision
                if (collider.Parent.Id == other.Parent.Id) continue;
                
                // If bounding boxes intersect, handle collision
                if (BoundingBox.Intersects(aabbCollider, other.GetAABBCollider()))
                {
                    if (collider.GetType() == other.GetType() && collider.GetType() == typeof(AABBCollider))
                    {
                        // Handle AABB vs AABB
                        HandleAABBCollision((AABBCollider)collider, (AABBCollider)other);
                    }
                    else if (collider.GetType() == typeof(ConvexCollider) || other.GetType() == typeof(ConvexCollider))
                    {
                        // Handle Convex collision
                        var (collides, penetrationDepth, axis) = ConvexCollisionHandler.CollisionCheck(collider, other);
                        if (collides) ConvexCollisionHandler.ResolveCollision(collider, penetrationDepth.Value, axis.Value);
                    }
                    else
                    {
                        global::Logger.Error($"Cant find a method to handle collider {collider.GetType().Name} with {other.GetType().Name}");
                    }
                }
                
            }
        }
        collidingObjects.Clear();
    }
    
    // Resolves a collision between two AABB colliders by pushing the first collider out
    public static void HandleAABBCollision(AABBCollider collider, AABBCollider other)
    {
        var aabbCollider = collider.GetAABBCollider();
        var aabbOther = other.GetAABBCollider();
        
        float overlapX = Math.Min(aabbCollider.maximum.X, aabbOther.maximum.X) - Math.Max(aabbCollider.minimum.X, aabbOther.minimum.X);
        float overlapZ = Math.Min(aabbCollider.maximum.Y, aabbOther.maximum.Y) - Math.Max(aabbCollider.minimum.Y, aabbOther.minimum.Y);
        
        var colliderCenter = aabbCollider.minimum + (aabbCollider.maximum - aabbCollider.minimum) / 2;
        var otherCenter = aabbOther.minimum + (aabbOther.maximum - aabbOther.minimum) / 2;
        
        // Resolve on the axis with the smallest overlap
        if (overlapX < overlapZ)
        {
            if (colliderCenter.X < otherCenter.X)
            {
                collider.Parent.Transform.Position -= new Vector3(overlapX, 0, 0);
            }
            else
            {
                collider.Parent.Transform.Position += new Vector3(overlapX, 0, 0);
            }
        }
        else
        {
            if (colliderCenter.Y < otherCenter.Y)
            {
                collider.Parent.Transform.Position -= new Vector3(0, 0, overlapZ);
            }
            else
            {
                collider.Parent.Transform.Position += new Vector3(0, 0, overlapZ);
            }
        }
    }
}