using System;
using System.Collections.Generic;
using System.Linq;
using DumpOrSlumpGame.Components;
using DumpOrSlumpGame.Components.CleanupObjects;
using DumpOrSlumpGame.Components.UI;
using DumpOrSlumpGame.Components.UI.Buttons;
using DumpOrSlumpGame.Levels;
using GameEngine;
using GameEngine.Components;
using GameEngine.Core;
using GameEngine.Physics.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using AudioEmitter = GameEngine.Components.AudioEmitter;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace DumpOrSlumpGame;

/// <summary>
/// Root MonoGame entry‑point that boots the engine, loads levels, routes input, updates game state,
/// handles camera panning and draws both 3D and UI passes with a pixelation post‑effect
/// </summary>
public class Game1 : Game
{
    // FIELDS / FLAGS 
    public static bool isGameWon = false; // global win flag
    public static bool isGameOver = false; // global loss flag
    
    protected GraphicsDeviceManager _graphics;
    protected SpriteBatch _spriteBatch;
    
    public static Game1 Instance { get; protected set; }
    public QuadTreeScene Scene { get; protected set; }

    public Node clutterHandler;
    public Node clothesHandler;
    public Node dustHandler;

    public bool canDropEquipment = false;
    
    public Camera CurrentCamera { get; protected set; }
    
    public bool finished = false;
    
    public static bool requestRestart = false;
    
    public TouchCollection touchCollection = new TouchCollection();
    
    public static bool pause = false;
    
    
    public LevelEnum requestedLevelToChangeTo = LevelEnum.Level1;
    public bool requestLevelChange = false;
    
    public Dictionary<int, GestureTracker> gestureTrackers = new Dictionary<int, GestureTracker>();
    public List<int> customGestures = new List<int>();
    public List<GestureSample> Gestures = new List<GestureSample>();

    // panning
    
    public bool isPanningToPosition = false;
    public bool isPanningBackFromPosition = false;
    public Vector3 PositionToPanTo = Vector3.Zero;
    public Vector3 PanningStartPosition = Vector3.Zero;
    public float PanningSpeed = 1f;
    public float PanningProgress = 0;
    public bool waitBetweenPanning = true;
    public RenderTarget2D renderTarget;

    // render targets / post fx
    
    public Effect pixelEffect;
    
    private VertexBuffer vertexBuffer;
    private IndexBuffer indexBuffer;

    private float pixelSize = 8.0f;

    protected (int width, int height) fullDimensions = (0, 0);
    
