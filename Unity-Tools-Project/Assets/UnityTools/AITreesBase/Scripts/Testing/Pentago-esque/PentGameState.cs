using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityTools.AITreesBase.Testing.Pentagoesque
{
    using UnityTools.AITreesBase.Enums;
    using UnityTools.AITreesBase.Interfaces;

    public class PentGameState : GridGameState
    {
        #region Variables
        /// <summary>
        /// y:6
        /// 
        /// 5: [ ][ ][ ]|[ ][ ][ ]
        /// 4: [ ][2][ ]|[ ][3][ ]
        /// 3: [ ][ ][ ]|[ ][ ][ ]
        ///    -------------------
        /// 2: [ ][ ][ ]|[ ][ ][ ]
        /// 1: [ ][0][ ]|[ ][1][ ]
        /// 0: [ ][ ][ ]|[ ][ ][ ]
        ///     0  1  2   3  4  5  x:6
        /// </summary>

        public const int GRID_SIZE = 6;
        public const int WIN_SEQUENCE_LENGTH = 5;

        private readonly int[,] _blockCenterIndicies = new int[,]
        {
            { 1,1 },
            { 4,1 },
            { 1,4 },
            { 4,4 },
        };
        public int[,] BlockCenterIndicies => _blockCenterIndicies;
        #endregion

        #region Constructors
        public PentGameState(string aPlayerA, string aPlayerB, PentActor aJustActed) : base(aPlayerA, aPlayerB, aJustActed, GRID_SIZE, GRID_SIZE)
        {
            for (int x = 0; x < GridWidth; ++x)
            {
                for (int y = 0; y < GridHeight; ++y)
                {
                    Grid[x, y] = null;
                }
            }
        }

        public PentGameState(string aPlayerA, string aPlayerB, PentActor aJustActed, AbstractActor[,] aGrid) : base(aPlayerA, aPlayerB, aJustActed, aGrid)
        {
        }

        public PentGameState(PentGameState aOtherGameState, PentActor aJustActed, AbstractActor[,] aGrid) : base(aOtherGameState.PlayerAName, aOtherGameState.PlayerBName, aJustActed, aGrid)
        {
        }
        #endregion

        #region IGameState
        public override IEnumerable<IAction> GetAllMoves()
        {
            List<IAction> actions = new List<IAction>();

            PentActor nowActing = new PentActor(ActorJustActed.Name.Equals(PlayerAName) ? PlayerBName : PlayerAName);

            //int x = 1, y = 0;
            //if (Grid[x, y] == null)
            //{
            //    actions.Add(new PentAction(nowActing, this, x, y, 0, -1));
            //}

            for (int x = 0; x < GridWidth; ++x)
            {
                for (int y = 0; y < GridHeight; ++y)
                {
                    if (Grid[x, y] == null)
                    {
                        bool addedNoRotateAction = false;
                        // check for rotations
                        for (int i = 0; i < BlockCenterIndicies.GetLength(0); ++i)
                        {
                            if (IsBlockNeutral(i))
                            {
                                if (!addedNoRotateAction)
                                {
                                    addedNoRotateAction = true;

                                    actions.Add(new PentAction(nowActing, this, x, y, -1, 0));
                                }
                            }
                            else
                            {
                                // rotate -90 degrees
                                actions.Add(new PentAction(nowActing, this, x, y, i, -1));
                                // rotate 90 degrees
                                actions.Add(new PentAction(nowActing, this, x, y, i, 1));
                            }
                        }
                    }
                }
            }

            return actions;
        }

        public override bool IsTerminal()
        {
            if (GetAllMoves().Count() == 0)
            {
                return true;
            }

            // also terminal if someone has won
            switch (GetResult(ActorJustActed))
            {
                case EEndGameStatus.Win:
                case EEndGameStatus.Loss:
                    return true;
                case EEndGameStatus.Draw:
                    return false;
            }

            return false;
        }

        private bool ProcessSequence(int x, int y, IActor actedPlayer, ref PentActor lastTrackedPlayer, ref int sequenceLength, out EEndGameStatus result)
        {
            result = EEndGameStatus.Draw;
            PentActor gridActor = Grid[x, y] as PentActor;

            if (gridActor == null)
            {
                lastTrackedPlayer = null;
                sequenceLength = 0;
                return false;
            }

            if (lastTrackedPlayer == null)
            {
                lastTrackedPlayer = gridActor;
                sequenceLength = 1;
            }
            else if (lastTrackedPlayer != gridActor)
            {
                lastTrackedPlayer = gridActor;
                sequenceLength = 1;
            }
            else // lastTrackedPlayer == gridActor
            {
                if (++sequenceLength >= WIN_SEQUENCE_LENGTH)
                {
                    result = lastTrackedPlayer == (PentActor)actedPlayer ? EEndGameStatus.Win : EEndGameStatus.Loss;
                    return true;
                }
            }

            return false;
        }

        public override EEndGameStatus GetResult(IActor player)
        {
            // check verticals for victory
            for (int x = 0; x < GridWidth; ++x)
            {
                int sequenceLength = 0;
                PentActor lastTrackedPlayer = null;
                for (int y = 0; y < GridHeight; ++y)
                {
                    if (ProcessSequence(x, y, player, ref lastTrackedPlayer, ref sequenceLength, out EEndGameStatus result))
                    {
                        return result;
                    }
                }
            }

            // check horizontals for victory
            for (int y = 0; y < GridHeight; ++y)
            {
                int sequenceLength = 0;
                PentActor lastTrackedPlayer = null;
                for (int x = 0; x < GridWidth; ++x)
                {
                    if (ProcessSequence(x, y, player, ref lastTrackedPlayer, ref sequenceLength, out EEndGameStatus result))
                    {
                        return result;
                    }
                }
            }

            // check diagonals for victory
            for (int x = 0; x < GridWidth; ++x)
            {
                for (int y = 0; y < GridHeight; ++y)
                {
                    int sequenceURLength = 0;
                    PentActor lastTrackedPlayerUR = null;

                    int sequenceULLength = 0;
                    PentActor lastTrackedPlayerUL = null;

                    for (int i = 0; i < WIN_SEQUENCE_LENGTH; i++)
                    {
                        try
                        {
                            // diagonally up-right
                            if (ProcessSequence(x + i, y + i, player, ref lastTrackedPlayerUR, ref sequenceURLength, out EEndGameStatus urResult))
                            {
                                return urResult;
                            }
                        }
                        catch (System.IndexOutOfRangeException e)
                        {
                            // out of range, break
                            break;
                        }

                        try
                        {
                            // diagonally up-left
                            if (ProcessSequence(x - i, y + i, player, ref lastTrackedPlayerUL, ref sequenceULLength, out EEndGameStatus ulResult))
                            {
                                return ulResult;
                            }
                        }
                        catch (System.IndexOutOfRangeException e)
                        {
                            // out of range, break
                            break;
                        }
                    }
                }
            }

            return EEndGameStatus.Draw;
        }

        public bool IsBlockNeutral(int aIndex)
        {
            if (aIndex < 0 || BlockCenterIndicies.GetLength(0) < aIndex)
            {
                throw new System.Exception("Index error.");
            }

            int xInd = BlockCenterIndicies[aIndex, 0];
            int yInd = BlockCenterIndicies[aIndex, 1];

            bool allAreEmpty = true;
            for (int x = -1; x <= 1 && allAreEmpty; ++x)
            {
                for (int y = -1; y <= 1 && allAreEmpty; ++y)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    allAreEmpty &= Grid[xInd + x, yInd + y] == null;
                }
            }

            return allAreEmpty;
        }
        #endregion

        #region object
        public override object Clone()
        {
            return new PentGameState(
                this,
                this.ActorJustActed.Clone() as PentActor,
                this.Grid.Clone() as AbstractActor[,]
            );
        }
        #endregion
    }
}
