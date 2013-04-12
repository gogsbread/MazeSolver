using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    /// <summary>
    /// Valid states for a walled maze.
    /// </summary>
    public enum WalledMazeNodeState : int
    {
        Open = 0,
        Blocked = 1
    }

    /// <summary>
    /// Implementaion of <see cref="IWalledMazeNode"/>.
    /// 
    /// </summary>
    public sealed class WalledMazeNode : IWalledMazeNode
    {
        const int DEFAULT_POSITION = 0;
        int _rowPosition, _colPosition = DEFAULT_POSITION;
        WalledMazeNodeState _state = WalledMazeNodeState.Open;

        public int RowPosition
        {
            get { return _rowPosition; }
        }

        public int ColPosition
        {
            get { return _colPosition; }
        }

        public WalledMazeNodeState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        public WalledMazeNode(int row, int col)
        {
            _rowPosition = row;
            _colPosition = col;
        }

        public override bool Equals(object obj)
        {
            WalledMazeNode mazeNode = obj as WalledMazeNode;
            if (mazeNode == null)
                return false;
            if(object.ReferenceEquals(mazeNode,this))
                return true;
            return (mazeNode.ColPosition == this.ColPosition && mazeNode.RowPosition == this.RowPosition && mazeNode.State == this.State);//all necessary informations are considered for equality
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + ColPosition + RowPosition + (int)State;//changed according to the implementation of Equals().
        }
    }
}
