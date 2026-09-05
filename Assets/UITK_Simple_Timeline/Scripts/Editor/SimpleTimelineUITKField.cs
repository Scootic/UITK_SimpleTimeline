#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using AssemblyDatabase = UITK_SimpleTimeline.UITK_SimpleTimeline_AssembliesDatabase;
using Helper = UITK_SimpleTimeline.SimpleTimelineUITK_Helper;
namespace UITK_SimpleTimeline.Editor
{
    [UxmlElement]
    public partial class SimpleTimelineUITKField : BaseField<SimpleTimeline>
    {
        protected readonly VisualElement SimpleTimelineInfoHolder, TimelineHolder, 
            TimelineControlsHolder, ScrollViewContent;
        protected readonly GuidelineVisualElement CurrentTimePreview, EndTimePreview, GrayOverlay;
        protected readonly TimelineRuler TimelineRuler;
        protected readonly TimelineKeyframe_CurveRenderer KeyframeCurvePreview;

        protected readonly IntegerField CurrentFrameField;
        protected readonly FloatField DurationField, CurrentSecondsField;
        protected readonly Toggle LoopField;
        protected readonly Button BackFrame, PlayPause, ForwardFrame, AddKeyframesButton;

        protected static Texture2D bckIco = null, fwdIco = null, playIco = null, pausIco = null, keyfrIco = null;

        [SerializeField] protected SimpleTimeline thisTimeline = new();

        /// <summary>
        /// The scrollable area
        /// </summary>
        protected readonly ScrollView TimelineScrollView, KeyframeControlsHolder;

        /// <summary>
        /// Keyframe Knob property viewer.
        /// </summary>
        protected readonly PropertyField CurrentKeyframeField;
        //? the guys that'll be displayed in the scroll view?
        protected readonly List<VisualElement>TimelineCurveFields = new();
        /// <summary>
        /// Right-click inside TimelineScrollView.
        /// </summary>
        protected GenericMenu AddNewCurveMenu;
        /// <summary>
        /// Is the timeline currently playing in the editor? (Is the red line moving???)
        /// </summary>
        protected bool playing = false;

        protected VisualElement CurTimelineKnob;

        protected float curT = 0, currentZoom = 1f, originalScrollMax;
        protected readonly float minZoom = 0.5f, maxZoom = 3f, zoomStep = 0.1f;
       
        public SimpleTimelineUITKField() : this(null) { }

        public SimpleTimelineUITKField(string labelText) : base(labelText, new VisualElement())
        {
            playing = false;
            curT = 0;
            Helper.ReceiveKeyframe = null;
            Helper.ReceiveKeyframe += DisplayKeyframeInformation;
            style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);
            style.flexDirection = FlexDirection.Column;

            GrabIcons();
            RemoveAt(0); //?
            style.position = Position.Absolute;
            style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);
            style.bottom = 0;
            style.right = 0;
            style.left = 0;
            style.top = 0;
            name = "SimpleTimelineField";

            #region TimelineInfoHolder
            SimpleTimelineInfoHolder = new() { name = "SimpleTimelineInfoHolder" };
            SimpleTimelineInfoHolder.style.backgroundColor = Helper.SecondLayerBG;
            SimpleTimelineInfoHolder.style.borderBottomWidth = 1;
            SimpleTimelineInfoHolder.style.borderBottomColor = Helper.SecondLayerBorder;
            SimpleTimelineInfoHolder.style.borderRightColor = Helper.SecondLayerBorder;
            SimpleTimelineInfoHolder.style.borderRightWidth = 1;
            SimpleTimelineInfoHolder.style.minWidth = 0;
            SimpleTimelineInfoHolder.style.minHeight = 0;
            SimpleTimelineInfoHolder.style.maxHeight = 80;
            SimpleTimelineInfoHolder.style.maxWidth = 235;
            Add(SimpleTimelineInfoHolder);

            DurationField = new("Duration:") { name = "TimelineDuration" };
            DurationField.style.color = Color.white;
            DurationField.style.width = 225;
            DurationField.value = thisTimeline.Duration;
            DurationField.RegisterValueChangedCallback(evt =>
            {
                thisTimeline.Duration = evt.newValue;
                Helper.SimpleTimelineProperty.FindPropertyRelative("Duration").floatValue = evt.newValue;
                UpdateTimelineScrollSizeBasedOnDuration(evt.newValue);
            });
            SimpleTimelineInfoHolder.Add(DurationField);

            LoopField = new("Loop:") { name = "TimelineLoop" };
            LoopField.style.color = Color.white;
            LoopField.style.width = 225;
            LoopField.value = thisTimeline.Loop;
            LoopField.RegisterValueChangedCallback(evt => 
            {
                Helper.SimpleTimelineProperty.FindPropertyRelative("Loop").boolValue = evt.newValue;
                thisTimeline.Loop = evt.newValue; 
            });
            SimpleTimelineInfoHolder.Add(LoopField);
            #endregion

