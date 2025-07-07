using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using DumpOrSlumpGame.Components.UI;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework.Input.Touch;

namespace DumpOrSlumpGame.Components.AI;

/// <summary>
/// Mother NPC state machine, that enters the room in a certain time and tracks the player, while also damaging when in proximity
/// </summary>
internal class Mother : Component
{
    public enum MotherState
    {
        NoWhere = 0,
        RunningAtChild = 1,
        Hitting = 2,
        Exiting = 3,
        MotherOpeningDoor = 4,
        MotherWalkingOut = 5,
        MotherClosingDoor = 6,
        GameWon = 7,
        GameLost = 8,
    }

    private bool outOfDoor = false;
    
    private Timer timerComponent;
    private Player player;
    private Door door;
    
    public MotherState state;

    private double _hittingTimer;
    private double _totalHitTimer;
    private double _hittingDistance = 2;

    private double _motherInViewTimeInSeconds = 60;
    private double _motherAppearTimeInSecondsAfterStart = 120;
    private Vector3 _spawnLocation;
    private bool appeared = false;
    private bool gameOverHasRun = false;
    private double _halfAngleOfAttack = 40f;
    private double upperLeftAngle;
    private double lowerLeftAngle;
    private double upperRightAngle;
    private double lowerRightAngle;
    private bool overwriteLeftDraw = false;
    private bool overwriteRightDraw = false;

    private double _openingDoorTimer = 0;
    
    private Vector3 MomWalkOutPosition = Vector3.Zero;

    private SpriteRenderer _spriteRenderer;
    
    private bool walkingOut = false;
    
    public Mother(Node parent) : base(parent) { }

    // Loads sprite animations, caches scene references, pre‑computes attack cone angles, and records spawn/exit positions
    public override void Start(IScene scene)
    {
        var animation = Globals.content.Load<Texture2D>("SpriteSheets/MotherAISpriteSheet");

        var animation_walk = new Rectangle[5];
        animation_walk[0] = new Rectangle(0, 0, 256, 256);
        animation_walk[1] = new Rectangle(256, 0, 256, 256);
        animation_walk[2] = new Rectangle(256 * 2, 0, 256, 256);
        animation_walk[3] = new Rectangle(256 * 3, 0, 256, 256);
        animation_walk[4] = new Rectangle(256 * 4, 0, 256, 256);

        var animation_mad = new Rectangle[5];
        animation_mad[0] = new Rectangle(0, 256 * 3, 256, 256);
        animation_mad[1] = new Rectangle(256, 256 * 3, 256, 256);
        animation_mad[2] = new Rectangle(256 * 2, 256 * 3, 256, 256);
        animation_mad[3] = new Rectangle(256 * 3, 256 * 3, 256, 256);
        animation_mad[4] = new Rectangle(256 * 4, 256 * 3, 256, 256);

        var animation_happy = new Rectangle[3];
        animation_happy[0] = new Rectangle(0, 256 * 2, 256, 256);
        animation_happy[1] = new Rectangle(256, 256 * 2, 256, 256);
        animation_happy[2] = new Rectangle(256 * 2, 256 * 2, 256, 256);
        
        var animation_hit = new Rectangle[5];
        animation_hit[0] = new Rectangle(0, 256, 256, 256);
        animation_hit[1] = new Rectangle(256, 256, 256, 256);
        animation_hit[2] = new Rectangle(256 * 2, 256, 256, 256);
        animation_hit[3] = new Rectangle(256 * 3, 256, 256, 256);
        animation_hit[4] = new Rectangle(256 * 4, 256, 256, 256);

        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        _spriteRenderer.AddAnimation("walk", new AnimationData(animation, animation_walk, 0.2f, Render2D: false));
        _spriteRenderer.AddAnimation("mad", new AnimationData(animation, animation_mad, 0.2f, Render2D: false));
        _spriteRenderer.AddAnimation("happy", new AnimationData(animation, animation_happy, 0.2f, Render2D: false));
        _spriteRenderer.AddAnimation("hit", new AnimationData(animation, animation_hit, 0.2f, LoopAnimation: false, Render2D: false));
        
        state = MotherState.NoWhere;
        
        timerComponent = scene.FindNodeByName("Timer").GetComponent<Timer>();
        player = scene.FindNodeByName("Player").GetComponent<Player>();
        door = scene.FindNodeByName("Door").GetComponent<Door>();
        
        _totalHitTimer = _spriteRenderer.GetAnimationTime("mad");
        
        // Pre‑calculate the angle cone for hits (±_halfAngleOfAttack around ±90°)
        upperLeftAngle = 90 - _halfAngleOfAttack;
        lowerLeftAngle = 90 + _halfAngleOfAttack;
        upperRightAngle = (360 - 90) + _halfAngleOfAttack;
        lowerRightAngle = (360 - 90) - _halfAngleOfAttack;
        
        var calcSpawnPosition = scene.FindNodeByName("Door").Transform.Position;
        calcSpawnPosition.Z -= 0.005f;
        _spawnLocation = calcSpawnPosition;
        
        MomWalkOutPosition = new Vector3(_spawnLocation.X, 0, _spawnLocation.Z + 1);
    }

    private void HandleHitting()
    {
        if (Time.totalTime - _hittingTimer > _totalHitTimer)
        {
            state = MotherState.RunningAtChild;
            _spriteRenderer.SetAnimation("walk");
        }
    }

    private void HandleExiting()
    {
        var dir = MomWalkOutPosition - Parent.Transform.Position;
        if (dir.Length() < 0.1f)
        {
            state = MotherState.MotherOpeningDoor;
            _spriteRenderer.SetAnimation("walk");
        }
        
        Move(Vector3.Normalize(dir));
    }

