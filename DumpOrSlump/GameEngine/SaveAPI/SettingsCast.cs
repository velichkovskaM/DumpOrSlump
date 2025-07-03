namespace Dump_Or_Slump_Android.SaveAPI;

/// <summary>
/// Represents the persistent game settings and session stats
/// Data structure that will be saved to or loaded from a file
/// </summary>
public class SettingsCast
{
    public float volume { get; set; } = 1;
    public bool sound_on_off { get; set; } = true;

    public bool have_played { get; set; } = false;
    
    public int clutter_picked_up { get; set; } = 0;
    public int dust_picked_up { get; set; } = 0;
    public int clothes_picked_up { get; set; } = 0;
    
    public int clutter_missed { get; set; } = 0;
    public int dust_missed { get; set; } = 0;
    public int clothes_missed { get; set; } = 0;
    
    public bool died { get; set; } = true;
    
    public float time_left { get; set; } = 0;
    public string reason { get; set; } = "";
    
}