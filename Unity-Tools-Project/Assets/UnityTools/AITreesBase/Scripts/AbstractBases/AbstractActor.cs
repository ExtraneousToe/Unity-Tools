using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AITreesBase
{
    using Interfaces;

    public abstract class AbstractActor : IActor
    {
        #region Variables
        public string Name { get; private set; }
        #endregion

        #region Constructor
        public AbstractActor(string aName)
        {
            this.Name = aName;
        }
        #endregion

        #region
        public abstract object Clone();

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as IActor);
        }

        public bool Equals(IActor other)
        {
            return Equals(other as AbstractActor);
        }

        public bool Equals(AbstractActor obj)
        {
            if (obj is AbstractActor)
            {
                return (obj as AbstractActor).Name.Equals(Name);
            }
            return false;
        }

        public static bool operator ==(AbstractActor aActorA, AbstractActor aActorB)
        {
            // if only one is null
            if (MathsUtility.XOR(object.Equals(aActorA, null), object.Equals(aActorB, null)))
            {
                // can't be equal if only one is null
                return false;
            }
            else if (object.Equals(aActorA, null) && object.Equals(aActorB, null))
            {
                // both null, so, technically equal?
                return true;
            }
            else
            {
                // neither null, so compare names
                return aActorA.Equals(aActorB);
            }
        }

        public static bool operator !=(AbstractActor aActorA, AbstractActor aActorB)
        {
            return !(aActorA == aActorB);
        }
        #endregion
    }
}
