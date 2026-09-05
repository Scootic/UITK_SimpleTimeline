#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace UITK_SimpleTimeline.Editor
{
    [UxmlElement]
    public partial class TimelineRuler : VisualElement
    {
        private static Sprite rulerImage = null;
        private static readonly Color bgColor = new (0.4f, 0.4f, 0.4f, 1);

        public TimelineRuler() : this(null) { }

        public TimelineRuler(string labelText)
        {
            GrabRulerImage();

            style.left = 0;
            style.right = 0;
            style.height = 25;
            style.backgroundColor = bgColor;
            style.backgroundImage = rulerImage.texture;
            style.unitySliceType = SliceType.Tiled;
            style.unitySliceBottom = 17;
            style.unitySliceScale = 1;
        }

        private void GrabRulerImage()
        {
            if (rulerImage != null) return;
            rulerImage = AssetDatabase.LoadAssetAtPath<Sprite>(SimpleTimelineUITK_Helper.EditorIconAssetPath+"/rulerimage.png");
            if (!rulerImage) Debug.LogError("TimelineRuler.cs couldn't find rulerimage.png. Did you move the UITK_Simple_Timeline folder from the root Asset folder?");
        }
    }
}
#endif