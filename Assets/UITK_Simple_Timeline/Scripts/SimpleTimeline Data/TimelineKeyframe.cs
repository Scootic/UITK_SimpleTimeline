
using System;
using UnityEngine;
namespace UITK_SimpleTimeline
{
    [Serializable]
    public class TimelineKeyframe<T> : TimelineKeyframe, IComparable
    {
        public T Value;
        
#if UNITY_EDITOR
        /// <summary>
        /// Deranged weirdo method to get a valueless unity keyframe out of a TimelineKeyframe<T>
        /// </summary>
        /// <param name="tk"></param>
        /// <returns></returns>
        public static Keyframe GetKeyframeFromTK(TimelineKeyframe<T> tk)
        {
            //doesn't set value at all?
            Keyframe toReturn = new();
#pragma warning disable CS0618
            toReturn.time = tk.Time;
            toReturn.weightedMode = tk.WeightedMode;
            toReturn.tangentMode = (int)tk.TangentMode;
#pragma warning restore CS0618
            toReturn.inWeight = tk.InWeight;
            toReturn.outWeight = tk.OutWeight;
            toReturn.inTangent = tk.InTangent;
            toReturn.outTangent = tk.OutTangent;

            return toReturn;
        }
#endif
    }

    public abstract class TimelineKeyframe : IComparable
    {
        public int KeyframeIndex, CurveIndex;
        /// <summary>
        /// In seconds.
        /// </summary>
        [Min(0)] public float Time;
        public float InSlope;
        public float OutSlope;

        public TimelineKeyframeTangentMode TangentMode;
        public float InTangent;
        public float OutTangent;

        public WeightedMode WeightedMode;
        public float InWeight;
        public float OutWeight;

        public int CompareTo(object obj) //super dee duper make sure we're ordering our lists by time, because duh
        {
            if (obj == null) return 1;
            TimelineKeyframe other = (TimelineKeyframe)obj;
            return Time.CompareTo(other.Time);
        }
    }
}
