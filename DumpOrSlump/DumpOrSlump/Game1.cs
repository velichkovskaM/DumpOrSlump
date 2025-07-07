using Android.Graphics;
using Android.OS;
using Android.Util;
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
    
    // Initialize ─ query device bounds before base init
    protected override void Initialize()
    {
        var windowManager = Game.Activity.WindowManager;
        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
        {
            Rect bounds = windowManager.CurrentWindowMetrics.Bounds;
            fullDimensions = (bounds.Width(), bounds.Height());
        } else {
            DisplayMetrics metrics = new DisplayMetrics();
            windowManager.DefaultDisplay.GetRealMetrics(metrics);
            int deviceWidth = metrics.WidthPixels;
            int deviceHeight = metrics.HeightPixels;

            fullDimensions = (deviceWidth, deviceHeight);
        }

        Logger.Error($"Full dimensions in the constructor: {fullDimensions.width}x{fullDimensions.height}");
        base.Initialize();
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