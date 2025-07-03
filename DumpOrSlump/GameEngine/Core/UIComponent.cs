namespace GameEngine.Core;

/// <summary>
/// Component for UI elements, inheriting base behavior from Component
/// </summary>
public class UIComponent : Component
{
    public UIComponent(Node parent, bool active = true) : base(parent, active) { }
}