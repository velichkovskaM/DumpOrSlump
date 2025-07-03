using System;
using Dump_Or_Slump_Android.SaveAPI;

namespace GameEngine;

/// <summary>
/// Static SaveAPI dispatcher — acts as a bridge to platform-specific static save/load implementations
/// Uses reflection to dynamically invoke static methods on the implementation Type
/// </summary>
public static class SaveAPI
{
    public static Type Instance { get; private set; }
    public static SettingsCast settings { get; set; }

    // Sets the implementation Type that will handle all save/load operations
    public static void SetClass(Type saveAPI)
    {
        Instance = saveAPI;   
    }
    
    // Calls the static SetDefualtSettings() on the implementation
    // Throws if the implementation is not set or the method is missing
    public static void SetDefualtSettings()
    {
        if (Instance == null)
        {
            throw new InvalidOperationException("Instance is not set.");
        }

        var method = Instance.GetMethod("SetDefualtSettings", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (method == null)
        {
            throw new MissingMethodException($"Static method 'SetDefaultSettings' not found in {Instance.FullName}.");
        }

        method.Invoke(null, null);
    }
    
    // Calls the static SettingsExists() on the implementation
    public static bool SettingsExists()
    {
        if (Instance == null)
        {
            throw new InvalidOperationException("Instance is not set.");
        }

        var method = Instance.GetMethod("SettingsExists", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (method == null)
        {
            throw new MissingMethodException($"Static method 'SettingsExists' not found in {Instance.FullName}.");
        }

        return (bool)method.Invoke(null, null);
    }
    
    // Passes the current in-memory settings
    public static bool SaveSettingsFile()
    {
        if (Instance == null)
        {
            throw new InvalidOperationException("Instance is not set.");
        }

        var method = Instance.GetMethod("SaveSettingsFile", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (method == null)
        {
            throw new MissingMethodException($"Static method 'SaveSettingsFile' not found in {Instance.FullName}.");
        }

        return (bool)method.Invoke(null, [settings]);
    }

    // Calls the static LoadSettingsFile() on the implementation, stores it in SaveAPI.settings and returns it
    public static SettingsCast LoadSettingsFile()
    {
        if (Instance == null)
        {
            throw new InvalidOperationException("Instance is not set.");
        }

        var method = Instance.GetMethod("LoadSettingsFile", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (method == null)
        {
            throw new MissingMethodException($"Static method 'LoadSettingsFile' not found in {Instance.FullName}.");
        }
        
        var settings = (SettingsCast)method.Invoke(null, null);
        SaveAPI.settings = settings;
        return settings;
    }

    public static bool SaveToFile(string content, string filePath)
    {
        if (Instance == null)
        {
            throw new InvalidOperationException("Instance is not set.");
        }

        var method = Instance.GetMethod("SaveToFile", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (method == null)
        {
            throw new MissingMethodException($"Static method 'SaveToFile' not found in {Instance.FullName}.");
        }

        return (bool)method.Invoke(null, [content, filePath]);
    }

    public static string LoadFromFile(string filePath)
    {
        if (Instance == null)
        {
            throw new InvalidOperationException("Instance is not set.");
        }

        var method = Instance.GetMethod("SetDefaultSettings", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (method == null)
        {
            throw new MissingMethodException($"Static method 'SetDefaultSettings' not found in {Instance.FullName}.");
        }

        return (string)method.Invoke(null, [filePath]);
    }
    
    public static string LoadFromAssets(string filePath)
    {
        if (Instance == null)
        {
            throw new InvalidOperationException("Instance is not set.");
        }

        var method = Instance.GetMethod("LoadFromAssetsFile", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (method == null)
        {
            throw new MissingMethodException($"Static method 'LoadFromAssetsFile' not found in {Instance.FullName}.");
        }

        return (string)method.Invoke(null, [filePath]);
    }
}