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

    /// <summary>
    /// Checks whether the transform’s current position still lies within the boundary of the QuadTree leaf that contains its parent node. If the
    /// object has crossed cell boundaries, it is removed and reinserted at the new location. Finally, the TreeUpdateMark flag is cleared
    /// </summary>
    public void HandleTreeUpdateMark()
    {
        // Early‑out if we’re not managed by a QuadTree node
        if (ParentNode?.QuadTreeParent != null)
        {
            // Only act if we have genuinely moved outside the node’s AABB and the new position is valid (NaN check guards against corrupt data)
            if (!ParentNode.QuadTreeParent._boundary.Contains(_position) && !float.IsNaN(Position.X))
            {
                var Scene = ParentNode.QuadTreeParent._Scene;
                
                // Remove from the old cell
                Scene.Remove(ParentNode);
                
                // Attempt to insert at the new
                bool inserted = Scene.Insert(ParentNode);
                if (!inserted)
                {
                    global::Logger.Error($"Couldnt insert {ParentNode.name} into the tree again at position {_position}");
                }
            }
        }
        // Mark handled
        TreeUpdateMark = false;
    }
}