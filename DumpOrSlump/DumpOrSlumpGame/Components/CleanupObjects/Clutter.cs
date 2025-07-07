using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace DumpOrSlumpGame.Components.CleanupObjects;

/// <summary>
/// Interactive scrap of clutter. Either sits on the floor waiting for the player’s cleanup gesture or, if spawned by
/// the child AI, flies toward a preset target first
/// </summary>
public class Clutter : Component
{
    Rectangle[] clutter;
    Texture2D clutter_assets;

    private float friction = 0.2f;
    private float velocityVariance = 0.15f;
    private int speedVariance = 50;
    
    private SpriteRenderer spriteRenderer;
    private SoundEffectInstance _soundEffect;
    
    private Vector3 _velocity;
    private Vector3 _target;
    private bool isMoving = false;
    
    private bool isCircleActivated;

    private Player _player;
    private Camera _camera;

    private float reachDistance = 1.5f;
    
    public bool inTrashCan = false;

    // Clutter placed in the level at design time
    public Clutter(Node parent, bool isCircleActivated = true) : base(parent)
    {
        clutter_assets = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
        clutter = new Rectangle[1];
        clutter[0] = new Rectangle(256 * 10, 0, 256, 256);
        
        spriteRenderer = parent.GetComponent<SpriteRenderer>();
        _soundEffect = Globals.content.Load<SoundEffect>("SoundEffect/pickup_paper").CreateInstance();
        _soundEffect.Volume = 1.0f;
        spriteRenderer.AddAnimation("idle", new AnimationData(
            clutter_assets, clutter, 0.2f, false, Render2D: false));
        
        this.isCircleActivated = isCircleActivated;
    }

    // Clutter spawned by the child, heads toward a point directly in front of THE player and stops
    public Clutter(Node parent, Node player, bool isCircleActivated = true) : base(parent)
    {
        clutter_assets = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
        clutter = new Rectangle[1];
        clutter[0] = new Rectangle(256 * 10, 0, 256, 256);
        
        _soundEffect = Globals.content.Load<SoundEffect>("SoundEffect/pickup_paper").CreateInstance();
        _soundEffect.Volume = 1.0f;
        
        spriteRenderer = parent.GetComponent<SpriteRenderer>();
        spriteRenderer.AddAnimation("idle", new AnimationData(
            clutter_assets, clutter, 0.2f, false, Render2D: false));
        
        var targetPos = player.Transform.Position;
        targetPos.Y = 0.4f;
        _target = targetPos;
        isMoving = true;
        
        _velocity = Vector3.Normalize(_target - Parent.Transform.Position);
        this.isCircleActivated = isCircleActivated;
    }
    
    public override void Start(IScene scene)
    {
        _player = Parent.QuadTreeParent._Scene.FindNodeByName("Player").GetComponent<Player>();
        _camera = Game1.Instance.Scene.root.Find(x => x.name == "Camera")?.GetComponent<Camera>();
    }

    public override void Update(GameTime gameTime, TouchCollection touches)
    {
        if (!isMoving)
        {
            if (_player.player_state != Player.player_states.cleaning) return;
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
        else
        {
            var velocity = Vector3.Normalize(_target - Parent.Transform.Position);
            Parent.Transform.Position += _velocity * 3 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Parent.Transform.Rotation += new Vector3(0, 0, 45) * (float)gameTime.ElapsedGameTime.TotalSeconds; 

            if ((Parent.Transform.Position - _target).Length() < 0.1f)
            {
                Parent.Transform.Position = new Vector3(_target.X, 0.4f, _target.Z);
                isMoving = false;
            }
        }
    }
    
    private void CheckClick(Vector2 point)
    {
        var ray = Game1.GetCamera().GenerateRayFromClick(point);
        var boundingBox = spriteRenderer.Get3DBoundingBox();

        if (ray.Intersects(boundingBox) != null)
        {
            Parent.active = false;
            _soundEffect.Play();
        }
    }
}
