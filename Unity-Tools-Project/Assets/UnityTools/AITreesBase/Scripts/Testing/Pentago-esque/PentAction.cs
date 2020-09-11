using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AITreesBase.Testing.Pentagoesque
{
    using UnityTools.AITreesBase.Interfaces;
    using UnityTools.AITreesBase.Enums;

    public class PentAction : AbstractAction
    {
        #region Variables
        private int _x, _y;
        private int _rotBlock, _rotBlockDir;
        private PentGameState _incomingState;

        public override string ASCIIRepresentation
        {
            get
            {
                return $"{ActorActing.Name} -> x[{_x}] y[{_y}]\nRot: [{_rotBlock}] Dir: [{_rotBlockDir}]";
            }
        }
        #endregion

        #region Constructor
        public PentAction(PentActor aActing, PentGameState aState, int x, int y, int aRotateBlockIndex, int aRotateBlockDirection) : base(aActing)
        {
            this._incomingState = aState;
            this._x = x;
            this._y = y;
            this._rotBlock = aRotateBlockIndex;
            this._rotBlockDir = aRotateBlockDirection;
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

            // place piece
            newGrid[_x, _y] = ActorActing as AbstractActor;

            // rotate block
            if (_rotBlock >= 0)
            {
                int centerX = _incomingState.BlockCenterIndicies[_rotBlock, 0];
                int centerY = _incomingState.BlockCenterIndicies[_rotBlock, 1];

                AbstractActor temp = null;
                int cornerNEX = centerX + 1;
                int cornerNEY = centerY + 1;

                int cornerNWX = centerX - 1;
                int cornerNWY = centerY + 1;

                int cornerSEX = centerX + 1;
                int cornerSEY = centerY - 1;

                int cornerSWX = centerX - 1;
                int cornerSWY = centerY - 1;

                int edgeNX = centerX;
                int edgeNY = centerY + 1;

                int edgeEX = centerX + 1;
                int edgeEY = centerY;

                int edgeSX = centerX;
                int edgeSY = centerY - 1;

                int edgeWX = centerX - 1;
                int edgeWY = centerY;

                if (_rotBlockDir == 1)
                {
                    // corners
                    // store NE as tNE
                    temp = newGrid[cornerNEX, cornerNEY];

                    // NE<-NW
                    newGrid[cornerNEX, cornerNEY] = newGrid[cornerNWX, cornerNWY];

                    // NW<-SW
                    newGrid[cornerNWX, cornerNWY] = newGrid[cornerSWX, cornerSWY];

                    // SW<-SE
                    newGrid[cornerSWX, cornerSWY] = newGrid[cornerSEX, cornerSEY];

                    // SE<-tNE
                    newGrid[cornerSEX, cornerSEY] = temp;

                    // edges
                    // store N as tN
                    temp = newGrid[edgeNX, edgeNY];

                    // N<-W
                    newGrid[edgeNX, edgeNY] = newGrid[edgeWX, edgeWY];

                    // W<-S
                    newGrid[edgeWX, edgeWY] = newGrid[edgeSX, edgeSY];

                    // S<-E
                    newGrid[edgeSX, edgeSY] = newGrid[edgeEX, edgeEY];

                    // E<-tN
                    newGrid[edgeEX, edgeEY] = temp;
                }
                else // _rotBlockDir == 1
                {
                    // corners
                    // store NE as tNE
                    temp = newGrid[cornerNEX, cornerNEY];

                    // NE<-SE
                    newGrid[cornerNEX, cornerNEY] = newGrid[cornerSEX, cornerSEY];

                    // SE<-SW
                    newGrid[cornerSEX, cornerSEY] = newGrid[cornerSWX, cornerSWY];

                    // SW<-NW
                    newGrid[cornerSWX, cornerSWY] = newGrid[cornerNWX, cornerNWY];

                    // NW<-tNE
                    newGrid[cornerNWX, cornerNWY] = temp;

                    // edges
                    // store N as tN
                    temp = newGrid[edgeNX, edgeNY];

                    // N<-E
                    newGrid[edgeNX, edgeNY] = newGrid[edgeEX, edgeEY];

                    // E<-S
                    newGrid[edgeEX, edgeEY] = newGrid[edgeSX, edgeSY];

                    // S<-W
                    newGrid[edgeSX, edgeSY] = newGrid[edgeWX, edgeWY];

                    // W<-tN
                    newGrid[edgeWX, edgeWY] = temp;
                }
            }

            return new PentGameState(
                _incomingState,
                ActorActing as PentActor,
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
