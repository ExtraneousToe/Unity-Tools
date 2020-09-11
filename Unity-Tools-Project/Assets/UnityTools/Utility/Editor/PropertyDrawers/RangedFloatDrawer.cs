using UnityEngine;
using UnityEditor;

namespace UnityTools
{
    /// <summary>
    /// Custom drawer for RangedFloat properties
    ///
    /// Shows as a two-ended slider, providing a range within a defined min/max
    /// </summary>
    [CustomPropertyDrawer(typeof(RangedFloat), true)]
    public class RangedFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            SerializedProperty minProp = property.FindPropertyRelative("minValue");
            SerializedProperty maxProp = property.FindPropertyRelative("maxValue");

            float minValue = minProp.floatValue;
            float maxValue = maxProp.floatValue;

            float rangeMin = 0;
            float rangeMax = 1;

            var ranges = (MinMaxRangeAttribute[])fieldInfo.GetCustomAttributes(typeof(MinMaxRangeAttribute), true);
            if (ranges.Length > 0)
            {
                rangeMin = ranges [0].Min;
                rangeMax = ranges [0].Max;
            }

            const float rangeBoundsLabelWidth = 40f;

            var rangeBoundsLabel1Rect = new Rect(position);
            rangeBoundsLabel1Rect.width = rangeBoundsLabelWidth;
            string numMin = GUI.TextField(rangeBoundsLabel1Rect, minValue.ToString("F2"));
            position.xMin += rangeBoundsLabelWidth;

            var rangeBoundsLabel2Rect = new Rect(position);
            rangeBoundsLabel2Rect.xMin = rangeBoundsLabel2Rect.xMax - rangeBoundsLabelWidth;
            string numMax = GUI.TextField(rangeBoundsLabel2Rect, maxValue.ToString("F2"));
            position.xMax -= rangeBoundsLabelWidth;

            EditorGUI.BeginChangeCheck();
            EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, rangeMin, rangeMax);
            if (EditorGUI.EndChangeCheck())
            {
                minProp.floatValue = minValue;
                maxProp.floatValue = maxValue;
            }
            else
            {
                float newMin, newMax;

                if (float.TryParse(numMax, out newMax))
                {
                    maxProp.floatValue = Mathf.Clamp(newMax, rangeMin, rangeMax);
                }

                if (float.TryParse(numMin, out newMin))
                {
                    minProp.floatValue = Mathf.Clamp(newMin, rangeMin, maxProp.floatValue);
                }
            }

            EditorGUI.EndProperty();
        }
    }
}