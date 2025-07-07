using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameEngine.Components;
using GameEngine.Core;

namespace DumpOrSlumpGame.Components.InteractableItems;

/// <summary>
/// Component representing an interactable clothes basket. Handles asset loading, player interaction, and visibility toggling
/// </summary>
public class ClothesBasket : Component
{
    Texture2D basket_asset;
    public bool isSorting = false;
    Player player { get; set; }
    private SpriteRenderer spriteRenderer;
    private Camera camera;


    public ClothesBasket(Node parent) : base(parent) { }

    // Start: load assets, configure animation, and grab needed references
    public override void Start(IScene scene)
    {
        basket_asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");

        var deadSpace = 30;
        
        var animData = new AnimationData(
            basket_asset,
            new [] { new Rectangle(0, 256, 256, 256) },
            0.2,
            LoopAnimation: false,
            Render2D: false
        );

        spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        
        spriteRenderer.AddAnimation("idle", animData);

        Node playerNode = Game1.Instance.Scene.root.Find(x => x.name == "Player");
        player = playerNode?.GetComponent<Player>();
        
        camera = Game1.Instance.Scene.root.Find(x => x.name == "Camera")?.GetComponent<Camera>();
        
    }

    // Update: per-frame input handling and basket interaction logic
    public override void Update(GameTime gameTime, TouchCollection touchCollection)
    {
        // Toggle visibility while sorting
        if (isSorting)
        {
            spriteRenderer.Disabled = true;
        }
        else
        {
            spriteRenderer.Disabled = false;
        }
        
        foreach (TouchLocation touch in touchCollection)
        {
            
            if (touch.State == TouchLocationState.Pressed && Vector3.Distance(player.Parent.Transform.Position, Parent.Transform.Position) <= 1.5f)
            {
                var ray = Game1.GetCamera().GenerateRayFromClick(touch.Position);
                var boundingBox = spriteRenderer.Get3DBoundingBox();

                if (ray.Intersects(boundingBox) != null)
                {
                    if (player != null) player.toggleClothesBasket();
                }
            }
        }
    }
}
