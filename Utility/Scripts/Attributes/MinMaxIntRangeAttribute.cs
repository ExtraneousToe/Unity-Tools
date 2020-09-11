namespace UnityTools
{
    /// <summary>
    /// Attribute to be applied to RangedInt properties
    /// </summary>
    public class MinMaxIntRangeAttribute : System.Attribute
    {
        public int Min { get; private set; }
        public int Max { get; private set; }

        public MinMaxIntRangeAttribute(int aMin, int aMax)
        {
            Min = aMin;
            Max = aMax;
        }
    }
}