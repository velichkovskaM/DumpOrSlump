using Microsoft.Xna.Framework;
namespace GameEngine;

public class Game1 : Game
{
    public static Game1 Instance { get; private set; } = null!;
}