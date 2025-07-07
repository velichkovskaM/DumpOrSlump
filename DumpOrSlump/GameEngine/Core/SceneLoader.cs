using GameEngine.Components;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Core;

/// <summary>
/// Provides a base class for loading, initializing, and managing a scene, including its camera, nodes, and rendering resources
/// </summary>
public abstract class SceneLoader
{
    public int ScaleHeight;
    public int ScaleWidth;

    // Takes width and height of the screen and adds them as scale height and width
    protected SceneLoader(GraphicsDevice graphicsDevice)
    {
        int width  = graphicsDevice.PresentationParameters.BackBufferWidth;
        int height = graphicsDevice.PresentationParameters.BackBufferHeight;
            
        ScaleWidth = width / Camera.prefferedWidth;
        ScaleHeight = height / Camera.prefferedHeight;
    }

    public abstract QuadTreeScene _Scene { get; set; }
    public abstract SpriteBatch _SpriteBatch { get; set; }
    public abstract GraphicsDevice _GraphicsDevice { get; set; }
    public abstract void SetScene(QuadTreeScene scene);
    public abstract Camera LoadNodes(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);

    public void InitNodes()
    {
        if (_Scene == null) return;
        
        _Scene.root.Start(_Scene);
        foreach (var node in _Scene.UiNodes)
        {
            foreach (var component in node.Components)
            {
                component.Start(_Scene);
            }
        }
    }
}