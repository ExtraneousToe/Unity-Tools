using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityTools.AITreesBase.Interfaces
{
    /// <summary>
    /// Actor interface.
    /// </summary>
    public interface IActor : ICloneable, IEquatable<IActor>
    {
        // simple identifier
        string Name { get; }
    }
}