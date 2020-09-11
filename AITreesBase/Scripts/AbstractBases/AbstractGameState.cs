using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace UnityTools.AITreesBase
{
    using Interfaces;
    using UnityTools.AITreesBase.Enums;

    public abstract class AbstractGameState : IGameState
    {
        #region Variables
        private readonly IActor _actorJustActed;
        public IActor ActorJustActed
        {
            get => _actorJustActed;
        }

        public abstract string ASCIIRepresentation { get; }

        public string PlayerAName { get; private set; }
        public string PlayerBName { get; private set; }
        #endregion

        #region Constructors
        public AbstractGameState(string aPlayerA, string aPlayerB, IActor aActorJustActed)
        {
            this.PlayerAName = aPlayerA;
            this.PlayerBName = aPlayerB;
            this._actorJustActed = aActorJustActed;
        }
        #endregion

        #region IGameState
        public abstract IEnumerable<IAction> GetAllMoves();
        public abstract EEndGameStatus GetResult(IActor player);
        public abstract bool IsTerminal();

        public IAction GetRandomMove()
        {
            List<IAction> possibleMoves = new List<IAction>(GetAllMoves());

            if (possibleMoves.Count == 0)
            {
                return null;
            }

            return possibleMoves[MOARandom.Instance.GetRange(0, possibleMoves.Count - 1)];
        }

        public Task<IGameState> SimulateToTerminal()
        {
            IGameState state = this;

            List<IAction> possibleMoves = new List<IAction>(state.GetAllMoves());

            while (possibleMoves.Count > 0 && !state.IsTerminal())
            {
                int index = MOARandom.Instance.GetRange(0, possibleMoves.Count - 1);
                IAction action = possibleMoves[index];

                state = action.DoAction() as IGameState;

                possibleMoves.Clear();
                possibleMoves.AddRange(state.GetAllMoves());
            }

            return Task.FromResult(state);
        }
        #endregion

        #region object
        public abstract object Clone();

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as IGameState);
        }

        public bool Equals(IGameState other)
        {
            return Equals(other as AbstractGameState);
        }

        public bool Equals(AbstractGameState obj)
        {
            if (obj is AbstractGameState)
            {
                return ASCIIRepresentation.Equals((obj as AbstractGameState).ASCIIRepresentation);
            }

            return false;
        }

        public static bool operator ==(AbstractGameState aActorA, AbstractGameState aActorB)
        {
            // if only one is null
            if (MathsUtility.XOR(object.Equals(aActorA, null), object.Equals(aActorB, null)))
            {
                // can't be equal if only one is null
                return false;
            }
            else if (object.Equals(aActorA, null) && object.Equals(aActorB, null))
            {
                // both null, so, technically equal?
                return true;
            }
            else
            {
                // neither null, so compare names
                return aActorA.Equals(aActorB);
            }
        }

        public static bool operator !=(AbstractGameState aActorA, AbstractGameState aActorB)
        {
            return !(aActorA == aActorB);
        }
        #endregion
    }
}