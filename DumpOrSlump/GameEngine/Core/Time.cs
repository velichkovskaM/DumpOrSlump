using Microsoft.Xna.Framework;

namespace GameEngine.Core;

/// <summary>
/// Provides time-tracking utilities for delta time, total time, and time since last reload
/// </summary>
public static class Time
{
    public static float deltaTime { get; private set; }
    public static float totalTime { get; private set; }
    public static float totalTimeSinceReload { get; private set; }

    public static void UpdateTimes(GameTime gameTime)
    {
        deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
        totalTimeSinceReload += deltaTime;
    }

    public static void Reload()
    {
        totalTimeSinceReload = 0;
    }
}