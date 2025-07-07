using GameEngine;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Levels;

/// <summary>
/// Loads and wires together all gameplay nodes, UI, and prefabs for Level 1
/// Instantiates camera, player, room, interactables, UI widgets, and sets background music
/// </summary>
public class Level1SceneLoader : SceneLoader
{
    // forward device from constructor
    public Level1SceneLoader(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
        _GraphicsDevice = graphicsDevice;
    }

    // scene‑level handles required by SceneLoader
    public override QuadTreeScene _Scene { get; set; }
    public override SpriteBatch _SpriteBatch { get; set; }
    public override GraphicsDevice _GraphicsDevice { get; set; }
    
    
    // cache scene reference
    public override void SetScene(QuadTreeScene scene)
    {
        _Scene = scene;
    }

    // orchestrate prefab creation & return camera
    public override Camera LoadNodes(SpriteBatch spriteBatch, GraphicsDevice graphics)
    {
        _SpriteBatch = spriteBatch;
        _GraphicsDevice = graphics;
        
        var camera = Prefabs.LoadCamera(_GraphicsDevice, _Scene);
        
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
        Prefabs.LoadCleanupManager(_Scene, false, 3, 3, 3);
        Prefabs.LoadDeEquipButton(_GraphicsDevice, _Scene);
        Prefabs.LoadTimer(_GraphicsDevice, _Scene, 300);
        Prefabs.LoadAfterGameMenu(_GraphicsDevice, _Scene, LevelEnum.Level2);
        Prefabs.LoadPauseButton(_GraphicsDevice, _Scene);
        Prefabs.LoadPauseMenu(_GraphicsDevice, _Scene);
        Prefabs.LoadHelpMenu(_GraphicsDevice, _Scene);
        Prefabs.LoadHearts(_GraphicsDevice, _Scene);
        Prefabs.LoadLoadButton(_GraphicsDevice, _Scene);
        Prefabs.LoadLoadButton(_GraphicsDevice, _Scene, "2");
        Prefabs.LoadClothesLoadArea(_Scene);
        Prefabs.LoadClutterDustLoadArea(_Scene);
        Prefabs.LoadWallPlant(_Scene);
        Prefabs.LoadInfoPopUp(_GraphicsDevice, _Scene, "\n\n\n\nTo pickup clothes, clutter or dust,\nwhen in close range, double click on them,\nwhile having the appropriate tool selected.\nWhen finished using the tool, dispose in\nthe trashcan or wardrobe accordingly.");


        MusicController.ChangeSong("background_wind");
        
        return camera;
    }
}