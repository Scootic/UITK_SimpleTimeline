#if UNITY_EDITOR
using UnityEngine;
using static UnityEditor.AnimationUtility;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace UITK_SimpleTimeline.Editor
{
    [CustomPropertyDrawer(typeof(TimelineKeyframe<>))]
    public class TimelineKeyframe_PropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement ve = new();
            ve.style.flexDirection = FlexDirection.Column;
            ve.style.left = 0;
            ve.style.right = 0;
            ve.style.bottom = 0;
            ve.style.top = 0;

            PropertyField timeField = new();
            timeField.BindProperty(property.FindPropertyRelative("Time"));
            ve.Add(timeField);

            ScalingPropertyField valueField = new(230);
            valueField.BindProperty(property.FindPropertyRelative("Value"));
            ve.Add(valueField);

            PropertyField inSlopeField = new();
            inSlopeField.BindProperty(property.FindPropertyRelative("InSlope"));
            ve.Add(inSlopeField);

            PropertyField outSlopeField = new();
            outSlopeField.BindProperty(property.FindPropertyRelative("OutSlope"));
            ve.Add(outSlopeField);

            PropertyField tangentModeField = new();
            tangentModeField.BindProperty(property.FindPropertyRelative("TangentMode"));
            ve.Add(tangentModeField);

            PropertyField inTangentField = new();
            inTangentField.BindProperty(property.FindPropertyRelative("InTangent"));
            ve.Add(inTangentField);

            PropertyField outTangentField = new();
            outTangentField.BindProperty(property.FindPropertyRelative("OutTangent"));
            ve.Add(outTangentField);

            PropertyField weightModeField = new();
            weightModeField.BindProperty(property.FindPropertyRelative("WeightedMode"));
            ve.Add(weightModeField);

            PropertyField inWeightField = new();
            inWeightField.BindProperty(property.FindPropertyRelative("InWeight"));
            ve.Add(inWeightField);

            PropertyField outWeightField = new();
            outWeightField.BindProperty(property.FindPropertyRelative("OutWeight"));
            ve.Add(outWeightField);

            return ve;
        }
    }
}
#endif