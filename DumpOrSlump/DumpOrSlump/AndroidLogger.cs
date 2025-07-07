using Android.Util;
using GameEngine.Logger;

namespace Dump_Or_Slump_Android;

/// <summary>
/// Routes log messages from the game’s interface to Android’s native Logcat system so they can be viewed with adb logcat
/// Provides standard Error, Debug, and Info severity levels
/// </summary>
public class AndroidLogger : ILogger
{
    // Error ─ high‑priority issues and exceptions
    public void Error(string message, string category = "")
    {
        Log.Error(category, message);
    }

    // Debug ─ diagnostic output useful during development
    public void Debug(string message, string category = "")
    {
        Log.Debug(category, message);
    }

    // Info ─ general informational messages about game state
    public void Info(string message, string category = "")
    {
        Log.Info(category, message);
    }
}