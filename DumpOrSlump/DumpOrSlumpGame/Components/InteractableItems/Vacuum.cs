using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameEngine.Components;
using GameEngine.Core;

namespace DumpOrSlumpGame.Components.InteractableItems;

/// <summary>
/// Interactable vacuum cleaner the player can equip for dust removal
/// Handles asset setup, interaction checks, and visibility based on usage
/// </summary>
public class Vacuum : Component
{
    Texture2D vacuum_asset;
    public bool isVacuuming = false;
    private GraphicsDevice graphicsDevice;
    private Player player { get; set; }
    
    private SpriteRenderer spriteRenderer;
    private Camera camera;

    public Vacuum(Node parent) : base(parent) { }

    // Start: load asset, setup animation, cache references
    public override void Start(IScene scene)
    {
        vacuum_asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");

        var deadSpace = 35;

        var animData = new AnimationData(
            vacuum_asset,
            new [] { new Rectangle(256 * 10, 256, 256, 256) },
            0.2,
            LoopAnimation: false,
            Render2D: false
        );

        spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        
        spriteRenderer.AddAnimation("idle", animData);

        Node playerNode = Game1.Instance.Scene.root.Find(x => x.name == "Player");
        player = playerNode?.GetComponent<Player>();
        
        camera = Game1.Instance.Scene.root.Find(x => x.name == "Camera")?.GetComponent<GameEngine.Components.Camera>();
    }

    // Update: toggle visibility and manage touch interaction
    public override void Update(GameTime gameTime, TouchCollection touchCollection)
    {
        // Show/Hide sprite while vacuuming
        if (isVacuuming)
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
                    if (player != null) player.toggleVacuum();
                }
            }
        }
    }
}
