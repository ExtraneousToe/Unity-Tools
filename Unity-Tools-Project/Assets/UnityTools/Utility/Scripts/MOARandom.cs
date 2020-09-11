using System;
using UnityEngine;

namespace UnityTools
{
    /// <summary>
    /// Mother-Of-All random.
    /// 
    /// Adapted from ftp://ftp.taygeta.com/pub/c/mother.c
    /// </summary>
    public class MOARandom
    {
        #region Static Variables
        /// <summary>
        /// A static instance of MOARandom, allowing easier Singleton access.
        /// </summary>
        private static MOARandom _sInstance = null;
        public static MOARandom Instance
        {
            get
            {
                // the first time, this will be null
                if (_sInstance == null)
                {
                    // create a new instance
                    _sInstance = new MOARandom();
                }

                return _sInstance;
            }
        }
        #endregion

        #region Variables
        private short[] m1;
        private short[] m2;

        private long seed;

        private const long m16Long = 65536L;
        private const int m16Mask = 0xFFFF;
        private const int m15Mask = 0x7FFF;
        private const int m31Mask = 0x7FFFFFFF;
        private const double m32Double = 4294967295.0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityTools.MOARandom"/> class.
        /// Using the default constructor will feed through a (long) of the time
        /// in milliseconds since [1/1/1970]
        /// </summary>
        public MOARandom() : this((long)System.DateTime.Now.Millisecond)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityTools.MOARandom"/> class
        /// using a given seed, allowing for repeatable testing.
        /// </summary>
        /// <param name="aSeed">Seed.</param>
        public MOARandom(long aSeed)
        {
            SetSeed(aSeed);
        }

        /// <summary>
        /// Initialise the instance using a string.
        /// </summary>
        /// <param name="aSeed"></param>
        public MOARandom(string aSeed)
        {
            // start with an empty string
            string seedString = "";

            // for-each character in the string
            foreach (char c in aSeed)
            {
                // append the ASCII value of the character to the string
                seedString += (int)c;
            }

            try
            {
                // attempt to set the seed with the created string
                SetSeed(long.Parse(seedString));
            }
            catch (Exception e)
            {
                // if it doesn't parse correctly, simply set it based on the time
                SetSeed((long)System.DateTime.Now.Millisecond);

                Logger.LogWarning(
                    GetType().Name,
                    $"SetSeed(long.Parse(seedString)) failed ({e.Message}). Setting seed via time."
                );
            }
        }
        #endregion

