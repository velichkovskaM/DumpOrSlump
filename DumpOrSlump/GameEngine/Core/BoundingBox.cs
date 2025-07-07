using System;
using Microsoft.Xna.Framework;

namespace GameEngine.Core;

/// <summary>
/// Represents a 2D axis-aligned bounding box with methods for translation, scaling, intersection tests,
/// containment checks, splitting, and vertex extraction
/// </summary>
public class BoundingBox
{
    public Vector2 minimum { get; set; }
    public Vector2 maximum { get; set; }
    
    public BoundingBox(Vector2 min, Vector2 max)
    {
        minimum = min;
        maximum = max;
    }
    
    // Translates the bounding box by the given vector
    public static BoundingBox operator +(Vector2 a, BoundingBox b)
    {
        b.minimum += a;
        b.maximum += a;
        return b;
    }

    // Translates the bounding box by the XZ components of the given 3D vector
    public static BoundingBox operator +(BoundingBox a, Vector3 b)
    {
        var newB = new Vector2(b.X, b.Z);
        return new BoundingBox(a.minimum + newB, a.maximum + newB);
    }
    
    // Scales the bounding box by the XZ components of the given 3D vector
    public static BoundingBox operator *(BoundingBox a, Vector3 b)
    {
        var newB = new Vector2(b.X, b.Z);
        return new BoundingBox(a.minimum * newB, a.maximum * newB);
    }
    
    // Returns true if the two bounding boxes intersect
    public static bool Intersects(BoundingBox box1, BoundingBox box2)
    {
        return !(
            box1.maximum.X < box2.minimum.X ||
            box1.minimum.X > box2.maximum.X ||
            box1.maximum.Y < box2.minimum.Y ||
            box1.minimum.Y > box2.maximum.Y
            );
    }

    // Returns true if the ray intersects this bounding box in the XZ plane
    public bool Intersects(Ray ray)
    {
        var dirInv = new Vector3(
            ray.Direction.X != 0 ? 1f / ray.Direction.X : float.PositiveInfinity,
            ray.Direction.Y != 0 ? 1f / ray.Direction.Y : float.PositiveInfinity,
            ray.Direction.Z != 0 ? 1f / ray.Direction.Z : float.PositiveInfinity
        );
        var t1 = (new Vector3(minimum.X, 0, minimum.Y) - ray.Position) * dirInv;
        var t2 = (new Vector3(maximum.X, 0, maximum.Y) - ray.Position) * dirInv;

        var tmin = Vector3.Min(t1, t2);
        var tmax = Vector3.Max(t1, t2);
        
        var tminVal = Math.Max(tmin.X, Math.Max(tmin.Y, tmin.Z));
        var tmaxVal = Math.Min(tmax.X, Math.Min(tmax.Y, tmax.Z));

        return tminVal < tmaxVal;
    }

    // Returns true if the point is strictly inside the bounding box
    public bool Contains(Vector2 point)
    {
        if (
            point.X > minimum.X &&
            point.X < maximum.X &&
            point.Y > minimum.Y &&
            point.Y < maximum.Y)
        {
            return true;
        }

        return false;
    }

    // Returns true if the 3D point is inside the bounding box in the XZ plane
    public bool Contains(Vector3 point)
    {
        if (point.X >= (minimum.X - float.Epsilon) && 
            point.X <= (maximum.X + float.Epsilon) &&
            point.Z >= (minimum.Y - float.Epsilon) && 
            point.Z <= (maximum.Y + float.Epsilon))
        {
            return true;
        }

        return false;
    }

    // Splits the bounding box into four equal quadrants
    public BoundingBox[] split()
    {
        var center = (maximum + minimum) / 2;

        return new BoundingBox[]
        {
            // Top-left quadrant
            new(minimum, new Vector2(center.X, center.Y)),
        
            // Top-right quadrant
            new(new Vector2(center.X, minimum.Y), new Vector2(maximum.X, center.Y)),
        
            // Bottom-right quadrant
            new(center, maximum),
        
            // Bottom-left quadrant
            new(new Vector2(minimum.X, center.Y), new Vector2(center.X, maximum.Y))
        };
    }

    // Returns the four corner vertices of the bounding box
    public Vector2[] ToVertices()
    {
        var l = new Vector2[]
        {
            minimum,
            new Vector2(minimum.X, maximum.Y),
            maximum,
            new Vector2(maximum.X, minimum.Y),
        };

        return l;
    }

    // Returns the area of this axis-aligned 2D bounding box
    public float Area()
    {
        return (maximum.X - minimum.X) * (maximum.Y - minimum.Y);
    }
}