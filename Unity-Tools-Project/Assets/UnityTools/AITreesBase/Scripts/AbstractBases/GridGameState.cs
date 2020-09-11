using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Text;

namespace UnityTools.AITreesBase
{
    using Interfaces;

    public abstract class GridGameState : AbstractGameState
    {
        #region Variables
        private readonly AbstractActor[,] _grid;
        public AbstractActor[,] Grid
        {
            get
            {
                return _grid;
            }
        }
        public int GridWidth => _grid.GetLength(0);
        public int GridHeight => _grid.GetLength(1);

        public override string ASCIIRepresentation
        {
            get
            {
                StringBuilder s = new StringBuilder();

                for (int y = _grid.GetLength(1) - 1; y >= 0; --y)
                {
                    for (int x = 0; x < _grid.GetLength(0); ++x)
                    {
                        s.Append($"[{(_grid[x, y] == null ? "?" : _grid[x, y].Name)}]");
                    }

                    s.AppendLine();
                }

                return s.ToString();
            }
        }
        #endregion

        #region Constructors
        public GridGameState(string aPlayerA, string aPlayerB, IActor aActorJustActed, int aGridWidth, int aGridHeight) : base(aPlayerA, aPlayerB, aActorJustActed)
        {
            _grid = new AbstractActor[aGridWidth, aGridHeight];
        }

        public GridGameState(string aPlayerA, string aPlayerB, IActor aActorJustActed, AbstractActor[,] aGrid) : base(aPlayerA, aPlayerB, aActorJustActed)
        {
            Assert.IsNotNull(aGrid);

            _grid = aGrid;
        }
        #endregion

        #region
        #endregion
    }
}
