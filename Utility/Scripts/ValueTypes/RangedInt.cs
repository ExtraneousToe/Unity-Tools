namespace UnityTools
{
    /// <summary>
    /// Ranged float property.
    /// 
    /// Used by the UnityEditor to easily define a min-max int range.
    /// </summary>
    [System.Serializable]
    public struct RangedInt
    {
        // min value for the range
        public int minValue;
        // max value for the range
        public int maxValue;

        public RangedInt(int aMinValue, int aMaxValue)
        {
            this.minValue = aMinValue;
            this.maxValue = aMaxValue;
        }

        /// <summary>
        /// Get a random value within the [min, max] range.
        /// </summary>
        /// <returns>The random value.</returns>
        public int GetRandomValue()
        {
            // use the static MOARandom instance to get the random value
            return MOARandom.Instance.GetRange(minValue, maxValue);
        }
    }
}