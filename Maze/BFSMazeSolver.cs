using System;
using System.Collections.Generic;

namespace Maze
{
    /// <summary>
    /// Solves a walled maze puzzle using Breadth First Algorithm.
    /// </summary>
    class BFSWalledMazeSolver : IMazeSolver
    {
        //BFS'es internal node states. Every solver can have its own state.
        enum BFSNodeState
        {
            NotVisited = 0, //White
            Visited = 1, //Black
            Queued = 2, //Gray
        }

        /// <summary>
        /// BFS'es internal node representation. Every solver can have its own representation.
        /// </summary>
        class BFSNode
        {
            public int RowPosition { get; set; }
            public int ColPosition { get; set; }
            //Internal state of the BFS node.
            public BFSNodeState State
            {
                get;
                set;
            }
            //Distance from the source node.
            public int Distance
            {
                get;
                set;
            }
            //Pointer to the previous node.
            public BFSNode Predecessor
            {
                get;
                set;
            }

            public BFSNode(int row, int col)
            {
                RowPosition = row;
                ColPosition = col;
            }
        }

        BFSNode[,] _bfsNodes = null;

        /// <summary>
        /// Implementatin of the <see cref="IMazeSolver"/>'s Solve() method. Uses the Breadth Fist Search Algorithm that tracks predecessors and distance from source.
        /// </summary>
        public void Solve(IWalledMaze maze, Action<IEnumerable<IWalledMazeNode>> solvedResultCallback)
        {
            InitializeSolver(maze);
            Queue<BFSNode> frontierQueue = new Queue<BFSNode>();//Queue that maintains the frontier to the explored.
            BFSNode startNode = GetBFSNode(maze.Start);//Start of the maze as defined by IWalledMaze.
            startNode.Distance = 0;
            startNode.Predecessor = null;
            startNode.State = BFSNodeState.Queued;//In BFS this is marked by the color GREY
            frontierQueue.Enqueue(startNode);

            while (frontierQueue.Count > 0)
            {
                BFSNode curBFSNode = frontierQueue.Dequeue();
                IWalledMazeNode curMazeNode = GetMazeNode(maze, curBFSNode);
                if (maze.IsGoal(curMazeNode))//Uses the goal defined by the IWalledMaze as terminating point.
                {
                    IEnumerable<IWalledMazeNode> solvedPath = TraceSolvedPath(maze, curBFSNode);
                    solvedResultCallback(solvedPath);//Calls the callback Action and returns.
                    return;
                }
                foreach (IWalledMazeNode adjMazeNode in maze.GetAdjacentNodes(curMazeNode))
                {
                    //Just use the x & Y positions from the adjNode and use the internal representation to do comparision.
                    BFSNode adjBFSNode = GetBFSNode(adjMazeNode);
                    if (adjBFSNode.State == BFSNodeState.NotVisited)
                    {
                        adjBFSNode.State = BFSNodeState.Queued;
                        adjBFSNode.Predecessor = curBFSNode;
                        adjBFSNode.Distance = curBFSNode.Distance + 1;
                        frontierQueue.Enqueue(adjBFSNode);
                    }
                }
                curBFSNode.State = BFSNodeState.Visited;//In BFS this is marked by the color BLACK
            }

            solvedResultCallback(null); //if it comes this far then no solution found.
        }

        /// <summary>
        /// Conversion function. Converts a maze node to a internal BFS node.
        /// </summary>
        private BFSNode GetBFSNode(IWalledMazeNode mazeNode)
        {
            return _bfsNodes[mazeNode.RowPosition, mazeNode.ColPosition]; //Both BFS and Maze node have positional relationship.
        }

        /// <summary>
        /// Conversion function. Converts a BFS node to a maze node.
        /// </summary>
        private IWalledMazeNode GetMazeNode(IWalledMaze maze, BFSNode bfsNode)
        {
            return maze.GetNode(bfsNode.RowPosition, bfsNode.ColPosition);//Both BFS and Maze node have positional relationship.
        }

        /// <summary>
        /// Scans all maze's nodes and converts into an internal solver's Node <see cref="BFSNode"/>.
        /// </summary>
        private void InitializeSolver(IWalledMaze maze)
        {
            _bfsNodes = new BFSNode[maze.Height, maze.Width];
            IEnumerator<IWalledMazeNode> mazeNodes = maze.GetNodes();
            while (mazeNodes.MoveNext())
            {
                IWalledMazeNode mazeNode = mazeNodes.Current;
                if (mazeNode.State == WalledMazeNodeState.Blocked)//Blocked cells are walls. This is internally represented as "Visited" nodes.
                    _bfsNodes[mazeNode.RowPosition, mazeNode.ColPosition] = new BFSNode(mazeNode.RowPosition, mazeNode.ColPosition) { State = BFSNodeState.Visited, Distance = int.MaxValue };
                else if (mazeNode.State == WalledMazeNodeState.Open)//Other cells are open. These nodes are represented as "Not-Visited" nodes.
                    _bfsNodes[mazeNode.RowPosition, mazeNode.ColPosition] = new BFSNode(mazeNode.RowPosition, mazeNode.ColPosition) { State = BFSNodeState.NotVisited, Distance = int.MaxValue };
            }
        }
        /// <summary>
        /// Traces the solved path and builds the internal BFS tree.
        /// </summary>
        private IEnumerable<IWalledMazeNode> TraceSolvedPath(IWalledMaze maze, BFSNode endNode)
        {
            BFSNode curNode = endNode;
            ICollection<IWalledMazeNode> pathTrace = new List<IWalledMazeNode>();
            while (curNode != null)
            {
                pathTrace.Add(GetMazeNode(maze, curNode));
                curNode = curNode.Predecessor;
            }
            return pathTrace;
        }
    }
}
