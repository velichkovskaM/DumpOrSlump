using System;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace DumpOrSlumpGame.Components.UI
{
    public enum Direction
    {
        Idle,
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// On-screen joystick that translates touch drag into a cardinal direction
    /// Handles sprite changes, drag tracking, and exposes the current drag vector
    /// </summary>
    public class Controller : Component
    {
        Rectangle[] controller;
        Texture2D controls;

        private bool is_dragging;
        private Vector2 drag_start_position;
        public static Vector2 drag_direction;
        private Rectangle movementBounds;

        public static int currentMovementId;
        
        private Direction currentDirection = Direction.Idle;
        
        Vector2 position = Vector2.Zero;
        
        float scale = 0.7f;
        
        SpriteRenderer spriteRenderer;

        // Constructor: load sprites, set up animations & initial state
        public Controller(Node parent) : base(parent)
        {
            controls = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
            
            spriteRenderer = parent.GetComponent<SpriteRenderer>();
            
            spriteRenderer.AddAnimation("idle",
                new AnimationData(
                    controls,
                    new Rectangle[] {new (0, 256 * 8, 256 * 2, 256 * 2)},
                    0.2f,
                    false,
                    isUI: true
                    )
                );
            
            spriteRenderer.AddAnimation("up",
                new AnimationData(
                    controls,
                    new Rectangle[] {new (256 * 6, 256 * 8, 256 * 2, 256 * 2)},
                    0.2f,
                    false,
                    isUI: true
                )
            );
            
            spriteRenderer.AddAnimation("down",
                new AnimationData(
                    controls,
                    new Rectangle[] {new (256 * 8, 256 * 8, 256 * 2, 256 * 2)},
                    0.2f,
                    false,
                    isUI: true
                )
            );
            
            spriteRenderer.AddAnimation("left",
                new AnimationData(
                    controls,
                    new Rectangle[] {new (256 * 2, 256 * 8, 256 * 2, 256 * 2)},
                    0.2f,
                    false,
                    isUI: true
                )
            );
            spriteRenderer.AddAnimation("right",
                new AnimationData(
                    controls,
                    new Rectangle[] {new (256 * 4, 256 * 8, 256 * 2, 256 * 2)},
                    0.2f,
                    false,
                    isUI: true
                )
            );
            
            drag_direction = Vector2.Zero;
        }

        // start drag: capture id & initial position
        public void OnClick(TouchLocation touch)
        {
            is_dragging = true;
            drag_start_position = touch.Position;
            currentMovementId = touch.Id;
        }
        
        // per‑frame: update drag vector, determine direction, switch sprites
        public override void Update(GameTime gameTime, TouchCollection touchCollection)
        {
            position = new Vector2(180, Globals._Graphics.GraphicsDevice.Viewport.Height - 200);

            foreach (TouchLocation touch in touchCollection)
            {

                if (touch.State == TouchLocationState.Moved && touch.Id == currentMovementId && is_dragging)
                {
                    drag_direction = touch.Position - drag_start_position;
                    if (drag_direction.Length() > 0)
                    {
                        drag_direction.Normalize();
                        if (Math.Abs(drag_direction.X) > Math.Abs(drag_direction.Y))
                        {
                            if (drag_direction.X > 0)
                                currentDirection = Direction.Right;
                            else
                                currentDirection = Direction.Left;
                        }
                        else
                        {
                            if (drag_direction.Y > 0)
                                currentDirection = Direction.Down;
                            else
                                currentDirection = Direction.Up;
                        }
                    }
                }

                if (touch.State == TouchLocationState.Released && touch.Id == currentMovementId && is_dragging)
                {
                    is_dragging = false;
                    drag_direction = Vector2.Zero;
                    currentDirection = Direction.Idle;
                }
            }
            
            switch (currentDirection)
            {
                case Direction.Idle:
                    spriteRenderer.SetAnimation("idle");
                    break;
                case Direction.Up:
                    spriteRenderer.SetAnimation("up");
                    break;
                case Direction.Down:
                    spriteRenderer.SetAnimation("down");
                    break;
                case Direction.Left:
                    spriteRenderer.SetAnimation("left");
                    break;
                case Direction.Right:
                    spriteRenderer.SetAnimation("right");
                    break;
            }
        }
    }
}
