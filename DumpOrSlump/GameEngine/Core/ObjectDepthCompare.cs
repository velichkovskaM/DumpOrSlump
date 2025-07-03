using System;
using System.Collections.Generic;

namespace GameEngine.Core;

/// <summary>
/// Compares two nodes based on their transform's Z position,  for depth sorting in the scene
/// </summary>
public class ObjectDepthCompare : IComparer<Node>
{
    public int Compare(Node x, Node y)
    {
        // Less than zero if x precedes y, zero if they are equal,  greater than zero if x follows y
        return Nullable.Compare(x?.Transform.Position.Z, y?.Transform.Position.Z);
    }
}