using System;
using System.Collections.Generic;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace GameEngine.Scene;

/// <summary>
/// A single node in a QuadTree spatial partitioning structure
/// It holds game Nodes inside a bounding box and can split into 4 child quadrants when capacity is exceeded
/// Handles inserting, removing, subdividing, querying, and updating nodes
/// </summary>
public class QuadTreeNode
{
    public QuadTreeScene _Scene;
    public BoundingBox _boundary;
    List<Node> _children; // List of contained nodes if leaf
    QuadTreeNode[] _quadTreeChildren = new QuadTreeNode[4]; // Subdivided children
    private int _capacity; // Max children before subdividing

    public QuadTreeNode(BoundingBox boundary, int capacity, QuadTreeScene scene)
    {
        _boundary = boundary;
        _capacity = capacity;
        _children = new List<Node>();
        _Scene = scene;
    }

    // Add a Node to this QuadTreeNode's children
    public void AddNodeChild(Node node)
    {
        node.QuadTreeParent = this;
        _children.Add(node);
    }

    // Remove a Node from this QuadTreeNode's children
    public bool RemoveNodeChild(Node node)
    {
        if (node == null)
        {
            global::Logger.Error($"Node is null");
        }

        if (_children.Contains(node))
        {
            global::Logger.Error($"Node does not exist in this: {node.name}");
        }

        node.QuadTreeParent = null;
        return _children.Remove(node);
    }

    // Split this node into 4 sub-nodes and redistribute children
    public bool SubDivide()
    {
        BoundingBox[] newBounds = _boundary.split();
        
        _quadTreeChildren[0] = new QuadTreeNode(newBounds[0], _capacity, _Scene);
        _quadTreeChildren[1] = new QuadTreeNode(newBounds[1], _capacity, _Scene);
        _quadTreeChildren[2] = new QuadTreeNode(newBounds[2], _capacity, _Scene);
        _quadTreeChildren[3] = new QuadTreeNode(newBounds[3], _capacity, _Scene);

        var foundAllBoundaries = 0;
        var childrenThatIsContained = 0;
        var childCount = _children.Count;
        foreach (var child in _children)
        {
            foreach (var tree in _quadTreeChildren)
            {
                if (tree._boundary.Contains(child.Transform.Position))
                {
                    childrenThatIsContained++;
                    tree._children.Add(child);
                    child.QuadTreeParent = tree;
                    foundAllBoundaries++;
                    break;
                }
            }
        }
        
        var returnValue = foundAllBoundaries == _children.Count;

        if (!returnValue)
        {
            global::Logger.Error("Failed to subdivide quad tree");
        }
        
        _children = null;

        return returnValue;
    }
    
    // Try to insert a Node into this node or its children
    public bool Insert(Node node)
    {
        if (_children == null)
        {
            bool foundBoundry = false;
            foreach (var quadChild in _quadTreeChildren)
            {
                if (quadChild._boundary.Contains(node.Transform.Position))
                {
                    foundBoundry = quadChild.Insert(node);
                    break;
                }
            }
            
            return foundBoundry;
            
        }
        
        AddNodeChild(node);

        if (_children.Count > _capacity)
        {
            return SubDivide();
        }
        return true;
    }

    // Find the first Node matching the predicate
    public Node? Find(Predicate<Node> predicate)
    {
        if (_children == null)
        {
            foreach (var child in _quadTreeChildren)
            {
                var node = child.Find(predicate);
                if (node != null)
                {
                    return node;
                }
            }
        }
        else
        {
            foreach (var child in _children)
            {
                if (predicate(child))
                {
                    return child;
                }
            }
        }

        return null;
    }

    // Find all Nodes matching a predicate and add them to the provided list
    public List<Node> FindAll(Predicate<Node> predicate, List<Node> nodes)
    {
        if (_children == null)
        {
            foreach (var child in _quadTreeChildren)
            {
                child.FindAll(predicate);
            }
        }
        else
        {
            foreach (var child in _children)
            {
                if (predicate(child))
                {
                    nodes.Add(child);
                }
            }
        }

        return nodes;
    }

