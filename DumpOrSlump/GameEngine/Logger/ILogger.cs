namespace GameEngine.Logger;

/// <summary>
/// Defines a basic logging interface for debug, info, and error messages
/// </summary>
public interface ILogger
{
    public void Debug(string message, string category = "");
    public void Info(string message, string category = "");
    public void Error(string message, string category = "");
}