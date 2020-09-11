using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UnityTools.AITreesBase.Testing.C4
{
    using UnityTools.AITreesBase.Interfaces;
    using UnityTools.AITreesBase.Enums;

    public class C4GameState : GridGameState
    {
        #region Variables
        /// <summary>
        /// y:6
        /// 
        /// 5: [ ][ ][ ][ ][ ][ ][ ]
        /// 4: [ ][ ][ ][ ][ ][ ][ ]
        /// 3: [ ][ ][ ][ ][ ][ ][ ]
        /// 2: [ ][ ][ ][ ][ ][ ][ ]
        /// 1: [ ][ ][ ][ ][ ][ ][ ]
        /// 0: [ ][ ][ ][ ][ ][ ][ ]
        ///     0  1  2  3  4  5  6  x:7
        /// </summary>

        public const int GRID_WIDTH = 7;
        public const int GRID_HEIGHT = 6;
        public const int WIN_SEQUENCE_LENGTH = 4;
        #endregion

        #region Constructors
        public C4GameState(string aPlayerA, string aPlayerB, C4Actor aJustActed) : base(aPlayerA, aPlayerB, aJustActed, GRID_WIDTH, GRID_HEIGHT)
        {
            for (int x = 0; x < GridWidth; ++x)
            {
                for (int y = 0; y < GridHeight; ++y)
                {
                    Grid[x, y] = null;
                }
            }
        }

        public C4GameState(string aPlayerA, string aPlayerB, C4Actor aJustActed, AbstractActor[,] aGrid) : base(aPlayerA, aPlayerB, aJustActed, aGrid)
        {
        }

        public C4GameState(C4GameState aOtherGameState, C4Actor aJustActed, AbstractActor[,] aGrid) : base(aOtherGameState.PlayerAName, aOtherGameState.PlayerBName, aJustActed, aGrid)
        {
        }
        #endregion

        #region IGameState
        public override IEnumerable<IAction> GetAllMoves()
        {
            List<IAction> actions = new List<IAction>();

            C4Actor nowActing = new C4Actor(ActorJustActed.Name.Equals(PlayerAName) ? PlayerBName : PlayerAName);

            for (int x = 0; x < GridWidth; ++x)
            {
                for (int y = 0; y < GridHeight; ++y)
                {
                    if (Grid[x, y] == null)
                    {
                        IAction newAction = new C4Action(nowActing, this, x, y);
                        actions.Add(newAction);
                        // only one action is possible per column
                        break;
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

        private bool ProcessSequence(int x, int y, IActor actedPlayer, ref C4Actor lastTrackedPlayer, ref int sequenceLength, out EEndGameStatus result)
        {
            result = EEndGameStatus.Draw;
            C4Actor gridActor = Grid[x, y] as C4Actor;

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
                    result = lastTrackedPlayer == (C4Actor)actedPlayer ? EEndGameStatus.Win : EEndGameStatus.Loss;
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
                C4Actor lastTrackedPlayer = null;
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
                C4Actor lastTrackedPlayer = null;
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
                    C4Actor lastTrackedPlayerUR = null;

                    int sequenceULLength = 0;
                    C4Actor lastTrackedPlayerUL = null;

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
        #endregion

        #region object
        public override object Clone()
        {
            return new C4GameState(
                this,
                this.ActorJustActed.Clone() as C4Actor,
                this.Grid.Clone() as AbstractActor[,]
            );
        }
        #endregion
    }
}