using UnityEngine;
using System.Collections;

namespace UnityTools
{
    /// <summary>
    /// Base behaviour.
    /// </summary>
    public abstract class BaseBehaviour : MonoBehaviour
    {
        #region Variables
        /// <summary>
        /// The transform reference for caching.
        /// Will/should reduce processing load.
        /// </summary>
        private Transform _transform = null;
        public new Transform transform
        {
            get
            {
                return _transform ?? (_transform = base.transform);
            }
        }

        [Header("Debugging")]
        [SerializeField]
        private bool _isDebugging = false;
        public bool IsDebugging
        {
            get { return _isDebugging; }
        }
        #endregion

        #region Mono
        /// <summary>
        /// Reset this instance.
        /// </summary>
        protected virtual void Reset()
        {
            _transform = base.transform;
        }

        /// <summary>
        /// Awake this instance.
        /// </summary>
        protected virtual void Awake()
        {
            if (!_transform)
            {
                _transform = base.transform;
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        protected virtual void Start() { }
        #endregion
    }
}