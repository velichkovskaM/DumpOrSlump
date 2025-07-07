using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace DumpOrSlumpGame.Components.InteractableItems;

/// <summary>
/// Trigger zone for loading sorted clothes into a machine
/// Activates the UI button when the player, in sorting state, is inside
/// </summary>
public class LoadClothsArea : Component
{
    BoundingBox boundingBox;
    
    public LoadClothsArea(Node parent, bool active = true) : base(parent, active) { }

    // Initialize bounding box around load area
    public override void Start(IScene scene)
    {
        boundingBox = new BoundingBox(
            new Vector2(Parent.Transform.Position.X - 1.7f, Parent.Transform.Position.Z),
            new Vector2(Parent.Transform.Position.X + 1.7f, Parent.Transform.Position.Z + 2)
        );
    }

    // Per-frame: show/hide load button based on player state & position
    public override void Update(GameTime gameTime, TouchCollection touches)
    {
        var _player = Parent.QuadTreeParent._Scene.FindNodeByName("Player");
        var playerScript = _player.GetComponent<Player>();
        
        
        if (boundingBox.Contains(_player.Transform.Position) &&
            playerScript.player_state == Player.player_states.sorting &&
            !Game1.Instance.canDropEquipment)
        {
            Parent.QuadTreeParent._Scene.UiNodes.Find(x => x.name == "LoadClothesButton1").active = true;
        }
        else
        {
            Parent.QuadTreeParent._Scene.UiNodes.Find(x => x.name == "LoadClothesButton1").active = false;
        }
    }
}