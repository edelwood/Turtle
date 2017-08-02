using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Reflection;
using System.Xml.Serialization;

namespace TurtleChallengeCSharp
{
    class TurtleChallenge
    {
        private static readonly System.Collections.Specialized.NameValueCollection appSettingsCollection = ConfigurationManager.AppSettings;

        public static void Main()
        {
            /*
            * Assumption:
            * If the exit is reached, no more moves are executed
            */
            var gameSettings = GetGameSettings();

            //Populate Matrix
            //default= 0, mine = 1, exit = 2
            int[,] matrix = new int[gameSettings.BoardSize.Height, gameSettings.BoardSize.Width];
            matrix = SetMineExit(matrix, gameSettings.Mines, gameSettings.ExitPoint.Position);

            //StartMoves
            ExecuteMoves(matrix, gameSettings.StartingPoint.Position, gameSettings.StartingPoint.Direction);

            ExitGame();
        }

        #region private methods

        private static void ExitGame()
        {
            Console.Write("Game complete, press any key to exit, ");
            var anyKey = Console.ReadKey();

            Environment.Exit(0);
        }
        private static void ExecuteMoves(int[,] matrix, Position startPoint,string direction)
        {
            //ReadMovesFile
            var moves = ReadMovesFile();
            var resultMessage = "";

            Console.WriteLine("Land on start point");
            var newX = startPoint.X;
            var newY = startPoint.Y;
            if (IsExitReached(matrix[newY, newX], out resultMessage))
            {           
                Console.WriteLine(resultMessage);
                return;
            }

            var newDirection = direction;
            var totalNumberOfElements = matrix.Length;

            foreach (var move in moves)
            {
                if (move == "m")
                {
                    newX = GetNewX(newX, newDirection);
                    newY = GetNewY(newY, newDirection);

                    var numnerOfElements = (newX + 1) * (newY + 1);
                    if (numnerOfElements < 0 || numnerOfElements > totalNumberOfElements)
                    {
                        Console.WriteLine("Matrix boundaries exceeded");
                        throw new Exception("Matrix boundaries exceeded");
                    }

                    if (IsExitReached(matrix[newY, newX], out resultMessage))
                    {
                        Console.WriteLine(resultMessage);
                        return;
                    }
                    Console.WriteLine(resultMessage);
                }
                else if (move == "r")
                {
                    newDirection = GetNextDirection(newDirection);
                    Console.WriteLine("Change direction, now facing "+ newDirection);
                }
                else
                {
                    Console.Write("Invalid string in moves file, "+ move);
                    throw new Exception("Invalid string in moves file, " + move);
                }
            }            
        }

        private static int GetNewY(int currentY, string direction)
        {

            switch (direction)
            {
                case Directions.North:
                    currentY = currentY - 1;
                    break;
                case Directions.East:
                    break;
                case Directions.South:
                    currentY = currentY + 1; ;
                    break;
                case Directions.West:
                    break;
            }
            return currentY;
        }

        private static int GetNewX(int currentX, string direction)
        {

            switch (direction)
            {
                case Directions.North:
                    
                    break;
                case Directions.East:
                    currentX = currentX + 1;
                    break;
                case Directions.South:
                    break;
                case Directions.West:
                    currentX = currentX - 1;
                    break;
            }
            return currentX;
        }

        // 90 degrees to the right.
        private static string GetNextDirection(string direction)
        {
            var newDirection = direction;
            switch (direction)
            {
                case Directions.North:
                    newDirection = Directions.East;
                    break;
                case Directions.East:
                    newDirection = Directions.South;
                    break;
                case Directions.South:
                    newDirection = Directions.West;
                    break;
                case Directions.West:
                    newDirection = Directions.North;
                    break;
            }
            return newDirection;
        }

        private static bool IsExitReached(int matrixValue, out string moveResultMessage)
        {
            var isExitReached = false;
            moveResultMessage = string.Empty;
            switch (matrixValue)
            {
                case 0:
                {
                    moveResultMessage = "Successful move.";
                    break;
                }
                case 1:
                {
                    moveResultMessage = "Mine hit!!";
                    break;
                }
                case 2:
                {
                    isExitReached = true;
                    moveResultMessage = "Exit reached!";
                    break;
                }
            }
            return isExitReached;
        }

        private static int[,] SetMineExit(int[,] matrix, List<Mine> mines, Position exitPoint)
        {
            try
            {
                matrix[exitPoint.Y, exitPoint.X] = 2;
                foreach (var mine in mines)
                {
                    matrix[mine.Position.Y, mine.Position.X] = 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SetMineExit, matrix point does not exist. Check GameSettings points are valid.\n");
                Console.WriteLine(e);
                throw;
            }

            return matrix;
        }
        private static GameSettings GetGameSettings()
        {
            //Read GameSetting file
            using (var stream = File.OpenRead("GameSettings.xml"))
            {
                var serializer = new XmlSerializer(typeof(GameSettings));
                var gameSettings = (GameSettings)serializer.Deserialize(stream);
                return gameSettings;
            }
        }
        private static string[] ReadMovesFile()
        {
            string fileName = appSettingsCollection["MovesFileName"];
            string filePath = appSettingsCollection["MovesFilePath"];

            return  ReadFile(fileName, filePath);
        }
        private static string[]  ReadFile(string fileName, string filePath)
        {
            if(!File.Exists(filePath + fileName))
                filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)+"\\MoveFile\\";

            using (StreamReader sr = File.OpenText(filePath + fileName))
            {
                string newLine = String.Empty;
                while ((newLine = sr.ReadLine()) != null)
                {
                    var movesArray = newLine.Split(',');
                    Console.Write("ReadFile complete, Moves file contains "+ movesArray.Length+" moves.");
                    return movesArray;
                }
            }

            return null;
        }
        #endregion
    }
}
