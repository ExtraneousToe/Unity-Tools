using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UnityTools.TouchInput
{
    using Enums;

    /// <summary>
    /// Touch chain.
    /// </summary>
    public class TouchChain
    {
        //        public const float N = 0f;
        //        public const float NNE = 22.5f;
        //        public const float NE = 45f;
        //        public const float ENE = 67.5f;
        //
        //        public const float E = 90f;
        //        public const float ESE = 112.5f;
        //        public const float SE = 135f;
        //        public const float SSE = 157.5f;
        //
        //        public const float S = 180f;
        //        public const float SSW = 202.5f;
        //        public const float SW = 225f;
        //        public const float WSW = 247.5f;
        //
        //        public const float W = 270f;
        //        public const float WNW = 292.5f;
        //        public const float NW = 315f;
        //        public const float NNW = 337.5f;

        #region Variables
        // list of touch instances
        private List<Touch> _touches;
        public List<Touch> Touches
        {
            get
            {
                if (_touches == null)
                {
                    _touches = new List<Touch>();
                }

                return _touches;
            }
        }

        // the fingerID of the given chain
        private readonly int _fingerID;
        // public accessor for the fingerID
        public int FingerID
        {
            get
            {
                return _fingerID;
            }
        }

        private Touch _lastTouch;
        /// <summary>
        /// Gets a value indicating whether this 
        /// <see cref="UnityTools.TouchInput.TouchChain"/> is terminating.
        /// </summary>
        /// <value><c>true</c> if is terminating; otherwise, <c>false</c>.</value>
        public bool IsTerminating
        {
            get
            {
                return _lastTouch.phase == TouchPhase.Ended;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this 
        /// <see cref="UnityTools.TouchInput.TouchChain"/> was canceled.
        /// </summary>
        /// <value><c>true</c> if is canceled; otherwise, <c>false</c>.</value>
        public bool IsCancelled
        {
            get
            {
                return _lastTouch.phase == TouchPhase.Canceled;
            }
        }

        /// <summary>
        /// The move segments.
        /// </summary>
        private List<TouchMovementSegment> _moveSegments;
        public List<TouchMovementSegment> MoveSegments
        {
            get
            {
                if (_moveSegments == null)
                {
                    _moveSegments = new List<TouchMovementSegment>();
                }

                return _moveSegments;
            }
        }

        /// <summary>
        /// The angle accuracy.
        /// 
        /// Used as below,
        /// 1 << 2 == 4 -> N,S,E,W
        /// 1 << 3 == 8 -> + NE,SE,SW,NW
        /// 1 << 4 == 16 -> + NNE,ENE,...
        /// </summary>
        private int _angleAccuracy;
        public int AngleAccuracy
        {
            get => _angleAccuracy;
            private set
            {
                _angleAccuracy = Mathf.Clamp(value, 2, 4);
            }
        }
        private readonly int[] STEPS = new int[] { 4, 2, 1 };
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityTools.Input.TouchChain"/>
        ///  class.
        /// </summary>
        /// <param name="aFirstTouch">First touch.</param>
        public TouchChain(Touch aFirstTouch, int aAccuracy = 4)
        {
            _moveSegments = null;
            AngleAccuracy = aAccuracy;

            _fingerID = aFirstTouch.fingerId;
            Touches.Add(aFirstTouch);
            _lastTouch = aFirstTouch;
        }
        #endregion

        #region
        /// <summary>
        /// Append another touch object to the chain.
        /// </summary>
        /// <param name="t">T.</param>
        public void Append(Touch t)
        {
            // only append Touch objects with a matching fingerId
            if (FingerID != t.fingerId)
            {
                return;
            }

            // append the new Touch instance
            Touches.Add(t);
            _lastTouch = t;
        }

        public ETouchResult Parse()
        {
            // if there was no movement, it was a tap
            if (!Touches.Any(t => t.phase == TouchPhase.Moved))
            {
                return ETouchResult.Tap;
            }

            // parse movement into a sequence of swipe directions
            List<ESwipeResult> swipeDirList = new List<ESwipeResult>();
            for (int i = 1; i < Touches.Count; i++)
            {
                Touch t = Touches[i];
                Vector2 dPos = Touches[i].deltaPosition;

                // opposite, adjacent, and hypotenuse
                float o, a, h;
                h = dPos.magnitude;

                if (h == 0)
                    continue;

                dPos.Normalize();
                // a normalised vector is always of length 1
                h = 1;
                o = dPos.y;
                a = dPos.x;

                float theta = 0;

                // cos(theta) = a/h, therefore
                theta = Mathf.Acos(a / h);
                // the resulting angle from this is (0,180) with the 0 along the
                // positive x-axis
                // To fix this the angle is converted to degrees, if the y-pos
                // is above the line is it given a negative value and 90˚ added
                // to it, then if the angle is greater than 180˚ subtract 360˚

                theta *= Mathf.Rad2Deg * -Mathf.Sign(o);
                theta += 90f;

                if (theta > 180f)
                {
                    theta -= 360f;
                }

                // magic numbers, bad practice
                int angles = 1 << AngleAccuracy;
                float offset = 360f / angles;
                float halfOffset = offset * 0.5f;
                int step = STEPS[AngleAccuracy - 2];

                theta = MathsUtility.RemapAngleDegrees(theta);

                // check through angles based on accuracy
                for (int j = 0; j < angles; j++)
                {
                    float subTheta = j * offset;

                    float subThetaM = 0;
                    float subThetaP = 0;

                    subTheta = MathsUtility.RemapAngleDegrees(subTheta);

                    subThetaM = subTheta - halfOffset;
                    subThetaP = subTheta + halfOffset;

                    subThetaM = MathsUtility.RemapAngleDegrees(subThetaM);
                    subThetaP = MathsUtility.RemapAngleDegrees(subThetaP);

                    float otherSubTheta = subTheta * (Mathf.Sign(subThetaP) == -1 && Mathf.Sign(subTheta) == 1 ? -1 : 1);

                    if ((subThetaM < theta && theta <= subTheta) ||
                        (otherSubTheta < theta && theta <= subThetaP))
                    {
                        AppendTouchMovement((ESwipeResult)(j * step), t);
                        break;
                    }
                }
            }

            // determine result
            // if there was only a single direction, it was a swipe
            if (MoveSegments.Count == 1)
                return ETouchResult.Swipe;

            // otherwise, it was a shape ... but which shape?!?! oooOOOOoooOO
            // TODO: Parse Shapes ...

            return ETouchResult.NumResults;
        }

        private void AppendTouchMovement(ESwipeResult swipeRes, Touch tInfo)
        {
            TouchMovementSegment tms = null;

            // if there is nothing in the list
            if (MoveSegments.Count == 0)
            {
                // first time, so create a new segment object
                tms = new TouchMovementSegment(swipeRes, tInfo);

                MoveSegments.Add(tms);
            }
            else
            {
                // get the reference to the most recent one
                tms = MoveSegments[MoveSegments.Count - 1];

                // if it fails to append, the direction must not match, so a new
                // segment must be created
                if (!tms.AppendMovement(swipeRes, tInfo))
                {
                    tms = new TouchMovementSegment(swipeRes, tInfo);

                    MoveSegments.Add(tms);
                }
            }
        }
        #endregion
    }
}

