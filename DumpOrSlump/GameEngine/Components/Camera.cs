using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace GameEngine.Components;

/// <summary>
/// Represents a 3D camera component for rendering the scene 
/// Manages projection and view matrices, world-to-screen conversions, and ray generation
/// </summary>
public class Camera : Component
{
    public static float zoom = 1f;
    public GraphicsDevice _graphicsDevice;
    
    public Matrix projectionMatrix;
    public Matrix viewMatrix;

    public float spaceWidth;
    public float spaceHeight;
    
    public float aspectRatio;

    public static float pixelToUnitX;
    public static float pixelToUnitZ;
    
    public const float PIXELS_PER_UNIT = 256f;
    public const float UNITS_PER_PIXEL = 100f / PIXELS_PER_UNIT;

    public float fieldOfView = MathHelper.ToRadians(25);
    public float nearClipPlane = 0.1f;
    public float farClipPlane = 150f;
    
    public static int prefferedHeight = 954;
    public static int prefferedWidth = 2120;

    public static float scale = 0;

    // Initializes a new camera attached to the given parent node
    /// Sets up the projection matrix based on the graphics device
    public Camera(Node parent, GraphicsDevice graphicsDevice) : base(parent)
    {
        _graphicsDevice = graphicsDevice;
        UpdateProjectionMatrix();
    }

    // Updates the projection matrix based on the current screen size and field of view
    // Updates the camera scale factor
    public void UpdateProjectionMatrix()
    {
        int width  = _graphicsDevice.PresentationParameters.BackBufferWidth;
        int height = _graphicsDevice.PresentationParameters.BackBufferHeight;
        
        scale = height / (float)prefferedHeight;
        
        float aspectRatio = (float)width / (float)height;
        
        projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            fieldOfView, 
            aspectRatio, 
            nearClipPlane, 
            farClipPlane
        );

        UpdateViewMatrix();
    }

    // Updates the view matrix based on the parent node's position and default orientation
    public void UpdateViewMatrix()
    {
        // Position of the camera
        Vector3 cameraPosition = Parent.Transform.Position;

        // Combine Down + Forward and normalize to get the "halfway" forward direction
        Vector3 forward = Vector3.Forward;
        forward.Normalize();
        
        Vector3 targetPosition = cameraPosition + forward;

        Vector3 up = Vector3.Up;
        up.Normalize();
        
        viewMatrix = Matrix.CreateLookAt(cameraPosition, targetPosition, up);

    }
    
    public Matrix Projection()
    {
        return projectionMatrix;
    }

    public Matrix View()
    {
        return viewMatrix;
    }

    // Converts a world position to screen coordinates
    public Vector3 GetScreenPosition(Vector3 worldPosition)
    {
        return _graphicsDevice.Viewport.Project(
            worldPosition,
            projectionMatrix,
            viewMatrix,
            Matrix.Identity
        );
    }

    // Converts a screen position to a world position at the near clipping plane
    public Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Vector3 nearPoint = new Vector3(screenPosition.X, screenPosition.Y, 0);
        Vector3 worldPosition = _graphicsDevice.Viewport.Unproject(
            nearPoint,
            projectionMatrix,
            viewMatrix,
            Matrix.Identity
        );

        return worldPosition;
    }

    // Gets the camera's current visible bounding box in world coordinates, including a margin
    public BoundingBox GetBoundingBox()
    {
        var halfWidth = spaceWidth / 2;
        var halfHeight = spaceHeight / 2;
        var margin = 100f;

        var b = new BoundingBox(
            new Vector2(Parent.Transform.Position.X - halfWidth - margin, Parent.Transform.Position.Z - halfHeight - margin),
            new Vector2(Parent.Transform.Position.X + halfWidth + margin, Parent.Transform.Position.Z + halfHeight + margin)
            );
        return b;
    }

    // Generates a ray from a screen click position for 3D picking
    public Ray GenerateRayFromClick(Vector2 screenPosition)
    {
        Vector2 correctedPosition;
        float scale = (float)_graphicsDevice.PresentationParameters.BackBufferHeight / (float)prefferedHeight;
        float scaledWidth = prefferedWidth * scale;
        var leftBlackBar = (_graphicsDevice.PresentationParameters.BackBufferWidth - scaledWidth) * 0.5f;

        correctedPosition = screenPosition - new Vector2(leftBlackBar * 0.5f, 0);

        Vector3 mousePosNear = new Vector3(correctedPosition.X, correctedPosition.Y, 0f);
        Vector3 mousePosFar  = new Vector3(correctedPosition.X, correctedPosition.Y, 1f);
        
        Vector3 nearPoint = _graphicsDevice.Viewport.Unproject(
            mousePosNear, projectionMatrix,viewMatrix, Matrix.Identity);
        Vector3 farPoint = _graphicsDevice.Viewport.Unproject(
            mousePosFar, projectionMatrix,viewMatrix, Matrix.Identity);
        
        Vector3 direction = Vector3.Normalize(farPoint - nearPoint);
        return new Ray(nearPoint, direction);
    }
}
