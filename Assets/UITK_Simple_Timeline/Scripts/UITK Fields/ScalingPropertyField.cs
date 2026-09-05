#if UNITY_EDITOR
using System.Linq;
using System;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using Helper = UITK_SimpleTimeline.SimpleTimelineUITK_Helper;
namespace UITK_SimpleTimeline
{
    [UxmlElement]
    public partial class ScalingPropertyField : PropertyField
    {
        protected VisualElement subField = null;
        protected readonly float width;
        public ScalingPropertyField()
        {
            subField = null;
            schedule.Execute((Action)delegate { ResizeElements(); }).Until(() => subField != null);
        }

        public ScalingPropertyField(float w)
        {
            style.width = w;
            width = w;

            subField = null;
            schedule.Execute((Action)delegate { ResizeElements(); }).Until(() => subField != null);
        }

        protected void ResizeElementsOnSwap(SerializedPropertyChangeEvent evt)
        {
            //Debug.Log("Resizing!");
            subField = Children().ToArray()[0];
            subField.style.flexDirection = FlexDirection.Column;
            subField.style.flexWrap = Wrap.Wrap;
            subField.style.top = 0;
            subField.style.bottom = 0;
            subField.style.left = 0;
            subField.style.right = 0;
            SerializedProperty newProp = evt.changedProperty;
            //if the property is a struct
            if (newProp.propertyType == SerializedPropertyType.Generic && !newProp.isArray)
            {
                Foldout f = subField.Q<Foldout>();
                f.RemoveFromClassList("unity-foldout");
                f.RemoveFromClassList("unity-foldout--depth-0");

                Toggle t = f.Q<Toggle>();
                t.value = true; //open that boy up by default
                t.ClearClassList();

                VisualElement structHolder = f.Children().ToArray()[1];
                structHolder.RemoveFromClassList("unity-foldout__content");
                structHolder.style.left = 0;
                structHolder.style.right = 0;
                structHolder.style.flexWrap = Wrap.Wrap;
                structHolder.style.flexDirection = FlexDirection.Column;

                foreach(VisualElement babyPF in structHolder.Children())
                {
                    babyPF.style.flexWrap = Wrap.Wrap;
                    babyPF.style.flexDirection = FlexDirection.Column;
                    babyPF.style.left = 0;
                    babyPF.style.right = 15;

                    VisualElement babyPFSub = babyPF.Children().First();
                    babyPFSub.ClearClassList();
                    babyPFSub.style.flexDirection = FlexDirection.Column;

                    Label subbyLabel = babyPFSub.Q<Label>();
                    subbyLabel.style.flexGrow = -1;
                    subbyLabel.style.flexShrink = 1;

                    VisualElement secondary = babyPF.Children().ToArray()[1];
                    secondary.style.flexGrow = 1;
                    secondary.style.flexShrink = -1;
                }
            }
            //if it's just some class.
            else
            {
                Label subLabel = subField.Q<Label>();
                subLabel.style.maxWidth = width - 10;
                subLabel.style.flexGrow = -1;
                subLabel.style.flexShrink = 1;
                subLabel.style.left = 10;
                subLabel.style.right = 10;
                subLabel.style.top = 0;
                subLabel.style.bottom = 0;

                VisualElement dragDropBox = subField.Children().ToArray()[1];
                dragDropBox.style.flexGrow = 1;
                dragDropBox.style.maxWidth = width - 10;
                dragDropBox.style.left = 10;
                dragDropBox.style.right = 10;
                dragDropBox.style.top = 0;
                dragDropBox.style.bottom = 0;
            }
        }
        protected void ResizeElements() //horrid... :(
        {
            try
            {
                subField = Children().ToArray()[0];
                subField.style.flexDirection = FlexDirection.Column;
                subField.style.flexWrap = Wrap.Wrap;
                subField.style.top = 0;
                subField.style.bottom = 0;
                subField.style.left = 0;
                subField.style.right = 0;
            }
            catch
            {
                //no sub field
                return;
            }

            SerializedProperty stinker = Helper.GetBoundProperty(this);
            //if the property is a struct that's a foldout
            //the WORST try-catch nesting in history!
            if (stinker.propertyType == SerializedPropertyType.Generic && !stinker.isArray)
            {
                try
                {
                    Foldout f = subField.Q<Foldout>();
                    f.style.left = 0;
                    f.style.right = 0;
                    f.style.flexWrap = Wrap.Wrap;
                    f.style.flexDirection = FlexDirection.Column;

                    Toggle t = f.Q<Toggle>();
                    t.value = true; //open that boy up by default

                    VisualElement[] childPropertyFields = f.Children().ToArray();

                    foreach (VisualElement babyPF in childPropertyFields)
                    {
                        if (babyPF == t) continue;

                        //god i hope there's no sub-structs!
                        babyPF.style.flexWrap = Wrap.Wrap;
                        babyPF.style.flexDirection = FlexDirection.Column;
                        babyPF.style.left = -15;
                        babyPF.style.right = 15;
                        try
                        {
                            VisualElement babyPFSub = babyPF.Children().First();
                            babyPFSub.style.flexDirection = FlexDirection.Column;

                            try
                            {
                                Label subbyLabel = babyPFSub.Q<Label>();
                                subbyLabel.style.flexGrow = -1;
                                subbyLabel.style.flexShrink = 1;
                            }
                            catch
                            {
                                //no label element?
                            }

                            try
                            {
                                VisualElement secondary = babyPFSub.Children().ToArray()[1];
                                secondary.style.flexGrow = 1;
                                secondary.style.flexShrink = -1;
                                secondary.style.flexDirection = FlexDirection.Column;
                                secondary.style.flexWrap = Wrap.Wrap;
                            }
                            catch
                            {
                                //no secondary sub element?
                            }
                        }
                        catch
                        {
                            //no babyPFSub?!?
                        }
                    }
                }
                catch
                {
                    try
                    {
                        Label subLabel = subField.Q<Label>();
                        subLabel.style.maxWidth = width - 10;
                        subLabel.style.flexGrow = -1;
                        subLabel.style.flexShrink = 1;
                        subLabel.style.left = 10;
                        subLabel.style.right = 10;
                        subLabel.style.top = 0;
                        subLabel.style.bottom = 0;
                    }
                    catch
                    {
                        //no subLabel?
                    }
                    try
                    {
                        VisualElement dragDropBox = subField.Children().ToArray()[1];
                        dragDropBox.style.flexGrow = 1;
                        dragDropBox.style.flexDirection = FlexDirection.Column;
                        dragDropBox.style.maxWidth = width - 10;
                        dragDropBox.style.left = 10;
                        dragDropBox.style.right = 10;
                        dragDropBox.style.top = 0;
                        dragDropBox.style.bottom = 0;
                    }
                    catch
                    {
                        //no drag-drop box???
                    }
                }
            }
            //if it's just some class. no sub-nesting necessary.
            else
            {
                try
                {
                    Label subLabel = subField.Q<Label>();
                    subLabel.style.maxWidth = width - 10;
                    subLabel.style.flexGrow = -1;
                    subLabel.style.flexShrink = 1;
                    subLabel.style.left = 10;
                    subLabel.style.right = 10;
                    subLabel.style.top = 0;
                    subLabel.style.bottom = 0;
                }
                catch
                {
                    //no subLabel?
                }
                try
                {
                    VisualElement dragDropBox = subField.Children().ToArray()[1];
                    dragDropBox.style.flexGrow = 1;
                    dragDropBox.style.maxWidth = width - 10;
                    dragDropBox.style.left = 10;
                    dragDropBox.style.right = 10;
                    dragDropBox.style.top = 0;
                    dragDropBox.style.bottom = 0;
                }
                catch
                {
                    //no drag-drop box???
                }
            }
        }
    }
}
#endif