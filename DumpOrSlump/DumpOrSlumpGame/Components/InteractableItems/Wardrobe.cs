using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BoundingBox = Microsoft.Xna.Framework.BoundingBox;

namespace DumpOrSlumpGame.Components.InteractableItems;

/// <summary>
/// Interactable wardrobe that stores cleaned clothes when the player taps it
/// Sets up its model, bounding box, and links to player/camera for interaction checks
/// </summary>
public class Wardrobe : Component
{
    Model wardrobe_assets;
    ModelRenderer modelRenderer;
    private GameEngine.Core.BoundingBox boundingBox;
    private Camera camera;
    private Player player;
    private BoundingBox _boundingBox;
    private float range = 4.5f;

    // Constructor: load model, configure renderer, cache references, build bounding box
    public Wardrobe(Node parent) : base(parent)
    {
        wardrobe_assets = Globals.content.Load<Model>("3DModels/Wardrobe");
        
        modelRenderer = parent.GetComponent<ModelRenderer>();
        modelRenderer.AddModel(wardrobe_assets);
        player = Game1.Instance.Scene.root.Find(x => x.name == "Player")?.GetComponent<Player>();
        
        camera = Game1.Instance.Scene.root.Find(x => x.name == "Camera")?.GetComponent<Camera>();

        _boundingBox = new BoundingBox(
        new Vector3(-1.2f + Parent.Transform.Position.X, 0, 0 + Parent.Transform.Position.Z),
        new Vector3(1.2f + Parent.Transform.Position.X, 7, 2 + Parent.Transform.Position.Z)
        );
    }
}
