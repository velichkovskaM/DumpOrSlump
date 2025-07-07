using GameEngine;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Levels;

/// <summary>
/// Scene loader for Level2. Instantiates camera, player, room, interactables, UI widgets, dog companion and sets background music
/// Leverages prefab helpers to keep the loader readable
/// </summary>
public class Level2SceneLoader : SceneLoader
{
    // forward graphics device
    public Level2SceneLoader(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
        _GraphicsDevice = graphicsDevice;
    }

    // SceneLoader contract props
    public override QuadTreeScene _Scene { get; set; }
    public override SpriteBatch _SpriteBatch { get; set; }
    public override GraphicsDevice _GraphicsDevice { get; set; }
    
    
    // cache scene ref
    public override void SetScene(QuadTreeScene scene)
    {
        _Scene = scene;
    }

    // build all nodes & return active camera
    public override Camera LoadNodes(SpriteBatch spriteBatch, GraphicsDevice graphics)
    {
        _SpriteBatch = spriteBatch;
        _GraphicsDevice = graphics;
        
        var camera = Prefabs.LoadCamera(_GraphicsDevice, _Scene);
        
        // core gameplay
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
        Prefabs.LoadCleanupManager(_Scene, false, 6, 6, 6); // spawn clutter/dust/clothes counts
        Prefabs.LoadDeEquipButton(_GraphicsDevice, _Scene);
        Prefabs.LoadTimer(_GraphicsDevice, _Scene, 300);
        Prefabs.LoadAfterGameMenu(_GraphicsDevice, _Scene, LevelEnum.Level3);
        Prefabs.LoadPauseButton(_GraphicsDevice, _Scene);
        Prefabs.LoadPauseMenu(_GraphicsDevice, _Scene);
        Prefabs.LoadHelpMenu(_GraphicsDevice, _Scene);
        Prefabs.LoadHearts(_GraphicsDevice, _Scene);
        Prefabs.LoadLoadButton(_GraphicsDevice, _Scene);
        Prefabs.LoadLoadButton(_GraphicsDevice, _Scene, "2");
        Prefabs.LoadClothesLoadArea(_Scene);
        Prefabs.LoadClutterDustLoadArea(_Scene);
        Prefabs.LoadWallPlant(_Scene);
        
        // companion dog decoration
        Prefabs.LoadDog(_Scene, new Vector3(10, 0.2f, 5), false);

        // ambience
        MusicController.ChangeSong("background_wind");

        return camera;
    }
}