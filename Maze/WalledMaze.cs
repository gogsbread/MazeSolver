using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Maze
{
    /// <summary>
    /// Encapsulation for all the colors defined in this maze. Should ideally be using the <see cref="System.Drawing.KnownColor"/> but faced some issues during color identification during implementation.
    /// </summary>
    public struct HexCode
    {
        public const string BLACK = "ff000000";
        public const string WHITE = "ffffffff";
        public const string RED = "ffff0000";
        public const string BLUE = "ff0000ff";
        public const string GREEN = "ff00ff00";

        static public bool Contains(string color)
        {
            return (color == WHITE || color == BLACK || color == RED || color == BLUE || color == GREEN);
        }
    }

    /// <summary>
    /// Implementaion of <see cref="IWalledMaze"/>.
    /// Implements all necessary information for maze model
    /// </summary>
    class WalledMaze : IWalledMaze
    {
        WalledMazeNode _source = null;
        WalledMazeNode _destination = null;
        WalledMazeNode[,] _mazeMap = null;

        int _width = 0;
        int _height = 0;

        //Indicates the total width of the maze. Used by the solver during its initialization.
        public int Width
        {
            get { return _width; }
        }

        //Indicates the total height of the maze. Used by the solver during its initialization.
        public int Height
        {
            get { return _height; }
        }

        //Indicates the start point of the maze
        public IWalledMazeNode Start
        {
            get { return _source; }
        }

        //Indicate the end point of the maze
        public IWalledMazeNode Finish
        {
            get { return _destination; }
        }

        //Used by a <see cref="IMazeSolver"/> to determine if it has arrived at a destination.
        public bool IsGoal(IWalledMazeNode curNode)
        {
            if (curNode == null)
                return false;
            return curNode.Equals(_destination);//calls MazeNode's overridden Equals().
        }

        public WalledMaze(string imagePath, string wallColor, string openColor, string startColor, string finishColor)
        {
            if (!File.Exists(imagePath))
                throw new FileNotFoundException(string.Format("File {0} not found", imagePath), "imagePath");
            if (!HexCode.Contains(wallColor) || !HexCode.Contains(openColor) || !HexCode.Contains(startColor) || !HexCode.Contains(finishColor))
                throw new ArgumentException("Please check your color codes. One\\Many of them are wrong");//Color codes used by the maze should be defined in <see cref="HexCode"/>.
            InitializeMaze(imagePath, wallColor, openColor, startColor, finishColor);
        }

        /// <summary>
        /// Gets adjacents for a node. Any node can have at most 8 adjacents.
        /// </summary>
        public IEnumerable<IWalledMazeNode> GetAdjacentNodes(IWalledMazeNode curNode)
        {
            int rowPosition = curNode.RowPosition;
            int colPosition = curNode.ColPosition;

            if (rowPosition < 0 || rowPosition >= _height || colPosition < 0 || colPosition >= _width) //if given node is out of bounds return a empty list as adjacents.
                return new List<WalledMazeNode>(0);

            List<WalledMazeNode> adjacents = new List<WalledMazeNode>(8);
            for (int i = rowPosition - 1; i <= rowPosition + 1; i++)
            {
                for (int j = colPosition - 1; j <= colPosition + 1; j++)
                {
                    if (i < 0 || i >= _height || j < 0 || j >= _width || (i == rowPosition && j == colPosition))//eliminates out of bounds from being sent as adjacents.
                        continue;
                    adjacents.Add(_mazeMap[i, j]);
                }
            }
            return adjacents;
        }

        /// <summary>
        /// Gets a node.
        /// </summary>
        public IWalledMazeNode GetNode(int row, int col)
        {
            GuardPosition(row, col,"row\\col");
            return _mazeMap[row, col];
        }

        /// <summary>
        /// Gets all maze nodes.
        /// </summary>
        public IEnumerator<IWalledMazeNode> GetNodes()
        {
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    yield return _mazeMap[i, j];
        }

        /// <summary>
        /// Function that solves the maze using the solver.
        /// </summary>
        public void Solve(IMazeSolver solver, Action<IEnumerable<IWalledMazeNode>> solvedResultCallback)
        {
            if (solver == null)
                throw new ArgumentNullException("Solver cannot be null", "solver");
            if(solvedResultCallback == null)
                throw new ArgumentNullException("Please provide a callback action", "solvedResultCallback");
            //calls solver's solve method.
            solver.Solve(this, (solvedPath) =>
            {
                if (solvedPath == null)
                    solvedResultCallback(new List<IWalledMazeNode>(0));//return a empty path if the solver could not solve the maze.
                else
                    solvedResultCallback(solvedPath);
            });
        }

        /// <summary>
        /// Scans the image and stores each pixel information as <see cref="IWalledMazeNode"/>.
        /// </summary>
        private void InitializeMaze(string imagePath, string wallColor, string openColor, string startColor, string finishColor)
        {
            if (!File.Exists(imagePath))
                throw new FileNotFoundException(string.Format("{0} not found", imagePath));

            Bitmap image = null;
            try
            {
                image = new Bitmap(imagePath);
            }
            catch (ArgumentException)//Image is not a valid bitmap(.jpg,.png etc..). Framework exception message should be appropriate
            {
                throw;
            }

            _mazeMap = new WalledMazeNode[image.Height, image.Width];
            _height = image.Height;
            _width = image.Width;

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    Color c = image.GetPixel(j, i);//Could also use lockbits() over GetPixel() for faster processing.

                    if (c.Name == wallColor)//Black Color is a wall. Mark them as blocked
                    {
                        _mazeMap[i, j] = new WalledMazeNode(i, j) { State = WalledMazeNodeState.Blocked };
                        continue;
                    }
                    else if (c.Name == startColor)
                    {
                        _mazeMap[i, j] = new WalledMazeNode(i, j) { State = WalledMazeNodeState.Open };
                        if (_source == null)//pick the first red color as the source node.
                            _source = _mazeMap[i, j];
                    }
                    else if (c.Name == finishColor)
                    {
                        _mazeMap[i, j] = new WalledMazeNode(i, j) { State = WalledMazeNodeState.Open };
                        if (_destination == null)//pick the first blue color as the destination node.
                            _destination = _mazeMap[i, j];
                    }
                    else if (c.Name == openColor)//Open Color is mostly White.
                    {
                        _mazeMap[i, j] = new WalledMazeNode(i, j) { State = WalledMazeNodeState.Open };
                    }
                    else
                    {
                        //Invalid color.
                        //Eventhough anomalous colors could be defined as OPEN nodes, such implementations may confuse the solver when BLACK walls are defined with a hue. 
                        //I restricted such usage by throwing exceptions for unknown colors. Better safe than sorry. :)
                        throw new Exception(string.Format("I found a color {0} that this maze was not configured to handle. Please use colors that you originally specified.", c.Name));
                    }
                }
            }
        }

        # region GuardMethods
        public void GuardPosition(IWalledMazeNode node, string exceptionParamName)
        {
            int rowPosition = node.RowPosition;
            int colPosition = node.ColPosition;

            GuardPosition(rowPosition, colPosition, exceptionParamName);
        }

        private void GuardPosition(int rowPosition, int colPosition, string exceptionParamName)
        {
            if (rowPosition < 0 || rowPosition >= _height || colPosition < 0 || colPosition >= _width)
                throw new ArgumentException("The supplied node is out of bounds", exceptionParamName);
        }
        # endregion
    }
}
