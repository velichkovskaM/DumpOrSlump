using Android.OS;
using Android.Util;
using Android.Views;
using Dump_Or_Slump_Android;
using Microsoft.Xna.Framework;

namespace DumpOrSlump;

/// <summary>
/// Android entry‑point that extends the shared game core with platform‑specific initialization for logging, save‑file access,
/// and device resolution detection
/// </summary>
public class Game1 : DumpOrSlumpGame.Game1
{
    // ctor ─ sets up logging and platform services
    public Game1() : base()
    {
        Logger.Initialize(new AndroidLogger());
        GameEngine.SaveAPI.SetClass(typeof(AndroidSaveAPI));
        Instance = this;

    }
    
    protected override void Initialize()
    {
        // 1. Hide system bars in sticky immersive mode
        EnterStickyImmersiveMode(); 

        // 2. Now query drawable area and size your back-buffer…
        fullDimensions = GetWindowBounds();
        _graphics.PreferredBackBufferWidth  = fullDimensions.width;
        _graphics.PreferredBackBufferHeight = fullDimensions.height;
        _graphics.IsFullScreen = true;
        _graphics.HardwareModeSwitch = false;
        _graphics.ApplyChanges();

        base.Initialize();
    }
    
    // Edge-to-edge immersive mode  (works from API 21 → 34+)
    void EnterStickyImmersiveMode()
    {
        var decor = Game.Activity.Window.DecorView;
        var uiOptions =
            SystemUiFlags.LayoutStable              // keep layout from resizing
            | SystemUiFlags.LayoutFullscreen           // allow content under status bar
            | SystemUiFlags.LayoutHideNavigation       // allow content under nav bar
            | SystemUiFlags.Fullscreen                 // hide status bar
            | SystemUiFlags.HideNavigation             // hide nav bar
            | SystemUiFlags.ImmersiveSticky;           // keep them hidden after interaction

        decor.SystemUiVisibility = (StatusBarVisibility)uiOptions;
    }
    
    // Full window size helper (no inset subtraction)
    private (int width, int height) GetWindowBounds()
    {
        var wm = Game.Activity.WindowManager;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
        {
            var b = wm.CurrentWindowMetrics.Bounds;
            return (b.Width(), b.Height());
        }
        else
        {
            var dm = new DisplayMetrics();
            wm.DefaultDisplay.GetRealMetrics(dm);
            return (dm.WidthPixels, dm.HeightPixels);
        }
    }
    
    protected override void LoadContent()
    {
        base.LoadContent();
    }
    
    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
    
}