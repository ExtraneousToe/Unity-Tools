using UnityEngine;
using System.Collections;

namespace UnityTools
{
    /// <summary>
    /// Base animation state.
    /// 
    /// Used to cache the attached BaseBehaviour
    /// </summary>
    public abstract class BaseAnimationState<T> : StateMachineBehaviour where T : BaseBehaviour
    {
        private T _attached;

        /// <summary>
        /// Gets the BaseBehaviour attached to the given animator.
        /// </summary>
        /// <returns>The attached.</returns>
        /// <param name="anim">Animation.</param>
        public T GetAttached(Animator anim)
        {
            return _attached ?? (_attached = anim.GetComponent<T>());
        }
    }
}