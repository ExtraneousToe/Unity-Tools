using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AITreesBase.Interfaces
{
    /// <summary>
    /// Action interface.
    /// </summary>
    public interface IAction
    {
        // indentifier for the actor that will perform the action
        IActor ActorActing { get; }

        // returns the state created by performing the given action
        IGameState DoAction();

        // simple identifier
        string ASCIIRepresentation { get; }

        string ToString();
    }
}