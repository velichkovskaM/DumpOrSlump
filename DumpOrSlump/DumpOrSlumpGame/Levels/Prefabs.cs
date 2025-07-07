using System.Collections.Generic;
using DumpOrSlumpGame.Components;
using DumpOrSlumpGame.Components.AI;
using DumpOrSlumpGame.Components.CleanupObjects;
using DumpOrSlumpGame.Components.InteractableItems;
using DumpOrSlumpGame.Components.StaticObjects;
using DumpOrSlumpGame.Components.UI;
using DumpOrSlumpGame.Components.UI.Buttons;
using DumpOrSlumpGame.Components.UI.Buttons.HelpMenu;
using DumpOrSlumpGame.Components.UI.Buttons.InfoMenu;
using DumpOrSlumpGame.Components.UI.Buttons.MenuButtons;
using DumpOrSlumpGame.Components.UI.Buttons.PauseMenu;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using AudioEmitter = GameEngine.Components.AudioEmitter;
using AudioListener = GameEngine.Components.AudioListener;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace DumpOrSlumpGame.Levels;

/// <summary>
/// Static helper class that spawns prefab objects (camera, player, UI, interactables, NPCs) into a scene
/// Centralizes object creation so individual SceneLoaders stay concise
/// </summary>
public static class Prefabs
{
    // CORE ENGINE OBJECTS

    // camera
    public static Camera LoadCamera(GraphicsDevice _GraphicsDevice, IScene _Scene)
    {
        var Camera = new Node("Camera");
        var CameraComponent = new Camera(Camera, _GraphicsDevice);
        Camera.AddComponent(CameraComponent);
        _Scene.Insert(Camera);

        return CameraComponent;
    }

    // player
    public static void LoadPlayer(IScene _Scene, Vector3 _Position)
    {
        var player = new Node("Player",
            position: _Position + new Vector3(0, 0.2f, 0),
            scale: new Vector3(0.9f, 0.9f, 0.9f));
        SpriteRenderer spriteRendererPlayer = new SpriteRenderer(player, Globals._Graphics.GraphicsDevice);
        player.AddComponent(spriteRendererPlayer);

        var AudioListener = new AudioListener(player);
        player.AddComponent(AudioListener);
        
        var playerScript = new Player(player);
        player.AddComponent(playerScript);
        
        
        var collider = new AABBCollider(player, new BoundingBox(
            new Vector2(-0.2f, -0.1f),
            new Vector2(0.2f, 0.1f)
            ));
        player.AddComponent(collider);
        _Scene.Insert(player);
    }

    // UI ELEMENTS 

    // joystick
    public static void LoadController(GraphicsDevice _GraphicsDevice, IScene _Scene)
    {
        var Controller =
            new UINode("Controller",
                position: new Vector3(180 * Camera.scale, 2, _GraphicsDevice.Viewport.Height - 200 * Camera.scale),
                scale: new Vector3(0.6f, 1f, 0.6f)
                
                );
        SpriteRenderer spriteRendererController = new SpriteRenderer(Controller, Globals._Graphics.GraphicsDevice);
        Controller.AddComponent(spriteRendererController);
        var controllerScript = new Controller(Controller);
        Controller.AddComponent(controllerScript);
        _Scene.UiNodes.Add(Controller);
    }

    // hearts
    public static void LoadHearts(GraphicsDevice _GraphicsDevice, IScene _Scene)
    {
        var heartsController =
            new Node("StaminaController", position: new Vector3(256 * 0.6f + 230 * Camera.scale, 2, _GraphicsDevice.Viewport.Height - 100 * Camera.scale));
        var heartControllerScript = new StaminaController(heartsController);
        heartsController.AddComponent(heartControllerScript);
        _Scene.UiNodes.Add(heartsController);
    }
    
