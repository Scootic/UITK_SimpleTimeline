using UnityEngine;
using System.Threading;
namespace UITK_SimpleTimeline
{
    public class SimpleTimelineAnimationComponent : MonoBehaviour
    {
        [SerializeField, Tooltip("Leave empty if you want this GameObject to be the root.")] private GameObject root;
        [SerializeField] private SimpleTimelineAsset[] timelineAssets;
        [Header("Settings")]
        [SerializeField, Tooltip("Decides if the the copies of the SimpleTimelines (using JSONUtility) made in Awake should be done async on a background thread or not.")] private bool asyncCopying = false;
        [SerializeField, Tooltip("Decides if the component should play the timeline at index 0 on start.")] private bool playInitialOnStart = false;
        [SerializeField, Tooltip("Decides if the currently playing timeline should be fully evaluated before swapping to a new one. [Fires TimelineResult()]")] private bool finishAnimationOnSwap = true;
        [SerializeField, Tooltip("Decides if the SimpleTimeline should log EvaluationMessages() while playing.")] private bool debug = false;
        private SimpleTimeline[] timelines;
        private Awaitable curTimeline = null;
        private int curIndex;

        private void Awake()
        {
            if (root == null) root = gameObject;
            timelines = new SimpleTimeline[timelineAssets.Length];
            if (!asyncCopying)
            {
                //should be setting copies? We don't want to assign Scene GameObjects to raw timelineAsset timelines
                //because they might not need them, AND it'd be bad to override other gameobjects.
                for (int i = 0; i < timelines.Length; i++)
                {
                    timelines[i] = new SimpleTimeline(timelineAssets[i].Timeline)
                    {
                        SetSceneObject = root
                    };
                }
            }
            else
            {
                CopyTimelinesAsync();
            }
        }

        void Start()
        {
            if (playInitialOnStart && timelines.Length > 0 && !asyncCopying)
            {
                foreach (SimpleTimeline st in timelines) {
                    Debug.Log($"curve amnt: {st.Curves.Count}");
                }
                PlayTimeline(0);
            }
        }

        private async void CopyTimelinesAsync()
        {
            for(int i = 0; i < timelines.Length; i++)
            {
                timelines[i] = await SimpleTimeline.CopySimpleTimeline(timelineAssets[i].Timeline);
                timelines[i].SetSceneObject = root;

                if(i == 0 && playInitialOnStart)
                {
                    PlayTimeline(0);
                }
            }
        }

        public async void PlayTimeline(int index)
        {
            if (index < timelines.Length && index >= 0)
            {
                if (curTimeline != null && !curTimeline.IsCompleted)
                {
                    curTimeline.Cancel();
                    if (finishAnimationOnSwap) timelines[curIndex].TimelineResult();
                }

                curIndex = index;

                curTimeline = debug? timelines[curIndex].RunThroughTimelineDebug(new CancellationToken()) : timelines[curIndex].RunThroughTimeline(new CancellationToken());
                await curTimeline;
            }
            else
            {
                Debug.LogWarning($"SimpleTimelineAnimationComponent: {name}, was given an index that was outside of timeline array range. ({index})");
            }
        }

        public void HaltCurrentTimeline()
        {
            curTimeline?.Cancel();
        }
    }
}
