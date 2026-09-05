#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace UITK_SimpleTimeline.Editor
{
    [CustomPropertyDrawer(typeof(SimpleTimeline))]
    public class SimpleTimeline_PropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(GUILayout.Button("Open Timeline in Editor Window"))
            {
                //open up an editor window
                try
                {
                    SimpleTimeline_EditorWindow.OpenWindow(property, property.serializedObject);
                }
                catch
                {
                    Debug.LogError("Property Drawer doesn't have a valid SimpleTimeline to give Editor Window?!?!?");
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}
#endif