    // de‑equip btn
    public static void LoadDeEquipButton(GraphicsDevice _GraphicsDevice, IScene _Scene)
    {
        var node = new UINode("DeEquipButton",
            position: new Vector3(_GraphicsDevice.Viewport.Width - 200 * Camera.scale, 2, _GraphicsDevice.Viewport.Height - 250 * Camera.scale),
            scale: new Vector3(1, 1, 1));
        SpriteRenderer spriteRendererController = new SpriteRenderer(node, Globals._Graphics.GraphicsDevice);
        node.AddComponent(spriteRendererController);
        var script = new DeEquip(node);
        node.AddComponent(script);
        _Scene.UiNodes.Add(node);
    }
    
    // loader btn
    public static void LoadLoadButton(GraphicsDevice _GraphicsDevice, IScene _Scene, string suffix = "1")
    {
        var node = new UINode("LoadClothesButton" + suffix,
            position: new Vector3(_GraphicsDevice.Viewport.Width - 200 * Camera.scale, 2, _GraphicsDevice.Viewport.Height - 250 * Camera.scale),
            scale: new Vector3(1, 1, 1));
        SpriteRenderer spriteRendererController = new SpriteRenderer(node, Globals._Graphics.GraphicsDevice);
        node.AddComponent(spriteRendererController);
        var script = new LoadButton(node);
        node.AddComponent(script);
        node.SetActive(false);
        _Scene.UiNodes.Add(node);
    }

    // pause btn
    public static void LoadPauseButton(GraphicsDevice _GraphicsDevice, IScene _Scene)
    {
        var node = new UINode("PauseButton",
            position: new Vector3(_GraphicsDevice.Viewport.Width - 200 * Camera.scale, 2, _GraphicsDevice.Viewport.Height - 100 * Camera.scale),
            scale: new Vector3(1, 1, 1));
        SpriteRenderer spriteRendererController = new SpriteRenderer(node, Globals._Graphics.GraphicsDevice);
        node.AddComponent(spriteRendererController);
        var script = new PauseButton(node);
        node.AddComponent(script);
        _Scene.UiNodes.Add(node);
    }

    // timer widget
    public static void LoadTimer(GraphicsDevice _GraphicsDevice, IScene _Scene, float totalTimeForLevel)
    {
        var node = new UINode("Timer", position: new Vector3(_GraphicsDevice.Viewport.Width - 500 * Camera.scale, 2, _GraphicsDevice.Viewport.Height - 100 * Camera.scale), scale: new Vector3(1, 1, 1));
        SpriteRenderer spriteRendererTimer = new SpriteRenderer(node, Globals._Graphics.GraphicsDevice);
        node.AddComponent(spriteRendererTimer);
        TextRenderer textRendererTimer = new TextRenderer(node);
        node.AddComponent(textRendererTimer);
        Timer timer = new Timer(node, totalTimeForLevel);
        node.AddComponent(timer);
        _Scene.UiNodes.Add(node);
    }

    // after‑game menu
    public static void LoadAfterGameMenu(GraphicsDevice _GraphicsDevice, IScene _Scene, LevelEnum nextLevel)
    {
        var endScreenCensor = new Node( "EndScreenCensorScreen",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 5f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(20f, 0, 20f));
        endScreenCensor.AddComponent(new SpriteRenderer(endScreenCensor, Globals._Graphics.GraphicsDevice));
        var endScreenCensorScript = new CensorScript(endScreenCensor);
        endScreenCensor.AddComponent(endScreenCensorScript);
        
        var node = new UINode( "WinLoseText", position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 5.1f, _GraphicsDevice.Viewport.Height / 2));
        TextRenderer textRendererWinLoseText = new TextRenderer(node);
        node.AddComponent(textRendererWinLoseText);
        WinLoseText winLoseText = new WinLoseText(node);
        node.AddComponent(winLoseText);
        
