using UnityEngine;
using System.Collections;

namespace UnityTools
{
    /// <summary>
    /// Holds static Maths utility methods.
    /// </summary>
    public static class MathsUtility
    {
        #region float
        /// <summary>
        /// Remaps the given angle to the range (-180, 180] or
        ///  -180 < angle <= 180
        /// </summary>
        /// <returns>The angle.</returns>
        /// <param name="angle">Angle.</param>
        public static float RemapAngleDegrees(this float aTheta)
        {
            while (aTheta > 180f)
            {
                aTheta -= 360f;
            }

            while (aTheta <= -180f)
            {
                aTheta += 360f;
            }

            return aTheta;
        }

        /// <summary>
        /// Remaps the given angle to the range (-pi, pi] or
        ///  -pi < angle <= pi
        /// </summary>
        /// <returns>The angle.</returns>
        /// <param name="angle">Angle.</param>
        public static float RemapAngleRadians(this float aRadians)
        {
            while (aRadians > Mathf.PI)
            {
                aRadians -= 2 * Mathf.PI;
            }

            while (aRadians <= -Mathf.PI)
            {
                aRadians += 2 * Mathf.PI;
            }

            return aRadians;
        }
        #endregion

        #region Vector3
        public readonly static Vector3 XZ_FLAT = new Vector3(1, 0, 1);

        /// <summary>
        /// Remaps the vector.
        /// </summary>
        /// <returns>The vector.</returns>
        /// <param name="aVec">Vec.</param>
        public static Vector3 RemapVector(this Vector3 aVec, bool aAsDegrees = true)
        {
            return new Vector3(
                aAsDegrees ? RemapAngleDegrees(aVec.x) : RemapAngleRadians(aVec.x),
                aAsDegrees ? RemapAngleDegrees(aVec.y) : RemapAngleRadians(aVec.y),
                aAsDegrees ? RemapAngleDegrees(aVec.z) : RemapAngleRadians(aVec.z)
            );
        }

        /// <summary>
        /// Clamps the vector, using a given RangedFloat with the MinMax values.
        /// </summary>
        /// <returns>The vector.</returns>
        /// <param name="aToClamp">To clamp.</param>
        /// <param name="aRestrictedRange">Restricted range.</param>
        public static Vector3 ClampVector(this Vector3 aToClamp, RangedFloat aRestrictedRange)
        {
            return ClampVector(aToClamp, aRestrictedRange.minValue, aRestrictedRange.maxValue);
        }

        /// <summary>
        /// Clamps the vector, based on passed in MinMax floats.
        /// </summary>
        /// <returns>The vector.</returns>
        /// <param name="aToClamp">To clamp.</param>
        /// <param name="aMin">Minimum.</param>
        /// <param name="aMax">Max.</param>
        public static Vector3 ClampVector(this Vector3 aToClamp, float aMin, float aMax)
        {
            return new Vector3(
                Mathf.Clamp(aToClamp.x, aMin, aMax),
                Mathf.Clamp(aToClamp.y, aMin, aMax),
                Mathf.Clamp(aToClamp.z, aMin, aMax)
            );
        }
        #endregion

        #region Logic
        /// <summary>
        /// Extension boolean operator for exclusive-OR.
        /// </summary>
        /// <param name="aOne">If set to <c>true</c> one.</param>
        /// <param name="aTwo">If set to <c>true</c> two.</param>
        public static bool XOR(bool aOne, bool aTwo)
        {
            return (aOne || aTwo) && !(aOne && aTwo);
        }
        #endregion

        #region Beizer
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0">Start Point</param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3">End Point</param>
        /// <param name="t">0..1 along the curve</param>
        /// <returns></returns>
        public static Vector3 BeizerPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;
            var oneMinusTSqr = oneMinusT * oneMinusT;
            var tSqr = t * t;
            return oneMinusT * oneMinusTSqr * p0
                + 3f * oneMinusTSqr * t * p1
                + 3f * oneMinusT * tSqr * p2
                + t * tSqr * p3;
        }
        #endregion
    }
}
