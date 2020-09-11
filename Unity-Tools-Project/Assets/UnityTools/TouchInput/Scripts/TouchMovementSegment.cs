using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.TouchInput
{
    using Enums;

    /// <summary>
    /// Touch movement segment.
    /// 
    /// Used to break a full 'touch input' down into useable chunks, or segments
    /// </summary>
    public class TouchMovementSegment
    {
        #region Variables
        private ESwipeResult _swipeDir;
        public ESwipeResult SwipeDir
        {
            get => _swipeDir;
        }

        private List<Touch> _touchParts;
        public List<Touch> TouchParts
        {
            get => _touchParts;
        }

        /// <summary>
        /// The start and end positions of the segment.
        /// </summary>
        private Vector2 _startPos, _endPos;
        /// <summary>
        /// Gets the start position.
        /// </summary>
        /// <value>The start position.</value>
        public Vector2 StartPos
        {
            get
            {
                return _startPos;
            }
        }
        /// <summary>
        /// Gets the last position.
        /// </summary>
        /// <value>The last position.</value>
        public Vector2 EndPos
        {
            get => _endPos;
        }

        /// <summary>
        /// The total time of the segment
        /// </summary>
        private float _totalTime;
        public float TotalTime
        {
            get => _totalTime;
        }
        #endregion

        #region Contructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityTools.TouchInput.
        /// TouchMovementSegment"/> class.
        /// </summary>
        /// <param name="aSwipeDir">Swipe dir.</param>
        public TouchMovementSegment(ESwipeResult aSwipeDir)
        {
            _swipeDir = aSwipeDir;
            _totalTime = 0;
            _touchParts = new List<Touch>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityTools.TouchInput.
        /// TouchMovementSegment"/> class.
        /// </summary>
        /// <param name="aSwipeDir">Swipe dir.</param>
        /// <param name="aTouchInfo">T info.</param>
        public TouchMovementSegment(ESwipeResult aSwipeDir, Touch aTouchInfo) : this(aSwipeDir)
        {
            _startPos = aTouchInfo.position;
            TouchParts.Add(aTouchInfo);
        }
        #endregion

        #region Mutators
        /// <summary>
        /// Appends the movement info.
        /// 
        /// If the given ESwipeResult does not match it is rejected.
        /// </summary>
        /// <param name="aSwipeDir">Swipe dir.</param>
        /// <param name="aTouchInfo">T info.</param>
        public bool AppendMovement(ESwipeResult aSwipeDir, Touch aTouchInfo)
        {
            // only append if the direction matches
            if (SwipeDir != aSwipeDir)
                return false;

            TouchParts.Add(aTouchInfo);
            _endPos = aTouchInfo.position;

            return true;
        }
        #endregion
    }
}

