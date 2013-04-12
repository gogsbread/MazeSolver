using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    /// <summary>
    /// Represents a interface for solver.
    /// </summary>
    public interface IMazeSolver
    {
        void Solve(IWalledMaze maze, Action<IEnumerable<IWalledMazeNode>> solvedResultCallback);
    }
}
