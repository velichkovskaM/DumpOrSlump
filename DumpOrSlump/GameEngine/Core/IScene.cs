using System;
using System.Collections.Generic;

namespace GameEngine.Core;

/// <summary>
///  Defines the contract for a scene that manages nodes, transforms, safe insertion/removal,
///  UI nodes, spatial queries, and component lookups within a game world
/// </summary>
public interface IScene
{
    // Puts node into list, and at the very end of the update loop, puts all the objects into the GameWorld
    public List<Node> SafeInsertedNodes { get; set; }
    public List<Node> SafeInsertedUINodes { get; set; }
    public List<Transform> UpdatedTransforms { get; set; }
    
    // List of UI nodes that will render after everything else is rendered
    public List<Node> UiNodes { get; set; }
    
    // Queues inserts
    public bool Insert(Node node);
    public bool SafeInsert(Node node);
    public bool SafeInsertUi(Node node);
    
    // Calls inserts
    public bool InsertAllSafeInsertedNotes();
    
    // Grabbing, filtering, fetching data
    public Node FindNodeByName(string name);
    public Node FindNode(Predicate<Node> predicate);
    public bool Remove(Node node);
    public List<Node> Query(BoundingBox boundingBox);
    public List<T> FindAllComponents<T>();
}