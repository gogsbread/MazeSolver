using System;
using System.Collections.Generic;

namespace Maze
{
    /// <summary>
    /// Represents a node for the maze model.
    /// </summary>
    
    public interface IWalledMazeNode
    {
        int RowPosition { get; }//'y' coordinate
        int ColPosition { get; }//'x' coordinate
        WalledMazeNodeState State { get; set; } //State information.Valid states for a walled maze are OPEN and CLOSED <see cref="WalledMazeNodeState"/>
    }
}
