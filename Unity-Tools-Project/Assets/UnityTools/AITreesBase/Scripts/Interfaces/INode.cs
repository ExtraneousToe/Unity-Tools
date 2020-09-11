using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityTools.AITreesBase.Enums;

namespace UnityTools.AITreesBase.Interfaces
{
    /// <summary>
    /// Node interface.
    /// </summary>
    public interface INode<T> where T : INode<T>
    {
        /// <summary>
        /// Gets the depth of the node in the tree.
        /// </summary>
        /// <value>The depth.</value>
        uint Depth { get; }

        /// <summary>
        /// Gets the parent of the node.
        /// </summary>
        /// <value>The parent.</value>
        T Parent { get; }

        /// <summary>
        /// Gets the last action performed leading into this node.
        /// </summary>
        /// <value>The last action.</value>
        IAction IncomingAction { get; }

        /// <summary>
        /// Gets the actor that just moved.
        /// </summary>
        /// <value>The actor just moved.</value>
        IActor ActorJustActed { get; }

        /// <summary>
        /// Gets the IGameState of the node.
        /// </summary>
        /// <value>The state of the node.</value>
        IGameState NodeState { get; }

        /// <summary>
        /// Gets the expanded children nodes.
        /// </summary>
        /// <value>The child.</value>
        IEnumerable<T> Children { get; }

        /// <summary>
        /// Gets the untried actions.
        /// </summary>
        /// <value>The untried actions.</value>
        IEnumerable<IAction> UntriedActions { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is fully expanded 
        /// and nonterminal.
        /// </summary>
        /// <value><c>true</c> if this instance is fully expanded and 
        /// nonterminal; otherwise, <c>false</c>.</value>
        bool IsFullyExpandedAndNonterminal { get; }

        /// <summary>
        /// Adds a child to the node given a constructor.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="nodeConst">Node const.</param>
        T AddChild(Func<T> nodeConst);

        /// <summary>
        /// Gets an untried action.
        /// </summary>
        /// <returns>The next untried action.</returns>
        IAction GetRandomUntriedAction();

        /// <summary>
        /// Selects the child given a selection policy.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="policy">Policy.</param>
        T SelectChildByPolicy();

        /// <summary>
        /// Gets the Best Child based on the programmed policy.
        /// </summary>
        /// <returns>The child.</returns>
        T GetBestChild();

        /// <summary>
        /// Gets the Best Action based on the programmed policy.
        /// </summary>
        /// <returns>The child.</returns>
        IAction GetBestAction();
    }
}