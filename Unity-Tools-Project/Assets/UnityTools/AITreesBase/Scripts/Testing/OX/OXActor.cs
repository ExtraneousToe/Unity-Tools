using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AITreesBase.Testing.OX
{
    using UnityTools.AITreesBase.Interfaces;
    using UnityTools.AITreesBase.Enums;

    public class OXActor : AbstractActor
    {
        #region Variables
        #endregion

        #region Constructor
        public OXActor(string identifier) : base (identifier)
        {
        }
        #endregion

        #region
        public override object Clone()
        {
            return new OXActor(Name);
        }
        #endregion
    }
}