    // Core chasing logic: run at the player, check angle cone & distance for hit
    private void HandleRunningAtChild()
    {
        var dir = player.Parent.Transform.Position - Parent.Transform.Position;
        
        var angleToPlayerInDegrees = Utils.GetAngle(dir);
        var inCorrectAngleLeft = angleToPlayerInDegrees > upperLeftAngle && angleToPlayerInDegrees < lowerLeftAngle;
        var inCorrectAngleRight = angleToPlayerInDegrees > lowerRightAngle && angleToPlayerInDegrees < upperRightAngle;
        
        if (dir.Length() < _hittingDistance && (inCorrectAngleLeft || inCorrectAngleRight))
        {
            state = MotherState.Hitting;
            _hittingTimer = Time.totalTime;
            _spriteRenderer.SetAnimation("hit");
            Game1.GetScene().FindNodeByName("StaminaController")?.GetComponent<StaminaController>()?.HandleHit();
        }
        else
        {
            dir = Vector3.Normalize(HandleWalkingOverlap(dir));
            Move(dir);
        }
    }

    // Adjusts to avoid X‑axis overlap with the player, forcing a slight side‑step so sprites don’t clip
    // Also sets flags for sprite flipping overrides
    private Vector3 HandleWalkingOverlap(Vector3 dir)
    {
        var newPos = Parent.Transform.Position + Vector3.Normalize(dir) * 0.5f * Time.deltaTime;
        
        if (Math.Abs(Parent.Transform.Position.X - player.Parent.Transform.Position.X) < 0.3f)
        {
            if (Parent.Transform.Position.X - player.Parent.Transform.Position.X < 0)
            {
                dir.X = -dir.Z;
                overwriteRightDraw = true;
            }
            else
            {
                dir.X = dir.Z;
                overwriteLeftDraw = true;
            }
        } else if (Math.Abs(newPos.X - player.Parent.Transform.Position.X) < 0.3f)
        {
            dir.X = 0;
        }
        
        return Vector3.Normalize(dir);
    }

    // Handles timed appearance, state machine delegation, game end animation, and sprite flipping each frame
    public override void Update(GameTime gameTime, TouchCollection touchCollection)
    {
        overwriteLeftDraw = false;
        overwriteRightDraw = false;
        
        if (state != MotherState.NoWhere)
        {
            if (timerComponent.timer > _motherAppearTimeInSecondsAfterStart + _motherInViewTimeInSeconds && !walkingOut)
            {
                state = MotherState.Exiting;
                walkingOut = true;
            }
            
            switch (state)
            {
                case MotherState.RunningAtChild:
                    HandleRunningAtChild();
                    break;
                case MotherState.Hitting:
                    HandleHitting();
                    break;
                case MotherState.Exiting:
                    HandleExiting();
                    break;
                case MotherState.MotherOpeningDoor:
                    HandleMotherOpeneingDoor();
                    break;
                case MotherState.MotherWalkingOut:
                    HandleMotherWalkingOut();
                    break;
                case MotherState.MotherClosingDoor:
                    HandleMotherClosingDoor();
                    break;
            }
        }
        
        // First appearance
        if (timerComponent.timer > _motherAppearTimeInSecondsAfterStart && !appeared)
        {
            Parent.Transform.Position = _spawnLocation;
            Game1.SetPanningCamera(Parent.Transform.Position);
            state = MotherState.MotherOpeningDoor;
            _openingDoorTimer = Time.totalTime;
            door.SwitchDoorState();
            appeared = true;
        }

        // Won/Over animations
        if (Game1.isGameWon && !gameOverHasRun)
        {
            _spriteRenderer.SetAnimation("happy");
        }

        else if (Game1.isGameOver && !gameOverHasRun)
        {
            _spriteRenderer.SetAnimation("mad");
        }
        
        if (((Parent.Transform.Position - Parent.Transform.PreviousPosition).X > 0 && !overwriteLeftDraw) || overwriteRightDraw)
        {
            _spriteRenderer.Flipped = false;
        }
        else
        {
            _spriteRenderer.Flipped = true;
        }
    }

    // When the door’s closing animation finishes: either disappear (if walking out) or start chase
    private void HandleMotherClosingDoor()
    {
        if (door.Parent.GetComponent<SpriteRenderer>().AnimationEnded())
        {
            if (walkingOut)
            {
                state = MotherState.NoWhere;
                door.SwitchDoorState();
                Parent.active = false;
            }
            else
            {
                state = MotherState.RunningAtChild;
                _spriteRenderer.SetAnimation("walk");
            }
        }
    }

    // Move either toward the exit point or back toward the spawn, depending on flag
    private void HandleMotherWalkingOut()
    {
        Vector3 dir = walkingOut ? _spawnLocation - Parent.Transform.Position : MomWalkOutPosition - Parent.Transform.Position;

        if (dir.Length() < 0.1f)
        {
            state = MotherState.MotherClosingDoor;
            _openingDoorTimer = Time.totalTime;
            door.SwitchDoorState();
            _spriteRenderer.SetAnimation("walk");
        }
        else
        {
            Move(Vector3.Normalize(dir));
        }
    }

    private void HandleMotherOpeneingDoor()
    {
        if (door.Parent.GetComponent<SpriteRenderer>().AnimationEnded())
        {
            state = MotherState.MotherWalkingOut;
            outOfDoor = true;
            _spriteRenderer.SetAnimation("walk");
        }
    }

    private void Move(Vector3 dir)
    {
        Parent.Transform.Position += dir * 2 * Time.deltaTime;
    }
}