            KeyframeControlsHolder = new() { name = "KeyframeControls" };
            KeyframeControlsHolder.style.backgroundColor = Helper.SecondLayerBG;
            KeyframeControlsHolder.style.borderRightColor = Helper.SecondLayerBorder;
            KeyframeControlsHolder.style.borderRightWidth = 1;
            KeyframeControlsHolder.style.borderTopWidth = 1;
            KeyframeControlsHolder.style.borderTopColor = Helper.SecondLayerBorder;
            KeyframeControlsHolder.style.minWidth = 0;
            KeyframeControlsHolder.style.bottom = 0;
            KeyframeControlsHolder.style.flexGrow = 1;
            KeyframeControlsHolder.style.maxWidth = 235;
            KeyframeControlsHolder.style.minHeight = 100;
            Add(KeyframeControlsHolder);

            CurrentKeyframeField = new() { name = "CurrentKeyframe" };
            KeyframeControlsHolder.Add(CurrentKeyframeField);

            #region TimelineHolder
            //holds all timeline chicanery
            TimelineHolder = new() { name = "TimelineHolder" };
            TimelineHolder.style.backgroundColor = style.backgroundColor;
            TimelineHolder.style.position = Position.Absolute;
            TimelineHolder.style.minHeight = 235;
            TimelineHolder.style.minWidth = 500;
            TimelineHolder.style.left = 235;
            TimelineHolder.style.top = 0;
            TimelineHolder.style.bottom = 0;
            TimelineHolder.style.right = 0;
            TimelineHolder.style.maxWidth = 9999;
            TimelineHolder.style.maxHeight = 9999;
            TimelineHolder.style.flexDirection = FlexDirection.Column;
            Add(TimelineHolder);

            #region TimelineControlsHolder
            //holds the play/pause button and stuff!
            TimelineControlsHolder = new() { name = "TimelineControlsHolder" };
            TimelineControlsHolder.style.backgroundColor = Helper.SecondLayerBG;
            TimelineControlsHolder.style.borderRightColor = Helper.SecondLayerBorder;
            TimelineControlsHolder.style.borderLeftColor = Helper.SecondLayerBorder;
            TimelineControlsHolder.style.borderBottomColor = Helper.SecondLayerBorder;
            TimelineControlsHolder.style.borderRightWidth = 1;
            TimelineControlsHolder.style.borderLeftWidth = 1;
            TimelineControlsHolder.style.borderBottomWidth = 1;
            TimelineControlsHolder.style.flexDirection = FlexDirection.Row;
            TimelineControlsHolder.style.minHeight = 50;
            TimelineControlsHolder.style.minWidth = 500;
            TimelineHolder.Add(TimelineControlsHolder);

            BackFrame = new(() =>
            {
                if (!playing) CurrentFrameField.value = Mathf.Max(CurrentFrameField.value - 1, 0);
            })
            { name = "GoBackAFrame" };
            BackFrame.iconImage = bckIco;
            Image bf = BackFrame.Q<Image>();
            bf.scaleMode = ScaleMode.ScaleToFit;
            bf.style.height = 45;
            bf.style.width = 45;
            BackFrame.style.height = 50;
            BackFrame.style.width = 50;
            TimelineControlsHolder.Add(BackFrame);

            PlayPause = new(() =>
            {
                playing = !playing;
                PlayPause.iconImage = playing ? pausIco : playIco;
            })
            { name = "Play/Pause" };
            PlayPause.iconImage = playIco;
            Image pp = PlayPause.Q<Image>();
            pp.scaleMode = ScaleMode.ScaleToFit;
            pp.style.height = 45;
            pp.style.width = 45;
            PlayPause.style.height = 50;
            PlayPause.style.width = 50;
            TimelineControlsHolder.Add(PlayPause);

            ForwardFrame = new(() =>
            {
                if (!playing) CurrentFrameField.value = Mathf.Min(CurrentFrameField.value + 1, Mathf.FloorToInt(DurationField.value * 60));
            }
            )
            { name = "GoForwardAFrame" };
            ForwardFrame.iconImage = fwdIco;
            Image ff = ForwardFrame.Q<Image>();
            ff.scaleMode = ScaleMode.ScaleToFit;
            ff.style.height = 45;
            ff.style.width = 45;
            ForwardFrame.style.height = 50;
            ForwardFrame.style.width = 50;
            TimelineControlsHolder.Add(ForwardFrame);

            AddKeyframesButton = new(() =>
            {
                if (!playing)
                {
                    for (int i = 0; i < Helper.CurvesProperty.arraySize; i++)
                    {
                        SerializedProperty sp = Helper.CurvesProperty.GetArrayElementAtIndex(i);
                        TimelineCurve c = sp.managedReferenceValue as TimelineCurve;
                        c.AddKeyframeToCurve(curT);
                        sp.managedReferenceValue = c;
                    }
                    Helper.ApplyChangesToObject();
                }
            })
            { name = "AddKeyframesButton" };
            AddKeyframesButton.iconImage = keyfrIco;
            Image ak = AddKeyframesButton.Q<Image>();
            ak.scaleMode = ScaleMode.ScaleToFit;
            ak.style.height = 45;
            ak.style.width = 45;
            AddKeyframesButton.style.height = 50;
            AddKeyframesButton.style.width = 50;
            TimelineControlsHolder.Add(AddKeyframesButton);

