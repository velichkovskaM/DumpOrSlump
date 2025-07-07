using System;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace DumpOrSlumpGame.Components.UI.Buttons.HelpMenu;

/// <summary>
/// Scrollable help screen. Handles touch-drag scrolling of a tall texture inside a fixed viewport
/// </summary>
public class HelpMenu : Component
{
    private SpriteRenderer _spriteRenderer;
    private Rectangle _rect;
    private Rectangle[] _background;
    private Vector2? _touchStartPosition;
    private int _originalRectY;
    
    private Texture2D asset;
    
    private const int ViewportHeight = 1000;
    private const int MaxScrollY = 1700;
    
    public HelpMenu(Node parent, bool active = true) : base(parent, active) { }

    // load texture & set initial animation frame
    public override void Start(IScene scene)
    {
        asset = Globals.content.Load<Texture2D>("SpriteSheets/HelpMenu");
        _rect = new Rectangle(0, 0, asset.Width, ViewportHeight);
        _background = new[] { _rect };

        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
        _spriteRenderer.AddAnimation("idle", new AnimationData(
            asset,
            _background,
            1,
            false,
            isUI: true
        ));
    }

    // handle drag scrolling & update animation
    public override void Update(GameTime gameTime, TouchCollection touches)
    {
        foreach (var touch in touches)
        {
            if (touch.State == TouchLocationState.Pressed)
            {
                _touchStartPosition = touch.Position;
                _originalRectY = _background[0].Y;
            }

            if (touch.State == TouchLocationState.Moved)
            {
                if (!_touchStartPosition.HasValue) return;
                
                int deltaY = (int)(touch.Position.Y - _touchStartPosition.Value.Y);
                int newY = _originalRectY - deltaY;
                
                newY = Math.Clamp(newY, 0, MaxScrollY);
                _rect.Y = newY;
                _background[0] = _rect;
                
                _spriteRenderer.AddAnimation("idle", new AnimationData(
                    asset,
                    _background,
                    1,
                    false,
                    isUI: true
                ));
            }

            if (touch.State == TouchLocationState.Released)
            {
                if (!_touchStartPosition.HasValue) return;
                
                int deltaY = (int)(touch.Position.Y - _touchStartPosition.Value.Y);
                int newY = _originalRectY - deltaY;
                newY = Math.Clamp(newY, 0, MaxScrollY);
                _rect.Y = newY;
                _background[0] = _rect;
                
                _touchStartPosition = null;
                
                _spriteRenderer.AddAnimation("idle", new AnimationData(
                    asset,
                    _background,
                    1,
                    false,
                    isUI: true
                ));
            }
        }
    }
}