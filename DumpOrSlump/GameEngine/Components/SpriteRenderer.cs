using System;
using System.Collections.Generic;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using BoundingBox = Microsoft.Xna.Framework.BoundingBox;

namespace GameEngine.Components;

/// <summary>
/// Renders animated 2D sprites as textured quads in world space or screen space
/// Handles multiple named animations and simple frame updates
/// </summary>
public class SpriteRenderer : Component
{
    public Dictionary<String, AnimationData> Animations;
    
    public String CurrentKey { get; set; }
    public AnimationData CurrentAnimation { get; set; }
    public int CurrentFrame { get; set; }
    public double AnimationTimer = 0;
    public bool Flipped = false;
    public bool Disabled = false;
    
    private BasicEffect _effect;
    private VertexPositionTexture[] _vertices;
    private short[] _indices;
    
    private GraphicsDevice _device;
    private float width = 0;
    private float height = 0;
    
    // Creates a new sprite renderer attached to a parent node
    public SpriteRenderer(Node parent, GraphicsDevice graphicsDevice) : base(parent)
    {
        Animations = new Dictionary<string, AnimationData>();
        
        _device = graphicsDevice;

        InitializeQuad();
        InitializeEffect(graphicsDevice);
    }
    
    // Sets up the vertex array for the quad
    private void InitializeQuad()
    {
        _vertices = new VertexPositionTexture[4];
    }

    // Updates the quad’s size if the current frame dimensions have changed
    public void UpdateQuad()
    {
        var frame = CurrentAnimation.frames[CurrentFrame];
        float frameHeight = (float)frame.Height / 256 * 2;
        float frameWidth = (float)frame.Width / 256 * 2;
        
        float halfWidth = frameWidth / 2f;
        float fullHeight = frameHeight;

        if (Math.Abs(width - frameWidth) > float.Epsilon || Math.Abs(height - frameHeight) > float.Epsilon)
        {
            
            width = frameWidth;
            height = frameHeight;
            
            _vertices[0] = new VertexPositionTexture(new Vector3(-halfWidth, 0, 0), new Vector2(0, 1));
            _vertices[1] = new VertexPositionTexture(new Vector3(halfWidth, 0, 0), new Vector2(1, 1));
            _vertices[2] = new VertexPositionTexture(new Vector3(-halfWidth, fullHeight, 0), new Vector2(0, 0));
            _vertices[3] = new VertexPositionTexture(new Vector3(halfWidth, fullHeight, 0), new Vector2(1, 0));
        
            _indices = [0, 1, 2, 2, 1, 3];
        }
    }

    // Calculates the 3D world-space bounding box for this sprite
    public BoundingBox Get3DBoundingBox()
    {
        Matrix world = Matrix.CreateScale(Parent.Transform.Scale) *
                       Matrix.CreateRotationX(MathHelper.ToRadians(Parent.Transform.Rotation.X)) *
                       Matrix.CreateRotationY(MathHelper.ToRadians(Parent.Transform.Rotation.Y)) *
                       Matrix.CreateRotationZ(MathHelper.ToRadians(Parent.Transform.Rotation.Z)) *
                       Matrix.CreateTranslation(Parent.Transform.Position);
        
        Vector3[] transformedVertices = new Vector3[_vertices.Length];
        for (int i = 0; i < _vertices.Length; i++)
        {
            transformedVertices[i] = Vector3.Transform(_vertices[i].Position, world);
        }
        
        Vector3 min = transformedVertices[0];
        Vector3 max = transformedVertices[0];
        foreach (var vertex in transformedVertices)
        {
            min = Vector3.Min(min, vertex);
            max = Vector3.Max(max, vertex);
        }
        
        max.Z += 0.1f;
        
        return new BoundingBox(min, max);
    }

    // Prepares the effect for textured rendering
    private void InitializeEffect(GraphicsDevice graphicsDevice)
    {
        _effect = new BasicEffect(graphicsDevice)
        {
            TextureEnabled = true,
            VertexColorEnabled = false
        };
    }

    // Returns the current frame’s raw pixel dimensions
    public (float width, float height) GetDimensions()
    {
        var currentFrame = CurrentAnimation.frames[CurrentFrame];
        return (currentFrame.Width, currentFrame.Height);
    }

    // Adds or replaces an animation by name (auto select the first added animation)
    public void AddAnimation(String name, AnimationData animation)
    {
        if (Animations.ContainsKey(name))
        {
            Animations[name] = animation;
        }
        else
        {
            Animations.Add(name, animation);
        }

        if (Animations.Count == 1)
        {
            SetAnimation(name);
            
        }
    }
    
    // Switches to another animation if available
    public bool SetAnimation(String name)
    {
        if (CurrentKey == name) return true;
        if (Animations.TryGetValue(name, out var animation))
        {
            CurrentAnimation = animation;
            CurrentKey = name;
            CurrentFrame = 0;
            AnimationTimer = 0;
            return true;
        }

        return false;
    }

    public double GetAnimationTime(string key)
    {
        var animation = Animations[key];
        return animation._frametime * animation.frames.Length;
    }

    // Gets the total playtime in seconds for the given animation
    public double GetCurrentAnimationTime()
    {
        return CurrentAnimation.frames.Length * CurrentAnimation._frametime;
    }
    
    // Gets the total playtime for the current animation
    private void handleAnimation(GameTime gameTime)
    {
        AnimationTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (AnimationTimer > CurrentAnimation._frametime)
        {
            AnimationTimer = 0;
            CurrentFrame++;
            if (CurrentAnimation.LoopAnimation)
            {
                if (CurrentFrame >= CurrentAnimation.frames.Length)
                {
                    CurrentFrame = 0;
                }
            } else if (CurrentFrame >= CurrentAnimation.frames.Length)
            {
                CurrentFrame--;
            }
        }
    }

