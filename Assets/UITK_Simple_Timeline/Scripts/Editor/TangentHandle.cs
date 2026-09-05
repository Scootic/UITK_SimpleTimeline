#if UNITY_EDITOR
using UnityEditor;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Helper = UITK_SimpleTimeline.SimpleTimelineUITK_Helper;
namespace UITK_SimpleTimeline.Editor
{
    /// <summary>
    /// A goofy ah element that do be rotating, and sending its current angle somewhere. Grabbable and draggable by mouse.
    /// </summary>
    public class TangentHandle : VisualElement
    {
        private static Texture2D handleBase, handleConnector, handleEnd;

        private readonly VisualElement baseElement, connectorElement, endElement;
        private readonly float initialRotation, maxRotation, defaultAngle;

        public float GetRotationAngle => baseElement.style.rotate.value.angle.value;
        /// <summary>
        /// Always invoked after the user rotates the element through dragging.
        /// </summary>
        public Action<float> GiveCurAngle;

        /// <summary>
        /// New tangent handle, please :)
        /// </summary>
        /// <param name="positionOffset">X is dist from left, Y is dist from top. Absolute position.</param>
        /// <param name="initialRotationAngle">Positive rotations are clockwise.</param>
        /// <param name="maximumValidRotation">Clamps rotations compared to the initial rotation, in both directions.</param>
        public TangentHandle(Vector2 positionOffset, float defaultRotationAngle, float initialRotationAngle, string toolTip, float maximumValidRotation = 90f) 
        {
            initialRotation = initialRotationAngle;
            maxRotation = maximumValidRotation;
            defaultAngle = defaultRotationAngle;
            //nasty and gross image check.
            if (handleBase == null) handleBase = AssetDatabase.LoadAssetAtPath<Texture2D>(Helper.EditorIconAssetPath + "/handlebase.png");
            if (!handleBase) Debug.LogWarning("TangentHandle.cs can't find handlebase.png. Make sure the SimpleTimelineUITK_Helper.EditorIconAssetPath matches your project.");
            if (handleConnector == null) handleConnector = AssetDatabase.LoadAssetAtPath<Texture2D>(Helper.EditorIconAssetPath + "/handleconnector.png");
            if (!handleConnector) Debug.LogWarning("TangentHandle.cs can't find handleconnector.png. Make sure the SimpleTimelineUITK_Helper.EditorIconAssetPath matches your project.");
            if (handleEnd == null) handleEnd = AssetDatabase.LoadAssetAtPath<Texture2D>(Helper.EditorIconAssetPath + "/handleend.png");
            if (!handleEnd) Debug.LogWarning("TangentHandle.cs can't find handleend.png. Make sure the SimpleTimelineUITK_Helper.EditorIconAssetPath matches your project.");

            baseElement = this;
            baseElement.style.backgroundImage = handleBase;
            baseElement.style.width = 25f;
            baseElement.style.height = 25f;
            baseElement.style.position = Position.Absolute;
            baseElement.style.left = positionOffset.x; 
            baseElement.style.top = positionOffset.y;
            baseElement.style.transformOrigin = new TransformOrigin(12.5f, 12.5f);
            baseElement.style.rotate = new Rotate(initialRotation);
            baseElement.tooltip = toolTip;
            baseElement.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button != 1) return;
                GenericMenu gm = new();
                gm.AddItem(new GUIContent("Reset to Zero"), false, delegate
                {
                    baseElement.style.rotate = new Rotate(defaultAngle);
                    GiveCurAngle?.Invoke(GetRotationAngle);
                });
                gm.ShowAsContext();
                evt.StopPropagation();
            });

            connectorElement = new();
            connectorElement.style.backgroundImage = handleConnector;
            connectorElement.style.width = 75f;
            connectorElement.style.height = 25f;
            connectorElement.style.left = 12.5f;
            connectorElement.RegisterCallback<PointerMoveEvent>(CommitRotation);
            baseElement.Add(connectorElement);

            endElement = new();
            endElement.style.backgroundImage = handleEnd;
            endElement.style.position = Position.Relative;
            endElement.style.width = 25f;
            endElement.style.height = 25f;
            endElement.style.left = 60f;
            endElement.RegisterCallback<PointerMoveEvent>(CommitRotation);
            connectorElement.Add(endElement);
        }

        private void CommitRotation(PointerMoveEvent pme)
        {
            if ((pme.pressedButtons & 1) != 1) return;
            //Debug.Log($"MouseLoc: {pme.localPosition}");
            float yLoc = pme.localPosition.y - 12.5f; //subtract localPos by half of height to get the pos as understandable in a Vec3 worldspace?!?
            float desAngle = Mathf.Clamp(GetRotationAngle + yLoc, defaultAngle - maxRotation, defaultAngle + maxRotation);

            baseElement.style.rotate = new Rotate(desAngle);
            GiveCurAngle?.Invoke(GetRotationAngle);
        }
    }
}
#endif