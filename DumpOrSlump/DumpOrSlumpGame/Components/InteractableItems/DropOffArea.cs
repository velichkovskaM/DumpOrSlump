using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace DumpOrSlumpGame.Components.InteractableItems;

/// <summary>
/// Area where the player can drop equipment. Activates the de‑equip UI when the player, in a valid state, is inside the bounds.
/// </summary>
public class DropOffArea : Component
{
    BoundingBox boundingBox;
    
    public DropOffArea(Node parent, bool active = true) : base(parent, active)
    { }

    // Initialize axis‑aligned bounding box around the drop‑off zone
    public override void Start(IScene scene)
    {
        boundingBox = new BoundingBox(
            new Vector2(Parent.Transform.Position.X - 2f, Parent.Transform.Position.Z),
            new Vector2(Parent.Transform.Position.X + 2.75f, Parent.Transform.Position.Z + 2)
        );
    }

    // Per‑frame: show/hide de‑equip button based on player position & state
    public override void Update(GameTime gameTime, TouchCollection touches)
    {
        if (!Game1.Instance.canDropEquipment) return;
        
        var _player = Parent.QuadTreeParent._Scene.FindNodeByName("Player");
        var playerScript = _player.GetComponent<Player>();
        
        if (boundingBox.Contains(_player.Transform.Position) &&
            (playerScript.player_state == Player.player_states.cleaning ||
            playerScript.player_state == Player.player_states.vacuuming ||
            playerScript.player_state == Player.player_states.sorting))
        {
            Parent.QuadTreeParent._Scene.UiNodes.Find(x => x.name == "DeEquipButton").active = true;
        }
        else
        {
            Parent.QuadTreeParent._Scene.UiNodes.Find(x => x.name == "DeEquipButton").active = false;
        }
    }
}