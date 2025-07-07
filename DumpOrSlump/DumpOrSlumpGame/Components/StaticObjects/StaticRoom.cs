using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Components.StaticObjects;

/// <summary>
/// Static non-interactable room model. Loads a single 3D room asset at startup and attaches it to the scene’s ModelRenderer.
/// </summary>
public class StaticRoom : Component
{
    ModelRenderer modelRenderer;

    public StaticRoom(Node parent) : base(parent) { }

    // Load room model and register it with renderer
    public override void Start(IScene scene)
    {
        var assets = Globals.content.Load<Model>("3DModels/TestRoom");
            
        modelRenderer = Parent.GetComponent<ModelRenderer>();
        modelRenderer.AddModel(assets);
    }
}