using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;

namespace GameEngine;

/// <summary>
/// MusicController is a static manager for background music
/// It stores all loaded songs and controls playback globally via MonoGame's MediaPlayer
/// </summary>
public static class MusicController
{
    public static Dictionary<string, Song> Songs = new();

    public static void AddSong(string key, Song song)
    {
        Songs.Add(key, song);
    }

    public static void SetVolume(float volume)
    {
        MediaPlayer.Volume = volume;
    }

    public static void setIsDeaf(bool isDeaf)
    {
        MediaPlayer.IsMuted = isDeaf;
    }

    public static bool ChangeSong(string key)
    {
        if (!Songs.ContainsKey(key)) return false;
        MediaPlayer.Play(Songs[key]);
        return true;
    }

    public static bool PauseSong()
    {
        MediaPlayer.Pause();
        return true;
    }

    public static bool ResumeSong()
    {
        MediaPlayer.Resume();
        return true;
    }

    public static bool StopSong()
    {
        MediaPlayer.Stop();
        return true;
    }
}