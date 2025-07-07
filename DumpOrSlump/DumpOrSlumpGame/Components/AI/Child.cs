using DumpOrSlumpGame.Components.CleanupObjects;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using AudioEmitter = GameEngine.Components.AudioEmitter;

namespace DumpOrSlumpGame.Components.AI;

/// <summary>
/// Finite‑state machine that controls the child NPC. Responsible for movement, animation switching, clutter spawning,
/// and triggering camera pans when the child enters the scene
/// </summary>
internal class Child : Component
{
    public enum ChildState
    {
        PlayingWithToys = 0,
        WalkingToLadder = 1,
        Shooting = 2,
        Crawling = 3,
        CrawlingForExit = 4,
        WalkingBackToPlayWithToy = 5
    }
    
    ChildState state = ChildState.PlayingWithToys;

    private double ShootingStartTimer = 0;
    
    private Vector3 crawlingPosition = Vector3.Zero;

    private Node ClutterHolder;

    private double spawnTime = 60;
    private double timeToAppearFor = 15;
    
    private Vector3 _ladderStartPosition = Vector3.Zero;
    private Vector3 _playeringWithToysPosition;
    
    private bool appeared = false;

    private SpriteRenderer _spriteRenderer;
    private AudioEmitter _audioEmitter;

    // Constructor stores the ladder start and initial toy position
    public Child(Node parent, Vector3 ladderStartPosition) : base(parent)
    {
        _ladderStartPosition = ladderStartPosition;
        _playeringWithToysPosition = parent.Transform.Position;
    }

    // Loads animations, sound, and finds scene nodes. Called once the component is added
    public override void Start(IScene scene)
    {
        ClutterHolder = scene.FindNodeByName("ClutterHolder");
        
        var _texture = Globals.content.Load<Texture2D>("SpriteSheets/BrotherAISpriteSheet");
        var _animationCrawling = new Rectangle[2];
        _animationCrawling[0] = new Rectangle(0, 256 * 2, 256, 256);
        _animationCrawling[1] = new Rectangle(256, 256 * 2, 256, 256);
        
        var _animationShooting = new Rectangle[3];
        _animationShooting[0] = new Rectangle(0, 256, 256, 256);
        _animationShooting[1] = new Rectangle(256, 256, 256, 256);
        _animationShooting[2] = new Rectangle(256 * 2, 256, 256, 256);
        
        var _animationWalking = new Rectangle[4];
        _animationWalking[0] = new Rectangle(0, 0, 256, 256);
        _animationWalking[1] = new Rectangle(256, 0, 256, 256);
        _animationWalking[2] = new Rectangle(256 * 2, 0, 256, 256);
        _animationWalking[3] = new Rectangle(256 * 3, 0, 256, 256);
        
        var _animationPlayingWithToys = new Rectangle[1];
        _animationPlayingWithToys[0] = new Rectangle(256 * 2, 256, 256, 256);

        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        _spriteRenderer.AddAnimation("idle", new AnimationData(
            _texture,
            _animationWalking,
            0.2f,
            Render2D: false
            ));
        
        _spriteRenderer.AddAnimation("walk", new AnimationData(
            _texture,
            _animationWalking,
            0.2f,
            Render2D: false
        ));
        
        _spriteRenderer.AddAnimation("crawl", new AnimationData(
            _texture,
            _animationCrawling,
            0.2f,
            Render2D: false
        ));
        
        _spriteRenderer.AddAnimation("shooting", new AnimationData(
            _texture,
            _animationShooting,
            0.2f,
            Render2D: false
        ));

        _audioEmitter = Parent.GetComponent<AudioEmitter>();
        _audioEmitter.SetSoundEffect(Globals.content.Load<SoundEffect>("SoundEffect/throw"));
        _audioEmitter.CurrentSoundInstance.Volume = 1.0f;
        
        state = ChildState.PlayingWithToys;
    }

    // Advances timers, checks for state transitions, and delegates movement behaviour to per‑state handlers
    public override void Update(GameTime gameTime, TouchCollection touchCollection)
    {
        _spriteRenderer.Flipped = true;
        
        // One‑shot: decide when to appear for the first time
        if (state == ChildState.PlayingWithToys && Time.totalTimeSinceReload >= spawnTime && !appeared)
        {
            state = ChildState.WalkingToLadder;
            
            // Tell the camera director to pan over so the player sees the child enter
            Game1.SetPanningCamera(Parent.Transform.Position + (Vector3.Backward * 3 + Vector3.Down + Vector3.Left) );
            appeared = true;
        }

        switch (state)
        {
            case ChildState.PlayingWithToys:
                break;
            case ChildState.WalkingToLadder:
                HandleWalkingToLadder();
                break;
            case ChildState.Shooting:
                HandleShooting();
                break;
            case ChildState.Crawling:
                HandlingCrawling();
                break;
            case ChildState.CrawlingForExit:
                HandleCrawlingForExit();
                break;
            case ChildState.WalkingBackToPlayWithToy:
                HandleWalkingBackToPlayWithToy();
                break;
        }
    }