        var EndScreenMenuButton = new UINode( "EndScreenMenuButton",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 - 250 * Camera.scale, 5.1f, _GraphicsDevice.Viewport.Height / 2 + 100 * Camera.scale),
            scale: new Vector3(0.75f, 0, 0.75f)
        );
        EndScreenMenuButton.AddComponent(new SpriteRenderer(EndScreenMenuButton, Globals._Graphics.GraphicsDevice));
        var EndScreenMenuButtonScript = new ButtonMenu(EndScreenMenuButton);
        EndScreenMenuButton.AddComponent(EndScreenMenuButtonScript);
        
        var EndScreenRestart = new UINode( "EndScreenRestartButton",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 - 25 * Camera.scale, 5.1f, _GraphicsDevice.Viewport.Height / 2 + 100 * Camera.scale),
            scale: new Vector3(0.75f, 0, 0.75f)
        );
        EndScreenRestart.AddComponent(new SpriteRenderer(EndScreenRestart, Globals._Graphics.GraphicsDevice));
        var EndScreenRestartScript = new ButtonRestart(EndScreenRestart);
        EndScreenRestart.AddComponent(EndScreenRestartScript);
        
        var NextLevelScreenRestart = new UINode( "NextLevelScreenRestartButton",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 + 200 * Camera.scale, 5.1f, _GraphicsDevice.Viewport.Height / 2 + 100 * Camera.scale),
            scale: new Vector3(0.75f, 0, 0.75f)
        );
        NextLevelScreenRestart.AddComponent(new SpriteRenderer(NextLevelScreenRestart, Globals._Graphics.GraphicsDevice));
        var nextLevelButtonScript = new ButtonNextLevel(NextLevelScreenRestart, nextLevel);
        NextLevelScreenRestart.AddComponent(nextLevelButtonScript);
        
        node.Children.Add(EndScreenMenuButton);
        node.Children.Add(EndScreenRestart);
        node.Children.Add(NextLevelScreenRestart);
        node.Children.Add(endScreenCensor);
        node.SetActive(false);
        _Scene.UiNodes.Add(endScreenCensor);
        _Scene.UiNodes.Add(EndScreenMenuButton);
        _Scene.UiNodes.Add(EndScreenRestart);
        _Scene.UiNodes.Add(NextLevelScreenRestart);
        _Scene.UiNodes.Add(node);
    }
    
    // pause menu
    public static void LoadPauseMenu(GraphicsDevice _GraphicsDevice, IScene _Scene)
    {
        
        var pauseCensor = new Node( "PauseMenuCensor",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 2.1f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(20f, 0, 20f));
        pauseCensor.AddComponent(new SpriteRenderer(pauseCensor, Globals._Graphics.GraphicsDevice));
        var pauseCensorScript = new CensorScript(pauseCensor);
        pauseCensor.AddComponent(pauseCensorScript);
        
        var pauseMenuBackground = new UINode( "PauseMenuBackground",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 2.5f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(1.5f, 0, 1.5f));
        pauseMenuBackground.AddComponent(new SpriteRenderer(pauseMenuBackground, Globals._Graphics.GraphicsDevice));
        var pauseMenuScript = new PauseMenuBackground(pauseMenuBackground);
        pauseMenuBackground.AddComponent(pauseMenuScript);
        
        var pauseRestart = new UINode( "pauseRestartButton",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 - 180 * Camera.scale, 3f, _GraphicsDevice.Viewport.Height / 2 - 50 * Camera.scale),
            scale: new Vector3(0.5f, 0, 0.5f)
            );
        pauseRestart.AddComponent(new SpriteRenderer(pauseRestart, Globals._Graphics.GraphicsDevice));
        var pauseRestartScript = new ButtonRestart(pauseRestart);
        pauseRestart.AddComponent(pauseRestartScript);
        
        var pauseExit = new UINode( "pauseExitButton",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 + 500 * Camera.scale, 3f, _GraphicsDevice.Viewport.Height / 2 - 280 * Camera.scale),
            scale: new Vector3(0.5f, 0, 0.5f)
        );
        pauseExit.AddComponent(new SpriteRenderer(pauseExit, Globals._Graphics.GraphicsDevice));
        var pauseExitScript = new ButtonExit(pauseExit);
        pauseExit.AddComponent(pauseExitScript);
        
        var pauseHelpButton = new UINode( "pauseHelpButton",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 + 200 * Camera.scale, 3f, _GraphicsDevice.Viewport.Height / 2 - 80 * Camera.scale),
            scale: new Vector3(0.5f, 0, 0.5f)
        );
        pauseHelpButton.AddComponent(new SpriteRenderer(pauseHelpButton, Globals._Graphics.GraphicsDevice));
        var pauseHelpScript = new ButtonHelp(pauseHelpButton);
        pauseHelpButton.AddComponent(pauseHelpScript);
        
        var pauseSoundOnOff = new UINode( "pauseSoundOnOff",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 - 225 * Camera.scale, 3f, _GraphicsDevice.Viewport.Height / 2 + 210 * Camera.scale),
            scale: new Vector3(0.5f, 0, 0.5f)
        );
        pauseSoundOnOff.AddComponent(new SpriteRenderer(pauseSoundOnOff, Globals._Graphics.GraphicsDevice));
        var pauseSoundOnOffScript = new SoundControl(pauseSoundOnOff);
        pauseSoundOnOff.AddComponent(pauseSoundOnOffScript);
        
        var pauseMenuButton = new UINode( "pauseMenuButton",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 + 350 * Camera.scale, 3f, _GraphicsDevice.Viewport.Height / 2 + 150 * Camera.scale),
            scale: new Vector3(0.5f, 0, 0.5f)
        );
        pauseMenuButton.AddComponent(new SpriteRenderer(pauseMenuButton, Globals._Graphics.GraphicsDevice));
        var pauseMenuButtonScript = new ButtonMenu(pauseMenuButton);
        pauseMenuButton.AddComponent(pauseMenuButtonScript);
        
        var soundbar = new UINode( "pauseSoundBar",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 - 275 * Camera.scale, 3f, _GraphicsDevice.Viewport.Height / 2 + 57 * Camera.scale),
            scale: new Vector3(0.4f, 0, 0.4f)
        );
        soundbar.AddComponent(new SpriteRenderer(soundbar, Globals._Graphics.GraphicsDevice));
        var pauseSoundbarScript = new SoundBar(soundbar);
        soundbar.AddComponent(pauseSoundbarScript);
        
        pauseMenuBackground.Children.Add(pauseRestart);
        pauseMenuBackground.Children.Add(pauseExit);
        pauseMenuBackground.Children.Add(pauseHelpButton);
        pauseMenuBackground.Children.Add(pauseCensor);
        pauseMenuBackground.Children.Add(pauseSoundOnOff);
        pauseMenuBackground.Children.Add(pauseMenuButton);
        pauseMenuBackground.Children.Add(soundbar);
        pauseMenuBackground.SetActive(false);
        _Scene.UiNodes.Add(pauseCensor);
        _Scene.UiNodes.Add(pauseMenuBackground);
        _Scene.UiNodes.Add(pauseRestart);
        _Scene.UiNodes.Add(pauseExit);
        _Scene.UiNodes.Add(pauseHelpButton);
        _Scene.UiNodes.Add(pauseSoundOnOff);
        _Scene.UiNodes.Add(pauseMenuButton);
        _Scene.UiNodes.Add(soundbar);
    }

    public static void LoadInfoPopUp(GraphicsDevice _GraphicsDevice, IScene _Scene, string text)
    {
        var popUpCensor = new Node( "PopUpCensor",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 4f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(20f, 0, 20f));
        popUpCensor.AddComponent(new SpriteRenderer(popUpCensor, Globals._Graphics.GraphicsDevice));
        var popUpCensorScript = new CensorScript(popUpCensor);
        popUpCensor.AddComponent(popUpCensorScript);
        
        var popUpBackground = new Node( "PopUpBackground",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 4.1f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(1.75f, 0, 1.75f));
        popUpBackground.AddComponent(new SpriteRenderer(popUpBackground, Globals._Graphics.GraphicsDevice));
        popUpBackground.AddComponent(new TextRenderer(popUpBackground));
        var popUpScript = new PopupMenu(popUpBackground, text: text);
        popUpBackground.AddComponent(popUpScript);
        
        var popUpExit = new UINode( "PopUpExit",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 + 380 * Camera.scale, 4.2f, _GraphicsDevice.Viewport.Height / 2 - 120 * Camera.scale),
            scale: new Vector3(0.5f, 0, 0.5f));
        popUpExit.AddComponent(new SpriteRenderer(popUpExit, Globals._Graphics.GraphicsDevice));
        var popUpExitScript = new ButtonPopupExit(popUpExit);
        popUpExit.AddComponent(popUpExitScript);
        
        popUpBackground.Children.Add(popUpCensor);
        popUpBackground.Children.Add(popUpExit);
        _Scene.UiNodes.Add(popUpBackground);
        _Scene.UiNodes.Add(popUpCensor);
        _Scene.UiNodes.Add(popUpExit);
    }
    
    public static void LoadHelpMenu(GraphicsDevice _GraphicsDevice, IScene _Scene)
    {
        var helpCensor = new Node( "HelpMenuCensor",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 4f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(20f, 0, 20f));
        helpCensor.AddComponent(new SpriteRenderer(helpCensor, Globals._Graphics.GraphicsDevice));
        var helpCensorScript = new CensorScript(helpCensor);
        helpCensor.AddComponent(helpCensorScript);
        
        var helpMenuBackground = new Node( "helpMenuBackground",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 4.1f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(0.75f, 0, 0.75f));
        helpMenuBackground.AddComponent(new SpriteRenderer(helpMenuBackground, Globals._Graphics.GraphicsDevice));
        var helpMenuScript = new HelpMenu(helpMenuBackground);
        helpMenuBackground.AddComponent(helpMenuScript);
        
        var helpMenuExit = new UINode( "HelpMenuExit",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 + 380 * Camera.scale, 4.2f, _GraphicsDevice.Viewport.Height / 2 - 120 * Camera.scale),
            scale: new Vector3(0.5f, 0, 0.5f));
        helpMenuExit.AddComponent(new SpriteRenderer(helpMenuExit, Globals._Graphics.GraphicsDevice));
        var helpMenuExitScript = new ButtonHelpExit(helpMenuExit);
        helpMenuExit.AddComponent(helpMenuExitScript);
        
        helpMenuBackground.Children.Add(helpCensor);
        helpMenuBackground.Children.Add(helpMenuExit);
        helpMenuBackground.SetActive(false);
        _Scene.UiNodes.Add(helpMenuBackground);
        _Scene.UiNodes.Add(helpCensor);
        _Scene.UiNodes.Add(helpMenuExit);
    }

    public static void LoadDropOffArea(IScene _Scene)
    {
        var node = new Node("DropOffArea",
            position: new Vector3(10f, 0, 0.5f),
            scale: new Vector3(1f, 1, 1f)
            
            );
        node.AddComponent(new DropOffArea(node));
        _Scene.Insert(node);
    }
    
    public static void LoadClothesLoadArea(IScene _Scene)
    {
        var node = new Node("clothesLoadArea",
            position: new Vector3(14f, 0, 2f),
            scale: new Vector3(1f, 1, 1f)
            
        );
        node.AddComponent(new LoadClothsArea(node));
        _Scene.Insert(node);
    }
    
    public static void LoadClutterDustLoadArea(IScene _Scene)
    {
        var node = new Node("clutterDustLoadArea",
            position: new Vector3(17f, 0, 0.5f),
            scale: new Vector3(1f, 1, 1f)
            
        );
        node.AddComponent(new LoadClutterDustArea(node));
        _Scene.Insert(node);
    }

    public static void LoadVacuumCleaner(IScene _Scene)
    {
        var vacuum =
            new Node("Vacuum",
                position: new Vector3(12, 0.2f, 1),
                scale: new Vector3(1f, 1, 1f)
                );
        SpriteRenderer spriteRendererController = new SpriteRenderer(vacuum, Globals._Graphics.GraphicsDevice);
        vacuum.AddComponent(spriteRendererController);
        var vacuumCleanerScript = new Vacuum(vacuum);
        vacuum.AddComponent(vacuumCleanerScript);
        _Scene.Insert(vacuum);
    }

    public static void LoadGarbageBag(IScene _Scene)
    {
        var garbageBag =
            new Node("GarbageBag",
                position: new Vector3(10.6f, -0.2f, 1),
                scale: new Vector3(0.92f, 0.92f, 0.92f)
                );
        SpriteRenderer spriteRendererController = new SpriteRenderer(garbageBag, Globals._Graphics.GraphicsDevice);
        garbageBag.AddComponent(spriteRendererController);
        var garbageBagScript = new GarbageBag(garbageBag);
        garbageBag.AddComponent(garbageBagScript);
        _Scene.Insert(garbageBag);
    }
    
    public static void LoadCleaningBasket(IScene _Scene)
    {
        var cleaningBasket =
            new Node("ClothesBasket",
                position: new Vector3(9.5f, 0.1f, 1),
                scale: new Vector3(0.8f, 0.8f, 0.8f));
        SpriteRenderer spriteRendererController = new SpriteRenderer(cleaningBasket, Globals._Graphics.GraphicsDevice);
        cleaningBasket.AddComponent(spriteRendererController);
        var cleaningBagBasketScript = new ClothesBasket(cleaningBasket);
        cleaningBasket.AddComponent(cleaningBagBasketScript);
        _Scene.Insert(cleaningBasket);
    }

    public static void LoadTrashCan(IScene _Scene)
    {
        var trashCan =
            new Node("TrashCan",
                position: new Vector3(16.5f, 0.1f, 1.1f),
                scale: new Vector3(0.8f, 0.8f, 0.8f)
                );
        SpriteRenderer spriteRendererController = new SpriteRenderer(trashCan, Globals._Graphics.GraphicsDevice);
        trashCan.AddComponent(spriteRendererController);
        var trashCanScript = new TrashCan(trashCan);
        trashCan.AddComponent(trashCanScript);
        _Scene.Insert(trashCan);
    }

    public static void LoadCleanupManager(IScene _Scene, bool clickWithCircle = false, int clothpieces = 3, int clutterpieces = 3, int dustpieces = 3)
    {
        var cleanUpManger =
            new Node("CleanManager", position: new Vector3(500, 0, 500));
        var clutterNode = new Node("ClutterHolder", position: new Vector3(0, 0, 0));
        var clothesNode = new Node("ClothesHolder", position: new Vector3(0, 0, 0));
        var dustNode = new Node("DustHolder", position: new Vector3(0, 0, 0));
        _Scene.Insert(clutterNode);
        _Scene.Insert(clothesNode);
        _Scene.Insert(dustNode);

        var boundryList = new List<BoundingBox>()
        {
            new (new Vector2(3f,5f), new Vector2(18, 10)), // between bed and table
            new (new Vector2(3f,10f), new Vector2(22.5f, 11)), // bottom left side
            new (new Vector2(22.5f,11f), new Vector2(25.5f, 11)), // front car toys
            new (new Vector2(25.5f,10f), new Vector2(28f, 11)), // bottom right side
            new (new Vector2(19.5f,4.4f), new Vector2(22.5f, 10)), // Under Child playroom left side
            new (new Vector2(22.5f,4.4f), new Vector2(24.7f, 8.5f)), // back of toy cars
            new (new Vector2(25.7f,4.4f), new Vector2(28f, 10)), // Under Child playroom right side
            //new (new Vector2(27f,1f), new Vector2(30f, 4.4f)), // Under Child bed
            new (new Vector2(20f,1f), new Vector2(26f, 4.4f)), // All the way to the wall under child bedroom
            new (new Vector2(4.5f,1.5f), new Vector2(18f, 5f)), // In front of door
            //new (new Vector2(3f,1f), new Vector2(4f, 3f)), // Under bed
        };
        
        
        var cleanUpMangerScript = new CleanUpManger(
            cleanUpManger,
            clothpieces, 
            clutterpieces, 
            dustpieces,
            boundryList,
            clothesNode,
            clutterNode,
            dustNode,
            clickWithCircle);
        cleanUpManger.AddComponent(cleanUpMangerScript);
        _Scene.Insert(cleanUpManger);
    }

    public static void LoadDog(IScene _Scene, Vector3 position, bool isCircleActivated)
    {
        var node = new Node("Dog",
            position: position,
            scale: new Vector3(0.6f, 0.6f, 0.6f));
        node.AddComponent(new SpriteRenderer(node, Globals._Graphics.GraphicsDevice));
        var dogScript = new Dog(node, 2, 5, new BoundingBox(
            new Vector2(1, 2f),
            new Vector2(20, 8)
        ),
            isCircleActivated);
        node.AddComponent(dogScript);
        _Scene.Insert(node);
    }

    public static void LoadWardrobe(IScene _Scene)
    {
        var node = new Node("Wardrobe",
            position: new Vector3(14.1f, 2.4f, 1.5f),
            scale: new Vector3(0.0045f, 0.0045f, 0.0045f)
            );
        node.AddComponent(new ModelRenderer(node, Globals.content.Load<Effect>("FX/LightShader")));
        var wardrobeScript = new Wardrobe(node);
        node.AddComponent(wardrobeScript);
        _Scene.Insert(node);
    }
    
    public static void LoadStaticRoom(IScene _Scene)
    {
        var node = new Node("StaticRoom", position: new Vector3(0, -1.63f, 13.5f), rotation: new Vector3(0, 0, 0), scale: new Vector3(0.009f, 0.009f, 0.009f));
        node.AddComponent(new ModelRenderer(node, Globals.content.Load<Effect>("FX/LightShader")));
        var nodeScript = new StaticRoom(node);
        node.AddComponent(nodeScript);
        var leftWallCollider = new AABBCollider(node,
            new BoundingBox(
                new Vector2(-1, -13.5f),
                new Vector2(0.3f, 2))
        );
        node.AddComponent(leftWallCollider);
        var BackWall = new AABBCollider(node,
            new BoundingBox(
                new Vector2(0, -14),
                new Vector2(30.5f, -13))
        );
        node.AddComponent(BackWall);
        var SideWall = new AABBCollider(node,
            new BoundingBox(
                new Vector2(30.5f, -13.5f),
                new Vector2(31, 2))
        );
        node.AddComponent(SideWall);
        
        var Front = new AABBCollider(node,
            new BoundingBox(
                new Vector2(0f, 0),
                new Vector2(30.5f, 0.5f))
        );
        node.AddComponent(Front);

        var Chest = new AABBCollider(node,
            new BoundingBox(
                new Vector2(28.5f, -4),
                new Vector2(31f, -1.5f))
        );
        node.AddComponent(Chest);
        
        var ChildBed = new AABBCollider(node,
            new BoundingBox(
                new Vector2(26.2f, -11),
                new Vector2(31f, -9f))
        );
        node.AddComponent(ChildBed);
        
        var Wardrope = new AABBCollider(node,
            new BoundingBox(
                new Vector2(12.7f, -13f),
                new Vector2(15.5f, -11f))
        );
        node.AddComponent(Wardrope);
        
        var Bed = new AABBCollider(node,
            new BoundingBox(
                new Vector2(1.3f, -13f),
                new Vector2(3.5f, -8.5f))
        );
        node.AddComponent(Bed);
        
        var tableNode = new Node("Table", position: node.Transform.Position, scale: Vector3.One);

        var tableAndChair = new ConvexCollider(tableNode,
            new List<Vector2>
            {
                new Vector2(0f,   0f),
                new Vector2(2.4f, 0f),
                new Vector2(3.3f, -1.3f),
                new Vector2(3.3f, -2.6f),
                new Vector2(2.4f, -3.85f),
                new Vector2(0f,   -3.85f)
            });
        tableNode.AddComponent(tableAndChair);
        
        node.Children.Add(tableNode);
        
        var PillarWallLeft = new AABBCollider(node,
            new BoundingBox(
                new Vector2(18.1f, -13f),
                new Vector2(18.8f, -12.5f))
        );
        node.AddComponent(PillarWallLeft);
        
        var PillarMiddleLeft = new AABBCollider(node,
            new BoundingBox(
                new Vector2(18.1f, -7.5f),
                new Vector2(18.8f, -6.9f))
        );
        node.AddComponent(PillarMiddleLeft);
        
        var PillarMiddleRight = new AABBCollider(node,
            new BoundingBox(
                new Vector2(30f, -7.5f),
                new Vector2(30.7f, -6.9f))
        );
        node.AddComponent(PillarMiddleRight);
        
        var PillarWallRight = new AABBCollider(node,
            new BoundingBox(
                new Vector2(30f, -13f),
                new Vector2(30.7f, -12.5f))
        );
        node.AddComponent(PillarWallRight);
        
        var ladder = new AABBCollider(node,
            new BoundingBox(
                new Vector2(20.7f, -7.1f),
                new Vector2(22.7f, -6.9f))
        );
        node.AddComponent(ladder);
        
        var Bunny = new AABBCollider(node,
            new BoundingBox(
                new Vector2(19.5f, -10.9f),
                new Vector2(20.1f, -10.3f))
        );
        node.AddComponent(Bunny);


         var n = new Node(
             "TableSoundEmitter",
            position: new Vector3(1.2f, 1, 11.8f)
        );
        
        var audioEmitter = new AudioEmitter(n);
        audioEmitter.SetSoundEffect(Globals.content.Load<SoundEffect>("SoundEffect/Radio"));
        audioEmitter.CurrentSoundInstance.IsLooped = true;
        audioEmitter.CurrentSoundInstance.Volume = 1f;
        audioEmitter.CurrentSoundInstance.Play();
        
        n.AddComponent(audioEmitter);
        
        _Scene.Insert(node);
        _Scene.Insert(n);
        _Scene.Insert(tableNode);
    }

    public static void LoadChild(IScene _Scene, Vector3 ladderStart)
    {
        ladderStart.Y += 0f;
        var node = new Node("Child", 
            position: ladderStart - new Vector3(-1, 0, 1) * 4 + Vector3.UnitY * 0.8f,
            scale: new Vector3(0.8f, 0.8f, 0.8f));
        node.AddComponent(new SpriteRenderer(node, Globals._Graphics.GraphicsDevice));
        node.AddComponent(new AudioEmitter(node));
        var childScript = new Child(node, ladderStart + Vector3.UnitY * 0.8f);
        node.AddComponent(childScript);
        _Scene.Insert(node);
    }

    public static void LoadMom(IScene _Scene)
    {
        var node = new Node("Mom", position: new Vector3(-500, 0.2f, -500), scale: new Vector3(1.1f, 1.1f, 1.1f));
        node.AddComponent(new SpriteRenderer(node, Globals._Graphics.GraphicsDevice));
        var momScript = new Mother(node);
        node.AddComponent(momScript);
        _Scene.Insert(node);
    }

    public static void LoadDoor(IScene _Scene)
    {
        var node = new Node("Door",
            position: new Vector3(7.1f, 0.2f, 0.08f),
            scale: new Vector3(1.2f, 1.2f, 1.2f)
            );
        node.AddComponent(new SpriteRenderer(node, Globals._Graphics.GraphicsDevice));
        node.AddComponent(new AudioEmitter(node));
        var doorScript = new Door(node);
        node.AddComponent(doorScript);
        _Scene.Insert(node);
    }

    public static void LoadWallPlant(IScene _Scene)
    {
        var node = new Node("WallPlant",
            position: new Vector3(8.445f, 0f, 0.05f),
            scale: new Vector3(1.5f, 1.35f, 1.35f)
        );
        node.AddComponent(new SpriteRenderer(node, Globals._Graphics.GraphicsDevice));
        var wallPlantScript = new WallPlant(node);
        node.AddComponent(wallPlantScript);
        _Scene.Insert(node);
    }
}