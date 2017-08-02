using System.Collections.Generic;

namespace TurtleChallengeCSharp
{
    public class GameSettings
    {
        public BoardSize BoardSize { get; set; }
        public StartingPoint StartingPoint { get; set; }
        public ExitPoint ExitPoint { get; set; }
        public List<Mine> Mines { get; set; }

    }

    public class BoardSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    public class StartingPoint
    {
        public Position Position { get; set; }
        public string Direction { get; set; }//todo enum
    }

    public class ExitPoint
    {
        public Position Position { get; set; } 

    }

    public class Mine
    {
        public Position Position { get; set; }
    }

}
