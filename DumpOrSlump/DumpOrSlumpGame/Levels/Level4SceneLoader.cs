using GameEngine;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Levels;

/// <summary>
/// Scene loader for the final Level4. Spawns camera, full room setup, increased clutter/dust counts, AI mom & child, dog companion,
/// and comprehensive HUD/UI — then cues background music. End‑of‑level overlay redirects to the main menu.
/// </summary>
public class Level4SceneLoader : SceneLoader
{
    // forward graphics device reference
    public Level4SceneLoader(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
        _GraphicsDevice = graphicsDevice;
    }

    // SceneLoader contract state
    public override QuadTreeScene _Scene { get; set; }
    public override SpriteBatch _SpriteBatch { get; set; }
    public override GraphicsDevice _GraphicsDevice { get; set; }
    
    // remember scene reference
    public override void SetScene(QuadTreeScene scene)
    {
        _Scene = scene;
    }

    // instantiate prefabs & return active camera
    public override Camera LoadNodes(SpriteBatch spriteBatch, GraphicsDevice graphics)
    {
        _SpriteBatch = spriteBatch;
        _GraphicsDevice = graphics;
        
        var camera = Prefabs.LoadCamera(_GraphicsDevice, _Scene);
        
        
        // core gameplay objects
        Prefabs.LoadPlayer(_Scene, new Vector3(5, 0f, 5));
        Prefabs.LoadStaticRoom(_Scene);
        Prefabs.LoadController(_GraphicsDevice, _Scene);
        Prefabs.LoadDoor(_Scene);
        Prefabs.LoadCleaningBasket(_Scene);
        Prefabs.LoadGarbageBag(_Scene);
        Prefabs.LoadVacuumCleaner(_Scene);
        Prefabs.LoadWardrobe(_Scene);
        Prefabs.LoadDropOffArea(_Scene);
        Prefabs.LoadTrashCan(_Scene);
        Prefabs.LoadCleanupManager(_Scene, true, 12, 12, 12);// still needs more areas coded in
        Prefabs.LoadDeEquipButton(_GraphicsDevice, _Scene);
        Prefabs.LoadTimer(_GraphicsDevice, _Scene, 300);
        Prefabs.LoadAfterGameMenu(_GraphicsDevice, _Scene, LevelEnum.MainMenu);
        Prefabs.LoadPauseButton(_GraphicsDevice, _Scene);
        Prefabs.LoadPauseMenu(_GraphicsDevice, _Scene);
        Prefabs.LoadHelpMenu(_GraphicsDevice, _Scene);
        Prefabs.LoadHearts(_GraphicsDevice, _Scene);
        Prefabs.LoadLoadButton(_GraphicsDevice, _Scene);
        Prefabs.LoadLoadButton(_GraphicsDevice, _Scene, "2");
        Prefabs.LoadClothesLoadArea(_Scene);
        Prefabs.LoadClutterDustLoadArea(_Scene);
        Prefabs.LoadMom(_Scene); // AI mom
        Prefabs.LoadWallPlant(_Scene);
        
        Prefabs.LoadDog(_Scene, new Vector3(10, 0.2f, 5), true); // AI dog

        Prefabs.LoadChild(_Scene, new Vector3(21.5f, 5f, 6.5f)); // AI child
        
        // ambience
        MusicController.ChangeSong("background_wind");
        
        return camera;
    }
}