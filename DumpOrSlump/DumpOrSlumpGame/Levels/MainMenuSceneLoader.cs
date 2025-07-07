using DumpOrSlumpGame.Components.UI;
using DumpOrSlumpGame.Components.UI.Buttons;
using DumpOrSlumpGame.Components.UI.Buttons.HelpMenu;
using DumpOrSlumpGame.Components.UI.Buttons.Settings;
using DumpOrSlumpGame.Components.UI.MainMenu;
using GameEngine;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DumpOrSlumpGame.Levels;

/// <summary>
/// Scene loader for the main-menu level. Instantiates camera, background, UI panels, buttons, and decorative elements,
/// wiring them into the QuadTreeScene and setting up the initial game state (music, requested level, etc.)
/// </summary>
public class 
    MainMenuSceneLoader : SceneLoader
{
    public MainMenuSceneLoader(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
        _GraphicsDevice = graphicsDevice;
    }

    // exposed scene/spritebatch/device references
    public override QuadTreeScene _Scene { get; set; }
    public override SpriteBatch _SpriteBatch { get; set; }
    public override GraphicsDevice _GraphicsDevice { get; set; }
    
    // store scene reference
    public override void SetScene(QuadTreeScene scene)
    {
        _Scene = scene;
    }
    
    // build camera node & insert into scene
    public Camera LoadCamera()
    {
        var Camera = new Node("Camera", position: new Vector3(0, 50, 0));
        var CameraComponent = new Camera(Camera, _GraphicsDevice);
        Camera.AddComponent(CameraComponent);
        _Scene.Insert(Camera);

        return CameraComponent;
    }

    // add static main‑menu background sprite
    public void LoadMainMenuBackground()
    {
        var mainMenuBackground = new UINode( "MainMenuBackground",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 0f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(1f, 0, 1f));
        mainMenuBackground.AddComponent(new SpriteRenderer(mainMenuBackground, Globals._Graphics.GraphicsDevice));
        var mainMenuBackgroudnScript = new MainMenu(mainMenuBackground);
        mainMenuBackground.AddComponent(mainMenuBackgroudnScript);
        
        _Scene.UiNodes.Add(mainMenuBackground);
    }
    
    // help popup hierarchy (background, censor, exit)
    public void LoadHelpMenu()
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
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 + 470 * Camera.scale, 4.2f, _GraphicsDevice.Viewport.Height / 2 - 185 * Camera.scale),
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
    
    // stats popup hierarchy
    public void LoadStatsMenu()
    {
        var helpCensor = new Node( "StatsMenuCensor",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 4f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(20f, 0, 20f));
        helpCensor.AddComponent(new SpriteRenderer(helpCensor, Globals._Graphics.GraphicsDevice));
        var helpCensorScript = new CensorScript(helpCensor);
        helpCensor.AddComponent(helpCensorScript);
        
        var helpMenuBackground = new UINode( "statsMenuBackground",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 4.1f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(1.5f, 0, 1.5f));
        helpMenuBackground.AddComponent(new SpriteRenderer(helpMenuBackground, Globals._Graphics.GraphicsDevice));
        var helpMenuScript = new StatsMenu(helpMenuBackground);
        helpMenuBackground.AddComponent(helpMenuScript);

        var node = new UINode( "Stats",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 5f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(1f, 0, 1f));
        node.AddComponent(new TextRenderer(node));
        node.AddComponent(new TextRenderer(node));
        var statsScript = new StatsScreen(node);
        node.AddComponent(statsScript);
        
        var helpMenuExit = new UINode( "statsMenuExit",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 + 380 * Camera.scale, 4.2f, _GraphicsDevice.Viewport.Height / 2 - 120 * Camera.scale),
            scale: new Vector3(0.5f, 0, 0.5f));
        helpMenuExit.AddComponent(new SpriteRenderer(helpMenuExit, Globals._Graphics.GraphicsDevice));
        var helpMenuExitScript = new ButtonStatsExit(helpMenuExit);
        helpMenuExit.AddComponent(helpMenuExitScript);
        
        helpMenuBackground.Children.Add(helpCensor);
        helpMenuBackground.Children.Add(helpMenuExit);
        helpMenuBackground.Children.Add(node);
        helpMenuBackground.SetActive(false);
        _Scene.UiNodes.Add(helpMenuBackground);
        _Scene.UiNodes.Add(helpCensor);
        _Scene.UiNodes.Add(helpMenuExit);
        _Scene.UiNodes.Add(node);
    }

    // create and add main buttons
    public void LoadStartButton()
    {
        var startButton = new UINode( "StartButton",
            position: new Vector3(_GraphicsDevice.Viewport.Width - 400 * Camera.scale, 3f, _GraphicsDevice.Viewport.Height / 2 - 325 * Camera.scale),
            scale: new Vector3(0.75f, 0, 0.75f)
        );
        startButton.AddComponent(new SpriteRenderer(startButton, Globals._Graphics.GraphicsDevice));
        var startButtonScript = new StartButton(startButton);
        startButton.AddComponent(startButtonScript);
        _Scene.UiNodes.Add(startButton);
    }

    // help‑menu opener
    public void LoadHelpButton()
    {
        var helpButton = new UINode( "HelpButton",
            position: new Vector3(_GraphicsDevice.Viewport.Width - 400 * Camera.scale, 3f, _GraphicsDevice.Viewport.Height / 2 - 200 * Camera.scale),
            scale: new Vector3(0.75f, 0, 0.75f)
        );
        helpButton.AddComponent(new SpriteRenderer(helpButton, Globals._Graphics.GraphicsDevice));
        var helpButtonScript = new ButtonHelp(helpButton);
        helpButton.AddComponent(helpButtonScript);
        _Scene.UiNodes.Add(helpButton);
    }

    // settings opener
    public void LoadSettingsButton()
    {
        var settingsButton = new UINode( "SettingsButton",
            position: new Vector3(_GraphicsDevice.Viewport.Width - 400  * Camera.scale, 3f, _GraphicsDevice.Viewport.Height / 2 - 75 * Camera.scale),
            scale: new Vector3(0.75f, 0, 0.75f)
        );
        settingsButton.AddComponent(new SpriteRenderer(settingsButton, Globals._Graphics.GraphicsDevice));
        var settingsButtonScript = new ButtonSettings(settingsButton);
        settingsButton.AddComponent(settingsButtonScript);
        _Scene.UiNodes.Add(settingsButton);
    }
    
    // stats opener button
    public void LoadStatsButton()
    {
        var helpButton = new UINode( "StatsButton",
            position: new Vector3(_GraphicsDevice.Viewport.Width - 400 * Camera.scale, 3f, _GraphicsDevice.Viewport.Height / 2 + 50 * Camera.scale),
            scale: new Vector3(0.75f, 0, 0.75f)
        );
        helpButton.AddComponent(new SpriteRenderer(helpButton, Globals._Graphics.GraphicsDevice));
        var helpButtonScript = new ButtonStats(helpButton);
        helpButton.AddComponent(helpButtonScript);
        _Scene.UiNodes.Add(helpButton);
    }

    // build settings popup with sliders, toggles & level buttons
    public void LoadSettingsMenu()
    {
        var settingsCensor = new Node( "settingsMenuCensor",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 4f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(20f, 0, 20f));
        settingsCensor.AddComponent(new SpriteRenderer(settingsCensor, Globals._Graphics.GraphicsDevice));
        var settingsCensorScript = new CensorScript(settingsCensor);
        settingsCensor.AddComponent(settingsCensorScript);
        
        var settingsMenuBackground = new UINode( "settingsMenuBackground",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 4.1f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(1.5f, 0, 1.5f));
        settingsMenuBackground.AddComponent(new SpriteRenderer(settingsMenuBackground, Globals._Graphics.GraphicsDevice));
        var settingsMenuScript = new SettingsMenu(settingsMenuBackground);
        settingsMenuBackground.AddComponent(settingsMenuScript);
        
        var settingsSoundOnOff = new UINode( "settingsSoundOnOff",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 + 135 * Camera.scale, 4.2f, _GraphicsDevice.Viewport.Height / 2 - 35 * Camera.scale),
            scale: new Vector3(0.4f, 0, 0.4f)
        );
        settingsSoundOnOff.AddComponent(new SpriteRenderer(settingsSoundOnOff, Globals._Graphics.GraphicsDevice));
        var settingsSoundOnOffScript = new SoundControl(settingsSoundOnOff);
        settingsSoundOnOff.AddComponent(settingsSoundOnOffScript);
        
        var soundbar = new UINode( "settingsSoundBar",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 - 275 * Camera.scale, 4.2f, _GraphicsDevice.Viewport.Height / 2 - 35 * Camera.scale),
            scale: new Vector3(0.3f, 0, 0.3f)
        );
        soundbar.AddComponent(new SpriteRenderer(soundbar, Globals._Graphics.GraphicsDevice));
        var soundbarScript = new SoundBar(soundbar);
        soundbar.AddComponent(soundbarScript);
        
        var settingsExit = new UINode( "settingsExit",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 + 300 * Camera.scale, 4.2f, _GraphicsDevice.Viewport.Height / 2 - 130 * Camera.scale),
            scale: new Vector3(0.4f, 0, 0.4f)
        );
        settingsExit.AddComponent(new SpriteRenderer(settingsExit, Globals._Graphics.GraphicsDevice));
        var settingsExitScript = new ButtonSettingsExit(settingsExit);
        settingsExit.AddComponent(settingsExitScript);
        
        var settingsLevel1 = new UINode( "settingsLevel1",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 - 450 * Camera.scale, 4.2f, _GraphicsDevice.Viewport.Height / 2 + 115 * Camera.scale),
            scale: new Vector3(0.4f, 0, 0.4f)
        );
        settingsLevel1.AddComponent(new SpriteRenderer(settingsLevel1, Globals._Graphics.GraphicsDevice));
        var settingsLevel1Script = new Level1Button(settingsLevel1);
        settingsLevel1.AddComponent(settingsLevel1Script);
        
        var settingsLevel2 = new UINode( "settingsLevel2",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 - 350 * Camera.scale, 4.2f, _GraphicsDevice.Viewport.Height / 2 + 115 * Camera.scale),
            scale: new Vector3(0.4f, 0, 0.4f)
        );
        settingsLevel2.AddComponent(new SpriteRenderer(settingsLevel2, Globals._Graphics.GraphicsDevice));
        var settingsLevel2Script = new Level2Button(settingsLevel2);
        settingsLevel2.AddComponent(settingsLevel2Script);
        
        var settingsLevel3 = new UINode( "settingsLevel3",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 - 250 * Camera.scale, 4.2f, _GraphicsDevice.Viewport.Height / 2 + 115 * Camera.scale),
            scale: new Vector3(0.4f, 0, 0.4f)
        );
        settingsLevel3.AddComponent(new SpriteRenderer(settingsLevel3, Globals._Graphics.GraphicsDevice));
        var settingsLevel3Script = new Level3Button(settingsLevel3);
        settingsLevel3.AddComponent(settingsLevel3Script);
        
        var settingsLevel4 = new UINode( "settingsLevel3",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 - 150 * Camera.scale, 4.2f, _GraphicsDevice.Viewport.Height / 2 + 115 * Camera.scale),
            scale: new Vector3(0.4f, 0, 0.4f)
        );
        settingsLevel4.AddComponent(new SpriteRenderer(settingsLevel4, Globals._Graphics.GraphicsDevice));
        var settingsLevel4Script = new Level4Button(settingsLevel4);
        settingsLevel4.AddComponent(settingsLevel4Script);
        
        settingsMenuBackground.Children.Add(settingsCensor);
        settingsMenuBackground.Children.Add(settingsSoundOnOff);
        settingsMenuBackground.Children.Add(soundbar);
        settingsMenuBackground.Children.Add(settingsExit);
        settingsMenuBackground.Children.Add(settingsLevel1);
        settingsMenuBackground.Children.Add(settingsLevel2);
        settingsMenuBackground.Children.Add(settingsLevel3);
        settingsMenuBackground.Children.Add(settingsLevel4);
        settingsMenuBackground.SetActive(false);
        _Scene.UiNodes.Add(settingsMenuBackground);
        _Scene.UiNodes.Add(settingsCensor);
        _Scene.UiNodes.Add(soundbar);
        _Scene.UiNodes.Add(settingsSoundOnOff);
        _Scene.UiNodes.Add(settingsExit);
        _Scene.UiNodes.Add(settingsLevel1);
        _Scene.UiNodes.Add(settingsLevel2);
        _Scene.UiNodes.Add(settingsLevel3);
        _Scene.UiNodes.Add(settingsLevel4);
    }

    // decorative sleeping dog sprite
    public void LoadDogSleeping()
    {
        var node = new UINode( "dog",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 - 200 * Camera.scale, 1f, _GraphicsDevice.Viewport.Height / 2 + 100 * Camera.scale),
            scale: new Vector3(2.5f, 0, 2.5f)
        );
        node.AddComponent(new SpriteRenderer(node, Globals._Graphics.GraphicsDevice));
        var UIDogScript = new UIDog(node);
        node.AddComponent(UIDogScript);
        _Scene.UiNodes.Add(node);
    }

    // credits popup hierarchy
    public void LoadCreditsMenu()
    { 
        var creditsCensor = new Node( "creditsMenuCensor",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 4f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(20f, 0, 20f));
        creditsCensor.AddComponent(new SpriteRenderer(creditsCensor, Globals._Graphics.GraphicsDevice));
        var helpCensorScript = new CensorScript(creditsCensor);
        creditsCensor.AddComponent(helpCensorScript);
        
        var creditsMenuBackground = new UINode( "creditsMenuBackground",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 4.1f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(1.5f, 0, 1.5f));
        creditsMenuBackground.AddComponent(new SpriteRenderer(creditsMenuBackground, Globals._Graphics.GraphicsDevice));
        var helpMenuScript = new StatsMenu(creditsMenuBackground);
        creditsMenuBackground.AddComponent(helpMenuScript);

        var node = new UINode( "Credits",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2, 5f, _GraphicsDevice.Viewport.Height / 2),
            scale: new Vector3(1f, 0, 1f));
        node.AddComponent(new TextRenderer(node));
        node.AddComponent(new TextRenderer(node));
        var creditsScript = new CreditScreen(node);
        node.AddComponent(creditsScript);
        
        var creditsMenuExit = new UINode( "statsMenuExit",
            position: new Vector3(_GraphicsDevice.Viewport.Width / 2 + 380 * Camera.scale, 4.2f, _GraphicsDevice.Viewport.Height / 2 - 120 * Camera.scale),
            scale: new Vector3(0.5f, 0, 0.5f));
        creditsMenuExit.AddComponent(new SpriteRenderer(creditsMenuExit, Globals._Graphics.GraphicsDevice));
        var helpMenuExitScript = new ButtonCreditsExit(creditsMenuExit);
        creditsMenuExit.AddComponent(helpMenuExitScript);
        
        creditsMenuBackground.Children.Add(creditsCensor);
        creditsMenuBackground.Children.Add(creditsMenuExit);
        creditsMenuBackground.Children.Add(node);
        creditsMenuBackground.SetActive(false);
        _Scene.UiNodes.Add(creditsMenuBackground);
        _Scene.UiNodes.Add(creditsCensor);
        _Scene.UiNodes.Add(creditsMenuExit);
        _Scene.UiNodes.Add(node);
    }
    
    // credits opener button
    public void LoadCreditsButton()
    {
        var creditsButton = new UINode( "CreditsButton",
            position: new Vector3(_GraphicsDevice.Viewport.Width - 400 * Camera.scale, 3f, _GraphicsDevice.Viewport.Height / 2 + 175 * Camera.scale),
            scale: new Vector3(0.75f, 0, 0.75f)
        );
        creditsButton.AddComponent(new SpriteRenderer(creditsButton, Globals._Graphics.GraphicsDevice));
        var helpButtonScript = new CreditsButton(creditsButton);
        creditsButton.AddComponent(helpButtonScript);
        _Scene.UiNodes.Add(creditsButton);
    }
    
    // main entry: orchestrate all loaders, set music & default level
    public override Camera LoadNodes(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        _SpriteBatch = spriteBatch;
        _GraphicsDevice = graphicsDevice;
        
        var camera = LoadCamera();
        LoadHelpMenu();
        LoadStartButton();
        LoadHelpButton();
        LoadSettingsButton();
        LoadStatsButton();
        LoadCreditsButton();
        
        LoadDogSleeping();
        
        LoadStatsMenu();
        
        LoadCreditsMenu();

        LoadSettingsMenu();
        LoadMainMenuBackground();

        MusicController.ChangeSong("floating_cat");
        Game1.Instance.requestedLevelToChangeTo = LevelEnum.Level1;

        return camera;
    }
}