using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

using System;

using UnityTools.AITreesBase.Enums;

namespace UnityTools.AITreesBase.Interfaces
{
    /// <summary>
    /// GameState interface.
    /// </summary>
    public interface IGameState : ICloneable, IEquatable<IGameState>
    {
        /// <summary>
        /// return all moves available from this state for the current player
        /// </summary>
        IEnumerable<IAction> GetAllMoves();

        /// <summary>
        /// Gets a random move available from the GetAllMoves() call.
        /// </summary>
        /// <returns></returns>
        IAction GetRandomMove();

        /// <summary>
        /// given an actor, did that player win/lose/draw
        /// </summary>
        EEndGameStatus GetResult(IActor player);

        /// <summary>
        /// reference to the actor that just acted
        /// </summary>
        IActor ActorJustActed { get; }

        /// <summary>
        /// Simulates to terminal.
        /// </summary>
        Task<IGameState> SimulateToTerminal();

        /// <summary>
        /// Determines whether this instance is terminal.
        /// </summary>
        /// <returns><c>true</c> if this instance is terminal; otherwise, <c>false</c>.</returns>
        bool IsTerminal();

        /// <summary>
        /// simple representation of the IGameState. Used for console and comparing.
        /// </summary>
        string ASCIIRepresentation { get; }
    }
}