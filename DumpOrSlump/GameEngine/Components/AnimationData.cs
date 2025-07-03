using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Components;

/// <summary>
/// Defines the data for a sprite sheet animation, frames, frame timing, and rendering options
/// </summary>
public record AnimationData(Texture2D spriteSheet, Rectangle[] frames, double _frametime, bool LoopAnimation = true, bool OriginCenter = true, bool Render2D = true, bool isUI = false) {}