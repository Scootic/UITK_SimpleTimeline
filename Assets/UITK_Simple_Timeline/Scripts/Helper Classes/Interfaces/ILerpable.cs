using System;
using UnityEngine;
using UnityEngine.UIElements;
namespace UITK_SimpleTimeline
{
    public interface ILerpable
    {
        /// <summary>
        /// Equivalent to AnimationCurve.Evaluate. Subclasses should handle the effects.
        /// </summary>
        /// <param name="t"></param>
        public void Evaluate(float t);
        /// <summary>
        /// Essentially for Debug.Logging what's happening in Evaluate().
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public string EvaluateMessage(float t);

        /// <summary>
        /// Only really applies to in-scene curves.
        /// </summary>
        /// <param name="go">GameObject to set.</param>
        public void SetRootObject(GameObject go);
#if UNITY_EDITOR
        /// <summary>
        /// everyone has to have their own uitk rep :(
        /// </summary>
        /// <returns></returns>
        public VisualElement UITKRepresentation(int index);
#endif
    }
}
