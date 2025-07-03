using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace GameEngine;

/// <summary>
/// SoundEffectHandler stores all short sound effects as SoundEffectInstances
/// Use it to manage effects like UI clicks, footsteps
/// </summary>
public class SoundEffectHandler
{
    public static Dictionary<string, SoundEffectInstance> SoundEffects = new Dictionary<string, SoundEffectInstance>();
}