namespace Battleships.Client
{
    using System;

    public class GameMain
    {
        public static void Main()
        {
            var gameEngine = new GameEngine();

            gameEngine.Run();
        }
    }
}
