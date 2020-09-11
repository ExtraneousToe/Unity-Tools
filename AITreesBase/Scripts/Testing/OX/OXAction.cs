using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AITreesBase.Testing.OX
{
    using UnityTools.AITreesBase.Interfaces;
    using UnityTools.AITreesBase.Enums;

    public class OXAction : AbstractAction
    {
        #region Variables
        private int _x, _y;
        private OXGameState _incomingState;

        public override string ASCIIRepresentation
        {
            get
            {
                return string.Format("{2} -> x[{0}] y[{1}]", _x, _y, ActorActing.Name);
            }
        }
        #endregion

        #region Constructor
        public OXAction(OXActor aActing, OXGameState aState, int x, int y) : base(aActing)
        {
            this._incomingState = aState;
            this._x = x;
            this._y = y;
        }
        #endregion

        #region
        public override IGameState DoAction()
        {
            AbstractActor[,] newGrid = _incomingState.Grid.Clone() as AbstractActor[,];
            for (int i = 0; i < _incomingState.GridWidth; i++)
            {
                for (int j = 0; j < _incomingState.GridHeight; j++)
                {
                    newGrid[i, j] = _incomingState.Grid[i, j];
                }
            }

            newGrid[_x, _y] = ActorActing as AbstractActor;

            return new OXGameState(
                _incomingState,
                ActorActing as OXActor,
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