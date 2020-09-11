using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

namespace UnityTools.AITreesBase.Testing
{
    using OX;
    using C4;
    using Pentagoesque;

    using UnityTools.AITreesBase.Interfaces;
    using UnityTools.AITreesBase.MCTS;

    public enum TestingGame
    {
        OX,
        C4,
        Pentagoesque
    }

    public enum PlayerBehaviour
    {
        Random,
        MCTS,
        UCT
    }

    public class AITreeTester : MonoBehaviour
    {
        [Serializable]
        public class Player
        {
            public string Name;
            public PlayerBehaviour Behaviour;
            public int Iterations = 1000;

            public bool RetainTree = true;
            [HideInInspector]
            public MCTSConfig Config;

            [HideInInspector]
            public MCTSNode TreeRoot { get; set; }
        }

        [Range(0f, 1f)]
        public float waitTime = 0.5f;

        [Range(0f, 2f)]
        public float uctExploration = Mathf.Sqrt(2);

        public bool verbose = true;

        [SerializeField]
        private TestingGame _testingGame = TestingGame.OX;

        [Header("Player")]
        [SerializeField]
        private bool _startPlayerOne = true;

        [SerializeField]
        private Player _playerOne = new Player
        {
            Name = "X",
            Behaviour = PlayerBehaviour.Random
        };

        [SerializeField]
        private Player _playerTwo = new Player
        {
            Name = "O",
            Behaviour = PlayerBehaviour.Random
        };

        private Task _runningTask;

        private void Awake()
        {
        }

        // Use this for initialization
        void Start()
        {
            try
            {
                _runningTask = MCTSRoutine();
            }
            catch (System.Exception e)
            {
                Logger.LogException(e);
            }
        }

        private async Task MCTSRoutine()
        {
            System.Action<string> print = (s) => Logger.Log(s);
            MOARandom r = new MOARandom();

            IActor playerOne = null;
            IGameState game = null;
            switch (_testingGame)
            {
                case TestingGame.OX:
                    playerOne = new OXActor(_startPlayerOne ? _playerTwo.Name : _playerOne.Name);
                    game = new OXGameState(_playerOne.Name, _playerTwo.Name, playerOne as OXActor);
                    break;
                case TestingGame.C4:
                    playerOne = new C4Actor(_startPlayerOne ? _playerTwo.Name : _playerOne.Name);
                    game = new C4GameState(_playerOne.Name, _playerTwo.Name, playerOne as C4Actor);
                    break;
                case TestingGame.Pentagoesque:
                    playerOne = new PentActor(_startPlayerOne ? _playerTwo.Name : _playerOne.Name);
                    game = new PentGameState(_playerOne.Name, _playerTwo.Name, playerOne as PentActor);
                    break;
                default:
                    break;
            }

            _playerOne.Config = new MCTSConfig
            {
                MaxIterations = (uint)_playerOne.Iterations,
                Verbose = verbose,
                PrintFn = print,
                UCTK = uctExploration
            };
            switch (_playerOne.Behaviour)
            {
                case PlayerBehaviour.MCTS:
                    _playerOne.TreeRoot = new MCTSNode(0, null, null, game.ActorJustActed, game);
                    break;
                case PlayerBehaviour.UCT:
                    _playerOne.TreeRoot = new UCTNode(0, null, null, game.ActorJustActed, game, _playerOne.Config.UCTK);
                    break;
            }

            _playerTwo.Config = new MCTSConfig
            {
                MaxIterations = (uint)_playerTwo.Iterations,
                Verbose = verbose,
                PrintFn = print,
                UCTK = uctExploration
            };
            switch (_playerTwo.Behaviour)
            {
                case PlayerBehaviour.MCTS:
                    _playerTwo.TreeRoot = new MCTSNode(0, null, null, game.ActorJustActed, game);
                    break;
                case PlayerBehaviour.UCT:
                    _playerTwo.TreeRoot = new UCTNode(0, null, null, game.ActorJustActed, game, _playerTwo.Config.UCTK);
                    break;
            }

            while (!game.IsTerminal() && Application.isPlaying)
            {
                IAction action = null;

                Player actingPlayer = null;
                Player otherPlayer = null;

                if (game.ActorJustActed.Name == _playerOne.Name)
                {
                    actingPlayer = _playerTwo;
                    otherPlayer = _playerOne;
                }
                else
                {
                    actingPlayer = _playerOne;
                    otherPlayer = _playerTwo;
                }

                Logger.Log(GetType().Name, $"Player Acting: {actingPlayer.Name}, Behaviour: {actingPlayer.Behaviour}");

                MCTSNode node = null;
                switch (actingPlayer.Behaviour)
                {
                    case PlayerBehaviour.Random:
                        action = game.GetRandomMove();
                        break;
                    case PlayerBehaviour.MCTS:
                        node = await MCTS.ProcessTree(
                            actingPlayer.RetainTree ? actingPlayer.TreeRoot : new MCTSNode(0, null, null, game.ActorJustActed, game),
                            game,
                            actingPlayer.Config
                            );

                        action = node.IncomingAction;
                        actingPlayer.TreeRoot = node.ConstructAsRoot();
                        break;
                    case PlayerBehaviour.UCT:
                        node = await MCTS.ProcessTree(
                            actingPlayer.RetainTree ? actingPlayer.TreeRoot : new UCTNode(0, null, null, game.ActorJustActed, game, actingPlayer.Config.UCTK),
                            game,
                            actingPlayer.Config
                        );

                        action = node.IncomingAction;
                        actingPlayer.TreeRoot = node.ConstructAsRoot();
                        break;
                    default:
                        break;
                }

                if (action != null)
                {
                    game = action.DoAction();

                    Logger.Log(action.ASCIIRepresentation);

                    otherPlayer.TreeRoot = otherPlayer.TreeRoot?.GetChildWithState(game)?.ConstructAsRoot();
                }
                else
                {
                    Logger.Log("STALLING!!");
                    break;
                }

                Logger.Log(game.ASCIIRepresentation);

                if (waitTime != 0f)
                {
                    await Task.Delay((int)(waitTime * 1000));
                }
            }

            Logger.LogFormat("{0} {1}",
                game.ActorJustActed.Name, game.GetResult(game.ActorJustActed));
        }
    }
}