#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Helper = UITK_SimpleTimeline.SimpleTimelineUITK_Helper;
namespace UITK_SimpleTimeline
{
    [UxmlElement]
    public partial class TimelineKnob<T> : BaseField<TimelineKeyframe<T>>
    {
        protected static TimelineKnob<T> clipboard = null;
        protected static Texture2D knobImage = null;
        protected readonly float halfwayOffset;
        protected PointerMoveEvent dragKnob;
        public Action DeleteKnobAction;
        public SerializedProperty KnobProperty;
        
        public GenericMenu DeleteMe
        {
            get
            {
                GenericMenu toReturn = new();

                toReturn.AddItem(new GUIContent($"Delete Knob at {value.Time}s."), false, delegate
                {
                    DeleteKnobAction.Invoke();
                });

                toReturn.AddSeparator("Clipboard");
                toReturn.AddItem(new GUIContent($"Copy Knob at {value.Time}s."), false, delegate { clipboard = this; });
                if (clipboard == null) toReturn.AddDisabledItem(new GUIContent($"No knob to paste."));
                else toReturn.AddItem(new GUIContent($"Paste Knob from {value.Time}s."), false, delegate
                {
                    float oldTime = value.Time;
                    TimelineKeyframe<T> toSet = clipboard.value;
                    toSet.Time = oldTime;

                    value = toSet;
                });

                return toReturn;
            }
        }

        public TimelineKnob() : this(null) { }
        public TimelineKnob(string labelText) : base(labelText, new VisualElement())
        {
            if (knobImage == null) knobImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UITK_Simple_Timeline/UITK_SimpleTimelineIcons/timelinediamond.png");
            if (knobImage == null) Debug.LogError("TimelineKnob.cs can't find timelinediamond.png. Did you move the UITK_Simple_Timeline from your root Asset folder?");
            style.backgroundImage = knobImage;
            style.position = Position.Absolute;
            style.height = 35f;
            style.width = 35f;
        }
        public TimelineKnob(string labelText, SerializedProperty knobProperty, int kindex, int cindex) : base(labelText, new VisualElement())
        {
            value = knobProperty.boxedValue as TimelineKeyframe<T>;
            if (knobImage == null) knobImage = AssetDatabase.LoadAssetAtPath<Texture2D>(Helper.EditorIconAssetPath + "/timelinediamond.png");
            if (knobImage == null) Debug.LogError("TimelineKnob.cs can't find timelinediamond.png. Did you move the UITK_Simple_Timeline from your root Asset folder?");
            style.backgroundImage = knobImage;
            style.position = Position.Absolute;
            style.height = 35f;
            style.width = 35f;
            style.top = 15;
            halfwayOffset = (style.width.value.value * 0.5f) - 2;
            KnobProperty = knobProperty;
            KnobProperty.FindPropertyRelative("KeyframeIndex").intValue = kindex;
            KnobProperty.FindPropertyRelative("CurveIndex").intValue = cindex;
            Helper.ApplyChangesToObject();
            RegisterCallback<PointerMoveEvent>(DragKnob);
            Helper.ReceiveKeyframe += SelectKnobColoring;
        }

        protected void SelectKnobColoring(SerializedProperty sp, VisualElement ve)
        {
            if(sp == KnobProperty)
            {
                style.unityBackgroundImageTintColor = Helper.SelectedKeyframe;
                BringToFront();
            }
            else
            {
                style.unityBackgroundImageTintColor = Color.white;
            }
        }
        protected void DragKnob(PointerMoveEvent pme)
        {
            if ((pme.pressedButtons & 1) == 1 && style.unityBackgroundImageTintColor == Helper.SelectedKeyframe)
            {
                Vector3 curPos = transform.position;
                //assuming that origin is the center of the element?
                float newX = Mathf.Clamp(curPos.x + pme.deltaPosition.x, -halfwayOffset, Helper.MaxPixelWidth - halfwayOffset - 3);
                newX = (float)Math.Round(newX, 1);
                curPos = new Vector3(newX, curPos.y, curPos.z);
                transform.position = curPos;
                float newTime = (curPos.x + halfwayOffset + 2) / Helper.PixelWidthPerSeconds;
                //super duper make sure time is clamped. good god...
                newTime = Mathf.Clamp(newTime, 0, Helper.SimpleTimelineProperty.FindPropertyRelative("Duration").floatValue);
                KnobProperty.FindPropertyRelative("Time").floatValue = newTime;
                Helper.ApplyChangesToObject();
                value = KnobProperty.boxedValue as TimelineKeyframe<T>;
            }
        }
    }
}
#endif