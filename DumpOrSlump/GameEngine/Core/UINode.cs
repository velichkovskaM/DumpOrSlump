using GameEngine.Components;
using Microsoft.Xna.Framework;

namespace GameEngine.Core;

/// <summary>
/// Represents a scene graph node specifically for UI elements, applying UI-specific scaling during construction
/// </summary>
public class UINode : Node
{
    public UINode(string name, Vector3? position = null, Vector3? rotation = null, Vector3? scale = null) : base(
        name,
        position,
        rotation,
        scale * new Vector3(Camera.scale,1, Camera.scale)
        ) { }
}