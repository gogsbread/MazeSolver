using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    /// <summary>
    /// Represents a interface for a walled maze. Any solver should be able to use the interfaces provided to solve the maze puzzle.
    /// </summary>
    public interface IWalledMaze
    {
        int Height { get; }
        int Width { get; }
        IWalledMazeNode Start { get; } //Indicates the start point of the maze
        IWalledMazeNode Finish { get; } //Indicate the end point of the maze
        bool IsGoal(IWalledMazeNode curNode); //Used by a <see cref="IMazeSolver"/> to determine if it has arrived at a destination.
        void Solve(IMazeSolver solver, Action<IEnumerable<IWalledMazeNode>> solvedResultCallback); //Used by the client to solve the maze.
        IEnumerable<IWalledMazeNode> GetAdjacentNodes(IWalledMazeNode curNode); //Lists all adjacent nodes for a given node. Used by the solver to find neighboring nodes during scan.
        IWalledMazeNode GetNode(int row, int col); //Gets a specific node given the position.
        IEnumerator<IWalledMazeNode> GetNodes();//Gets all nodes for the maze.
    }
}
