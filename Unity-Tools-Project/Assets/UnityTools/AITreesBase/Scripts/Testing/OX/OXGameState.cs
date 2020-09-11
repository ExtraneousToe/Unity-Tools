using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UnityTools.AITreesBase.Testing.OX
{
    using UnityTools.AITreesBase.Interfaces;
    using UnityTools.AITreesBase.Enums;

    public class OXGameState : GridGameState
    {
        #region Variables
        public const int GRID_SIZE = 3;

        public override string ASCIIRepresentation
        {
            get
            {
                string s = "";
                for (int y = GridHeight - 1; y >= 0; --y)
                {
                    for (int x = 0; x < GridWidth; x++)
                    {
                        if (Grid[x, y] == null)
                            s += "#";
                        else
                            s += Grid[x, y].Name;
                    }
                    s += "\n";
                }

                return s;
            }
        }
        #endregion

        #region Constructors
        public OXGameState(string aPlayerA, string aPlayerB, OXActor aJustActed) : base(aPlayerA, aPlayerB, aJustActed, GRID_SIZE, GRID_SIZE)
        {
            for (int x = 0; x < GridWidth; ++x)
            {
                for (int y = 0; y < GridHeight; ++y)
                {
                    Grid[x, y] = null;
                }
            }
        }

        public OXGameState(string aPlayerA, string aPlayerB, OXActor aJustActed, AbstractActor[,] aGrid) : base(aPlayerA, aPlayerB, aJustActed, aGrid)
        {
        }

        public OXGameState(OXGameState aOtherGameState, OXActor aJustActed, AbstractActor[,] aGrid) : base(aOtherGameState.PlayerAName, aOtherGameState.PlayerBName, aJustActed, aGrid)
        {
        }
        #endregion

        #region IGameState
        public override IEnumerable<IAction> GetAllMoves()
        {
            List<IAction> actions = new List<IAction>();

            OXActor nowActing = new OXActor(ActorJustActed.Name.Equals(PlayerAName) ? PlayerBName : PlayerAName);

            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    if (Grid[x, y] == null)
                    {
                        IAction newAction = new OXAction(nowActing, this, x, y);
                        actions.Add(newAction);
                    }
                }
            }

            return actions;
        }

        public override bool IsTerminal()
        {
            if (GetAllMoves().Count() == 0)
                return true;

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

        public override EEndGameStatus GetResult(IActor player)
        {
            for (int i = 0; i < GridWidth; i++)
            {
                if (Grid[i, 0] != null &&
                    Grid[i, 0].Equals(Grid[i, 1]) &&
                    Grid[i, 0].Equals(Grid[i, 2]))
                {
                    if (Grid[i, 0].Equals(player))
                        return EEndGameStatus.Win;
                    else
                        return EEndGameStatus.Loss;
                }

                if (Grid[0, i] != null &&
                    Grid[0, i].Equals(Grid[1, i]) &&
                    Grid[0, i].Equals(Grid[2, i]))
                {
                    if (Grid[0, i].Equals(player))
                        return EEndGameStatus.Win;
                    else
                        return EEndGameStatus.Loss;
                }
            }

            // check diagonals
            if (Grid[1, 1] != null &&
                ((Grid[1, 1].Equals(Grid[0, 0]) &&
                Grid[1, 1].Equals(Grid[2, 2])) ||
                (Grid[1, 1].Equals(Grid[0, 2]) &&
                Grid[1, 1].Equals(Grid[2, 0]))))
            {
                if (Grid[1, 1].Equals(player))
                    return EEndGameStatus.Win;
                else
                    return EEndGameStatus.Loss;
            }

            return EEndGameStatus.Draw;
        }
        #endregion

        #region object
        public override object Clone()
        {
            return new OXGameState(
                this,
                this.ActorJustActed.Clone() as OXActor,
                this.Grid.Clone() as AbstractActor[,]
            );
        }
        #endregion
    }
}