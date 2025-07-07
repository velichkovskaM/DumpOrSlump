using GameEngine;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Levels;

/// <summary>
/// Scene loader for Level3. Creates camera, room, player, interactables, AI mom, UI widgets, dog companion, and sets background music.
/// Uses prefab helpers to avoid clutter and passes the next-level enum to the after-game overlay.
/// </summary>
public class Level3SceneLoader : SceneLoader
{
    // forward graphics device from constructor
    public Level3SceneLoader(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
        _GraphicsDevice = graphicsDevice;
    }

    // SceneLoader contract properties
    public override QuadTreeScene _Scene { get; set; }
    public override SpriteBatch _SpriteBatch { get; set; }
    public override GraphicsDevice _GraphicsDevice { get; set; }
    
    
    // cache scene reference
    public override void SetScene(QuadTreeScene scene)
    {
        _Scene = scene;
    }

    // orchestrate prefabs & return main camera
    public override Camera LoadNodes(SpriteBatch spriteBatch, GraphicsDevice graphics)
    {
        _SpriteBatch = spriteBatch;
        _GraphicsDevice = graphics;
        
        var camera = Prefabs.LoadCamera(_GraphicsDevice, _Scene);
        
        // core gameplay assets
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
        Prefabs.LoadCleanupManager(_Scene, true, 9, 9, 9);// still needs more areas coded in
        Prefabs.LoadDeEquipButton(_GraphicsDevice, _Scene);
        Prefabs.LoadTimer(_GraphicsDevice, _Scene, 300);
        Prefabs.LoadAfterGameMenu(_GraphicsDevice, _Scene, LevelEnum.Level4);
        Prefabs.LoadPauseButton(_GraphicsDevice, _Scene);
        Prefabs.LoadPauseMenu(_GraphicsDevice, _Scene);
        Prefabs.LoadHelpMenu(_GraphicsDevice, _Scene);
        Prefabs.LoadHearts(_GraphicsDevice, _Scene);
        Prefabs.LoadLoadButton(_GraphicsDevice, _Scene);
        Prefabs.LoadLoadButton(_GraphicsDevice, _Scene, "2");
        Prefabs.LoadClothesLoadArea(_Scene);
        Prefabs.LoadClutterDustLoadArea(_Scene);
        Prefabs.LoadMom(_Scene); // AI
        Prefabs.LoadWallPlant(_Scene);
        
        // info popup hint
        Prefabs.LoadInfoPopUp(_GraphicsDevice, _Scene, "\nTo pick up clothes, clutter or dust,\nyou will need to draw a circle around\nthe object in order to pick it up.");
        
        Prefabs.LoadDog(_Scene, new Vector3(10, 0.2f, 5), true); // AI

        // ambience
        MusicController.ChangeSong("background_wind");

        return camera;
    }
}