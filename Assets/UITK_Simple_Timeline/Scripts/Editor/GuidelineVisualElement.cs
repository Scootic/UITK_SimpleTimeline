using UnityEngine;
using UnityEngine.UIElements;

namespace UITK_SimpleTimeline.Editor
{
    /// <summary>
    /// Type of line/block that spans all the way from the bottom to the top. <br/><br/>
    /// Adjust the GuidelineVisualElement's style.left to move the guideline around its space.
    /// </summary>
    [UxmlElement]
    public partial class GuidelineVisualElement : VisualElement
    {
        public GuidelineVisualElement() 
        {
            style.backgroundColor = Color.white;
            style.width = 1f;
            style.maxWidth = 1f;
            style.minHeight = 500;
            //style.maxHeight = 99999;
            style.top = 0;
            style.bottom = 0;
            style.position = Position.Absolute;
            style.left = 0f;//this is the boy that gets adjusted when draggening?
            focusable = false;
            pickingMode = PickingMode.Ignore;
        }
        public GuidelineVisualElement(float width) 
        {
            style.backgroundColor = Color.white;
            style.width = width;
            style.maxWidth = width;
            style.minHeight = 500;
            //style.maxHeight = 99999;
            style.top = 0;
            style.bottom = 0;
            style.position = Position.Absolute;
            style.left = 0f;//this is the boy that gets adjusted when draggening?
            focusable = false;
            pickingMode = PickingMode.Ignore;
        }
        public GuidelineVisualElement(Color c) 
        {
            style.backgroundColor = c;
            style.width = 1f;
            style.maxWidth = 1f;
            style.minHeight = 500;
           //style.maxHeight = 99999;
            style.top = 0;
            style.bottom = 0;
            style.position = Position.Absolute;
            style.left = 0f;//this is the boy that gets adjusted when draggening?
            focusable = false;
            pickingMode = PickingMode.Ignore;
        }
        public GuidelineVisualElement(float width, Color c) 
        {
            style.backgroundColor = c;
            style.width = width;
            style.maxWidth = width;
            style.minHeight = 500;
            //style.maxHeight = 99999;
            style.top = 0;
            style.bottom = 0;
            style.position = Position.Absolute;
            style.left = 0f;//this is the boy that gets adjusted when draggening?
            focusable = false;
            pickingMode = PickingMode.Ignore;
        }
    }
}
