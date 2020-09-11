using UnityEngine;
using System.Collections;

namespace UnityTools
{
    /// <summary>
    /// PDG singleton.
    /// </summary>
    public abstract class BaseSingleton<T> : BaseBehaviour where T : BaseBehaviour
    {
        #region Static Variables
        /// <summary>
        /// The static instance reference.
        /// </summary>
        private static T _sInstance;

        /// <summary>
        /// Returns the instance of this singleton.
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (_sInstance == null)
                {
                    _sInstance = FindObjectOfType<T>();

                    if (_sInstance == null)
                    {
                        Logger.LogError(
                            typeof(T).Name,
                            $"An instance of {typeof(T).Name} is needed in the scene, but there is none."
                        );
                    }
                }

                return _sInstance;
            }
        }
        #endregion

        #region Variables
        /// <summary>
        /// It's usually Singletons that shouldn't be destroyed between scenes
        /// so this will simplify the setup of future code.
        /// </summary>
        [SerializeField]
        private bool _dontDestroyOnLoad = false;
        #endregion

        /// <summary>
        /// Awake this instance.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // if the static reference for this Singleton class is already 
            // assigned, and not to this object
            if (_sInstance != null && _sInstance != this)
            {
                Logger.LogError(
                    GetType().Name,
                    $"An instance of {typeof(T)} already exists. Destroying {this.name}."
                );

                // then destroy it
                Destroy(gameObject);
            }
            else
            {
                // all fine, assign this as the reference
                _sInstance = this as T;

                if (_dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }
    }
}