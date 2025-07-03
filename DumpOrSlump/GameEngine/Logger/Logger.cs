using GameEngine.Logger;

/// <summary>
/// Static global logger for the whole engine.
/// Delegates logging calls to the configured ILogger
/// </summary>
public static class Logger
{
    private static ILogger _logger;

    // Initializes the logger system with a specific logger backend
    // This must be called once during startup
    public static void Initialize(ILogger logger)
    {
        _logger = logger;
    }

    // Logs an informational message
    public static void Info(string message, string category = "GameEngine")
    {
        _logger.Info(category, message);
    }
    
    // Logs an error message
    public static void Error(string message, string category = "GameEngine")
    {
        _logger?.Error(message, category);
    }

    // Logs a debug-level message
    public static void Debug(string message, string category = "GameEngine")
    {
        _logger?.Debug(message, category);
    }
}