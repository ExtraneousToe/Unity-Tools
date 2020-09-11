using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AITreesBase.Testing.Pentagoesque
{
    using UnityTools.AITreesBase.Interfaces;
    using UnityTools.AITreesBase.Enums;

    public class PentActor : AbstractActor
    {
        #region Variables
        #endregion

        #region Constructor
        public PentActor(string identifier) : base(identifier)
        {
        }
        #endregion

        #region
        public override object Clone()
        {
            return new PentActor(Name);
        }
        #endregion
    }
}
