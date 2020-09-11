using UnityEngine;
using UnityEditor;

namespace UnityTools
{
    /// <summary>
    /// Custom drawer for RangedInt properties
    ///
    /// Shows as a two-ended slider, providing a range within a defined min/max
    /// </summary>
    [CustomPropertyDrawer(typeof(RangedInt), true)]
    public class RangedIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            SerializedProperty minProp = property.FindPropertyRelative("minValue");
            SerializedProperty maxProp = property.FindPropertyRelative("maxValue");

            float minValue = minProp.intValue;
            float maxValue = maxProp.intValue;

            float rangeMin = 0;
            float rangeMax = 1;

            var ranges = (MinMaxIntRangeAttribute[])fieldInfo.GetCustomAttributes(typeof(MinMaxIntRangeAttribute), true);
            if (ranges.Length > 0)
            {
                rangeMin = Mathf.RoundToInt(ranges [0].Min);
                rangeMax = Mathf.RoundToInt(ranges [0].Max);
            }

            const float rangeBoundsLabelWidth = 40f;

            var rangeBoundsLabel1Rect = new Rect(position);
            rangeBoundsLabel1Rect.width = rangeBoundsLabelWidth;
            string numMin = GUI.TextField(rangeBoundsLabel1Rect, minValue.ToString());
            position.xMin += rangeBoundsLabelWidth;

            var rangeBoundsLabel2Rect = new Rect(position);
            rangeBoundsLabel2Rect.xMin = rangeBoundsLabel2Rect.xMax - rangeBoundsLabelWidth;
            string numMax = GUI.TextField(rangeBoundsLabel2Rect, maxValue.ToString());
            position.xMax -= rangeBoundsLabelWidth;

            EditorGUI.BeginChangeCheck();
            EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, rangeMin, rangeMax);
            if (EditorGUI.EndChangeCheck())
            {
                minProp.intValue = Mathf.RoundToInt(minValue);
                maxProp.intValue = Mathf.RoundToInt(maxValue);
            }
            else
            {
                float newMin, newMax;

                if (float.TryParse(numMax, out newMax))
                {
                    maxProp.intValue = Mathf.RoundToInt(Mathf.Clamp(newMax, rangeMin, rangeMax));
                }

                if (float.TryParse(numMin, out newMin))
                {
                    minProp.intValue = Mathf.RoundToInt(Mathf.Clamp(newMin, rangeMin, (float)maxProp.intValue));
                }
            }

            EditorGUI.EndProperty();
        }
    }
}