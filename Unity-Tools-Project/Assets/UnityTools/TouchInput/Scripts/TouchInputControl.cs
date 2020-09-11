using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityTools.TouchInput
{
    /// <summary>
    /// Input Touch Control
    /// 
    /// Reads and processes touch input.
    /// </summary>
    public class TouchInputControl : BaseSingleton<TouchInputControl>
    {
        #region Delegates
        public UnityEvent<TouchChain> OnTouchChainComplete;
        #endregion

        #region Variables
        private Dictionary<int, TouchChain> _touches = null;
        public Dictionary<int, TouchChain> Touches
        {
            get
            {
                if (_touches == null)
                {
                    _touches = new Dictionary<int, TouchChain>();
                }

                return _touches;
            }
        }

        [SerializeField]
        [Range(2, 4)]
        private int _angleAccuracy = 4;
        #endregion

        #region Mono
        /// <summary>
        /// Update the instance in the regular time interval.
        /// </summary>
        public void Update()
        {
            // store all of the Touch instances within the TouchChain instances
            foreach (Touch t in Input.touches)
            {
                // get the id
                int id = t.fingerId;

                // if the ID is already in the Dictionary
                if (Touches.ContainsKey(id))
                {
                    // simply append the instance to the given chain
                    Touches[id].Append(t);
                }
                else
                {
                    // start a new chain
                    Touches[id] = new TouchChain(t, _angleAccuracy);
                }

                // if the touch was cancelled
                if (Touches[id].IsCancelled)
                {
                    // get rid of it
                    Touches.Remove(id);
                }
                else if (Touches[id].IsTerminating)
                {
                    // get a reference
                    TouchChain tc = Touches[id];
                    Touches.Remove(id);

                    Logger.Log(GetType().Name, tc.Parse());
                    OnTouchChainComplete?.Invoke(tc);
                }
            }
        }
        #endregion
    }
}