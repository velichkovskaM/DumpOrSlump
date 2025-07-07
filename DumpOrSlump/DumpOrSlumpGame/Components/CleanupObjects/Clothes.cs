using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input.Touch;
using AudioEmitter = GameEngine.Components.AudioEmitter;

namespace DumpOrSlumpGame.Components.CleanupObjects;

/// <summary>
/// Interactive pickup object representing a piece of clothing. When the player is in “sorting” state and performs either
/// a circle gesture or a double‑tap within reachDistance
/// </summary>
public class Clothes : Component
{
    Rectangle[] clothes;
    Texture2D clothes_assets;
    SpriteRenderer spriteRenderer;

    private AudioEmitter audioEmitter;

    private bool isCircleActivated;

    private Player _player;
    private Camera _camera;
    
    private float reachDistance = 1.5f;

    public bool inWardrobe = false;

    // Constructor selects a random clothes frame and registers whether circle gesture recognition is enabled
    public Clothes(Node parent, bool isCircleActivated = true) : base(parent) 
    {
        clothes_assets = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
        clothes = new Rectangle[5];
        clothes[0] = new Rectangle(256 * 1, 256, 256, 256);
        clothes[1] = new Rectangle(256 * 2, 256, 256, 256);
        clothes[2] = new Rectangle(256 * 3, 256, 256, 256);
        clothes[3] = new Rectangle(256 * 4, 256, 256, 256);
        clothes[4] = new Rectangle(256 * 5, 256, 256, 256);
        
        spriteRenderer = parent.GetComponent<SpriteRenderer>();
        var activeClothes = new Rectangle[1];
        activeClothes[0] = clothes[Globals.rand.Next(0, 5)];
        spriteRenderer.AddAnimation("idle", new AnimationData(
            clothes_assets, activeClothes, 0.2f, false, Render2D: false));
        
        this.isCircleActivated = isCircleActivated;
        
    }

    // Caches references to player, camera, and pre‑loads pickup sound
    public override void Start(IScene scene)
    {
        _player = Parent.QuadTreeParent._Scene.FindNodeByName("Player").GetComponent<Player>();
        _camera = Game1.Instance.Scene.root.Find(x => x.name == "Camera")?.GetComponent<Camera>();
        
        audioEmitter = Parent.GetComponent<AudioEmitter>();
        audioEmitter.SetSoundEffect(Globals.content.Load<SoundEffect>("SoundEffect/pickup_clothes"));
        audioEmitter.CurrentSoundInstance.Volume = 1f;
    }
    
    // Checks each frame whether the player is sorting and performs the appropriate gesture within range, if so calls CheckClick
    public override void Update(GameTime gameTime, TouchCollection touches)
    {

        if (_player.player_state != Player.player_states.sorting) return;
        if (isCircleActivated)
        {
            foreach (var touchId in Game1.Instance.customGestures)
            {
                var gestureTouch = Game1.Instance.gestureTrackers[touchId];
                Vector2 point = Vector2.Zero;

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

    // Raycasts from the click position; if the ray hits this sprite’s 3D bounding box, deactivate the node and play a pickup sound
    private void CheckClick(Vector2 point)
    {
        Logger.Error($"Check click point: {point}");
        var ray = Game1.GetCamera().GenerateRayFromClick(point);
        var boundingBox = spriteRenderer.Get3DBoundingBox();

        if (ray.Intersects(boundingBox) != null)
        {
            Logger.Error($"Clicked insde");
            Parent.active = false;
            audioEmitter.PlaySound();
        }
    }
}

