using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AITreesBase.Testing.C4
{
    using UnityTools.AITreesBase.Interfaces;
    using UnityTools.AITreesBase.Enums;

    public class C4Action : AbstractAction
    {
        #region Variables
        private int _x, _y;
        private C4GameState _incomingState;

        public override string ASCIIRepresentation
        {
            get
            {
                return string.Format("{2} -> x[{0}] y[{1}]", _x, _y, ActorActing.Name);
            }
        }
        #endregion

        #region Constructor
        public C4Action(C4Actor aActing, C4GameState aState, int x, int y) : base(aActing)
        {
            this._incomingState = aState;
            this._x = x;
            this._y = y;
        }
        #endregion

        #region
        public override IGameState DoAction()
        {
            AbstractActor[,] newGrid = new AbstractActor[_incomingState.GridWidth, _incomingState.GridHeight];
            for (int i = 0; i < _incomingState.GridWidth; i++)
            {
                for (int j = 0; j < _incomingState.GridHeight; j++)
                {
                    newGrid[i, j] = _incomingState.Grid[i, j];
                }
            }

            newGrid[_x, _y] = ActorActing as AbstractActor;

            return new C4GameState(
                _incomingState,
                ActorActing as C4Actor,
                newGrid
            );
        }

        public override string ToString()
        {
            return ASCIIRepresentation;
        }
        #endregion
    }
}
