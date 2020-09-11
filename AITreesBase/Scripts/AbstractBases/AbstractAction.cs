using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AITreesBase
{
    using Interfaces;

    public abstract class AbstractAction : IAction
    {
        #region Variables
        public IActor ActorActing
        {
            get;
            private set;
        }
        public abstract string ASCIIRepresentation { get; }
        #endregion

        #region Constructors
        public AbstractAction(IActor aIsActing)
        {
            this.ActorActing = aIsActing;
        }
        #endregion

        #region AbstractAction
        public abstract IGameState DoAction();

        public override string ToString() => ASCIIRepresentation;
        #endregion
    }
}
