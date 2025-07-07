using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BoundingBox = GameEngine.Core.BoundingBox;


namespace DumpOrSlumpGame.Components.UI.Buttons
{
    /// <summary>
    /// Horizontal bar representing volume range. Hosts a draggable Knob child and computes its bounding area
    /// </summary>    
    public class SoundBar : Component
    {
        private float volume;
        private int touchId = 0;
        
        SpriteRenderer _spriteRenderer;
        public BoundingBox _boundingBox;
        
        public SoundBar(Node parent, bool active = true) : base(parent, active) { }

        // load bar sprite, spawn knob, compute bounding box
        public override void Start(IScene scene)
        {
            var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
            
            var bar = new [] {new Rectangle(256 * 4 + 10, 256 * 6, 256 * 2 - 10, 256)};
            _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
            _spriteRenderer.AddAnimation("idle", new AnimationData(
                asset,
                bar,
                1,
                false,
                isUI: true
            ));

            // spawn knob node centered on bar
            var knobNode = new UINode("knob",
                Parent.Transform.Position + new Vector3(0, 0.1f, 0),
                scale: new Vector3(0.4f, 1, 0.4f)
                );
            knobNode.AddComponent(new SpriteRenderer(knobNode, Globals._Graphics.GraphicsDevice));
            var knobScript = new Knob(knobNode, this);
            knobNode.AddComponent(knobScript);
            knobNode.SetActive(false);
            Game1.GetScene().SafeInsertUi(knobNode);
            
            Parent.Children.Add(knobNode);
            
            // calculate UI bounding box for touch interaction
            var (width, height) = _spriteRenderer.GetDimensions();
            _boundingBox = new BoundingBox(new Vector2(-(width / 2), -(height / 4)), new Vector2(width / 2, height / 4)) * Parent.Transform.Scale + Parent.Transform.Position;
        }
    }
}
