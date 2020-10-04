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
        [SerializeField] private bool m_doLogging = false;

        #region ScriptableObject
        protected virtual void Reset()
        {
            if (m_doLogging)
            {
                Logger.LogMethod(GetType().Name);
            }
        }

        protected virtual void Awake()
        {
            if (m_doLogging)
            {
                Logger.LogMethod(GetType().Name);
            }
        }

        protected virtual void OnEnable()
        {
            if (m_doLogging)
            {
                Logger.LogMethod(GetType().Name);
            }
        }

        protected virtual void OnDisable()
        {
            if (m_doLogging)
            {
                Logger.LogMethod(GetType().Name);
            }
        }

        protected virtual void OnDestroy()
        {
            if (m_doLogging)
            {
                Logger.LogMethod(GetType().Name);
            }
        }
        #endregion
    }
}