#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Helper class that... helps.
    /// </summary>
    public static class SimpleTimelineUITK_Helper 
    {
        /// <summary>
        /// The amount of pixels that equals one second along the TimelineRuler
        /// </summary>
        public static readonly float PixelWidthPerSeconds = 32f; //?
        /// <summary>
        /// The expected maximum width of the TimelineRuler. Should be set whenever you resize that element
        /// by setting a new duration.
        /// </summary>
        public static float MaxPixelWidth;

        public static readonly Color SecondLayerBG = new(0.2f, 0.2f, 0.2f, 1);
        public static readonly Color SecondLayerBorder = new(0.1f, 0.1f, 0.1f, 1);
        public static readonly Color ThirdLayerBorder = new(0.4f,0.4f,0.4f,1);
        public static readonly Color SelectedKeyframe = new(0.7f, 0.95f, 0, 1);
        public static readonly Color NeonGreen = new(0,1,0, 1f);
        public static readonly Color HalfTransparentWhite = new(1, 1, 1, 0.5f);
        public static readonly Color QuarterTransparentBlack = new(0, 0, 0, 0.25f);
        public static readonly Color QuarterTransparentWhite = new(1, 1, 1, 0.25f);

        /// <summary>
        /// The asset path to load all of the editor icon assets. Change this if you, for some reason, move
        /// the EditorIcons folder from outside of the UITK_Simple_Timeline folder, or otherwise nest it.
        /// <br/><br/>
        /// Should include the Assets folder as the root of the format: Assets/.../UITK_Simple_Timeline/EditorIcons
        /// </summary>
        public static readonly string EditorIconAssetPath = "Assets/UITK_Simple_Timeline/EditorIcons";
        public static readonly StyleLength Auto = new(StyleKeyword.Auto);
        /// <summary>
        /// Get a current keyframe whenever you click on a TimelineKnob in the Editor Window
        /// </summary>
        public static Action<SerializedProperty, VisualElement> ReceiveKeyframe;
        /// <summary>
        /// Fires when you right-click delete a TimelineCurve from the Editor Window
        /// </summary>
        public static Action<TimelineCurve> RemoveTimelineCurve;
        /// <summary>
        /// Texture used by the TimelineScrollView's content to be a slightly grayed out underlay to extend the ruler's
        /// measurements down without being too obtrusive. (Hopefully.)
        /// </summary>
        public static Texture2D FullRulerLength = null;

        public static SerializedObject WindowObject;
        public static SerializedProperty SimpleTimelineProperty, CurvesProperty;

        /// <summary>
        /// Do every single function that's relevant to saving and updating all of the SerializedProperties so that your
        /// data is actually saved after you're done with the Editor Window.
        /// </summary>
        public static void ApplyChangesToObject()
        {
            try
            {
                WindowObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(WindowObject.targetObject);
                WindowObject.Update();
            }
            catch
            {
                Debug.LogWarning("The SimpleTimelineUITK_Helper doesn't have a WindowObject! Domain probably reloaded, try again.");
            }
        }

        /// <summary>
        /// Extract the SerializedProperty out of a PropertyField using Reflection, because Unity is so racist you can't just do that.
        /// </summary>
        /// <param name="field">The PropertyField you want to get the SerializedProperty out of.</param>
        /// <returns>The bound SerializedProperty (m_SerializedProperty in the PropertyField class). Null if the process fails.</returns>
        public static SerializedProperty GetBoundProperty(this PropertyField field)
        {
            SerializedProperty toReturn;
            FieldInfo spField;
            BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;
            try
            {
                spField = typeof(PropertyField).GetField("m_SerializedProperty", bf);
            }
            catch
            {
                Debug.LogWarning("FieldInfo couldn't grab the stinkin' m_SerializedProperty?!?!");
                return null;
            }
            try
            {
                toReturn = spField.GetValue(field) as SerializedProperty;
            }
            catch
            {
                Debug.LogWarning("Given PropertyField can't handle getting its m_SerializedProperty read?!");
                return null;
            }

            return toReturn;
        }

        public static bool IsSubclassOfGenericType(this Type toCheck, Type baseType)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                Type cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;

                if (baseType == cur) return true;

                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static Type[] GetValidTimelineCurveTypes()
        {
            Assembly aToCheck = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "UITKSimpleTimeline_Curves");

            return aToCheck.GetTypes().Where(a => a.IsSubclassOfGenericType(typeof(TypedTimelineCurve<,>)) && !a.IsAbstract).ToArray();
        }
    }

}
#endif