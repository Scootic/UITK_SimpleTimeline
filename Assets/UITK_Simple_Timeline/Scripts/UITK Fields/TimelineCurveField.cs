#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Helper = UITK_SimpleTimeline.SimpleTimelineUITK_Helper;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// guh
    /// </summary>
    /// <typeparam name="T">The values that are lerped</typeparam>
    /// <typeparam name="U">The object that is affected</typeparam>
    [UxmlElement]
    public partial class TimelineCurveField<T,U> : BaseField<TypedTimelineCurve<T,U>>, IRegeneratableElement where U : notnull
    {
        //public Action DeleteMeAction;
        protected readonly Dictionary<float,TimelineKnob<T>> KeyframeIcons;
        
        protected readonly VisualElement CurveDataContainer, KeyframeContainer;
        protected VisualElement o, w;
        protected readonly Label TypeLabel;
        /// <summary>
        /// Should hold the data for the TimelineCurve's U value.
        /// </summary>
        protected readonly PropertyField ToBeAffectedField;
        protected readonly PropertyField WrapModeField;
        protected GenericMenu AddNewKeyframeMenu, DeleteCurveMenu;

        protected SerializedProperty curveProperty, keyframesProperty;
        protected readonly int myPropertyIndex;

        public TimelineCurveField() : this(null) { }
        //grumpus constructor that's bad!
        public TimelineCurveField(string labelText) : base(labelText, new VisualElement())
        {
            //value = curve;
            //myPropertyIndex = index;
            Remove(Children().ToArray()[0]);
            KeyframeIcons = new();
            style.height = 150;
            style.right = 0;
            style.left = -155;
            style.top = 0;
            style.bottom = Helper.Auto;
            style.flexGrow = 1;
            style.flexShrink = -1;
            style.maxHeight = 150;
            style.width = Helper.Auto;
            style.minWidth = 150;
            style.maxWidth = 9999999999;
            style.backgroundColor = Helper.SecondLayerBG;
            style.borderBottomColor = Helper.SecondLayerBorder;
            style.borderLeftColor = Helper.SecondLayerBorder;
            style.borderTopColor = Helper.SecondLayerBorder;
            style.borderRightColor = Helper.SecondLayerBorder;
            style.borderRightWidth = 1;
            style.borderBottomWidth = 1;
            style.borderTopWidth = 1;
            style.borderLeftWidth = 1;
            style.flexDirection = FlexDirection.Row;
            style.position = Position.Absolute;

            CurveDataContainer = new() { name = "CurveDataContainer" };
            CurveDataContainer.style.width = 150;
            CurveDataContainer.style.minWidth = 150;
            CurveDataContainer.style.maxWidth = 150;
            CurveDataContainer.style.left = 0;
            CurveDataContainer.style.right = 5;
            CurveDataContainer.style.height = 150;
            CurveDataContainer.style.flexGrow = 0;
            CurveDataContainer.style.flexShrink = 1;
            CurveDataContainer.style.backgroundColor = Helper.SecondLayerBG;
            CurveDataContainer.style.borderBottomColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderRightColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderTopColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderLeftColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderBottomWidth = 1;
            CurveDataContainer.style.borderRightWidth = 1;
            CurveDataContainer.style.borderTopWidth = 1;
            CurveDataContainer.style.borderLeftWidth = 1;
            Add(CurveDataContainer);

            ToBeAffectedField = new() { name = "ToBeAffectedField" };
            //curveProperty = Helper.CurvesProperty.GetArrayElementAtIndex(index);
            ToBeAffectedField.style.width = 150;
            ToBeAffectedField.style.height = 150;
            //ToBeAffectedField.BindProperty(curveProperty.FindPropertyRelative("ToAffect")); //?

            ToBeAffectedField.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                ResizeLabel();
            });

            CurveDataContainer.Add(ToBeAffectedField);

            KeyframeContainer = new() { name = "KeyframeContainer" };
            KeyframeContainer.style.left = 151;
            KeyframeContainer.style.right = -150;
            KeyframeContainer.style.height = 150;
            KeyframeContainer.style.maxHeight = 150;
            KeyframeContainer.style.position = Position.Absolute;
            KeyframeContainer.style.backgroundColor = Helper.SecondLayerBorder;
            KeyframeContainer.style.backgroundImage = Helper.FullRulerLength;
            KeyframeContainer.style.unityBackgroundImageTintColor = Helper.HalfTransparentWhite;
            KeyframeContainer.style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Left, 0f));
            KeyframeContainer.style.backgroundRepeat = new BackgroundRepeat(Repeat.Repeat, Repeat.Repeat);
            KeyframeContainer.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(32, 32));
            KeyframeContainer.style.flexGrow = 1;
            KeyframeContainer.style.flexShrink = -1;
            Add(KeyframeContainer);

            SpawnKeyframeKnobs();
            RegisterGenericMenus();
        }
        public TimelineCurveField(string labelText, TypedTimelineCurve<T, U> curve, int index) : base(labelText, new VisualElement())
        {
            value = curve;
            myPropertyIndex = index;
            Remove(Children().ToArray()[0]);
            KeyframeIcons = new();
            style.height = 150;
            style.right = 0;
            style.left = -155;
            style.top = 150 * index + 25;
            style.bottom = Helper.Auto;
            style.flexGrow = 1;
            style.flexShrink = -1;
            style.maxHeight = 150;
            style.width = Helper.Auto;
            style.minWidth = 150;
            style.maxWidth = 9999999999;
            style.backgroundColor = Helper.SecondLayerBG;
            style.borderBottomColor = Helper.SecondLayerBorder;
            style.borderLeftColor = Helper.SecondLayerBorder;
            style.borderTopColor = Helper.SecondLayerBorder;
            style.borderRightColor = Helper.SecondLayerBorder;
            style.borderRightWidth = 1;
            style.borderBottomWidth = 1;
            style.borderTopWidth = 1;
            style.borderLeftWidth = 1;
            style.flexDirection = FlexDirection.Row;
            style.position = Position.Absolute;

            CurveDataContainer = new() { name = "CurveDataContainer" };
            CurveDataContainer.style.width = 160;
            CurveDataContainer.style.minWidth = 160;
            CurveDataContainer.style.maxWidth = 160;
            CurveDataContainer.style.left = -25;
            CurveDataContainer.style.right = 25;
            CurveDataContainer.style.height = 150;
            CurveDataContainer.style.flexGrow = 1;
            CurveDataContainer.style.flexShrink = 1;
            CurveDataContainer.style.backgroundColor = Helper.SecondLayerBG;
            CurveDataContainer.style.borderBottomColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderRightColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderTopColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderLeftColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderBottomWidth = 1;
            CurveDataContainer.style.borderRightWidth = 1;
            CurveDataContainer.style.borderTopWidth = 1;
            CurveDataContainer.style.borderLeftWidth = 1;
            Add(CurveDataContainer);

            TypeLabel = new() { name = "TypeLabel"};
            TypeLabel.text = value.ShorthandCurveName();
            TypeLabel.style.left = 25;
            TypeLabel.style.right = -25;
            TypeLabel.style.flexWrap = Wrap.Wrap;
            TypeLabel.style.maxHeight = 30f;
            TypeLabel.style.maxWidth = 125;
            TypeLabel.style.whiteSpace = WhiteSpace.Normal;
            CurveDataContainer.Add(TypeLabel);

            ToBeAffectedField = new() { name = "ToBeAffectedField" };
            ToBeAffectedField.RemoveFromClassList(alignedFieldUssClassName);
            curveProperty = Helper.CurvesProperty.GetArrayElementAtIndex(index);
            keyframesProperty = curveProperty.FindPropertyRelative("keyframes");
            ToBeAffectedField.style.width = 125;
            ToBeAffectedField.style.height = 75;
            ToBeAffectedField.style.left = 25;
            ToBeAffectedField.style.right = -25;
            ToBeAffectedField.style.flexWrap = Wrap.Wrap;
            ToBeAffectedField.style.flexGrow = 1;
            CurveDataContainer.Add(ToBeAffectedField);
            ToBeAffectedField.BindProperty(curveProperty.FindPropertyRelative("ToAffect")); //?

            WrapModeField = new() { name = "WrapModeField" };
            WrapModeField.RemoveFromClassList(alignedFieldUssClassName);
            WrapModeField.style.width = 125;
            WrapModeField.style.left = 25;
            WrapModeField.style.right = -25;
            WrapModeField.style.height = 75;
            WrapModeField.style.flexGrow = 1;
            CurveDataContainer.Add(WrapModeField);
            WrapModeField.BindProperty(curveProperty.FindPropertyRelative("WrappingMode"));

            KeyframeContainer = new() { name = "KeyframeContainer" };
            KeyframeContainer.style.left = 151;
            KeyframeContainer.style.right = -150;
            KeyframeContainer.style.height = 150;
            KeyframeContainer.style.maxHeight = 150;
            KeyframeContainer.style.position = Position.Absolute;
            KeyframeContainer.style.backgroundColor = Helper.SecondLayerBorder;
            KeyframeContainer.style.backgroundImage = Helper.FullRulerLength;
            KeyframeContainer.style.unityBackgroundImageTintColor = Helper.HalfTransparentWhite;
            KeyframeContainer.style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Left, 0f));
            KeyframeContainer.style.backgroundRepeat = new BackgroundRepeat(Repeat.Repeat, Repeat.Repeat);
            KeyframeContainer.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(32, 32));
            KeyframeContainer.style.flexGrow = 1;
            KeyframeContainer.style.flexShrink = -1;
            Add(KeyframeContainer);

            SpawnKeyframeKnobs();
            RegisterGenericMenus();
            MarkDirtyRepaint();

            schedule.Execute(ResizeLabel).Until(() => o != null && w != null);
        }

        public void RegenerateElement()
        {
            //remove all pre-existing timelineknobs before adding them again
            foreach (KeyValuePair<float, TimelineKnob<T>> kvp in KeyframeIcons)
            {
                KeyframeContainer.Remove(kvp.Value);
            }
            KeyframeIcons.Clear();
            //add knobs lmao
            SpawnKeyframeKnobs();
        }

        protected void SpawnKeyframeKnobs()
        {
            if (keyframesProperty == null) return;

            for(int i = 0; i < keyframesProperty.arraySize; i++)
            {
                TimelineKnob<T> tKnob = new("", keyframesProperty.GetArrayElementAtIndex(i), i, myPropertyIndex);
                float time = tKnob.value.Time;
                tKnob.transform.position = new Vector3(Helper.PixelWidthPerSeconds * time - tKnob.style.width.value.value * 0.5f - 2, 0, 0);
                tKnob.DeleteKnobAction += delegate
                {
                    KeyframeIcons[time].RemoveFromHierarchy();
                    KeyframeIcons.Remove(time);
                    TypedTimelineCurve<T, U> t = curveProperty.boxedValue as TypedTimelineCurve<T, U>;
                    t.RemoveKeyframeFromCurve(time);
                    curveProperty.boxedValue = t;
                };
                tKnob.RegisterCallback<PointerDownEvent>(evt => //by some divine mercy, works.
                {
                    if (evt.button == 1) tKnob.DeleteMe.ShowAsContext();
                    else if (evt.button == 0) 
                    { 
                        Helper.ReceiveKeyframe?.Invoke(tKnob.KnobProperty, tKnob); 
                    }
                    evt.StopPropagation();
                });
                KeyframeIcons.Add(time, tKnob);
                KeyframeContainer.Add(tKnob);
            }
        }

        protected void RegisterGenericMenus()
        {
            KeyframeContainer.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1) //if right click, spawn and set generic menu, get time to add based on mouse pos?
                {
                    float tToAddAt = evt.localPosition.x / Helper.PixelWidthPerSeconds;
                    tToAddAt = (float)Math.Round(tToAddAt,1, MidpointRounding.AwayFromZero);
                    AddNewKeyframeMenu = new();
                    AddNewKeyframeMenu.AddItem(new GUIContent($"Add Keyframe ({value.SpawnKeyframeName()}) at {tToAddAt}"), false, delegate
                    {
                        AddKeyframeAtTime(tToAddAt);
                    });
                    AddNewKeyframeMenu.AddSeparator("");
                    AddNewKeyframeMenu.AddItem(new GUIContent($"Closest Two Keyframes at {tToAddAt}"), false, delegate
                    {
                        TypedTimelineCurve<T,U> me = curveProperty.boxedValue as TypedTimelineCurve<T,U>;
                        int[] array = me.ClosestTwoIndexes(tToAddAt);
                        TimelineKeyframe<T> zero = me.Keyframes[array[0]];
                        TimelineKeyframe<T> one = me.Keyframes[array[1]];
                        Debug.Log($"The closest keyframes at {tToAddAt} are: {array[0]} - {zero.Time} and {array[1]} - {one.Time}");
                    });
                    AddNewKeyframeMenu.ShowAsContext();
                    evt.StopPropagation();
                }
            });

            CurveDataContainer.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1)
                {
                    DeleteCurveMenu = new();
                    DeleteCurveMenu.AddItem(new GUIContent($"Delete Curve {myPropertyIndex}: {value.ShorthandCurveName()}?!?"), false, delegate
                    {
                        Helper.RemoveTimelineCurve?.Invoke(curveProperty.boxedValue as TimelineCurve);
                    });
                    DeleteCurveMenu.ShowAsContext();
                   
                    evt.StopPropagation();
                }
            });
        }

        public void AddKeyframeAtTime(float t)
        {
            (curveProperty.boxedValue as TypedTimelineCurve<T, U>).AddKeyframeToCurve(t);
            Helper.ApplyChangesToObject();
            MarkDirtyRepaint();
            RegenerateElement();
        }

        protected void ResizeLabel()
        {
            try
            {
                o = ToBeAffectedField.Children().ToArray()[0];
                o.style.flexDirection = FlexDirection.Column;
                o.style.flexWrap = Wrap.Wrap;
                Label l = o.Q<Label>();
                l.text = value.ToAffectName();
                l.style.maxWidth = 120;
                l.style.minWidth = 50;
                l.style.flexGrow = -1;
                l.style.flexShrink = 1;
                VisualElement v = o.Children().ToArray()[1];
                v.style.flexGrow = 1;
                v.style.minHeight = 17;
                v.style.minWidth = 90;
                v.style.maxWidth = 120;

                w = WrapModeField.Children().ToArray()[0];
                w.style.flexDirection = FlexDirection.Column;
                w.style.flexWrap = Wrap.Wrap;
                Label l2 = w.Q<Label>();
                l2.style.maxWidth = 120;
                l2.style.minWidth = 50;
                l2.style.flexGrow = -1;
                l2.style.flexShrink = -1;
                VisualElement v2 = w.Children().ToArray()[1];
                v2.style.flexGrow = 1;
                v2.style.minHeight = 17;
                v2.style.minWidth = 90;
                v2.style.maxWidth = 120;
            }
            catch
            {
                //Log in-case you have dire expectations. Will always fire because of how the stupid PropertyFields work,
                //but if the code actually works it will stop eventually and not just run forever.
                ///Debug.LogWarning("Stupid ah label couldnae be found from ToBeAffectedField >:(");
            }
        }
    }
}
#endif