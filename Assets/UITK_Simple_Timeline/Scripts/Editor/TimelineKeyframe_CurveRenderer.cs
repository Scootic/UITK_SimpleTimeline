#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Helper = UITK_SimpleTimeline.SimpleTimelineUITK_Helper;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace UITK_SimpleTimeline.Editor
{
    [UxmlElement]
    public partial class TimelineKeyframe_CurveRenderer : VisualElement, IRegeneratableElement
    {
        private const int lineResolution = 50;
        SerializedProperty l = null, r = null;
        private TangentHandle outTanL, inTanR;
        private static Painter2D Painter;
        private Label label;
        private VisualElement tracker;

        private readonly FloatField tangentStrengthField;

        private TimelineKeyframe Left => l.boxedValue as TimelineKeyframe;
        private TimelineKeyframe Right => r.boxedValue as TimelineKeyframe;

        public TimelineKeyframe_CurveRenderer()
        {
            style.borderBottomColor = Helper.ThirdLayerBorder;
            style.borderRightColor = Helper.ThirdLayerBorder;
            style.borderLeftColor = Helper.ThirdLayerBorder;
            style.borderTopColor = Helper.ThirdLayerBorder;
            style.height = Length.Percent(100);
            style.width = Length.Percent(100);
            style.flexGrow = 1;
            style.flexShrink = -1;
            style.borderTopWidth = 1;
            style.borderRightWidth = 1;
            style.borderLeftWidth = 1;
            style.borderBottomWidth = 1;
            style.backgroundColor = Helper.SecondLayerBorder;

            tangentStrengthField = new("Tangent Handles Strength")
            {
                tooltip = "Defines how large the maximum/minimum tangent is whenever a handle is at a maxed out position.",
                value = EditorPrefs.GetFloat("uitk_tangentfieldstrength", 1f)
            };
            tangentStrengthField.RegisterValueChangedCallback(evt => { EditorPrefs.SetFloat("uitk_tangentfieldstrength", evt.newValue); });
            tangentStrengthField.style.top = Length.Percent(100f);
            tangentStrengthField.style.bottom = -20f;
            Add(tangentStrengthField);

            ReceiveKeyframes(null,null);
        }

        public void ReceiveKeyframes(SerializedProperty newL, SerializedProperty newR)
        {
            //no matter what, clean up the stinkin' tangent handles.
            if (outTanL != null)
            {
                Remove(outTanL);
                outTanL = null;
            }
            if (inTanR != null)
            {
                Remove(inTanR);
                inTanR = null;
            }

            if (newL != null && newR != null)
            {
                tangentStrengthField.visible = true;
                if (tracker != null) { Remove(tracker); tracker = null; }

                if (label != null)
                {
                    Remove(label);
                    label = null;
                }
                l = newL;
                r = newR;

                tracker = new VisualElement();
                tracker.style.height = 0;
                tracker.style.width = 0;
                Add(tracker);

                outTanL = new(new Vector2(0, style.height.value.value),0f, GetLAngle(), "Handles the out-tangent for left keyframe (the selected one).");
                outTanL.GiveCurAngle += ReceiveLAngle;
                inTanR = new(new Vector2(style.width.value.value * 2f, style.height.value.value),180f, GetRAngle(), "Handles the in-tangent for right keyframe (the one to right of selected one).");
                inTanR.GiveCurAngle += ReceiveRAngle;

                Add(outTanL);
                Add(inTanR);

                tracker.TrackPropertyValue(l, callback => { RegenerateElement(); });
                tracker.TrackPropertyValue(r, callback => { RegenerateElement(); });
                RegenerateElement();
            }
            else
            {
                tangentStrengthField.visible = false;
                style.backgroundImage = null;
                style.minHeight = 50f;

                if(Painter != null)
                {
                    Painter.Clear();
                    Painter.Dispose();
                    Painter = null;
                }
                if (label == null)
                {
                    label = new();
                    label.style.whiteSpace = WhiteSpace.Normal;
                    label.style.flexWrap = Wrap.Wrap;

                    if(newL == null)
                    {
                        label.text = "Select a keyframe to preview its curve.";
                    }else if(newR == null)
                    {
                        label.text = "No keyframe to the right of selected keyframe.";
                    }
                    Add(label);
                }
            }
        }

        public void RegenerateElement()
        {
            float minY = 0;
            float maxY = 1;
            if (l == null || r == null)
            {
                return;
            }
            if (Painter != null) { Painter.Clear(); Painter.Dispose(); }
            Painter = new();
            Painter.lineWidth = 1f;
            Painter.lineCap = LineCap.Round;
            Painter.lineJoin = LineJoin.Round;
            Painter.strokeColor = Helper.NeonGreen;
            Painter.BeginPath();
            float[] tangents = CurveMath.GetTangents(new TimelineKeyframe[]{Left,Right});
            Painter.MoveTo(Vector2.zero);
            for(int i = 0; i < lineResolution; i++)
            {
                float percT = (float)i / (float)lineResolution;
                Vector2 v2 = new(percT, CurveMath.NormalizedCubicHermiteSpline(percT, tangents[0], tangents[1], Left.TangentMode) * -1);
                maxY = Mathf.Max(maxY, v2.y * -1);
                minY = Mathf.Min(minY, v2.y * -1);
                v2 *= 200f;
                Painter.LineTo(v2);
            }
            Painter.Stroke();
            VectorImage goose = ScriptableObject.CreateInstance<VectorImage>();
            Painter.SaveToVectorImage(goose);

            float difference = maxY - minY;

            style.backgroundImage = new StyleBackground(Background.FromVectorImage(goose));
#pragma warning disable CS0618
            style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
#pragma warning restore
            style.minHeight = difference;
            style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Top,minY);
        }

        private void ReceiveLAngle(float angle)
        {
            //-90 is positive, 90 is negative
            float angleNormalized = angle / -90f;
            l.FindPropertyRelative("OutTangent").floatValue = angleNormalized * tangentStrengthField.value;
            Helper.ApplyChangesToObject();
        }

        private float GetLAngle()
        {
            float tangent = l.FindPropertyRelative("OutTangent").floatValue;
            float angleNormalized = tangent / tangentStrengthField.value;
            return Mathf.Clamp(angleNormalized * -90f, -90f, 90f);
        }

        private void ReceiveRAngle(float angle)
        {
            //270 is positive, 90 is negative
            float angleNormalized = (angle - 180) / 90f;
            r.FindPropertyRelative("InTangent").floatValue = angleNormalized * tangentStrengthField.value;
            Helper.ApplyChangesToObject();
        }

        private float GetRAngle()
        {
            float tangent = r.FindPropertyRelative("InTangent").floatValue;
            float angleNormalized = tangent / tangentStrengthField.value;
            return Mathf.Clamp(angleNormalized * 90f + 180f, 90f, 270f);
        }
    }
}
#endif