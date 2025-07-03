using Microsoft.Xna.Framework;

namespace GameEngine.Core;

/// <summary>
/// Provides extension methods for combining and converting between Vector2 and Vector3 types
/// </summary>
public static class Vector3Extensions
{
    public static Vector3 Add(this Vector3 v3, Vector2 v2)
    {
        return new Vector3(v3.X + v2.X, v3.Y, v3.Z + v2.Y);
    }

    public static Vector2 Add(this Vector2 v3, Vector3 v2)
    {
        return new Vector2(v3.X + v2.X, v3.Y + v2.Z);
    }

    public static Vector3 Subtract(this Vector3 v3, Vector2 v2)
    {
        return new Vector3(v3.X - v2.X, v3.Y, v3.Z - v2.Y);
    }

    public static Vector3 Subtract(this Vector2 v2, Vector3 v3)
    {
        return new Vector3(v3.X - v2.X, v3.Y, v3.Z - v2.Y);
    }

    public static Vector2 ToVector2(this Vector3 v3)
    {
        return new Vector2(v3.X, v3.Z);
    }
}