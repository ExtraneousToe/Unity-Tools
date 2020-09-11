using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Threading.Tasks;

namespace UnityTools.AITreesBase.MCTS
{
    using UnityTools.AITreesBase.Enums;
    using UnityTools.AITreesBase.Interfaces;
    using UnityTools.AITreesBase.Nodes;

    public struct MCTSConfig
    {
        public uint MaxIterations;
        public bool Verbose;
        public Action<string> PrintFn;
        public float UCTK;

        public MCTSConfig(
            uint aMaxIterations = 1000,
            bool aVerbose = false,
            Action<string> aPrintFn = null,
            float? aUCTK = null)
        {
            this.MaxIterations = aMaxIterations;
            this.Verbose = aVerbose;
            this.PrintFn = aPrintFn;
            this.UCTK = aUCTK.HasValue ? aUCTK.Value : Mathf.Sqrt(2);
        }
    }

    public static class MCTS
    {
        public async static Task<MCTSNode> ProcessTree(MCTSNode aRootNode, IGameState aGameState, MCTSConfig aConfig)
        {
            bool verbose = aConfig.Verbose;
            Action<string> printfn = aConfig.PrintFn;
            uint maxIters = aConfig.MaxIterations;

            MCTSNode root = aRootNode;

            bool done = false;

            int iters = 0;

            do
            {
                MCTSNode node = root;

                // Select
                while (node.IsFullyExpandedAndNonterminal)
                {
                    node = node.SelectChildByPolicy();
                }

                // Expand
                IAction newAction = node.GetRandomUntriedAction();
                if (newAction != null)
                {
                    IGameState state = newAction.DoAction();
                    System.Func<MCTSNode> nodeConst = node.GenerateChildFunction(newAction, state);
                    node = node.AddChild(nodeConst);
                }

                // getting state reference for Rollout and Backprop
                IGameState finalState = node.NodeState.Clone() as IGameState;

                // Rollout
                finalState = await finalState.SimulateToTerminal();

                // Backpropagate
                while (node != null)
                {
                    node.UpdateResult(
                        finalState.GetResult(
                            node.ActorJustActed
                        )
                    );
                    node = node.Parent;
                }

                if (++iters >= maxIters)
                {
                    done = true;
                }
            }
            while (!done);

            MCTSNode bestChild = root.GetBestChild();

            if (verbose)
            {
                printfn(root.DisplayTree(0));

                printfn(root.DisplayMostVisistedChild());
            }

            return bestChild;
        }

        public async static Task<IAction> GetAction(MCTSNode aRootNode, IGameState aGameState, MCTSConfig aConfig)
        {
            MCTSNode processedTree = await ProcessTree(aRootNode, aGameState, aConfig);

            return processedTree.IncomingAction;
        }
    }
}