            CurrentFrameField = new("Frame:") { name = "CurrentFrameField" };
            CurrentFrameField.style.height = 50;
            CurrentFrameField.focusable = true;
            CurrentFrameField.value = 0;
            CurrentFrameField.isReadOnly = false;
            CurrentFrameField.style.color = Color.white;
            Label l1 = CurrentFrameField.Q<Label>();
            l1.style.width = 50;
            l1.style.minWidth = 50;
            l1.style.unityTextAlign = TextAnchor.MiddleLeft;
            VisualElement v1 = CurrentFrameField.Q<VisualElement>("unity-text-input");
            v1.style.minWidth = 70;
            v1.style.minHeight = 30;
            v1.style.alignSelf = Align.Center;
            CurrentFrameField.RegisterValueChangedCallback(evt =>
            {
                curT = (float)evt.newValue / 60f;
                CurrentSecondsField.value = curT;
                //divide frame by 60, then multiply by PixelWidthPerSeconds?
                CurrentTimePreview.style.left = curT * (float)Helper.PixelWidthPerSeconds;
            });
            TimelineControlsHolder.Add(CurrentFrameField);

            CurrentSecondsField = new("Seconds:") { name = "CurrentSecondsLabel" };
            CurrentSecondsField.style.height = 50;
            CurrentSecondsField.focusable = false;
            CurrentSecondsField.value = 0;
            CurrentSecondsField.isReadOnly = true;
            Label l2 = CurrentSecondsField.Q<Label>();
            l2.style.width = 70;
            l2.style.minWidth = 70;
            l2.style.unityTextAlign = TextAnchor.MiddleLeft;
            VisualElement v2 = CurrentSecondsField.Q<VisualElement>("unity-text-input");
            v2.style.minWidth = 70;
            v2.style.minHeight = 30;
            v2.style.alignSelf = Align.Center;
            CurrentSecondsField.style.color = Color.white;
            TimelineControlsHolder.Add(CurrentSecondsField);
            #endregion

