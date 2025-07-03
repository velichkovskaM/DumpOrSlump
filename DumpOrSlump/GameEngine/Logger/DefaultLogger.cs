using System;

namespace GameEngine.Logger;

/// <summary>
/// Default logger that outputs messages to the console
/// </summary>
public class DefaultLogger : ILogger
{
    public void Debug(string message, string category = "")
    {
        Console.WriteLine($"DEBUG: {message}");
    }

    public void Info(string message, string category = "")
    {
        Console.WriteLine($"INFO: {message}");
    }

    public void Error(string message, string category = "")
    {
        Console.WriteLine($"ERROR: {message}");
    }
}