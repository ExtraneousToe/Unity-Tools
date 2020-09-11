using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityTools.AITreesBase.MCTS
{
    using UnityTools.AITreesBase.Enums;
    using UnityTools.AITreesBase.Interfaces;

    public class UCTNode : MCTSNode
    {
        #region Variables
        private readonly float _uctk;
        public float UCTK
        {
            get
            {
                return _uctk;
            }
        }
        #endregion

        #region Constructor
        public UCTNode(uint depth, MCTSNode parent, IAction lastAction,
            IActor actorJustActed, IGameState gameState, float uctk) :
        base(depth, parent, lastAction, actorJustActed, gameState)
        {
            _uctk = uctk;
        }
        #endregion

        #region Node
        internal override float CalculatePolicy(MCTSNode node)
        {
            if (node.Visits == 0)
                return 0;

            // base policy (wins/visits) + c * sqrt(ln(parentVisits)/(visits))
            // c usually equals sqrt(2)

            return base.CalculatePolicy(node) +
                (this.UCTK * Mathf.Sqrt(Mathf.Log(this.Visits) / (float)node.Visits));
        }

        public override MCTSNode ConstructAsRoot()
        {
            return new UCTNode(0, null, null, ActorJustActed, NodeState, UCTK);
        }

        public override Func<MCTSNode> GenerateChildFunction(IAction aLastAction, IGameState aState)
        {
            return () => new UCTNode(Depth + 1, this, aLastAction, aLastAction.ActorActing, aState, UCTK);
        }
        #endregion
    }
}