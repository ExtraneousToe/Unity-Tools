using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace UnityTools.AITreesBase.Nodes
{
    using UnityTools.AITreesBase.Interfaces;

    /// <summary>
    /// Abstract node, implementing INode
    /// </summary>
    public abstract class AbstractNode<T> : INode<T> where T : AbstractNode<T>
    {
        #region Variables
        /// <summary>
        /// The depth.
        /// </summary>
        private readonly uint _depth;
        public uint Depth
        {
            get
            {
                return _depth;
            }
        }

        /// <summary>
        /// The parent.
        /// </summary>
        private readonly T _parent;
        public T Parent
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>
        /// The last action.
        /// </summary>
        private readonly IAction _incomingAction;
        public IAction IncomingAction
        {
            get
            {
                return _incomingAction;
            }
        }

        /// <summary>
        /// The actor just moved.
        /// </summary>
        private readonly IActor _actorJustActed;
        public IActor ActorJustActed
        {
            get
            {
                return _actorJustActed;
            }
        }

        /// <summary>
        /// Gets the IGameState of the node.
        /// </summary>
        /// <value>The state of the node.</value>
        private readonly IGameState _gameState;

        public IGameState NodeState
        {
            get
            {
                return _gameState;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AITreesBase.Nodes.AbstractNode`1"/> class.
        /// </summary>
        /// <param name="depth">Depth.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="lastAction">Last action.</param>
        /// <param name="actorJustActed">Actor just acted.</param>
        /// <param name="gameState">Game state.</param>
        public AbstractNode(uint depth, T parent, IAction incomingAction,
            IActor actorJustActed, IGameState gameState)
        {
            this._depth = depth;
            this._parent = parent;
            this._incomingAction = incomingAction;
            this._actorJustActed = actorJustActed;
            this._gameState = gameState;
        }
        #endregion

        #region AbstractNode
        public abstract T ConstructAsRoot();

        /// <summary>
        /// Gets the expanded children nodes.
        /// </summary>
        /// <value>The child.</value>
        public abstract IEnumerable<T> Children { get; }

        /// <summary>
        /// Gets the untried actions.
        /// </summary>
        /// <value>The untried actions.</value>
        public abstract IEnumerable<IAction> UntriedActions { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is fully expanded and nonterminal.
        /// </summary>
        /// <value><c>true</c> if this instance is fully expanded and nonterminal; otherwise, <c>false</c>.</value>
        public abstract bool IsFullyExpandedAndNonterminal { get; }

        /// <summary>
        /// Adds a child to the node given a constructor.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="nodeConst">Node const.</param>
        public abstract T AddChild(Func<T> nodeConst);

        public abstract Func<T> GenerateChildFunction(IAction aAction, IGameState aState);

        /// <summary>
        /// Gets a random untried action.
        /// </summary>
        /// <returns>The next untried action.</returns>
        public abstract IAction GetRandomUntriedAction();

        /// <summary>
        /// Selects the child given a selection policy.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="policy">Policy.</param>
        public abstract T SelectChildByPolicy();

        /// <summary>
        /// Gets the Best Child based on the programmed policy.
        /// </summary>
        /// <returns>The child.</returns>
        public abstract T GetBestChild();

        /// <summary>
        /// Gets the Best Action based on the programmed policy.
        /// </summary>
        /// <returns>The child.</returns>
        public abstract IAction GetBestAction();

        public virtual T GetChildWithState(IGameState aStateToMatch)
        {
            return Children.Where(c => c.NodeState.Equals(aStateToMatch)).FirstOrDefault();
        }
        #endregion
    }
}