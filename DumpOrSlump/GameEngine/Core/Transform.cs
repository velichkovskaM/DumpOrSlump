using Microsoft.Xna.Framework;

namespace GameEngine.Core;

/// <summary>
/// Represents the position, rotation, and scale of a node, and manages spatial updates in the scene's quad tree
/// </summary>
public class Transform
{
    public Node ParentNode { get; set; }

    public bool TreeUpdateMark = false;
    
    private Vector3 _position;
    public Vector3 PreviousPosition;
    
    // The position of this transform in world space. Setting this value also handles marking the quad tree for updates
    public Vector3 Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                PreviousPosition = _position;
                _position = value;
                
                if (ParentNode.QuadTreeParent != null)
                {
                    TreeUpdateMark = true;
                    ParentNode.QuadTreeParent._Scene.UpdatedTransforms.Add(this);
                }
            }
        }
    }

    public Vector3 Rotation { get; set; }
    public Vector3 Scale { get; set; }

    // Creates a new transform attached to the given parent node
    public Transform(Node parentNode, Vector3? position = null, Vector3? rotation = null, Vector3? scale = null)
    {
        ParentNode = parentNode;
        _position = position ?? Vector3.Zero;
        Rotation = rotation ?? Vector3.Zero;
        Scale = scale ?? Vector3.One;
    }

    // Handles updating the node's position in the quad tree if it has moved outside its boundary
    public void HandleTreeUpdateMark()
    {
        if (ParentNode?.QuadTreeParent != null)
        {
            if (!ParentNode.QuadTreeParent._boundary.Contains(_position) && !float.IsNaN(Position.X))
            {
                var Scene = ParentNode.QuadTreeParent._Scene;
                Scene.Remove(ParentNode);
                bool inserted = Scene.Insert(ParentNode);
                if (!inserted)
                {
                    global::Logger.Error($"Couldnt insert {ParentNode.name} into the tree again at position {_position}");
                }
            }
        }
        TreeUpdateMark = false;
    }
}