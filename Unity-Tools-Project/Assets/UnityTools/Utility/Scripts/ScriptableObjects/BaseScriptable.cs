using UnityEngine;

namespace UnityTools
{
    /// <summary>
    /// Base scriptable class.
    /// 
    /// Contains method stubs for easier reference and overriding.
    /// </summary>
    public abstract class BaseScriptable : ScriptableObject
    {
        #region ScriptableObject
        protected virtual void Reset()
        {
            Logger.LogMethod(GetType().Name);
        }

        protected virtual void Awake()
        {
            Logger.LogMethod(GetType().Name);
        }

        protected virtual void OnEnable()
        {
            Logger.LogMethod(GetType().Name);
        }

        protected virtual void OnDisable()
        {
            Logger.LogMethod(GetType().Name);
        }

        protected virtual void OnDestroy()
        {
            Logger.LogMethod(GetType().Name);
        }
        #endregion
    }
}