using UnityEngine;

namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Only exists as a way to store SimpleTimelines. You don't necessarily need one of these; if you want to store
    /// SimpleTimelines in some other type of ScriptableObject, you'd just need to add an accessible SimpleTimeline to it.
    /// (Either public or [SerializeField].)<br/><br/>The SimpleTimeline PropertyDrawer should give you the button you need
    /// to open the SimpleTimeline_EditorWindow.
    /// </summary>
    [CreateAssetMenu(menuName = "UITK_SimpleTimeline/SimpleTimeline Asset")]
    public class SimpleTimelineAsset : ScriptableObject
    {
        public SimpleTimeline Timeline = new(5);

        public SimpleTimelineAsset(SimpleTimeline timeline)
        {
            Timeline = timeline;
        }
    }
}