        #region Mutators
        /// <summary>
        /// Sets the seed for the given instance from which to generate the
        /// random sequence.
        /// </summary>
        /// <param name="newSeed">New seed.</param>
        public void SetSeed(long newSeed)
        {
            m1 = new short[10];
            m2 = new short[10];

            this.seed = newSeed;

            ushort sn;
            long n;

            sn = (ushort)(this.seed & m16Mask);
            n = this.seed & m31Mask;

            int counter = 0;
            short[] arr = m1;
            for (int i = 18; i > 0; i--)
            {
                n = (long)(30903 * sn) + (n >> 16);

                sn = (ushort)(n & m16Mask);
                arr[counter++] = (short)sn;
                if (i == 9)
                {
                    arr = m2;
                    counter = 0;
                }
            }

            m1[0] &= m15Mask;
            m2[0] &= m15Mask;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Gets a random value between 0 and 1 inclusive.
        /// </summary>
        /// <value>The value.</value>
        public double Value
        {
            get
            {
                long n1, n2;

                for (int i = 8; i > 0; i--)
                {
                    m1[i + 1] = m1[i];
                    m2[i + 1] = m2[i];
                }

                n1 = (long)m1[0];
                n2 = (long)m2[0];

                n1 += (long)(1941 * m1[2]) + (long)(1860 * m1[3]) + (long)(1812 * m1[4])
                    + (long)(1776 * m1[5]) + (long)(1492 * m1[6]) + (long)(1215 * m1[7])
                        + (long)(1066 * m1[8]) + (long)(12013 * m1[9]);

                n2 += (long)(1111 * m2[2]) + (long)(2222 * m2[3]) + (long)(3333 * m2[4])
                    + (long)(4444 * m2[5]) + (long)(5555 * m2[6]) + (long)(6666 * m2[7])
                        + (long)(7777 * m2[8]) + (long)(9272 * m2[9]);

                m1[0] = (short)(n1 / m16Long);
                m2[0] = (short)(n2 / m16Long);

                m1[1] = (short)(m16Mask & n1);
                m2[1] = (short)(m16Mask & n2);

                this.seed = (long)((((long)m1[1]) << 16) + (long)m2[1]);

                return (((double)this.seed) / m32Double) + 0.5;
            }
        }

        /// <summary>
        /// Gets a random point within the range, min-max inclusive.
        /// </summary>
        /// <returns>The range.</returns>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Max.</param>
        public double GetRange(double min, double max)
        {
            if (min > max)
            {
                throw new Exception("max must be greater than min");
            }

            if (min == max)
            {
                return min;
            }

            double diff = max - min;
            // get Abs value
            if (diff < 0)
            {
                diff *= -1;
            }

            return min + Value * diff;
        }

        /// <summary>
        /// Gets the range, min-max inclusive.
        /// </summary>
        /// <returns>The range.</returns>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Max.</param>
        public float GetRange(float min, float max)
        {
            if (min > max)
            {
                throw new Exception("max must be greater than min");
            }

            if (min == max)
            {
                return min;
            }

            float diff = Mathf.Abs(max - min);

            return min + (float)Value * diff;
        }

        /// <summary>
        /// Gets the range, min-max inclusive.
        /// </summary>
        /// <returns>The range.</returns>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Max.</param>
        public int GetRange(int min, int max)
        {
            if (min > max)
            {
                throw new Exception("max must be greater than min");
            }

            if (min == max)
            {
                return min;
            }

            // get the absolute difference between the values
            double val = Value * Mathf.Abs(max - min);
            // get the output as the floor value
            int output = (int)val;
            // store fractional value for rounding
            double fractional = output != 0 ? val % output : val;

            // if the fractional value was half or more, round up
            if (fractional >= 0.5)
            {
                ++output;
            }

            return min + output;
        }

        /// <summary>
        /// Gets a point on the shell of the Unit Circle.
        /// </summary>
        /// <value>The on unit circle.</value>
        public Vector2 OnUnitCircle
        {
            get
            {
                Vector2 output;

                float rad = Mathf.Deg2Rad * GetRange(0f, 360f);

                output = new Vector2(
                    Mathf.Cos(rad),
                    Mathf.Sin(rad)
                );

                return output;
            }
        }

        /// <summary>
        /// Gets a point on the inside of the Unit Circle.
        /// </summary>
        /// <value>The inside unit circle.</value>
        public Vector2 InsideUnitCircle
        {
            get
            {
                return OnUnitCircle * (float)Value;
            }
        }

        /// <summary>
        /// Gets a point on the shell of the Unit Sphere.
        /// </summary>
        /// <value>The on unit sphere.</value>
        public Vector3 OnUnitSphere
        {
            get
            {
                Vector3 output;

                float radTheta = Mathf.Deg2Rad * GetRange(0f, 360f);
                float radPhi = Mathf.Deg2Rad * GetRange(0f, 180f);

                output = new Vector3(
                    Mathf.Cos(radTheta) * Mathf.Sin(radTheta),
                    Mathf.Sin(radTheta) * Mathf.Sin(radTheta),
                    Mathf.Cos(radPhi)
                );

                return output;
            }
        }

        /// <summary>
        /// Gets a point on the inside of a Unit Sphere.
        /// </summary>
        /// <value>The inside unit sphere.</value>
        public Vector3 InsideUnitSphere
        {
            get
            {
                return OnUnitSphere * (float)Value;
            }
        }
        #endregion
    }
}
