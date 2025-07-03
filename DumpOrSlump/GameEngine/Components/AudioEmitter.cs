using System.Collections.Generic;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameEngine.Components;

/// <summary>
/// Represents a 3D audio emitter component that can play, stop, and manage spatial sound effects within the scene
/// </summary>
public class AudioEmitter : Component
{
    public Microsoft.Xna.Framework.Audio.AudioEmitter CurrentAudioEmitter {get; set;}
    private List<AudioListener> Listeners = new List<AudioListener>();
    public SoundEffect CurrentSound { get; set; }
    public SoundEffectInstance CurrentSoundInstance { get; set; }
    
    // Creates a new audio emitter component attached to the given parent node
    public AudioEmitter(Node parent, bool active = true) : base(parent, active)
    {
        CurrentAudioEmitter = new Microsoft.Xna.Framework.Audio.AudioEmitter();
    }

    // Finds all AudioListener components in the scene
    public override void Start(IScene scene)
    {
        Listeners = scene.FindAllComponents<AudioListener>();
    }

    // Plays the current sound effect. Stops any instance already playing
    public void PlaySound()
    {
        if (CurrentSoundInstance == null) return;
        
        if (CurrentSoundInstance.State == SoundState.Playing)
            CurrentSoundInstance.Stop();
        
        CurrentSoundInstance.Play();
    }

    public void StopSound()
    {
        if (CurrentSoundInstance.State == SoundState.Playing)
            CurrentSoundInstance.Stop();
    }

    // Sets a new sound effect and creates a new instance for playback
    public void SetSoundEffect(SoundEffect soundEffect)
    {
        CurrentSoundInstance?.Dispose();
        CurrentSoundInstance = soundEffect?.CreateInstance();
    }

    // Updates the audio emitter's position to match the parent node's transform
    // Applies 3D audio calculations using the first audio listener found
    public override void Update(GameTime gameTime, TouchCollection touches)
    {
        CurrentAudioEmitter.Position = Parent.Transform.Position;
        
        if (Listeners.Count > 0 && CurrentSoundInstance != null)
        {
            CurrentSoundInstance.Apply3D(
                Listeners[0].CurrentAudioListener,
                CurrentAudioEmitter
            );
        }
    }
}