    // Remove node from quad tree by reference or predicate
    public bool Remove(Node node)
    {
        if (node.QuadTreeParent != null)
        {
            return node.QuadTreeParent.RemoveNodeChild(node);
        }
        
        return Remove(n => n.Id == node.Id);
    }

    public bool Remove(Predicate<Node> predicate)
    {
        if (_children == null)
        {
            foreach (var child in _quadTreeChildren)
            {
                if (child.Remove(predicate))
                {
                    return true;
                }
            }
        }
        else
        {
            foreach (var child in _children)
            {
                if (predicate(child))
                {
                    return RemoveNodeChild(child);
                }
            }
        }

        return false;
    }

    public List<Node> FindAll(Predicate<Node> predicate)
    {
        throw new NotImplementedException();
        return new List<Node>();
    }

    // Return all nodes inside the given boundary
    public List<Node> Query(BoundingBox boundary)
    {
        var l = new List<Node>();
        return Query(boundary, l);
    }
    
    // Recursive helper for Query
    public List<Node> Query(BoundingBox boundary, List<Node> nodes)
    {
        if (_children == null)
        {
            foreach (var child in _quadTreeChildren)
            {
                if (BoundingBox.Intersects(boundary, child._boundary)) child.Query(boundary, nodes);
            }
        }
        else
        {
            nodes.AddRange(_children);
        }
        
        return nodes;
    }

    // Recursively update all active components in all nodes below this quad
    public int Update(GameTime gameTime, TouchCollection touchCollection)
    {
        var updates = 0;
        if (_children == null)
        {
            foreach (var child in _quadTreeChildren)
            {
                updates += child.Update(gameTime, touchCollection);
            }
        }
        else
        {
            foreach (var child in _children)
            {
                if (child.active)
                {
                    foreach (var component in child.Components)
                    {
                        if (component.Active)
                        {
                            component.Update(gameTime, touchCollection);
                            updates++;
                        }
                    }
                }
            }
        }
        
        return updates;
    }

    // Draw all visible SpriteRenderers under this quad
    public int Draw(Camera camera, SpriteBatch spriteBatch, RenderTarget2D renderTarget2D, GraphicsDevice graphicsDevice)
    {
        int count = 0;
        if (_children == null)
        {
            foreach (var child in _quadTreeChildren)
            {
                count += child.Draw(camera, spriteBatch, renderTarget2D, graphicsDevice);
            }
        }
        else
        {
            foreach (var child in _children)
            {
                if (child.active)
                {
                    foreach (var component in child.Components)
                    {
                        if (component is SpriteRenderer renderer)
                        {
                            if (renderer.CurrentAnimation.Render2D == false)
                            {
                                continue;
                            }
                        }

                        if (component.Active)
                        {
                            component.Draw(camera, spriteBatch);
                            count++;
                        }
                    }
                }
            }
        }

        return count;
    }

    // Initialize all components below this quad
    public void Start(IScene scene)
    {
        if (_children == null)
        {
            foreach (var child in _quadTreeChildren)
            {
                child.Start(scene);
            }
        }
        else
        {
            foreach (var child in _children)
            {
                foreach (var component in child.Components)
                {
                    component.Start(scene);
                }
            }
        }
    }

    // Collect transforms that moved and mark them for re-insertion
    public void UpdateTreePositions(List<Transform> transforms)
    {
        
        if (_children == null)
        {
            foreach (var child in _quadTreeChildren)
            {
                child.UpdateTreePositions(transforms);
            }
        }
        else
        {
            foreach (var child in _children)
            {
                if (child.Transform.TreeUpdateMark)
                {
                    transforms.Add(child.Transform);
                }
            }
        }
    }

    // Collect all components of type T below this quad
    public void FindAllComponents<T>(List<T> nodes)
    {
        if (_children == null)
        {
            foreach (var child in _quadTreeChildren)
            {
                child.FindAllComponents(nodes);
            }
        }
        else
        {
            foreach (var child in _children)
            {
                var component = child.GetComponent<T>();
                if (component != null)
                {
                    nodes.Add(component);
                }
            }
        }
    }
}