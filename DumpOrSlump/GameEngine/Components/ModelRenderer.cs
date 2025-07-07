using System;
using System.Collections.Generic;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Components;

/// <summary>
/// Renders a 3D model attached to a node, with optional per-mesh lighting and support for multiple light sources
/// </summary>
public class ModelRenderer : Component
{
    public Model model;
    public Matrix worldMatrix;
    public Effect lightEffect;
    public bool renderWithLight = false;
    private Dictionary<ModelMeshPart, (Texture2D Texture, Vector4? SolidColor)> meshPartData = 
        new Dictionary<ModelMeshPart, (Texture2D, Vector4?)>();
    private Dictionary<ModelMeshPart, Effect> meshPartEffects = new Dictionary<ModelMeshPart, Effect>();

    // Structure to represent a light
    private struct Light
    {
        public Vector3 Position;
        public Vector3 Color;
        public float Range;
    }

    // Array to hold up to 8 lights and track active count
    private Light[] lights = new Light[8];
    private int activeLightCount = 0;

    public ModelRenderer(Node parent, Effect effect, bool active = true, bool renderWithLight = true) 
        : base(parent, active)
    {
        this.renderWithLight = renderWithLight;
        this.lightEffect = effect;

        var l1 = new Light();
        l1.Position = new Vector3(8, 7, 8); // Left mainlight
        l1.Color = Vector3.One/2;
        l1.Range = 25.0f;
        lights[0] = l1;
        
        
        var l2 = new Light();
        l2.Position = new Vector3(25f, 7, 8); // Right mainlight
        l2.Color = Vector3.One/2;
        l2.Range = 25.0f;
        lights[1] = l2;
        
        var l3 = new Light();
        l3.Position = new Vector3(30f, 3, 0.6f) + new Vector3(1, 0, -1) * 15; // Back right
        l3.Color = Vector3.One;
        l3.Range = 45.0f;
        lights[2] = l3;
        
        var l4 = new Light();
        l4.Position = new Vector3(1f, 3, 0.6f) + new Vector3(-1, 0, -1) * 15; // Back left
        l4.Color = Vector3.One;
        l4.Range = 45f;
        lights[3] = l4;
        
        var l5 = new Light();
        l5.Position = new Vector3(29.95f, 3.2f, 3.5f); // Under the Child overhead
        l5.Color = Vector3.One / 4 * 3;
        l5.Range = 7.0f;
        lights[4] = l5;
        
        var l6 = new Light();
        l6.Position = new Vector3(15f, 2, 20f); // Full front light
        l6.Color = Vector3.One;
        l6.Range = 30.0f;
        lights[5] = l6;

        var l7 = new Light();
        l7.Position = new Vector3(4.6f, 2, 2f); // Full backlight light
        l7.Color = Vector3.One;
        l7.Range = 30.0f;
        lights[6] = l7;
        
        activeLightCount = 7;
    }
    
    public BoundingSphere GetBoundingSphere()
    {
        // Merge all mesh spheres so we only do this once
        if (model == null) return new BoundingSphere();
            
        BoundingSphere sphere = model.Meshes[0].BoundingSphere;
        for (int i = 1; i < model.Meshes.Count; i++)
            sphere = BoundingSphere.CreateMerged(
                sphere, model.Meshes[i].BoundingSphere);

        // Push it into world-space
        sphere = sphere.Transform(worldMatrix);
        return sphere;
    }
    

    public void AddModel(Model name)
    {
        model = name;

        foreach (var mesh in model.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                var clonedEffect = lightEffect.Clone();
                if (meshPart.Effect is BasicEffect basicEffect)
                {
                    if (Parent.name.StartsWith("tile"))
                    {
                        global::Logger.Error($"{Parent.name} is a tile with basic effect");
                    }
                    bool hasTexture = basicEffect.TextureEnabled && basicEffect.Texture != null;
                    Vector4? solidColor = null;
                    if (hasTexture) 
                    {
                        clonedEffect.Parameters["ObjectTexture"]?.SetValue(basicEffect.Texture);
                        clonedEffect.Parameters["UseTexture"]?.SetValue(true);
                        global::Logger.Error($"Mesh part has texture and is {Parent.name}");
                    }
                    else
                    {
                        solidColor = new Vector4(basicEffect.DiffuseColor, 1.0f);
                        clonedEffect.Parameters["UseTexture"]?.SetValue(false);
                        clonedEffect.Parameters["ObjectColor"]?.SetValue(solidColor.Value);
                    }
                    meshPartData[meshPart] = (hasTexture ? basicEffect.Texture : null, solidColor);
                    meshPartEffects[meshPart] = clonedEffect;
                }
                else
                {
                    var effect = meshPart.Effect;
                    var hasTexture = effect.Parameters["UseTexture"].GetValueBoolean();
                    var solidColor = clonedEffect.Parameters["ObjectColor"]?.GetValueVector4();;
                    var texture = effect.Parameters["ObjectTexture"]?.GetValueTexture2D();
                    
                    meshPartData[meshPart] = (hasTexture ? texture : null, solidColor);
                    meshPartEffects[meshPart] = effect;
                    global::Logger.Error($"This message should never be seen the effect type is: {meshPart.Effect}");
                }
            }
        }
    }

    public override void Draw(Camera camera, SpriteBatch spriteBatch)
    {
        if (camera._graphicsDevice == null)
        {
            Console.WriteLine("GraphicsDevice is NULL! Rendering will not work.");
            return;
        }

        if (model == null)
        {
            return;
        }
        
        camera._graphicsDevice.DepthStencilState = DepthStencilState.Default;
        camera._graphicsDevice.RasterizerState = RasterizerState.CullNone;
        
        worldMatrix = Matrix.CreateScale(Parent.Transform.Scale) *
                      Matrix.CreateRotationX(MathHelper.ToRadians(Parent.Transform.Rotation.X)) *
                      Matrix.CreateRotationY(MathHelper.ToRadians(Parent.Transform.Rotation.Y)) *
                      Matrix.CreateRotationZ(MathHelper.ToRadians(Parent.Transform.Rotation.Z)) *
                      Matrix.CreateTranslation(Parent.Transform.Position);

        // Prepare arrays for up to 8 lights
        Vector3[] lightPositions = new Vector3[8];
        Vector3[] lightColors = new Vector3[8];
        float[] lightRanges = new float[8];

        // Fill arrays with active lights; pad with zeros for unused lights
        for (int i = 0; i < 8; i++)
        {
            if (i < activeLightCount)
            {
                lightPositions[i] = lights[i].Position;
                lightColors[i] = lights[i].Color;
                lightRanges[i] = lights[i].Range;
            }
            else
            {
                lightPositions[i] = Vector3.Zero;
                lightColors[i] = Vector3.Zero; // No light contribution
                lightRanges[i] = 0f;
            }
        }

        foreach (var mesh in model.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                var effect = meshPartEffects[meshPart];
                effect.Parameters["World"]?.SetValue(worldMatrix);
                effect.Parameters["ViewProjection"]?.SetValue(camera.viewMatrix * camera.projectionMatrix);
                effect.Parameters["WorldInverseTranspose"]?.SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));

                // Set multiple light parameters
                effect.Parameters["LightPositions"]?.SetValue(lightPositions);
                effect.Parameters["LightColors"]?.SetValue(lightColors);
                effect.Parameters["LightRanges"]?.SetValue(lightRanges);
                effect.Parameters["AmbientColor"]?.SetValue(new Vector3(0.1f, 0.1f, 0.1f));

                meshPart.Effect = effect;
            }
            mesh.Draw();
        }
    }
}