using System;
using System.Collections.Generic;
using DumpOrSlumpGame;
using Microsoft.Xna.Framework;

namespace GameEngine.Core;

/// <summary>
/// Tracks touch input for a single touch ID and analyzes the gesture, detecting patterns like circles
/// </summary>
public class GestureTracker(int touchId)
{
    public enum GestureType
    {
        Unfinished = 0,
        Unrecognized = 1,
        Circle = 2,
    }
    
    public int TouchId { get; private set; } = touchId;
    public Vector2 center { get; private set; } = Vector2.Zero;
    public GestureType Gesture { get; private set; } = GestureType.Unfinished;
    public List<Vector2> Touches { get; private set; } = new List<Vector2>(100);

    // Sets the gesture type, or marks it as unrecognized if no match is found
    public void SetGestureType()
    {
        if (CheckCircle()) return;

        Gesture = GestureType.Unrecognized;
    }

    // Checks if the touch points form a circle gesture
    private bool CheckCircle()
    {
        if (Touches.Count < 10) return false;
        
        Vector2 min = new Vector2(float.MaxValue);
        Vector2 max = new Vector2(float.MinValue);
           
        // Finds the bounding box (min and max) of the points
        foreach (Vector2 point in Touches)
        {
            min = Vector2.Min(min, point);
            max = Vector2.Max(max, point);
        }
        
        Vector2 center = (min + max) * 0.5f;
        float radius = Vector2.Distance(min, max) * 0.5f;
        
        // Counts how many points lie near the estimated circle
        int validPoints = 0;
        foreach (Vector2 point in Touches)
        {
            float distance = Vector2.Distance(point, center);
            if (Math.Abs(distance - radius) < radius * 0.50f)
                validPoints++;
        }
        
        // Checks for full loop by summing all the angles between the segments
        double totalRotation = 0;
        for (int i = 1; i < Touches.Count; i++)
        {
            Vector2 prevDir = Touches[i-1] - center;
            Vector2 currDir = Touches[i] - center;
            totalRotation += Utils.GetAngle(prevDir, currDir);
        }
        
        // If enough points are valid it sets the gesture to circle and saves the center
        var valid = (validPoints / (float)Touches.Count) > 0.8f && 
               Math.Abs(totalRotation) > MathHelper.TwoPi * 0.75f;
        
        if (!valid) return false;
        Gesture = GestureType.Circle;
        this.center = center;

        return true;
    }
}