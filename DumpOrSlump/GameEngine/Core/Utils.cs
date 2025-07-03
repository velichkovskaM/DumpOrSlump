using System;
using Microsoft.Xna.Framework;

namespace DumpOrSlumpGame;

/// <summary>
/// Provides utility math functions for angle calculations in 2D and 3D, with results returned in degrees
/// </summary>
public static class Utils
{
    private const double ConvertToDegrees = 57.29577951308232f;
    
    // Computes the angle between the given 3D vector and the negative Z-axis, returning the result in degrees
    public static double GetAngle(Vector3 v1)
    {
        return Math.Acos(Vector3.Dot(v1, -Vector3.UnitZ) / v1.Length()) * ConvertToDegrees;
    } 
    
    public static double GetAngleNormalized(Vector3 v1)
    {
        return Math.Acos(Vector3.Dot(v1, Vector3.UnitZ)) * ConvertToDegrees;
    }

    // Computes the angle in degrees between two 2D vectors. Handles zero-length vectors safely
    public static float GetAngle(Vector2 v1, Vector2 v2)
    {
        var v1L = v1.Length();
        var v2L = v2.Length();
        
        if (v1L < float.Epsilon || v2L < float.Epsilon) return 0f;
        
        float cosine = Vector2.Dot(v1, v2) / (v1L * v2L);
        
        cosine = Math.Clamp(cosine, -1f, 1f);
        
        return (float)(Math.Acos(cosine) * ConvertToDegrees);
    }
}