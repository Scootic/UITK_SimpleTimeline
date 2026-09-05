using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// AnimationClip-style struct to handle animating things based on data that isn't in-scene (but also some in-scene things, too).
    /// </summary>
    [Serializable]
    public struct SimpleTimeline : IDefaultableNotNull<SimpleTimeline>
    {
        public SimpleTimeline(float duration)
        {
            Loop = false;
            Duration = duration;
            sceneObject = null;
            Curves = new();
        }
        /// <summary>
        /// Copies the curves using SimpleTimelineExtensions' CopyClassListValuesThroughReflection
        /// </summary>
        /// <param name="copy"></param>
        public SimpleTimeline(SimpleTimeline copy)
        {
            Loop = copy.Loop;
            Duration = copy.Duration;
            sceneObject = null;
            Curves = SimpleTimelineExtensions.DeepCopyListFromJSON(copy.Curves);
        }

        public static async Awaitable<SimpleTimeline> CopySimpleTimeline(SimpleTimeline copy)
        {
            return new()
            {
                Duration = copy.Duration,
                Loop = copy.Loop,
                sceneObject = null,
                Curves = await SimpleTimelineExtensions.DeepCopyListFromJSONAsync(copy.Curves)
            };

        }

        public readonly SimpleTimeline Default()
        {
            SimpleTimeline d = new();

            d.Loop = false;
            d.Duration = 10;
            d.Curves = new();

            return d;
        }

        public bool Loop;
        [Min(0)]public float Duration;

        private GameObject sceneObject;
        /// <summary>
        /// Should only be used by the SimpleTimelineComponent, or another Scene-based script.
        /// </summary>
        public GameObject SetSceneObject 
        { 
            set 
            { 
                sceneObject = value; 
                foreach(TimelineCurve curve in Curves)
                {
                    curve.SetRootObject(sceneObject);
                }
            } 
        }
        public readonly bool HasSceneObject => sceneObject != null;

        /// <summary>
        /// Seconds per frame. Ie. "I want 60FPS! Do: 1f / 60f."
        /// </summary>
        public const float SPF = 1f / 60f;
        //figure out how to add timelinecurves of specific types and actually be able to interpret that?
        //object.ToString?
        [SerializeReference] public List<TimelineCurve> Curves;

        /// <summary>
        /// An awaitable that goes through the timeline's curves, Evaluating() them at the seconds elapsed.
        /// Advances every SPF in seconds (by default, SPF is 1/60 to replicate 60fps).
        /// </summary>
        /// <param name="ct">CancellationToken so you can bail out of the timeline whenever you feel like it.</param>
        /// <returns>Diddly squat.</returns>
        public readonly async Awaitable RunThroughTimeline(CancellationToken ct)
        {
            float secondsElapsed = 0;

            while(secondsElapsed < Duration || Loop)
            {
                if (ct.IsCancellationRequested) break;
                await Awaitable.WaitForSecondsAsync(SPF);
                secondsElapsed += SPF;

                foreach(TimelineCurve curve in Curves)
                {
                    curve.Evaluate(secondsElapsed);
                }

                if(Loop && secondsElapsed >= Duration)
                {
                    secondsElapsed = 0;
                }
            }
        }
        /// <summary>
        /// Same as RunThroughTimeline(), but also logs the EvaluateMessage() at seconds elapsed while also
        /// doing the Evaluate() behavior.
        /// </summary>
        /// <param name="ct">CancellationToken so you can bail out of the timeline whenever you feel like it.</param>
        /// <returns>Diddly squat 2.</returns>
        public readonly async Awaitable RunThroughTimelineDebug(CancellationToken ct)
        {
            float secondsElapsed = 0;

            while(secondsElapsed < Duration || Loop)
            {
                if (ct.IsCancellationRequested) break;
                await Awaitable.WaitForSecondsAsync(SPF);
                secondsElapsed += SPF;

                string msg = "";

                foreach(TimelineCurve curve in Curves)
                {
                    msg += $"\n{curve.EvaluateMessage(secondsElapsed)}";
                    curve.Evaluate(secondsElapsed);
                }

                Debug.Log(msg);

                if(Loop && secondsElapsed >= Duration)
                {
                    secondsElapsed = 0;
                }
            }
        }
        /// <summary>
        /// Evaluates the end of every curve in the timeline (Evaluate() at Duration).
        /// </summary>
        public readonly void TimelineResult()
        {
            foreach(TimelineCurve curve in Curves)
            { 
                curve.Evaluate(Duration);
            }
        }
        /// <summary>
        /// Same as TimelineResult, but also Debug.Logs the EvaluateMessage() at Duration.
        /// </summary>
        public readonly void TimelineResultDebug()
        {
            string msg = "";
            foreach(TimelineCurve curve in Curves)
            {
                msg += $"\n{curve.EvaluateMessage(Duration)}";
                curve.Evaluate(Duration);
            }
            Debug.Log(msg);
        }
        /// <summary>
        /// Evaluates the beginning of every curve in the timeline (Evaluate() at 0).
        /// </summary>
        public readonly void TimelineInitial()
        {
            foreach(TimelineCurve curve in Curves)
            {
                curve.Evaluate(0);
            }
        }
        /// <summary>
        /// Same as TimelineInitial, but also Debug.Logs the EvaluateMessage() at 0.
        /// </summary>
        public readonly void TimelineInitialDebug()
        {
            string msg = "";
            foreach (TimelineCurve curve in Curves)
            {
                msg += $"\n{curve.EvaluateMessage(0)}";
                curve.Evaluate(0);
            }
            Debug.Log(msg);
        }
    }
}
