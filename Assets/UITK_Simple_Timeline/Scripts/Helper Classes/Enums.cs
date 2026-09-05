using System;

namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Only exists because actual TangentMode is for some reason Editor only. Sad!
    /// </summary>
    [Serializable]
    public enum TimelineKeyframeTangentMode
    {
        /// <summary>
        /// Free to go as the tangents declare the curve to be.
        /// </summary>
        Free,
        /// <summary>
        /// Automatically creates a curve between the two keyframes. S shape?
        /// </summary>
        Auto,
        /// <summary>
        /// Transition between two keyframes linearly.
        /// </summary>
        Linear,
        /// <summary>
        /// Same as free, but values are clamped to never exceed starting or ending value.
        /// </summary>
        ClampedFree
    }
}