    // Walks horizontally towards the ladder start position. When close enough switches to the crawling state
    private void HandleWalkingToLadder()
    {
        var dir = _ladderStartPosition - Parent.Transform.Position;
        Move(dir);
        
        if (dir.Length() <= 0.1f)
        {
            _spriteRenderer.SetAnimation("crawl");
            Parent.Transform.Position = new Vector3(Parent.Transform.Position.X, Parent.Transform.Position.Y, _ladderStartPosition.Z + 0.1f);
            GenerateRandomPoint();
            state = ChildState.Crawling;
        }
    }

    // After the exit crawl, walk back to the toy area and resume idle
    private void HandleWalkingBackToPlayWithToy()
    {
        var dir = _playeringWithToysPosition - Parent.Transform.Position;
        Move(dir);
        
        if (dir.Length() <= 0.3f)
        {
            _spriteRenderer.SetAnimation("idle");
            state = ChildState.PlayingWithToys;
        }
    }

    private void HandleCrawlingForExit()
    {
        var dir = _ladderStartPosition - Parent.Transform.Position;
        Move(dir);

        if (dir.Length() <= 0.3f)
        {
            _spriteRenderer.SetAnimation("walk");
            state = ChildState.WalkingBackToPlayWithToy;
        }
    }

    // Crawls to a random rung. Once reached, switches to ChildState.Shooting
    // If the allotted on‑screen time has elapsed, begins exit crawl instead
    private void HandlingCrawling()
    {
        Game1.Instance.waitBetweenPanning = false;
        var dir = crawlingPosition - Parent.Transform.Position;
        Move(dir);

        if (dir.Length() <= 0.3f)
        {
            _spriteRenderer.SetAnimation("shooting");
            ShootingStartTimer = Time.totalTimeSinceReload;
            state = ChildState.Shooting;
        } else if (Time.totalTimeSinceReload - spawnTime > timeToAppearFor)
        {
            _spriteRenderer.SetAnimation("crawl");
            state = ChildState.CrawlingForExit;
        }
    }

    // Plays the throw animation and, when finished, spawns paper projectile node that homes towards the player.
    // Chooses a new crawl target or exits depending on the on‑screen timer.
    private void HandleShooting()
    {
        if (Time.totalTimeSinceReload - ShootingStartTimer > _spriteRenderer.GetAnimationTime("shooting"))
        {
            _audioEmitter.PlaySound();
            var nodePosition = Parent.Transform.Position;
            nodePosition.Y = -0.5f;
            
            Node player = Game1.GetScene().FindNodeByName("Player");
            var clutterHandler = Game1.GetScene().FindNodeByName("ClutterHolder");
            
            var node = new Node("Clutter", Parent.Transform.Position, scale: new Vector3(0.15f, 0.15f, 0.15f));
            var spriteRenderer = new SpriteRenderer(node, Globals._Graphics.GraphicsDevice);
            node.AddComponent(spriteRenderer);
            var clutterScript = new Clutter(node, player, true);
            node.AddComponent(clutterScript);
            
            Parent.GetScene().SafeInsert(node);
            clutterHandler.Children.Add(node);
            
            GenerateRandomPoint();

            // Decide next state
            if (Time.totalTimeSinceReload - spawnTime > timeToAppearFor)
            {
                _spriteRenderer.SetAnimation("crawl");
                state = ChildState.CrawlingForExit;
            }
            else
            {
                _spriteRenderer.SetAnimation("crawl");
                state = ChildState.Crawling;
            }
        }
    }

    // Picks a random ladder rung between 0 and 4 world‑units high
    private void GenerateRandomPoint()
    {
        var randomPoint = new Vector3(_ladderStartPosition.X, (float)Globals.rand.NextDouble() * 4, _ladderStartPosition.Z);
        crawlingPosition = randomPoint;
    }

    private void Move(Vector3 direction)
    {
        Parent.Transform.Position += Vector3.Normalize(direction) * Time.deltaTime * 2;
    }
}