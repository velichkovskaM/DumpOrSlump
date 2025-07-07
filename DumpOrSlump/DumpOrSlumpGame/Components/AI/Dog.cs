using DumpOrSlumpGame.Components.CleanupObjects;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace DumpOrSlumpGame.Components.AI;

/// <summary>
/// Implements the dog NPC’s simple finite‑state behaviour (wanders to random points inside its room bounds, occasionally spawning dust particles)
/// </summary>
public class Dog : Component
{
    public enum DogState
    {
        Sleeping,
        Sit,
        Walking
    }
    
    private DogState state;

    public float detectionRadius;
    public double walkDuration;
    public double walkTimer;
    private double startSitTime = 0;

    private double spawnDustTimer = 0;
    
    public BoundingBox roomBounds;
    
    public SpriteRenderer spriteRenderer;

    public Vector3 point;
    
    public bool isCircleActivated;

    // Constructor stores design‑time parameters such as detection radius, walk duration, and the bounding box the dog can roam within
    public Dog(Node parent,
        float _detectionRadius,
        float _walkDuration,
        BoundingBox _roomBounds,
        bool isCircleActivated = true,
        bool active = true) : base(parent, active)
    {
        
        detectionRadius = _detectionRadius;
        walkDuration = _walkDuration;
        walkTimer = 0;
        
        roomBounds = _roomBounds;
        
        this.isCircleActivated = isCircleActivated;
    }

    public override void Start(IScene scene)
    {
        var texture = Globals.content.Load<Texture2D>("SpriteSheets/DoggoSpriteSheet");
        
        var animation_walk = new Rectangle[5];
        animation_walk[0] = new Rectangle(0, 0, 256, 256);
        animation_walk[1] = new Rectangle(256, 0, 256, 256);
        animation_walk[2] = new Rectangle(256 * 2, 0, 256, 256);
        animation_walk[3] = new Rectangle(256 * 3, 0, 256, 256);
        animation_walk[4] = new Rectangle(256 * 4, 0, 256, 256);
            
        var animation_sit = new Rectangle[5];
        animation_sit[0] = new Rectangle(0, 256, 256, 256);
        animation_sit[1] = new Rectangle(256, 256, 256, 256);
        animation_sit[2] = new Rectangle(256 * 2, 256, 256, 256);
        animation_sit[3] = new Rectangle(256 * 3, 256, 256, 256);
        animation_sit[4] = new Rectangle(256 * 4, 256, 256, 256);
            
        var animation_sleep = new Rectangle[3];
        animation_sleep[0] = new Rectangle(256 * 5, 256 , 256, 256);
        animation_sleep[1] = new Rectangle(256 * 6, 256, 256, 256);
        animation_sleep[2] = new Rectangle(256 * 7, 256, 256, 256);

        spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        
        spriteRenderer.AddAnimation("sleep", new AnimationData(
            texture,
            animation_sleep,
            0.2,
            Render2D: false
        ));
        
        spriteRenderer.AddAnimation("walk", new AnimationData(
            texture,
            animation_walk,
            0.2,
            Render2D: false
            ));
        
        spriteRenderer.AddAnimation("sit", new AnimationData(
            texture,
            animation_sit,
            0.2,
            Render2D: false
        ));
        
        Parent.Transform.Position = new Vector3(
            (float)(roomBounds.minimum.X + Globals.rand.NextDouble() * (roomBounds.maximum.X - roomBounds.minimum.X)),
            0.2f,
            (float)(roomBounds.minimum.Y + Globals.rand.NextDouble() * (roomBounds.maximum.Y - roomBounds.minimum.Y))
        );
    }
    
    private void SetRandomPosition()
    {
        point = new Vector3(
            (float)(roomBounds.minimum.X + Globals.rand.NextDouble() * (roomBounds.maximum.X - roomBounds.minimum.X)),
            0.2f,
            (float)(roomBounds.minimum.Y + Globals.rand.NextDouble() * (roomBounds.maximum.Y - roomBounds.minimum.Y))
        );
    }

    public override void Update(GameTime gameTime, TouchCollection touches)
    {
        var player = Game1.GetScene().FindNodeByName("Player");
        if (player == null) return;
        switch (state)
        {
            case DogState.Sleeping:
                var dirPlayer = player.Transform.Position - Parent.Transform.Position;
                if (dirPlayer.Length() < detectionRadius)
                {
                    spriteRenderer.SetAnimation("walk");
                    walkTimer = gameTime.TotalGameTime.TotalSeconds;
                    SetRandomPosition();
                    state = DogState.Walking;
                    spawnDustTimer = 0;
                }
                break;
            case DogState.Sit:
                if (gameTime.TotalGameTime.TotalSeconds - startSitTime > spriteRenderer.GetAnimationTime("sit"))
                {
                    spriteRenderer.SetAnimation("sleep");
                    state = DogState.Sleeping;
                }

                break;
            case DogState.Walking:
                spawnDustTimer += gameTime.ElapsedGameTime.TotalSeconds;
                var dirPoint = point - Parent.Transform.Position;
                if (gameTime.TotalGameTime.TotalSeconds - walkTimer >= walkDuration)
                {
                    spriteRenderer.SetAnimation("sit");
                    startSitTime = gameTime.TotalGameTime.TotalSeconds;
                    state = DogState.Sit;
                    return;
                }

                var distanceToPoint = dirPoint.Length();

                if (distanceToPoint < 0.2f)
                {
                    SetRandomPosition();
                }

                // Dust effects every ~1.5s, 50% chance
                if (spawnDustTimer >= 1.5f)
                {
                    if (Globals.rand.NextDouble() > 0.5)
                    {
                        SpawnDust();
                    }

                    spawnDustTimer = 0;
                }

                Parent.Transform.Position += Vector3.Normalize(dirPoint) * Time.deltaTime * 2;
                break;
        }
        
        if ((Parent.Transform.Position - Parent.Transform.PreviousPosition).X < 0)
        {
            spriteRenderer.Flipped = true;
        }
        else
        {
            spriteRenderer.Flipped = false;
        }
    }
    
    private void SpawnDust()
    {
        var dustHandler = Game1.GetScene().FindNodeByName("DustHolder");
        
        var node = new Node("Dust",
            position: new Vector3(Parent.Transform.Position.X - 0.5f, 0.3f, Parent.Transform.Position.Z - 0.5f),
            scale: new Vector3(1f, 1f, 1f),
            rotation: new Vector3(90, 0, 0)
        );
        var spriteRenderer = new SpriteRenderer(node, Globals._Graphics.GraphicsDevice);
        node.AddComponent(spriteRenderer);
        var dustScript = new Dust(node, isCircleActivated);
        node.AddComponent(dustScript);
        
        dustHandler.Children.Add(node);
        
        Parent.GetScene().SafeInsert(node);
    }
}