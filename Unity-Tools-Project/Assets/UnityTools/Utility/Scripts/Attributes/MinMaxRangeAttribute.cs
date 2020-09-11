namespace UnityTools
{
    /// <summary>
    /// Attribute to be applied to RangedFloat properties
    /// </summary>
    public class MinMaxRangeAttribute : System.Attribute
    {
        public float Min { get; private set; }
        public float Max { get; private set; }

        public MinMaxRangeAttribute(float aMin, float aMax)
        {
            Min = aMin;
            Max = aMax;
        }
    }
}