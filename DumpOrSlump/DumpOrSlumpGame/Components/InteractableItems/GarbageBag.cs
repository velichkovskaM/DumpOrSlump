using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameEngine.Components;
using GameEngine.Core;

namespace DumpOrSlumpGame.Components.InteractableItems
{
    /// <summary>
    /// Interactable garbage bag that the player can open/close while cleaning
    /// Manages visibility, interaction raycasts, and ties into player state
    /// </summary>
    internal class GarbageBag : Component
    {
        Texture2D bag_asset;
        public bool isCleaning = false;
        private Player player;
        private Camera camera;
        private SpriteRenderer spriteRenderer;

        
        public GarbageBag(Node parent) : base(parent) { }

        // Start: load asset, set idle animation, fetch references
        public override void Start(IScene scene)
        {
            bag_asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");

            var deadSpace = 40;
            
            var animData = new AnimationData(
                bag_asset,
                new [] { new Rectangle(256 * 7, 256, 256, 256) },
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

        // Update: toggle visibility and handle touch-based interaction
        public override void Update(GameTime gameTime, TouchCollection touchCollection)
        {
            // Show/Hide while cleaning
            if (isCleaning)
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
                        if (player != null) player.toggleBin();
                    }
                }
            }
        }
    }
}