            CreateAddNewCurveMenu();
            TimelineScrollView = new() { name = "TimelineScrollView" };
            TimelineScrollView.style.backgroundColor = Helper.SecondLayerBorder;
            TimelineScrollView.style.minHeight = 235;
            TimelineScrollView.style.maxHeight = 9999;
            TimelineScrollView.style.minWidth = 500;
            TimelineScrollView.style.maxWidth = 9999;
            TimelineScrollView.style.bottom = 0;
            TimelineScrollView.style.right = 0;
            TimelineScrollView.style.flexDirection = FlexDirection.Column;
            TimelineScrollView.style.flexGrow = 1;
            TimelineScrollView.horizontalScroller.lowValue = 0;
            TimelineScrollView.horizontalScroller.highValue = 650;
            originalScrollMax = TimelineScrollView.horizontalScroller.highValue;
            ScrollViewContent = TimelineScrollView.Q<VisualElement>("unity-content-container");
            ScrollViewContent.style.backgroundImage = Helper.FullRulerLength;
            ScrollViewContent.style.unityBackgroundImageTintColor = Helper.QuarterTransparentWhite;
            ScrollViewContent.style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Left, 0f));
            ScrollViewContent.style.backgroundRepeat = new BackgroundRepeat(Repeat.Repeat, Repeat.Repeat);
            ScrollViewContent.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(32, 32));
            ScrollViewContent.style.flexGrow = 1;
            ScrollViewContent.style.right = -160f;
            ScrollViewContent.RegisterCallback<WheelEvent>(evt =>
            {
                if (!evt.ctrlKey) return;

                if (evt.delta.y < 0) currentZoom += zoomStep;
                else if (evt.delta.y > 0) currentZoom -= zoomStep;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
                ScrollViewContent.transform.scale = new Vector3(currentZoom, 1, 1);
                TimelineScrollView.horizontalScroller.highValue = originalScrollMax / currentZoom;
                //float ogValue = TimelineScrollView.horizontalScroller.value;
                //TimelineScrollView.horizontalScroller.value = ogValue * currentZoom;
                evt.StopPropagation();
            });
            TimelineScrollView.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1) //if right click
                {
                    AddNewCurveMenu.ShowAsContext();
                    evt.StopPropagation();
                }
            });
            TimelineHolder.Add(TimelineScrollView);

            TimelineRuler = new() { name = "TimelineRuler" };
            TimelineRuler.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (((evt.pressedButtons & 1) == 1) && !playing) //do the red ruler drag that sets cur time!
                {
                    //x seconds = localPos.x / PixelWidthPerSeconds. multiply by 60 for frame? floor???
                    CurrentFrameField.value = Mathf.FloorToInt(evt.localPosition.x /
                        (float)Helper.PixelWidthPerSeconds * 60f);
                    evt.StopPropagation();
                }
            });
            TimelineScrollView.Add(TimelineRuler);

            GrayOverlay = new(50f, Helper.QuarterTransparentBlack) { name = "GrayOverlay" };
            TimelineScrollView.Add(GrayOverlay);

            EndTimePreview = new(Color.cyan) { name = "EndTimePreview" };
            GrayOverlay.Add(EndTimePreview);

            CurrentTimePreview = new(Color.red) { name = "TimePreview" };
            TimelineScrollView.Add(CurrentTimePreview);

            #endregion
            //GenerateTimelineCurveFields(); no timelinecurves to generate with!

            schedule.Execute(PreviewTimelineUpdate).Every(16).StartingIn(0);
        }

        public SimpleTimelineUITKField(string labelText, SimpleTimeline st) : base(labelText, new VisualElement())
        {
            thisTimeline = (SimpleTimeline)Helper.SimpleTimelineProperty.boxedValue;
            //Helper.RemoveTimelineCurve = null;
            Helper.RemoveTimelineCurve += RemoveTimelineCurve; 
            playing = false;
            curT = 0;
            //Helper.ReceiveKeyframe = null;
            Helper.ReceiveKeyframe += DisplayKeyframeInformation;
            style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);
            style.flexDirection = FlexDirection.Column;

            GrabIcons();
            RemoveAt(0); //?
            style.position = Position.Absolute;
            style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);
            style.bottom = 0;
            style.right = 0;
            style.left = 0;
            style.top = 0;
            name = "SimpleTimelineField";

            #region TimelineInfoHolder
            SimpleTimelineInfoHolder = new() { name = "SimpleTimelineInfoHolder" };
            SimpleTimelineInfoHolder.style.backgroundColor = Helper.SecondLayerBG;
            SimpleTimelineInfoHolder.style.borderBottomWidth = 1;
            SimpleTimelineInfoHolder.style.borderBottomColor = Helper.SecondLayerBorder;
            SimpleTimelineInfoHolder.style.borderRightColor = Helper.SecondLayerBorder;
            SimpleTimelineInfoHolder.style.borderRightWidth = 1;
            SimpleTimelineInfoHolder.style.minWidth = 0;
            SimpleTimelineInfoHolder.style.minHeight = 0;
            SimpleTimelineInfoHolder.style.maxHeight = 80;
            SimpleTimelineInfoHolder.style.maxWidth = 235;
            Add(SimpleTimelineInfoHolder);

            DurationField = new("Duration:") { name = "TimelineDuration" };
            DurationField.style.color = Color.white;
            DurationField.style.width = 225;
            DurationField.value = Helper.SimpleTimelineProperty.FindPropertyRelative("Duration").floatValue;
            DurationField.RegisterValueChangedCallback(evt =>
            {
                thisTimeline.Duration = evt.newValue;
                Helper.SimpleTimelineProperty.FindPropertyRelative("Duration").floatValue = evt.newValue;
                Helper.WindowObject.ApplyModifiedProperties();
                UpdateTimelineScrollSizeBasedOnDuration(evt.newValue);
            });
            SimpleTimelineInfoHolder.Add(DurationField);

            LoopField = new("Loop:") { name = "TimelineLoop" };
            LoopField.style.color = Color.white;
            LoopField.style.width = 225;
            LoopField.value = Helper.SimpleTimelineProperty.FindPropertyRelative("Loop").boolValue;
            LoopField.RegisterValueChangedCallback(evt => 
            {
                Helper.SimpleTimelineProperty.FindPropertyRelative("Loop").boolValue = evt.newValue;
                Helper.WindowObject.ApplyModifiedProperties();
                thisTimeline.Loop = evt.newValue; 
            });
            SimpleTimelineInfoHolder.Add(LoopField);
            #endregion

            KeyframeControlsHolder = new() { name = "KeyframeControls" };
            KeyframeControlsHolder.style.backgroundColor = Helper.SecondLayerBG;
            KeyframeControlsHolder.style.borderRightColor = Helper.SecondLayerBorder;
            KeyframeControlsHolder.style.borderRightWidth = 1;
            KeyframeControlsHolder.style.borderTopWidth = 1;
            KeyframeControlsHolder.style.borderTopColor = Helper.SecondLayerBorder;
            KeyframeControlsHolder.style.minWidth = 0;
            KeyframeControlsHolder.style.bottom = 0;
            KeyframeControlsHolder.style.flexGrow = 1;
            KeyframeControlsHolder.style.maxWidth = 235;
            KeyframeControlsHolder.style.minHeight = 100;
            Add(KeyframeControlsHolder);

            CurrentKeyframeField = new() { name = "CurrentKeyframe" };
            CurrentKeyframeField.RemoveFromClassList(alignedFieldUssClassName);
            CurrentKeyframeField.style.flexWrap = Wrap.Wrap;
            KeyframeControlsHolder.Add(CurrentKeyframeField);

            KeyframeCurvePreview = new() { name = "KeyframeCurvePreview"};
            KeyframeControlsHolder.Add(KeyframeCurvePreview);

            #region TimelineHolder
            //holds all timeline chicanery
            TimelineHolder = new() { name = "TimelineHolder" };
            TimelineHolder.style.backgroundColor = style.backgroundColor;
            TimelineHolder.style.position = Position.Absolute;
            TimelineHolder.style.minHeight = 235;
            TimelineHolder.style.minWidth = 500;
            TimelineHolder.style.left = 235;
            TimelineHolder.style.top = 0;
            TimelineHolder.style.bottom = 0;
            TimelineHolder.style.right = 0;
            TimelineHolder.style.maxWidth = 9999;
            TimelineHolder.style.maxHeight = 9999;
            TimelineHolder.style.flexDirection = FlexDirection.Column;
            Add(TimelineHolder);

            #region TimelineControlsHolder
            //holds the play/pause button and stuff!
            TimelineControlsHolder = new() { name = "TimelineControlsHolder" };
            TimelineControlsHolder.style.backgroundColor = Helper.SecondLayerBG;
            TimelineControlsHolder.style.borderRightColor = Helper.SecondLayerBorder;
            TimelineControlsHolder.style.borderLeftColor = Helper.SecondLayerBorder;
            TimelineControlsHolder.style.borderBottomColor = Helper.SecondLayerBorder;
            TimelineControlsHolder.style.borderRightWidth = 1;
            TimelineControlsHolder.style.borderLeftWidth = 1;
            TimelineControlsHolder.style.borderBottomWidth = 1;
            TimelineControlsHolder.style.flexDirection = FlexDirection.Row;
            TimelineControlsHolder.style.minHeight = 50;
            TimelineControlsHolder.style.minWidth = 500;
            TimelineHolder.Add(TimelineControlsHolder);

            BackFrame = new(() =>
            {
                if (!playing) CurrentFrameField.value = Mathf.Max(CurrentFrameField.value - 1,0);
            }){ name = "GoBackAFrame" };
            BackFrame.iconImage = bckIco;
            Image bf = BackFrame.Q<Image>();
            bf.scaleMode = ScaleMode.ScaleToFit;
            bf.style.height = 45;
            bf.style.width = 45;
            BackFrame.style.height = 50;
            BackFrame.style.width = 50;
            TimelineControlsHolder.Add(BackFrame);

            PlayPause = new(() =>
            {
                playing = !playing;
                PlayPause.iconImage = playing ? pausIco : playIco;
                if (!playing) thisTimeline.TimelineInitial();
            }){ name = "Play/Pause" };
            PlayPause.iconImage = playIco;
            Image pp = PlayPause.Q<Image>();
            pp.scaleMode = ScaleMode.ScaleToFit;
            pp.style.height = 45;
            pp.style.width = 45;
            PlayPause.style.height = 50;
            PlayPause.style.width = 50;
            TimelineControlsHolder.Add(PlayPause);

            ForwardFrame = new(() => 
            { 
                if (!playing) CurrentFrameField.value = Mathf.Min(CurrentFrameField.value + 1, Mathf.FloorToInt(DurationField.value * 60)); 
            }
            ){ name = "GoForwardAFrame" };
            ForwardFrame.iconImage = fwdIco;
            Image ff = ForwardFrame.Q<Image>();
            ff.scaleMode = ScaleMode.ScaleToFit;
            ff.style.height = 45;
            ff.style.width = 45;
            ForwardFrame.style.height = 50;
            ForwardFrame.style.width = 50;
            TimelineControlsHolder.Add(ForwardFrame);

            AddKeyframesButton = new(() =>
            {
                if (!playing)
                {
                    //Debug.Log("Placing keyframes in existing curves!");
                    for(int i = 0; i < Helper.CurvesProperty.arraySize; i++)
                    {
                        SerializedProperty sp = Helper.CurvesProperty.GetArrayElementAtIndex(i);
                        (sp.boxedValue as TimelineCurve).AddKeyframeToCurve(curT);
                    }
                    Helper.ApplyChangesToObject();
                    foreach (VisualElement ve in TimelineCurveFields) 
                    {
                        IRegeneratableElement ire = ve as IRegeneratableElement;
                        ire?.RegenerateElement();
                    }
                }
            })
            { name = "AddKeyframesButton" };
            AddKeyframesButton.iconImage = keyfrIco;
            Image ak = AddKeyframesButton.Q<Image>();
            ak.scaleMode = ScaleMode.ScaleToFit;
            ak.style.height = 45;
            ak.style.width = 45;
            AddKeyframesButton.style.height = 50;
            AddKeyframesButton.style.width = 50;
            TimelineControlsHolder.Add(AddKeyframesButton);

            CurrentFrameField = new("Frame:") { name = "CurrentFrameField" };
            CurrentFrameField.style.height = 50;
            CurrentFrameField.focusable = true;
            CurrentFrameField.value = 0;
            CurrentFrameField.isReadOnly = false;
            CurrentFrameField.style.color = Color.white;
            Label l1 = CurrentFrameField.Q<Label>();
            l1.style.width = 50;
            l1.style.minWidth = 50;
            l1.style.unityTextAlign = TextAnchor.MiddleLeft;
            VisualElement v1 = CurrentFrameField.Q<VisualElement>("unity-text-input");
            v1.style.minWidth = 70;
            v1.style.minHeight = 30;
            v1.style.alignSelf = Align.Center;
            CurrentFrameField.RegisterValueChangedCallback(evt =>
            {
                curT = (float)evt.newValue / 60f;
                CurrentSecondsField.value = curT;
                //divide frame by 60, then multiply by PixelWidthPerSeconds?
                CurrentTimePreview.style.left = curT * (float)Helper.PixelWidthPerSeconds;
            });
            TimelineControlsHolder.Add(CurrentFrameField);

            CurrentSecondsField = new("Seconds:") { name = "CurrentSecondsLabel" };
            CurrentSecondsField.style.height = 50;
            CurrentSecondsField.focusable = false;
            CurrentSecondsField.value = 0;
            CurrentSecondsField.isReadOnly = true;
            Label l2 = CurrentSecondsField.Q<Label>();
            l2.style.width = 70;
            l2.style.minWidth = 70;
            l2.style.unityTextAlign = TextAnchor.MiddleLeft;
            VisualElement v2 = CurrentSecondsField.Q<VisualElement>("unity-text-input");
            v2.style.minWidth = 70;
            v2.style.minHeight = 30;
            v2.style.alignSelf = Align.Center;
            CurrentSecondsField.style.color = Color.white;
            TimelineControlsHolder.Add(CurrentSecondsField);
            #endregion

            CreateAddNewCurveMenu();
            TimelineScrollView = new() { name = "TimelineScrollView" };
            TimelineScrollView.style.backgroundColor = Helper.SecondLayerBorder;
            TimelineScrollView.style.minHeight = 235;
            TimelineScrollView.style.maxHeight = 9999;
            TimelineScrollView.style.minWidth = 500;
            TimelineScrollView.style.maxWidth = 9999;
            TimelineScrollView.style.bottom = 0;
            TimelineScrollView.style.right = 0;
            TimelineScrollView.style.flexDirection = FlexDirection.Column;
            TimelineScrollView.style.flexGrow = 1;
            TimelineScrollView.horizontalScroller.lowValue = 0;
            TimelineScrollView.horizontalScroller.highValue = 650;
            originalScrollMax = TimelineScrollView.horizontalScroller.highValue;
            ScrollViewContent = TimelineScrollView.Q<VisualElement>("unity-content-container");
            ScrollViewContent.style.backgroundImage = Helper.FullRulerLength;
            ScrollViewContent.style.unityBackgroundImageTintColor = Helper.QuarterTransparentWhite;
            ScrollViewContent.style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Left, 0f));
            ScrollViewContent.style.backgroundRepeat = new BackgroundRepeat(Repeat.Repeat, Repeat.Repeat);
            ScrollViewContent.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(32, 32));
            ScrollViewContent.style.flexGrow = 1;
            ScrollViewContent.style.right = -160f;
            ScrollViewContent.RegisterCallback<WheelEvent>(evt =>
            {
                if (!evt.ctrlKey) return;

                if (evt.delta.y < 0) currentZoom += zoomStep;
                else if(evt.delta.y > 0) currentZoom -= zoomStep;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
                ScrollViewContent.transform.scale = new Vector3(currentZoom, 1, 1);
                TimelineScrollView.horizontalScroller.highValue = originalScrollMax / currentZoom;
                //float ogValue = TimelineScrollView.horizontalScroller.value;
                //TimelineScrollView.horizontalScroller.value = ogValue * currentZoom;
                evt.StopPropagation();
            });
            TimelineScrollView.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1) //if right click
                {
                    AddNewCurveMenu.ShowAsContext();
                }
            });
            TimelineHolder.Add(TimelineScrollView);

            TimelineRuler = new() { name = "TimelineRuler" };
            TimelineRuler.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (((evt.pressedButtons & 1) == 1) && !playing) //do the red ruler drag that sets cur time!
                {
                    int frameValue = Mathf.FloorToInt(evt.localPosition.x / Helper.PixelWidthPerSeconds * 60f);
                    frameValue = Mathf.Clamp(frameValue, 0, Mathf.FloorToInt(Helper.SimpleTimelineProperty.FindPropertyRelative("Duration").floatValue * 60f));
                    CurrentFrameField.value = frameValue;
                }
            });
            TimelineScrollView.Add(TimelineRuler);

            GrayOverlay = new(50f, Helper.QuarterTransparentBlack) { name = "GrayOverlay" };
            TimelineScrollView.Add(GrayOverlay);

            EndTimePreview = new(Color.cyan) { name = "EndTimePreview" };
            GrayOverlay.Add(EndTimePreview);

            CurrentTimePreview = new(Color.red) { name = "TimePreview" };
            TimelineScrollView.Add(CurrentTimePreview);

            #endregion
            GenerateTimelineCurveFields();
            UpdateTimelineScrollSizeBasedOnDuration(Helper.SimpleTimelineProperty.FindPropertyRelative("Duration").floatValue);

            schedule.Execute(PreviewTimelineUpdate).Every(16).StartingIn(0);
        }
        //makes the timelineruler area grow based on the new duration.
        protected void UpdateTimelineScrollSizeBasedOnDuration(float newDurInSeconds)
        {
            //reset cur frame field
            CurrentFrameField.value = 0;
            CurrentSecondsField.value = 0;
            float newVal = newDurInSeconds * Helper.PixelWidthPerSeconds;
            GrayOverlay.style.left = newVal;
            ScrollViewContent.style.width = newVal + 50;
            Helper.MaxPixelWidth = newVal;
            for(int i = 0; i < Helper.CurvesProperty.arraySize; i++)
            {
                TimelineCurve c = Helper.CurvesProperty.GetArrayElementAtIndex(i).managedReferenceValue as TimelineCurve;
                c.CleanOutKeyframesAfterTime(newDurInSeconds);
                Helper.CurvesProperty.GetArrayElementAtIndex(i).managedReferenceValue = c;
            }
            Helper.ApplyChangesToObject();
        }

        public void AddNewTimelineCurve(TimelineCurve newCurve) 
        {
            if (thisTimeline.Curves == null) thisTimeline.Curves = new();
            thisTimeline.Curves.Add(newCurve);
            UpdateCurvesProperty();
        }

        public void RemoveTimelineCurve(TimelineCurve toRemove)
        {
            if (thisTimeline.Curves.Remove(toRemove))
            {
                CurrentKeyframeField.Unbind();
                UpdateCurvesProperty();
            }
            else
            {
                Debug.LogWarning($"Timeline.Curves doesn't contain {toRemove.ToString()}?!?");
            }
        }
        //update the serializedproperties whenever we add/remove a timelinecurve from the field
        protected void UpdateCurvesProperty()
        {
            try
            {
                Helper.CurvesProperty.ClearArray();
                Helper.CurvesProperty.arraySize = thisTimeline.Curves.Count;
                for (int i = 0; i < thisTimeline.Curves.Count; i++)
                {
                    Helper.CurvesProperty.GetArrayElementAtIndex(i).managedReferenceValue = thisTimeline.Curves[i];
                }
                Helper.ApplyChangesToObject();
                GenerateTimelineCurveFields();
            }
            catch
            {
                Debug.LogWarning("UpdateCurvesProperty went wrong. SOMEHOW?!?!");
            }
        }
        //spawn all of them stinkin' TimelineCurveFields
        protected void GenerateTimelineCurveFields()
        {
            foreach(VisualElement curveField in TimelineCurveFields)
            {
                try
                {
                    if(curveField.name != "TimePreview") curveField.RemoveFromHierarchy();
                }
                catch
                {
                    Debug.LogWarning($"Curve Field ({curveField.name}) not a child element?!?");
                    continue;
                }
            }
            TimelineCurveFields.Clear();
            if (Helper.CurvesProperty == null) return;
            for(int i = 0; i < Helper.CurvesProperty.arraySize; i++)
            {
                try 
                { 
                    TimelineCurve lerpable = Helper.CurvesProperty.GetArrayElementAtIndex(i).managedReferenceValue as TimelineCurve;
                    VisualElement rep = lerpable.UITKRepresentation(i);
                    TimelineScrollView.Add(rep); //adding to the timeline scrollview should place it in the content section? i hope?
                    TimelineCurveFields.Add(rep);//? i at 0 should be timeline ruler
                }
                catch
                {
                    Debug.LogWarning("Null lerpable?!? That, or the TimelineCurveField failed to construct, somehow.");
                }
            }
            GrayOverlay.BringToFront();
            CurrentTimePreview.BringToFront();
            MarkDirtyRepaint();
        }
        //sets the current TimelineKeyframe<T> to edit and current timeline knob to affect based on those values.
        protected void DisplayKeyframeInformation(SerializedProperty keyframeToDisplay, VisualElement ve)
        {
            Helper.ApplyChangesToObject();
            CurrentKeyframeField.UnregisterCallback<SerializedPropertyChangeEvent>(AdjustCurrentKeyframeBasedOnTime);
            CurrentKeyframeField.Unbind();
            CurTimelineKnob = ve;
            CurrentKeyframeField.BindProperty(keyframeToDisplay);
            CurrentKeyframeField.RegisterCallback<SerializedPropertyChangeEvent>(AdjustCurrentKeyframeBasedOnTime);
            int curveI = keyframeToDisplay.FindPropertyRelative("CurveIndex").intValue;
            int curveK = keyframeToDisplay.FindPropertyRelative("KeyframeIndex").intValue;
            SerializedProperty toRight = null;
            SerializedProperty keyframeList = Helper.CurvesProperty.GetArrayElementAtIndex(curveI).FindPropertyRelative("keyframes");
            if (curveK < keyframeList.arraySize - 1)
            {
                toRight = keyframeList.GetArrayElementAtIndex(curveK + 1);
                KeyframeCurvePreview.ReceiveKeyframes(keyframeToDisplay, toRight);
            }else KeyframeCurvePreview.ReceiveKeyframes(keyframeToDisplay, null);
        }
        //basically the same method as dragging, except it happens when you change the Time value directly in the property
        protected void AdjustCurrentKeyframeBasedOnTime(SerializedPropertyChangeEvent evt)
        {
            if (evt.changedProperty.displayName != "Time") return;
            evt.changedProperty.floatValue = Mathf.Clamp(evt.changedProperty.floatValue, 0, Helper.SimpleTimelineProperty.FindPropertyRelative("Duration").floatValue);
            float t = evt.changedProperty.floatValue;
            float timelineKnobHalfSize = CurTimelineKnob.style.width.value.value * 0.5f;
            float x = t * Helper.PixelWidthPerSeconds - timelineKnobHalfSize;
            x = Mathf.Clamp(x, -timelineKnobHalfSize, Helper.MaxPixelWidth);
            Vector3 newPos = new(x, CurTimelineKnob.transform.position.y, CurTimelineKnob.transform.position.z);
            CurTimelineKnob.transform.position = newPos;
            Helper.ApplyChangesToObject();
        }

        protected void CreateAddNewCurveMenu()
        {
            AddNewCurveMenu = new();
            if (AssemblyDatabase.GetValidTimelineCurveTypes == null) 
            {
                AddNewCurveMenu.AddDisabledItem(new GUIContent("No Valid Curve Types?!? Try checking the SimpleTimelineUITK_AssemblyDatabase."));
                return; 
            }

            foreach (Type t in AssemblyDatabase.GetValidTimelineCurveTypes)
            {
                //this mans should always be a stinkin' TimelineCurve. (God I hope...)
                //Debug.Log($"Adding type {t.Name} to curve menu!");
                object c = Activator.CreateInstance(t);
                AddNewCurveMenu.AddItem(new GUIContent($"Add New {c}"), false, delegate
                {
                    TimelineCurve curve = c as TimelineCurve;
                    AddNewTimelineCurve(curve);
                });
            }
        }
        //try to get all of the Texture2Ds if the field is missing any one of them
        protected void GrabIcons()
        {
            if (bckIco == null) bckIco = AssetDatabase.LoadAssetAtPath<Texture2D>(Helper.EditorIconAssetPath+ "/bckicon.png");
            if (!bckIco) Debug.LogError("SimpleTimelineUITKField.cs can't find bckicon.png. Did you move the UITK_Simple_Timeline folder from the root Asset folder?");
            if (fwdIco == null) fwdIco = AssetDatabase.LoadAssetAtPath<Texture2D>(Helper.EditorIconAssetPath+"/fwdicon.png");
            if (!fwdIco) Debug.LogError("SimpleTimelineUITKField.cs can't find fwdicon.png. Did you move the UITK_Simple_Timeline folder from the root Asset folder?");
            if (playIco == null) playIco = AssetDatabase.LoadAssetAtPath<Texture2D>(Helper.EditorIconAssetPath+"/playicon.png");
            if (!playIco) Debug.LogError("SimpleTimelineUITKField.cs can't find playicon.png. Did you move the UITK_Simple_Timeline folder from the root Asset folder?");
            if (pausIco == null) pausIco = AssetDatabase.LoadAssetAtPath<Texture2D>(Helper.EditorIconAssetPath+"/pauseicon.png");
            if (!pausIco) Debug.LogError("SimpleTimelineUITKField.cs can't find pauseicon.png. Did you move the UITK_Simple_Timeline folder from the root Asset folder?");
            if (keyfrIco == null) keyfrIco = AssetDatabase.LoadAssetAtPath<Texture2D>(Helper.EditorIconAssetPath + "/placekeyframeicon.png");
            if (!keyfrIco) Debug.LogError("SimpleTimelineUITKField.cs can't find placekeyframeicon.png. Did you move the UITK_Simple_Timeline folder from the root Asset folder?");
            if (Helper.FullRulerLength == null) Helper.FullRulerLength = AssetDatabase.LoadAssetAtPath<Texture2D>(Helper.EditorIconAssetPath+"/fullrulerlength.png");
            if (!Helper.FullRulerLength) Debug.LogError("SimpleTimelineUITKField.cs can't find fullrulerlength.png. Did you move the UITK_Simple_Timeline folder from the root Asset folder?");
        }
        //the same as the animation tab's preview, except the live playback because that would be some serious hogwash
        protected void PreviewTimelineUpdate()
        {
            if (!playing) return;
            if (curT >= DurationField.value)
            {
                curT = 0;
                CurrentFrameField.value = 0;
                if (!LoopField.value)
                {
                    CurrentFrameField.value = 0;
                    playing = false;
                    return;
                }
                else CurrentFrameField.value = -1;
            }
            //count up every frame and do debug.logs?!

            CurrentFrameField.value++;

            string msg = "";
            foreach(TimelineCurve curve in thisTimeline.Curves)
            {
                msg += $"\n{curve.EvaluateMessage(curT)}";
            }
            Debug.Log(msg);
        }
    }
}
#endif