using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityTools.AITreesBase.MCTS
{
    using UnityTools.AITreesBase.Nodes;
    using UnityTools.AITreesBase.Interfaces;
    using UnityTools.AITreesBase.Enums;


    /// <summary>
    /// MCTS node.
    /// </summary>
    public class MCTSNode : AbstractNode<MCTSNode>
    {
        #region Variables
        /// <summary>
        /// Gets the wins.
        /// </summary>
        /// <value>The wins.</value>
        private ulong _wins;
        public ulong Wins
        {
            get
            {
                return _wins;
            }
            private set
            {
                _wins = value;
            }
        }

        /// <summary>
        /// Gets the visits.
        /// </summary>
        /// <value>The visits.</value>
        private ulong _visits;
        public ulong Visits
        {
            get
            {
                return _visits;
            }
            private set
            {
                _visits = value;
            }
        }

        /// <summary>
        /// The children.
        /// </summary>
        private readonly List<MCTSNode> _children;
        /// <summary>
        /// Gets the expanded children nodes.
        /// </summary>
        /// <value>The child.</value>
        public override IEnumerable<MCTSNode> Children
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// The untried actions.
        /// </summary>
        private readonly Stack<IAction> _untriedActions;
        /// <summary>
        /// Gets the untried actions.
        /// </summary>
        /// <value>The untried actions.</value>
        public override IEnumerable<IAction> UntriedActions
        {
            get
            {
                return _untriedActions;
            }
        }

        public bool IsFullyExpanded => this._untriedActions.Count == 0;
        public bool IsNonTerminal => this._children.Count == 0;

        /// <summary>
        /// Gets a value indicating whether this instance is fully expanded and nonterminal.
        /// </summary>
        /// <value><c>true</c> if this instance is fully expanded and nonterminal; otherwise, <c>false</c>.</value>
        public override bool IsFullyExpandedAndNonterminal
        {
            get
            {
                // fully expanded if there are no untried moves left
                // nonterminal if there are children
                return IsFullyExpanded && IsNonTerminal;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityTools.AITreesBase.MCTS.MCTSNode"/> class.
        /// </summary>
        /// <param name="depth">Depth.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="lastAction">Last action.</param>
        /// <param name="actorJustActed">Actor just acted.</param>
        /// <param name="gameState">Game state.</param>
        public MCTSNode(uint depth, MCTSNode parent, IAction lastAction,
            IActor actorJustActed, IGameState gameState) :
        base(depth, parent, lastAction, actorJustActed, gameState)
        {
            _wins = 0;
            _visits = 0;

            _children = new List<MCTSNode>();

            IEnumerable<IAction> actions = NodeState.GetAllMoves();

            _untriedActions = new Stack<IAction>(actions.Shuffle());
        }
        #endregion

        #region Node
        public override MCTSNode ConstructAsRoot()
        {
            MCTSNode newRoot = new MCTSNode(0, null, null, ActorJustActed, NodeState);

            newRoot._wins = Wins;
            newRoot._visits = Visits;

            newRoot._children.AddRange(Children);

            _untriedActions.Clear();
            foreach (IAction action in UntriedActions)
            {
                newRoot._untriedActions.Push(action);
            }

            return newRoot;
        }

        /// <summary>
        /// Adds a child to the node given a constructor.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="nodeConst">Node const.</param>
        public override MCTSNode AddChild(Func<MCTSNode> nodeConst)
        {
            // create child using the provided Function
            MCTSNode newChild = nodeConst();

            // add the child to the list and return it
            this._children.Add(newChild);

            return newChild;
        }

        public override Func<MCTSNode> GenerateChildFunction(IAction aLastAction, IGameState aState)
        {
            return () => new MCTSNode(Depth + 1, this, aLastAction, aLastAction.ActorActing, aState);
        }

        /// <summary>
        /// Gets a random untried action.
        /// </summary>
        /// <returns>The next untried action.</returns>
        public override IAction GetRandomUntriedAction()
        {
            if (this._untriedActions.Count == 0)
                return null;

            return this._untriedActions.Pop();
        }

        internal virtual float CalculatePolicy(MCTSNode node)
        {
            if (node.Visits == 0)
                return 0;

            return node.Wins / (float)node.Visits;
        }

        /// <summary>
        /// Selects the child given a selection policy.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="policy">Policy.</param>
        public override MCTSNode SelectChildByPolicy()
        {
            return this._children.OrderByDescending(CalculatePolicy).First();
        }

        /// <summary>
        /// Gets the Best Child based on the programmed policy.
        /// </summary>
        /// <returns>The child.</returns>
        public override MCTSNode GetBestChild()
        {
            if (_children.Count == 0)
                return null;

            MCTSNode node = _children.OrderByDescending(n => n.Visits).First();

            return node;
        }

        /// <summary>
        /// Gets the Best Action based on the programmed policy.
        /// </summary>
        /// <returns>The child.</returns>
        public override IAction GetBestAction()
        {
            MCTSNode bestChild = GetBestChild();

            if (bestChild == null)
                return null;

            return bestChild.IncomingAction;
        }

        public override MCTSNode GetChildWithState(IGameState aStateToMatch)
        {
            MCTSNode child = base.GetChildWithState(aStateToMatch);

            if (child == null)
            {
                //Logger.Log($"Expanded child did not exist.\nUntried action count == {this._untriedActions.Count}\nChild Count == {this._children.Count}");

                while (!IsFullyExpanded)
                {
                    IAction newAction = GetRandomUntriedAction();
                    if (newAction != null)
                    {
                        IGameState state = newAction.DoAction();
                        System.Func<MCTSNode> nodeConst = GenerateChildFunction(newAction, state);
                        AddChild(nodeConst);
                    }
                }

                child = base.GetChildWithState(aStateToMatch);

                if (child == null)
                {
                    //Logger.Log($"Expanded child STILL did not exist.\nUntried action count == {this._untriedActions.Count}\nChild Count == {this._children.Count}");
                }
            }

            return child;
        }

        // update the node based on simuated results
        public void UpdateResult(EEndGameStatus status)
        {
            Visits++;
            if (status == EEndGameStatus.Win)
            {
                Wins++;
            }
        }
        #endregion

        #region Display
        public string DisplayMostVisistedChild()
        {
            var sb = new StringBuilder();
            foreach (var node in this.Children)
            {
                sb.Append($"N:{node.IncomingAction.ASCIIRepresentation}, D:{node.Depth}, W/V:{node.Wins}/{node.Visits}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        internal string DisplayBestWinVisitRatioChild()
        {
            var values = this.Children.Select(
                node => new Tuple<ulong, ulong, ulong, string>(
                    (ulong)(100 * node.Wins / (double)node.Visits),
                    node.Wins,
                    node.Visits,
                    DisplayNode(node)
                )).OrderByDescending(t => t.Item1);

            StringBuilder sb = new StringBuilder();

            sb.Append("MVC :");
            foreach (var value in values)
            {
                sb.AppendFormat(" {0}%={1}/{2} {3}",
                    value.Item1,
                    value.Item2,
                    value.Item3,
                    value.Item4
                );
            }
            return sb.ToString();
        }

        internal string DisplayUTC()
        {
            var values = this.Children.Select(
                node => new Tuple<double, string>(
                    CalculatePolicy(node),
                    DisplayNode(node))
            ).OrderByDescending(t => t.Item1);

            StringBuilder sb = new StringBuilder();

            sb.Append("UTC : ");
            foreach (var value in values)
            {
                sb.AppendFormat("{0:0.00} {1},", value.Item1, value.Item2);
            }
            return sb.ToString();
        }

        internal string Display()
        {
            var move = this.IncomingAction != null ? this.IncomingAction.ASCIIRepresentation : "No Move";

            return string.Format("[M: {0} D: {5} W/V:{1}/{2} U:{3} C:{4}]",
                move,
                this.Wins,
                this.Visits,
                this.UntriedActions.Count(),
                this.Children.Count(),
                this.Depth
            );
        }

        public string DisplayTree(int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.Append(new string('-', indent));
            sb.Append(this.Display());

            foreach (var child in this.Children)
            {
                sb.Append(child.DisplayTree(indent + 1));
            }
            return sb.ToString();
        }

        private string DisplayNode(INode<MCTSNode> node)
        {
            var list = new List<string>();
            var sb = new StringBuilder();
            while (node.IncomingAction != null)
            {
                list.Add(node.IncomingAction.ASCIIRepresentation);
                node = node.Parent;
            }

            list.Reverse();
            foreach (var move in list)
            {
                sb.AppendFormat("->{0}", move);
            }
            return sb.ToString();
        }
        #endregion
    }
}

