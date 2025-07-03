using Dump_Or_Slump_Android.SaveAPI;

namespace GameEngine;

/// <summary>
/// Intended for saving/loading settings and files
/// </summary>
public interface ISaveAPI
{
    // Set default app or game settings
    public static virtual void SetDefualtSettings() { }
    
    // Checks if a settings file already exists
    public static virtual bool SettingsExists()
    {
        return false;
    }

    // Saves the current settings to a file
    public static virtual bool SaveSettingsFile()
    {
        return false;
    }

    // Loads the current settings from file
    public static virtual SettingsCast LoadSettingsFile()
    {
        return new SettingsCast();
    }

    // Saves string content to a given file path
    public static virtual bool SaveToFile(string content, string filePath)
    {
        return false;
    }

    // Loads text content from a file path
    public static virtual string LoadFromFile(string filePath)
    {
        return "";
    }
}