using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Maze
{
    class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        public static void Main(string[] args)
        {
            if (args.Length != 2)//input from command line
            {
                Console.WriteLine("Invalid number of arguments");
                PrintUsage();
                return;
            }
            string imagePath = args[0];
            string outputImagePath = args[1];

            try
            {
                IWalledMaze mazeProblem = new WalledMaze(imagePath, HexCode.BLACK, HexCode.WHITE, HexCode.RED, HexCode.BLUE);//configures the maze.
                IMazeSolver bfsSolver = new BFSWalledMazeSolver();//intializes the solver. BFS in this case
                mazeProblem.Solve(bfsSolver, (solvedPath) => //callback defines action to perform after the maze is solved. Here the action is to save in "destination.png".
                {
                    Bitmap bitMap = new Bitmap(imagePath);
                    foreach (var node in solvedPath)
                    {
                        bitMap.SetPixel(node.ColPosition, node.RowPosition, Color.Green); //Could also use lockbits() over SetPixel() for faster processing.
                    }
                    bitMap.Save(outputImagePath);
                    Console.WriteLine();
                    Console.WriteLine("Completed:");
                    Console.WriteLine("=========");
                    Console.WriteLine("See '{0}' for solution", outputImagePath);
                    Console.WriteLine();
                });
            }
            catch (Exception e)//generic error handling. Specific errors are handled inside the main code.
            {
                Console.WriteLine();
                Console.WriteLine("ERROR:");
                Console.WriteLine("======");
                Console.WriteLine("{0}", e.Message);
            }
        }

        /// <summary>
        /// Prints the usage of the tool.
        /// </summary>

        static void PrintUsage()
        {
            Console.WriteLine("Usage");
            Console.WriteLine("=====");
            Console.WriteLine("maze source.[bmp,png,jpg] destination.[bmp,png,jpg]");
            Console.WriteLine();
        }
    }
}
