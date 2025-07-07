using GameEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Core;
using Microsoft.Xna.Framework.Input.Touch;

namespace DumpOrSlumpGame.Components.CleanupObjects;

/// <summary>
/// When the player is in “vacuuming” state and performs either a circle gesture or a double‑tap within reachDistance, the dust node is deleted
/// </summary>
public class Dust : Component
{
    Rectangle[] dust;
    Texture2D dust_asset;
    private SpriteRenderer _spriteRenderer;
    
    private bool isCircleActivated;

    private Player _player;
    private Camera _camera;
    
    private float reachDistance = 1.5f;
    
    public bool inTrashCan = false;

    // Picks a random dust frame and registers gesture preference
    public Dust(Node parent, bool isCircleActivated = true) : base (parent)
    {
        dust_asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
        dust = new Rectangle[2];
        dust[0] = new Rectangle(256 * 9, 256, 256, 256);
        dust[1] = new Rectangle(256 * 8, 256, 256, 256);
        
        var activeDust = new Rectangle[1];
        activeDust[0] = dust[Globals.rand.Next(0, 2)];
        
        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        _spriteRenderer.AddAnimation("idle", new AnimationData(
            dust_asset, activeDust, 0.2, false, Render2D: false
            ));
        
        this.isCircleActivated = isCircleActivated;
    }
    
    // Caches references to player and camera for distance checks and raycasts
    public override void Start(IScene scene)
    {
        _player = Parent.QuadTreeParent._Scene.FindNodeByName("Player").GetComponent<Player>();
        _camera = Game1.Instance.Scene.root.Find(x => x.name == "Camera")?.GetComponent<Camera>();
    }
    
    public override void Update(GameTime gameTime, TouchCollection touchCollection)
    {
        if (_player.player_state != Player.player_states.vacuuming) return;
        if (isCircleActivated)
        {
            foreach (var touchId in Game1.Instance.customGestures)
            {
                var gestureTouch = Game1.Instance.gestureTrackers[touchId];
                Vector2 point;

                switch (gestureTouch.Gesture)
                {
                    case GestureTracker.GestureType.Circle:
                        point = gestureTouch.center;
                        break;
                    default:
                        continue;
                }
                
                if (Vector3.Distance(_player.Parent.Transform.Position, Parent.Transform.Position) < reachDistance) CheckClick(point);
            }
        }
        else
        {
            foreach (var gesture in Game1.Instance.Gestures)
            {
                if (gesture.GestureType == GestureType.DoubleTap && Vector3.Distance(_player.Parent.Transform.Position, Parent.Transform.Position) < reachDistance)
                {
                    CheckClick(gesture.Position);
                }
            }
        }
    }
    
    // Raycasts through point if it intersects the dust’s bounding box, the node is deactivated (vacuumed)
    private void CheckClick(Vector2 point)
    {
        var ray = Game1.GetCamera().GenerateRayFromClick(point);
        var boundingBox = _spriteRenderer.Get3DBoundingBox();

        if (ray.Intersects(boundingBox) != null)
        {
            Parent.active = false;
        }
    }
}
