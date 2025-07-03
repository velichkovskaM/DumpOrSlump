using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameEngine.Components;

/// <summary>
/// Represents a 3D audio listener component that defines the listener’s position and orientation for spatial sound effects in the scene
/// </summary>
public class AudioListener : Component
{
    
    public Microsoft.Xna.Framework.Audio.AudioListener CurrentAudioListener {get; set;}
    public SpriteRenderer spriteRenderer {get; set;}

    // Creates a new audio listener attached to the given parent node
    // Initializes the listener and fetches the node’s sprite renderer, if present
    public AudioListener(Node parent, bool active = true) : base(parent, active)
    {
        CurrentAudioListener = new Microsoft.Xna.Framework.Audio.AudioListener();
        spriteRenderer = Parent.GetComponent<SpriteRenderer>();
    }

    public override void Start(IScene scene)
    {
        base.Start(scene);
    }

    // Updates the audio listener’s position and orientation each frame to match the parent node’s transform
    public override void Update(GameTime gameTime, TouchCollection touches)
    {
        CurrentAudioListener.Position = Parent.Transform.Position;
        CurrentAudioListener.Forward = Vector3.Forward;
        CurrentAudioListener.Up = Vector3.Up;
    }
}