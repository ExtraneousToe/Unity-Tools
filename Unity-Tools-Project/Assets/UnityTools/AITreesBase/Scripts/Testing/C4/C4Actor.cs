using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AITreesBase.Testing.C4
{
    using UnityTools.AITreesBase.Interfaces;
    using UnityTools.AITreesBase.Enums;

    public class C4Actor : AbstractActor
    {
        #region Variables
        #endregion

        #region Constructor
        public C4Actor(string identifier) : base(identifier)
        {
        }
        #endregion

        #region
        public override object Clone()
        {
            return new C4Actor(Name);
        }
        #endregion
    }
}