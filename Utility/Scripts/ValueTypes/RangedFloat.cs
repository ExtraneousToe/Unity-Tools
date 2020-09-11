namespace UnityTools
{
    /// <summary>
    /// Ranged float property.
    /// 
    /// Used by the UnityEditor to easily define a min-max float range.
    /// </summary>
    [System.Serializable]
    public struct RangedFloat
    {
        // min value for the range
        public float minValue;
        // max value for the range
        public float maxValue;

        public RangedFloat(float aMinValue, float aMaxValue)
        {
            this.minValue = aMinValue;
            this.maxValue = aMaxValue;
        }

        /// <summary>
        /// Get a random value within the [min, max] range.
        /// </summary>
        /// <returns>The random value.</returns>
        public float GetRandomValue()
        {
            // use the static MOARandom instance to get the random value
            return MOARandom.Instance.GetRange(minValue, maxValue);
        }
    }
}