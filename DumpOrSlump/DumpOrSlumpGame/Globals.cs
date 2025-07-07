using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DumpOrSlumpGame
{
    /// <summary>
    /// Centralized container that exposes the handful of MonoGame objects and utilities shared across the entire game.
    /// The static fields make it quick to grab frequently used services—such as SpriteBatch instances, the ContentManager,
    /// or a random‑number generator - without having to thread references through every constructor
    /// </summary>
    class Globals
    {
        public static SpriteBatch spriteBatch;
        public static SpriteBatch spriteBatchUI;
        public static ContentManager content;
        
        // 1×1 white texture handy for drawing debug lines and rectangles
        public static Texture2D LineTexture;
        public static GraphicsDeviceManager _Graphics;
        
        // Shared random‑number generator for non‑deterministic needs
        public static Random rand = new Random();
    }
}