    // Returns the active animation’s name
    public string GetCurrentKey()
    {
        return CurrentKey;
    }

    // Returns true if the current non-loop animation has finished
    public bool AnimationEnded()
    {
        if (CurrentAnimation.LoopAnimation)
        {
            return false;
        }

        if (CurrentFrame >= CurrentAnimation.frames.Length - 1)
        {
            return true;
        }

        return false;
    }

    // Updates the animation frame
    public override void Update(GameTime gameTime, TouchCollection touchCollection)
    {
        if (Animations.Count > 0)
            handleAnimation(gameTime);
    }
    
    // Updates quad UV coordinates for the current frame, respecting flip
    private void UpdateUV(Texture2D spriteSheet, Rectangle sourceRect)
    {
        float texWidth = spriteSheet.Width;
        float texHeight = spriteSheet.Height;

        float u1 = (float)sourceRect.X / texWidth;
        float v1 = (float)sourceRect.Y / texHeight;
        float u2 = (float)(sourceRect.X + sourceRect.Width) / texWidth;
        float v2 = (float)(sourceRect.Y + sourceRect.Height) / texHeight;
    
        if (Flipped)
        {
            _vertices[0].TextureCoordinate = new Vector2(u2, v2);
            _vertices[1].TextureCoordinate = new Vector2(u1, v2);
            _vertices[2].TextureCoordinate = new Vector2(u2, v1);
            _vertices[3].TextureCoordinate = new Vector2(u1, v1);
        }
        else
        {
            _vertices[0].TextureCoordinate = new Vector2(u1, v2);
            _vertices[1].TextureCoordinate = new Vector2(u2, v2);
            _vertices[2].TextureCoordinate = new Vector2(u1, v1);
            _vertices[3].TextureCoordinate = new Vector2(u2, v1);
        }
    }

    // Draws the sprite quad in either 3D or 2D mode
    public override void Draw(Camera camera, SpriteBatch spriteBatch)
    {
        if (!Disabled && CurrentAnimation != null)
        {
            if (CurrentAnimation.Render2D)
            {
                Render2D(camera, spriteBatch);
            }
            else
            {
                Render3D(camera);
            }
        }
    }

    // Renders the sprite as a 3D quad with BasicEffect
    private void Render3D(Camera camera)
    {
        UpdateQuad();
        
        _device.RasterizerState = RasterizerState.CullNone;
        _device.DepthStencilState = DepthStencilState.Default;
        _device.BlendState = BlendState.AlphaBlend;
        var frame = CurrentAnimation.frames[CurrentFrame];
        UpdateUV(CurrentAnimation.spriteSheet, frame);

        var worldMatrix = Matrix.CreateScale(Parent.Transform.Scale) * 
                          Matrix.CreateRotationX(MathHelper.ToRadians(Parent.Transform.Rotation.X)) *
                          Matrix.CreateRotationY(MathHelper.ToRadians(Parent.Transform.Rotation.Y)) *
                          Matrix.CreateRotationZ(MathHelper.ToRadians(Parent.Transform.Rotation.Z)) *
                          Matrix.CreateTranslation(Parent.Transform.Position);

        _effect.World = worldMatrix;
        _effect.View = camera.View();
        _effect.Projection = camera.Projection();
        _effect.Texture = CurrentAnimation.spriteSheet;

        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                _vertices, 0, 4,
                _indices, 0, 2
            );
        }
    }
    
    // Renders the sprite in 2D screen space with SpriteBatch
    private void Render2D(Camera camera, SpriteBatch spriteBatch)
    {
        Vector2 origin = CurrentAnimation.OriginCenter
            ? new Vector2(
                CurrentAnimation.frames[CurrentFrame].Width / 2f,
                CurrentAnimation.frames[CurrentFrame].Height / 2f
            )
            : Vector2.Zero;

        if (!CurrentAnimation.isUI)
        {
            var screenPosition = camera.GetScreenPosition(Parent.Transform.Position);
            var frame = CurrentAnimation.frames[CurrentFrame];

            float worldDepth = Vector3.Distance(camera.Parent.Transform.Position, Parent.Transform.Position);

            // Reference depth at which objects should appear 1:1 scale
            float referenceDepth = 10f;

            // Fix scale so objects shrink as they move away
            float scaleFactor = referenceDepth / Math.Max(worldDepth, 0.1f);

            float layerDepth = MathHelper.Clamp(
                (worldDepth - camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane),
                0f, 1f
            );

            spriteBatch.Draw(
                CurrentAnimation.spriteSheet,
                new Vector2(screenPosition.X, screenPosition.Y),
                frame,
                Color.White,
                MathHelper.ToRadians(Parent.Transform.Rotation.Y),
                origin,
                new Vector2(Parent.Transform.Scale.X * scaleFactor, Parent.Transform.Scale.Z * scaleFactor),
                Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth
            );
        }
        else
        {
            spriteBatch.Draw(
                CurrentAnimation.spriteSheet,
                new Vector2(Parent.Transform.Position.X, Parent.Transform.Position.Z),
                CurrentAnimation.frames[CurrentFrame],
                Color.White,
                MathHelper.ToRadians(Parent.Transform.Rotation.Y),
                origin,
                new Vector2(Parent.Transform.Scale.X, Parent.Transform.Scale.Z),
                SpriteEffects.None,
                0f
            );
        }
    }
}