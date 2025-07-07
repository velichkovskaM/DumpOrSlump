using DumpOrSlumpGame.Components.CleanupObjects;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace DumpOrSlumpGame.Components.UI.Buttons;

/// <summary>
/// Button that loads collected items (clothes, dust, clutter) into the appropriate container based on the player’s current activity
/// Plays a click sound and toggles item states on release
/// </summary>
public class LoadButton : ButtonComponent
{
    private SpriteRenderer _spriteRenderer;
    
    private BoundingBox _boundingBox;
    
    private bool clicked = false;

    private int touchId = 0;
    
    public LoadButton(Node parent) : base(parent) { }
    
    // provide textures, animations, and click sfx
    public override (Texture2D asset, AnimationData nonClicked, AnimationData clicked, SoundEffectInstance soundEffect) GetButtonData()
    {
        var asset = Globals.content.Load<Texture2D>("SpriteSheets/AssetSpriteSheet");
        var _soundEffect = Globals.content.Load<SoundEffect>("SoundEffect/Click").CreateInstance();
        _soundEffect.Volume = 1.0f;
        var clickedAnimation = new AnimationData(
            asset,
            new Rectangle[]{new Rectangle(256 * 5, 256 * 5, 256, 256)},
            0.2f,
            false,
            isUI: true
        );
        var normalAnimation = new AnimationData(
            asset,
            new Rectangle[]{new Rectangle(256 * 4, 256 * 5, 256, 256)},
            0.2f,
            false,
            isUI: true
        );
            
        return (asset, normalAnimation, clickedAnimation, _soundEffect);
    }

    // transfer items to destination based on player state
    public override void OnRelease()
    {
        var _player = Game1.GetScene().FindNodeByName("Player");
        var playerScript = _player.GetComponent<Player>();
        
        if (playerScript.player_state == Player.player_states.sorting)
            Game1.Instance.clothesHandler.Children.ForEach(n =>
            {
                if (!n.active)
                {
                    n.GetComponent<Clothes>().inWardrobe = true;
                }
            });
        else if (playerScript.player_state == Player.player_states.vacuuming)
            Game1.Instance.dustHandler.Children.ForEach(n =>
            {
                if (!n.active)
                {
                    n.GetComponent<Dust>().inTrashCan = true;
                }
            });
        else if (playerScript.player_state == Player.player_states.cleaning)
            Game1.Instance.clutterHandler.Children.ForEach(n =>
            {
                if (!n.active)
                {
                    n.GetComponent<Clutter>().inTrashCan = true;
                }
            });
    }
}