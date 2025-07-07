using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;

namespace DumpOrSlumpGame.Components.UI;

/// <summary>
/// Manages player stamina hearts. Instantiates heart icons on start and updates their state on each hit;
/// triggers game‑over when all hearts are empty
/// </summary>
public class StaminaController : Component
{
    public int hitCounter;
    public StaminaController(Node parent, bool active = true) : base(parent, active) { }

    // create three heart nodes & add to UI
    public override void Start(IScene scene)
    {
        var position = Parent.Transform.Position;
        var scale = new Vector3(0.5f, 1, 0.5f);
        for (int i = 0; i < 3; i++)
        {
            var node = new UINode("Heart", position + new Vector3(256 * i, 0, 0) * scale * Camera.scale, scale: scale);
            node.AddComponent(new SpriteRenderer(node, Globals._Graphics.GraphicsDevice));
            var heartScript = new StaminaHeart(node);
            node.AddComponent(heartScript);
            Parent.Children.Add(node);
            Game1.GetScene().SafeInsertUi(node);
        }
    }

    // increment hit counter & update heart states
    public void HandleHit()
    {
        hitCounter += 1;

        switch (hitCounter)
        {
            case 1:
                Parent.Children[2].GetComponent<StaminaHeart>().SetHalf();
                break;
            case 2:
                Parent.Children[2].GetComponent<StaminaHeart>().SetEmpty();
                break;
            case 3:
                Parent.Children[1].GetComponent<StaminaHeart>().SetHalf();
                break;
            case 4:
                Parent.Children[1].GetComponent<StaminaHeart>().SetEmpty();
                break;
            case 5:
                Parent.Children[0].GetComponent<StaminaHeart>().SetHalf();
                break;
            case 6:
                Parent.Children[0].GetComponent<StaminaHeart>().SetEmpty();
                Game1.Instance.SetGameLost("You lost all your health!");
                break;
        }
    }
}