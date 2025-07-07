using GameEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Core;

namespace DumpOrSlumpGame
{
    /// <summary>
    /// Interactable trash can that, when tapped, moves collected dust or clutter into the can 
    /// Loads a static sprite and prepares for potential touch interaction
    /// </summary>
    internal class TrashCan : Component
    {
        Rectangle[] trash;
        Texture2D trash_asset;
        private SpriteRenderer _spriteRenderer;

        private float range = 2.5f;

        public TrashCan(Node parent) : base(parent) { }

        // load sprite & register animation
        public override void Start(IScene scene)
        {
            trash_asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
            trash = new Rectangle[1];
            trash[0] = new Rectangle(256 * 6, 256, 256, 256);

            _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
            _spriteRenderer.AddAnimation("idle", new AnimationData(
                trash_asset, trash, 0.2, false, Render2D: false
                )
            );
        }
    }
}
