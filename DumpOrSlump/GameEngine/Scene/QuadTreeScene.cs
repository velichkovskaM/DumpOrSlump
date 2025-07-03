using System;
using System.Collections.Generic;
using GameEngine.Components;
using GameEngine.Core;
using GameEngine.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace GameEngine;

/// <summary>
/// A scene that uses a QuadTreeNode for spatial partitioning of game nodes
/// </summary>
public class QuadTreeScene : IScene
{
    //  Root QuadTree node containing all spatial (world) nodes
    public QuadTreeNode root;
    
    // Flat list of UI nodes that are not spatially partitioned
    public List<Node> UiNodes { get; set; } = new(); 
    
    // Reference to scene loader that creates nodes & the camera
    public SceneLoader _sceneLoader;
    
    // Buffers for safely adding/removing nodes during update
    public List<Node> SafeInsertedNodes { get; set; } = new List<Node>(100);
    public List<Node> SafeInsertedUINodes { get; set; } = new List<Node>(100);
    public List<Node> SafeRemoveNodes { get; set; } = new List<Node>(100);
    
    // Tracks transforms that moved and need re-insertion in the quad tree
    public List<Transform> UpdatedTransforms { get; set; } = new List<Transform>(100);

    // Create the scene with an initial quad tree boundary and capacity per quad
    public QuadTreeScene(int capacity, SceneLoader sceneLoader)
    {
        root = new QuadTreeNode(new BoundingBox(new Vector2(int.MinValue, int.MinValue), new Vector2(int.MaxValue, int.MaxValue)), capacity, this);
        _sceneLoader = sceneLoader;
    }

    // Initialize the scene: load nodes with SceneLoader and return the active Camera
    public Camera InitScene(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        var camera = _sceneLoader.LoadNodes(spriteBatch, graphicsDevice);
        _sceneLoader.InitNodes();

        return camera;
    }

    public bool Insert(Node node)
    {
        return root.Insert(node);
    }

    public bool SafeInsert(Node node)
    {
        SafeInsertedNodes.Add(node);
        
        return true;
    }

    public bool SafeInsertUi(Node node)
    {
        SafeInsertedUINodes.Add(node);
        return true;
    }

    public bool InsertAllSafeInsertedNotes()
    {
        foreach (var node in SafeRemoveNodes)
        {
            Remove(node);
        }
        
        var allInserted = 0;
        // Insert all queued world nodes into the quad tree
        foreach (var insertedNode in SafeInsertedNodes)
        {
            var nodeInserted = Insert(insertedNode);
            if(nodeInserted) allInserted++;
            else
            {
                global::Logger.Error($"This node couldnt be inserted: {insertedNode.name}");
            }
        }

        var allNodesInserted = allInserted == SafeInsertedNodes.Count;

        if (!allNodesInserted)
        {
            global::Logger.Error($"{allInserted} out of {SafeInsertedNodes.Count} could be inserted leaving {SafeInsertedNodes.Count - allInserted} not being inserted.");
        }
        
        foreach (var node in SafeInsertedNodes)
        {
            foreach (var component in node.Components)
            {
                component.Start(this);
            }
        }
        SafeInsertedNodes.Clear();
        
        foreach (var safeInsertedUiNode in SafeInsertedUINodes)
        {
            UiNodes.Add(safeInsertedUiNode);
        }
        
        foreach (var node in SafeInsertedUINodes)
        {
            foreach (var component in node.Components)
            {
                component.Start(this);
            }
        }
        SafeInsertedUINodes.Clear();

        return allNodesInserted;
    }

    // Find a node by name in the quad tree first, then in the UI list
    public Node FindNodeByName(string name)
    {
        if (root.Find(x => x.name == name) is Node node)
        {
            return node;
        }
        
        return UiNodes.Find(x => x.name == name);
    }

    public Node FindNode(Predicate<Node> predicate)
    {
        if (root.Find(predicate) is Node node)
        {
            return node;
        }
        
        return UiNodes.Find(predicate);
    }

    // Find a node using a custom predicate
    public List<Node> FindNodes(Predicate<Node> predicates)
    {
        var l = new List<Node>();
        root.FindAll(predicates, l);
        
        return UiNodes.FindAll(predicates);
    }
    
    public List<T> FindAllComponents<T>() {
        var l = new List<T>();
        root.FindAllComponents<T>(l);
        return l;
    }

    public bool Remove(Node node)
    {
        return root.Remove(node);
    }

    public List<Node> Query(BoundingBox boundingBox)
    {
        return root.Query(boundingBox);
    }

    public List<Node> Query(BoundingBox boundingBox, List<Node> nodes)
    {
        return root.Query(boundingBox, nodes);
    }
}