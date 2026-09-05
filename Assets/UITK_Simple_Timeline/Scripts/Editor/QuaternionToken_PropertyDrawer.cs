#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace UITK_SimpleTimeline.Editor
{
    [CustomPropertyDrawer(typeof(QuaternionToken))]
    public class QuaternionToken_PropertyDrawer : PropertyDrawer
    {
        private bool isQuaternionLocked;
        private SerializedProperty qtProperty;
        private Vector3Field rotationField;
        //why is this 
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            isQuaternionLocked = false;
            qtProperty = property;
            rotationField = new(property.displayName);
            if (property.boxedValue != null) 
            { 
                QuaternionToken qt = (QuaternionToken)property.boxedValue;
                rotationField.value = QuaternionToken.FromToken(qt).eulerAngles; 
            }
            rotationField.TrackPropertyValue(property, (newProp) => // Track Quaternion to Vector3
            {
                if (isQuaternionLocked) { return; }
                isQuaternionLocked = true;
                QuaternionToken qt = (QuaternionToken)property.boxedValue;
                rotationField.value = QuaternionToken.FromToken(qt).eulerAngles;
                isQuaternionLocked = false;
            });
            rotationField.RegisterCallback<KeyDownEvent>(evt => // Track Vector3 to Quaternion on enter press
            {
                if (evt.keyCode != KeyCode.Return) { return; }
                SetQT();
            });
            rotationField.RegisterCallback<BlurEvent>(evt => // Track Vector3 to Quaternion on deselect
            {
                SetQT();
            });
            rotationField.ClearClassList();
            rotationField.style.flexDirection = FlexDirection.Column;

            return rotationField;
        }

        private void SetQT()
        {
            if (isQuaternionLocked) { return; }
            isQuaternionLocked = true;
            qtProperty.boxedValue = new QuaternionToken(Quaternion.Euler(rotationField.value));
            qtProperty.serializedObject.ApplyModifiedProperties();
            isQuaternionLocked = false;
        }
    }
}
#endif