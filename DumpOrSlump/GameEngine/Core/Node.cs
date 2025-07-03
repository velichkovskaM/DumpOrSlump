using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Scene;
using Microsoft.Xna.Framework;

namespace GameEngine.Core;

/// <summary>
/// Represents a node in the scene graph with a transform, components, and children
/// </summary>
public class Node
{
    public Guid Id = Guid.NewGuid();
    public bool active = true;
    public String name;
    public QuadTreeNode QuadTreeParent;
    public Transform Transform;
    public List<Component> Components = new();
    public List<Node> Children = new();
    
    // Creates a new node with a name and optional transform properties
    public Node(string name, Vector3? position = null, Vector3? rotation = null, Vector3? scale = null)
    {
        Transform = new Transform(this, position, rotation, scale);
        this.name = name;
    }

    // Adds/removes components from node
    public void AddComponent(Component component)
    {
        Components.Add(component);
    }
    public void RemoveComponent(Component component)
    {
        Components.Remove(component);
    }

    // Fetches all components based on the specified type
    public List<T> GetComponents<T>()
    {
        return Components.OfType<T>().ToList();
    }

    // Fetches first/default component of the specified type
    public T GetComponent<T>()
    {
        return Components.OfType<T>().FirstOrDefault();
    }

    public void SetActive(bool active)
    {
        this.active = active;
        foreach (var child in Children)
        {
            child.SetActive(active);
        }
    }

    // Creates a new node with a name and optional transform properties
    public IScene GetScene()
    {
        return QuadTreeParent._Scene;
    }

    // Marks this node for safe removal from the scene
    public void Destroy()
    {
        QuadTreeParent._Scene.SafeRemoveNodes.Add(this);
    }
    
}