    // ctor 
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.IsFullScreen = true;
        _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        _graphics.ApplyChanges();
        Globals._Graphics = _graphics;
        Content.RootDirectory = "Content";
        Globals.content = Content;
        IsMouseVisible = true;
        TouchPanel.EnabledGestures = GestureType.DoubleTap;
    }

    // init graphics + engine
    protected override void Initialize()
    {
        // Grab the final back-buffer size once (fullscreen may not be ready during the constructor) and force the XNA viewport to it
        
        if (fullDimensions == (0, 0))
        {
            fullDimensions = (GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);
        }
        
        GraphicsDevice.Viewport = new Viewport(
            0, 0,
            fullDimensions.width,
            fullDimensions.height
        );
        
        // Full-screen quad for the pixel-shader post-effect
        vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);
        VertexPositionTexture[] vertices = new VertexPositionTexture[4]
        {
            new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1))
        };
        vertexBuffer.SetData(vertices);

        short[] indices = new short[] { 0, 1, 2, 1, 3, 2 };
        indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);
        indexBuffer.SetData(indices);

        // Off-screen colour / depth buffer (where the world is drawn)  
        
        int bbW = GraphicsDevice.PresentationParameters.BackBufferWidth;
        int bbH = GraphicsDevice.PresentationParameters.BackBufferHeight;

        renderTarget = new RenderTarget2D(
            GraphicsDevice,
            bbW, bbH, // 1-to-1 with the swap-chain
            false,
            SurfaceFormat.Color,
            DepthFormat.Depth24);

        // Pixelation shader
        
        pixelEffect = Content.Load<Effect>("FX/PixelationEffect");
        
        // Persisted settings (volume, etc.)
        if (!SaveAPI.SettingsExists())
        {
            SaveAPI.SetDefualtSettings();
            SaveAPI.LoadSettingsFile();
        }
        else
        {
            SaveAPI.LoadSettingsFile();
        }

        SoundEffect.MasterVolume = SaveAPI.settings.sound_on_off ? SaveAPI.settings.volume : 0;
        SoundEffect.SpeedOfSound *= 10f;
        SoundEffect.DistanceScale *= 10f;
        
        //  Music initialisation
        
        MediaPlayer.IsRepeating = true;
        var intro_music = Content.Load<Song>("Music/IntroMusic");
        var background_music = Content.Load<Song>("Music/main_level_background");

        MusicController.setIsDeaf(!SaveAPI.settings.sound_on_off);
        MusicController.AddSong("floating_cat", intro_music);
        MusicController.AddSong("background_wind", background_music);
        MusicController.SetVolume(SaveAPI.settings.volume);
        
        // Scene boot-strap (loads Main-Menu by default)
        
        Scene = new QuadTreeScene(8, new MainMenuSceneLoader(_graphics.GraphicsDevice));
        Scene._sceneLoader.SetScene(Scene);
        
        var camera = Scene.InitScene(_spriteBatch, GraphicsDevice);

        // pre-cache handlers for win-condition checks
        
        dustHandler = Scene.FindNodeByName("DustHolder");
        clothesHandler = Scene.FindNodeByName("ClothesHolder");
        clutterHandler = Scene.FindNodeByName("ClutterHolder");

        CurrentCamera = camera;
        
        // Shared sprite/line helpers
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.spriteBatch = _spriteBatch;
        Globals.LineTexture = new Texture2D(GraphicsDevice, 1, 1);
        Globals.LineTexture.SetData(new[] { Color.White });
        
        // allow Update/Draw loops to start
        finished = true;
    }

    // load static textures/sprites
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.spriteBatch = _spriteBatch;
        Globals.LineTexture = new Texture2D(GraphicsDevice, 1, 1);
        Globals.LineTexture.SetData(new[] { Color.White });
        
    }

    // per‑frame update loop (input, scene, camera, panning)
    protected override void Update(GameTime gameTime)
    {
        // Update shared timing helpers with the delta for this frame
        Time.UpdateTimes(gameTime);
        
        // Snapshot the current touch state coming from the platform
        var touchCollection = TouchPanel.GetState();

        // Clear the list that will store gestures detected this frame
        Gestures.Clear();
        
        // Drain the gesture queue completely so we don’t miss anything
        while (TouchPanel.IsGestureAvailable)
        {
            var gesture = TouchPanel.ReadGesture();
            Gestures.Add(gesture);
        }

        // Sort UI nodes from back (lowest Y) to front (highest Y)
        Scene.UiNodes.Sort((node1, node2) => 
            (node1?.Transform?.Position.Y ?? 0).CompareTo(node2?.Transform?.Position.Y ?? 0));
        Scene.UiNodes.Reverse();
        
        // List that will hold touches not consumed by the UI layer
        var touches = new List<TouchLocation>();
        
        // Dispatch raw touches to UI
        foreach (var touch in touchCollection)
        {
            var consumed = false;
            // We only treat newly‑pressed touches as potential clicks
            if (touch.State == TouchLocationState.Pressed)
            {
                // Iterate through UI nodes until one claims the touch
                foreach (var node in Scene.UiNodes)
                {
                    if (!node.active)
                    {
                        continue; // skip nodes disabled in the hierarchy
                    }
                    
                    var renderer = node.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        // Calculate the world‑space AABB of the sprite
                        var dim = renderer.GetDimensions();

                        var halfwidth = (dim.width / 2) * node.Transform.Scale.X;
                        var halfheight = (dim.height / 2) * node.Transform.Scale.Z;
                        
                        var rect = new BoundingBox(
                            new Vector2(node.Transform.Position.X - halfwidth, node.Transform.Position.Z - halfheight),
                            new Vector2(node.Transform.Position.X + halfwidth, node.Transform.Position.Z + halfheight)
                        );
                        
                        // If the touch is inside the node’s rectangle, fire callbacks
                        if (rect.Contains(touch.Position))
                        {
                            // Button‑style component
                            var button = node.GetComponent<ButtonComponent>();
                            button?.OnClick(touch);

                            // Knob that needs to remember touch‑id and offset for drags
                            var soundKnob = node.GetComponent<Knob>();
                            soundKnob?.SetTouchIdAndOffset(touch.Id, touch.Position);

                            // Two‑state button (e.g. mute/unmute)
                            var soundOnOffButton = node.GetComponent<StateButtonComponent>();
                            soundOnOffButton?.OnClick();
                            
                            // Generic controller component for custom logic
                            var controller = node.GetComponent<Controller>();
                            controller?.OnClick(touch);
                            
                            // Mark the touch as handled and stop searching
                            consumed = true;
                            break;
                        }
                    }
                }
            }

            // Touches that nobody claimed bubble down to the gameplay layer
            if (!consumed)
            {
                touches.Add(touch);
            }
        }
        
        // Replace the global touch collection with the unconsumed subset
        this.touchCollection = new TouchCollection(touches.ToArray());
        
        // High‑level gesture handling (pinch, swipe, etc.)
        HandleGestures();

        // Gameplay update (skipped when paused)
        if (!pause)
        {
            var done = checkGameOver();

            if (done)
            {
                SetGameWon();
            }

            HandleCameraFollow();

            // Update non‑UI scene graph using the filtered touches
            Scene.root.Update(gameTime, touchCollection);
        }

        // UI component updates
        foreach (var node in Scene.UiNodes)
        {
            if (node.active)
            {
                foreach (var component in node.Components)
                {
                    if (component.Active)
                    {
                        if (component is Timer && Game1.pause)
                        {
                            continue;
                        }
                        
                        component.Update(gameTime, touchCollection);
                    }
                }
            }
        }
        
        // Safely add any nodes queued during the update above
        Scene.InsertAllSafeInsertedNotes();
        
        HandleSceneUpdates();

        // Deferred scene‑level requests
        if (requestRestart)
        {
            RestartGame();
            requestRestart = false;
        }

        if (requestLevelChange)
        {
            ChangeLevel(requestedLevelToChangeTo);
            requestLevelChange = false;
        }
    }

    // camera follow + panning state machine
    private void HandleCameraFollow()
    {
        var player = Scene.FindNodeByName("Player");
        if (player != null && !isPanningToPosition && !isPanningBackFromPosition)
        {
            var ZValue = player.Transform.Position.Z + 10;
            var cameraStayInMapLocation = 22;
            var maxX = 27.8f;
            var minX = 3.8f;
            if (ZValue > cameraStayInMapLocation) ZValue = cameraStayInMapLocation;
            
            var XValue = player.Transform.Position.X;

            if (XValue < minX) XValue = minX;
            if (XValue > maxX) XValue = maxX;
            
            CurrentCamera.Parent.Transform.Position =
                new Vector3(XValue, 2, ZValue);
            CurrentCamera.UpdateViewMatrix();
        } else if (isPanningToPosition)
        {
            PanningProgress += PanningSpeed * Time.deltaTime;
            
            PanningProgress = MathHelper.Clamp(PanningProgress, 0, 1);
            
            CurrentCamera.Parent.Transform.Position = Vector3.Lerp(PanningStartPosition, PositionToPanTo, PanningProgress);

            CurrentCamera.UpdateViewMatrix();
            if (PanningProgress >= 1)
            {
                PanningProgress = 0;
                isPanningToPosition = false;
                isPanningBackFromPosition = true;
                waitBetweenPanning = true;
                PositionToPanTo = PanningStartPosition;
                PanningStartPosition = CurrentCamera.Parent.Transform.Position;
            }
        } else if (isPanningBackFromPosition && !waitBetweenPanning)
        {
            PanningProgress += PanningSpeed * Time.deltaTime;
            
            PanningProgress = MathHelper.Clamp(PanningProgress, 0, 1);
            
            CurrentCamera.Parent.Transform.Position = Vector3.Lerp(PanningStartPosition, player.Transform.Position + new Vector3(0, 2, 10), PanningProgress);

            CurrentCamera.UpdateViewMatrix();

            if (PanningProgress >= 1)
            {
                PanningProgress = 0;
                isPanningToPosition = false;
                isPanningBackFromPosition = false;
                waitBetweenPanning = false;
            }
        }
    }

    // gesture tracking & recognition
    private void HandleGestures()
    {
        // Cleanup trackers that were completed in the previous frame
        foreach (var customGestureId in customGestures)
        {
            // Skip IDs that were already removed for robustness
            if (!gestureTrackers.ContainsKey(customGestureId)) continue;
            gestureTrackers.Remove(customGestureId);
        }
        
        customGestures.Clear();
        
        // Update active trackers with current‑frame touch events
        foreach (var touch in touchCollection)
        {
            // Touch with this id is currently steering the player – ignore it
            if (touch.Id != Controller.currentMovementId)
            {
                // State‑machine style handling of the touch lifecycle
                if (touch.State == TouchLocationState.Pressed)
                {
                    gestureTrackers.Add(touch.Id, new GestureTracker(touch.Id));
                    gestureTrackers[touch.Id].Touches.Add(touch.Position);
                }
                
                if (!gestureTrackers.ContainsKey(touch.Id)) continue;

                if (touch.State == TouchLocationState.Moved)
                {
                    gestureTrackers[touch.Id].Touches.Add(touch.Position);
                }

                if (touch.State == TouchLocationState.Released)
                {
                    // Final point
                    gestureTrackers[touch.Id].Touches.Add(touch.Position);
                    
                    // Ask the tracker to classify the full path
                    gestureTrackers[touch.Id].SetGestureType();
                    Logger.Error($"Gesture {touch.Id} was released and is set to have gesture type: {gestureTrackers[touch.Id].Gesture}");
                    
                    // Hand off recognised gestures to whoever is listening
                    if (gestureTrackers[touch.Id].Gesture != GestureTracker.GestureType.Unfinished)
                        customGestures.Add(touch.Id);
                }
            }
        }
    }

    /// <summary>
    /// Performs spatial maintenance for all objects that moved this frame:
    /// collision resolution plus QuadTree housekeeping to keep the spatial index accurate
    /// </summary>
    private void HandleSceneUpdates()
    {
        // Make a local copy because the original collection can change during iteration if new objects move
        var updatedMarkedTransforms = Scene.UpdatedTransforms.ToList();
    
        // Collision resolution for movers
        foreach (var transform in updatedMarkedTransforms)
        {
            // Re-test the node against the world and resolve overlaps, using the active camera as context (for parallax or culling, etc.)
            CollisionDetection.HandleMovement(transform.ParentNode, CurrentCamera);
        }

        // Reset global counters so next frame’s diagnostics start at zero
        CollisionDetection.totalChecksMade = 0;
        CollisionDetection.totalPossibleChecks = 0;
    
        // QuadTree housekeeping
        foreach (var transform in updatedMarkedTransforms)
        {
            // Each transform decides if its parent node still fits in the
            // QuadTree leaf it currently resides in. If not, it removes and reinserts itself so lookups remain O(log n)
            transform.HandleTreeUpdateMark();
        }
        
        // Clear the list so that only next frame’s movers will be processed.
        Scene.UpdatedTransforms.Clear();
    }

    // draw 3D, then pixelate, then UI overlay
    protected override void Draw(GameTime gameTime)
    {
        // Abort the entire draw call while loading screens are still visible
        if (!finished)
            return;

        // Scene render to off‑screen target (for post‑processing)
        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(Color.Black); // Clear the render target

        // Build frustum once per frame 
        var frustum = CurrentCamera.GetFrustum();
        
        // Retrieve every SpriteRenderer currently in the scene graph
        var spriteRenderers = Scene.FindAllComponents<SpriteRenderer>();

        // Even when the game is paused we might want sprites to keep animating
        if (pause)
        {
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.Update(gameTime, new TouchCollection());
            }
        }
        
        // Draw 2D overlay sprites
        _spriteBatch.Begin();  
        foreach (var spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer.CurrentAnimation.Render2D && spriteRenderer.Parent.active && spriteRenderer.Active)
            {
                spriteRenderer.Draw(CurrentCamera, _spriteBatch);
            }
        }
        _spriteBatch.End();
        
        // Draw 3D model instances
        var modelRenderers = Scene.FindAllComponents<ModelRenderer>();
        foreach (var modelRenderer in modelRenderers)
        {
            if (modelRenderer.Active && modelRenderer.Parent.active)
            {
                if (!frustum.Intersects(modelRenderer.GetBoundingSphere())) continue;
                modelRenderer.Draw(CurrentCamera, _spriteBatch);
            }
        }
        
        // Draw depth‑sorted world sprites
        // (that exist in 3D and must respect camera distance)
        spriteRenderers.Sort((a, b) => a.Parent.Transform.Position.Z.CompareTo(b.Parent.Transform.Position.Z));
        foreach (var spriteRenderer in spriteRenderers)
        {
            if (!spriteRenderer.CurrentAnimation.Render2D && spriteRenderer.Parent.active && spriteRenderer.Active)
            {
                if (!frustum.Intersects(spriteRenderer.Get3DBoundingBox())) continue;
                spriteRenderer.Draw(CurrentCamera, _spriteBatch);
            }
        }
        
        // Post‑processing pass: pixelation effect
        GraphicsDevice.SetRenderTarget(null);   // back to swap‑chain
        GraphicsDevice.Clear(Color.Black);

        // Set shader parameters
        var resolution = new Vector2(renderTarget.Width, renderTarget.Height);
        pixelEffect.Parameters["SceneTexture"]?.SetValue(renderTarget);
        pixelEffect.Parameters["PixelSize"]?.SetValue(pixelSize);
        pixelEffect.Parameters["Resolution"]?.SetValue(resolution);
        pixelEffect.CurrentTechnique = pixelEffect.Techniques["PixelationTechnique"];
        
        // Full‑screen quad geometry already prepared during initialisation
        GraphicsDevice.SetVertexBuffer(vertexBuffer);
        GraphicsDevice.Indices = indexBuffer;
        
        // Apply the effect and draw
        foreach (var pass in pixelEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
        }

        // UI overlay pass (drawn without post‑processing)
        _spriteBatch.Begin();
        
        // Highest Y‑coordinate should be drawn last (front‑most)
        Scene.UiNodes.Sort((node1, node2) => Nullable.Compare(node1?.Transform.Position.Y, node2?.Transform.Position.Y));
        foreach (var node in Scene.UiNodes)
        {
            if (node.active)
            {
                foreach (var component in node.Components)
                {
                    if (component.Active)
                    {
                        component.Draw(CurrentCamera, _spriteBatch);
                    }
                }
            }
        }
        
        _spriteBatch.End();
        
        // Base class call for any additional framework behavior
        base.Draw(gameTime);
    }
    
    // evaluate victory / allow drop equipment
    protected bool checkGameOver()
    {
        if (dustHandler is null && clutterHandler is null && clothesHandler is null) return false;
        
        var takenNonDeliveredCount = 0;
        var allTaken = true;
        foreach (var dust in dustHandler.Children)
        {
            if (dust.active)
            {
                allTaken = false;
            }
            else if (!dust.GetComponent<Dust>().inTrashCan)
            {
                takenNonDeliveredCount++;
            }
        }
        
        foreach (var clutter in clutterHandler.Children)
        {
            if (clutter.active)
            {
                allTaken = false;
            }
            else if (!clutter.GetComponent<Clutter>().inTrashCan)
            {
                takenNonDeliveredCount++;
            }
        }
        
        foreach (var clothes in clothesHandler.Children)
        {
            if (clothes.active)
            {
                allTaken = false;
            }
            else if (!clothes.GetComponent<Clothes>().inWardrobe)
            {
                takenNonDeliveredCount++;
            }
        }

        if (takenNonDeliveredCount == 0)
        {
            canDropEquipment = true;
        }
        else
        {
            canDropEquipment = false;
        }
        
        return allTaken && takenNonDeliveredCount == 0;
    }
    
    // LEVEL & GAME FLOW HELPERS 

    // Requests the level to be changed 
    public void RequestChangeLevel(LevelEnum level)
    {
        requestLevelChange = true;
        requestedLevelToChangeTo = level;
    }
    
    // instantiate new scene loader depending on enum
    private void ChangeLevel(LevelEnum level)
    {
        pause = false;
        
        QuadTreeScene newScene = null;
        if (level == LevelEnum.MainMenu)
        {
            newScene = new QuadTreeScene(8, new MainMenuSceneLoader(_graphics.GraphicsDevice));
        } else if (level == LevelEnum.Level1)
        {
            newScene = new QuadTreeScene(8, new Level1SceneLoader(_graphics.GraphicsDevice));
        }
        else if (level == LevelEnum.Level2)
        {
            newScene = new QuadTreeScene(8, new Level2SceneLoader(_graphics.GraphicsDevice));
        }
        else if (level == LevelEnum.Level3)
        {
            newScene = new QuadTreeScene(8, new Level3SceneLoader(_graphics.GraphicsDevice));
        }
        else if (level == LevelEnum.Level4)
        {
            newScene = new QuadTreeScene(8, new Level4SceneLoader(_graphics.GraphicsDevice));
        }
        
        if (newScene == null) return;

        CollisionDetection.collideables.Clear();
        
        isPanningToPosition = false;
        isPanningBackFromPosition = false;
        PositionToPanTo = Vector3.Zero;
        PanningStartPosition = Vector3.Zero;
        PanningProgress = 0;

        Scene.SafeInsertedNodes.Clear();
        Scene.SafeInsertedUINodes.Clear();

        var emitters = Scene.FindAllComponents<AudioEmitter>();
        foreach (var emitter in emitters)
            emitter.StopSound();
        
        Scene.FindNodeByName("Player")?.GetComponent<Player>()?._soundEffect.Stop();
        
        Scene = newScene;
        Scene._sceneLoader.SetScene(Scene);

        var camera = Scene.InitScene(_spriteBatch, GraphicsDevice);

        dustHandler = Scene.FindNodeByName("DustHolder");
        clothesHandler = Scene.FindNodeByName("ClothesHolder");
        clutterHandler = Scene.FindNodeByName("ClutterHolder");

        CurrentCamera = camera;
        
        isGameOver = false;
        isGameWon = false;

        Time.Reload();
    }

    public void RestartGame()
    {
        var sceneLoader = Scene._sceneLoader.GetType();
        
        Game1.Instance.SetLastRunData(true, "Restarted the game");
        
        ChangeLevel(ConvertTypeToLevel(sceneLoader));
    }
    
    
    // get level enum from a sceneloader type
    public LevelEnum ConvertTypeToLevel(Type type)
    {
        if (type == typeof(Level1SceneLoader))
            return LevelEnum.Level1;
        if (type == typeof(Level2SceneLoader))
            return LevelEnum.Level2;
        if (type == typeof(Level3SceneLoader))
            return LevelEnum.Level3;
        if (type == typeof(Level4SceneLoader))
            return LevelEnum.Level4;

        return LevelEnum.MainMenu;
    }

    // starts a request to restart the current selected level
    public void RequestRestart()
    {
        requestRestart = true;
    }
    
    // gets the camera instance
    public static Camera GetCamera()
    {
        return Instance.CurrentCamera;
    }

    // gets the scene instance
    public static IScene GetScene()
    {
        return Instance.Scene;
    }
    
    // schedule camera pan to point-of-interest
    public static void SetPanningCamera(Vector3 toPosition)
    {
        var gameInstance = Instance;
        gameInstance.isPanningToPosition = true;
        toPosition = new Vector3(toPosition.X, toPosition.Y + 2, toPosition.Z + 10);
        gameInstance.PositionToPanTo = toPosition;
        gameInstance.PanningStartPosition = gameInstance.CurrentCamera.Parent.Transform.Position;
    }

    // set win state, record stats, freeze gameplay
    public void SetGameWon()
    {
        var node = Scene.FindNodeByName("WinLoseText");
        node.GetComponent<WinLoseText>().SetWinText();
        isGameWon = true;
        
        SetLastRunData(true, "Won the game");
        
        EndGameRemove();
    }

    // set lose state, record stats, freeze gameplay
    public void SetGameLost(string reason)
    {
        isGameOver = true;
        var node = Scene.FindNodeByName("WinLoseText");
        node.GetComponent<WinLoseText>().SetLoseText();
        
        SetLastRunData(true, reason);
        
        EndGameRemove();
    }
    
    // common SHOW end‑screen + disable gameplay UI
    public void EndGameRemove()
    {
        Scene.FindNodeByName("WinLoseText").SetActive(true);
        Scene.FindNodeByName("PauseMenuBackground").SetActive(false);
        Scene.FindNodeByName("helpMenuBackground").SetActive(false);
        
        Scene.FindNodeByName("DeEquipButton").SetActive(false);
        Scene.FindNodeByName("Controller").SetActive(false);
        Scene.FindNodeByName("Timer").SetActive(false);
        Scene.FindNodeByName("PauseButton").SetActive(false);
        Scene.FindNodeByName("StaminaController").SetActive(false);
        pause = true;
    }

    // write last‑run stats to SaveAPI
    public void SetLastRunData(bool died, string reason)
    {
        SaveAPI.settings.have_played = true;
        SaveAPI.settings.dust_missed = dustHandler.Children.Count(x => x.active);
        SaveAPI.settings.clutter_missed = clutterHandler.Children.Count(x => x.active);
        SaveAPI.settings.clothes_missed = clothesHandler.Children.Count(x => x.active);
        SaveAPI.settings.clothes_picked_up = clothesHandler.Children.Count(x => !x.active);
        SaveAPI.settings.clutter_picked_up = clutterHandler.Children.Count(x => !x.active);
        SaveAPI.settings.dust_picked_up = dustHandler.Children.Count(x => !x.active);
        SaveAPI.settings.died = died;
        SaveAPI.settings.reason = reason;
        var timer = Scene.FindNodeByName("Timer");
        var timerScript = timer?.GetComponent<Timer>();
        if (timerScript != null) SaveAPI.settings.time_left = MathF.Max((float)(timerScript?.maxTime - timerScript?.timer), 0);
        SaveAPI.SaveSettingsFile();
    }
}