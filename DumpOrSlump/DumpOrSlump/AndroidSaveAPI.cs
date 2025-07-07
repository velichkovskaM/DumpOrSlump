using System;
using System.IO;
using System.Text.Json;
using Android.App;
using Dump_Or_Slump_Android.SaveAPI;
using GameEngine;

namespace Dump_Or_Slump_Android;

/// <summary>
/// Android‑specific implementation that persists JSON files to the app’s private Context.FilesDir directory
/// It handles user settings as well as arbitrary text content, plus convenient helpers for loading bundled asset files
/// </summary>
public class AndroidSaveAPI : ISaveAPI
{
    // SetDefaultSettings ─ generate brand‑new settings.json on the first run
    public static void SetDefualtSettings()
    {
        var settings = new SettingsCast();
        
        SaveToFile(JsonSerializer.Serialize(settings), "settings.json");
    }

    // SettingsExists ─ quick existence check without opening the file
    public static bool SettingsExists()
    {
        return File.Exists(Path.Combine(Application.Context.FilesDir.AbsolutePath, "settings.json"));
    }
    
    // SaveSettingsFile ─ Saves the settings to settings.json
    public static bool SaveSettingsFile(SettingsCast settings)
    {
        try
        {
            string filePath = Path.Combine(Application.Context.FilesDir.AbsolutePath, "settings.json");
            File.WriteAllText(filePath, JsonSerializer.Serialize(settings));
            return true;
        }
        catch(Exception e)
        {
            return false;
        }
    }

    // LoadSettingsFile ─ returns SettingsCast or default on failure
    public static SettingsCast LoadSettingsFile()
    {
        try
        {
            string filePath = Path.Combine(Application.Context.FilesDir.AbsolutePath, "settings.json");
            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    var json = reader.ReadToEnd();
                    var settings = JsonSerializer.Deserialize<SettingsCast>(json);
                    return settings;
                }
            }
        }
        catch (Exception e)
        {
            return new SettingsCast();
        }
    }

    // SaveToFile ─ write arbitrary string content to internal storage
    public static bool SaveToFile(string content, string filename)
    {
        try
        {
            string filePath = Path.Combine(Application.Context.FilesDir.AbsolutePath, filename);
            File.WriteAllText(filePath, content);
            return true;
        }
        catch(Exception e)
        {
            return false;
        }
    }
    
    // LoadFromFile ─ read a text file from internal storage; empty string on error
    public static string LoadFromFile(string filePath)
    {
        try
        {
            string path = Path.Combine(Application.Context.FilesDir.AbsolutePath, filePath);
            using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    string txt = reader.ReadToEnd();
                    return txt;
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error($"Couldnt load file due to: {e}");
            return "";
        }
    }
    
    // LoadFromAssetsFile ─ loads assets from the content folder
    public static string LoadFromAssetsFile(string filePath)
    {
        using (var stream = Application.Context.Assets.Open("Content/" + filePath))
        {
            using (var reader = new StreamReader(stream))
            {
                string txt = reader.ReadToEnd();
                return txt;
            }
        }
    }
}