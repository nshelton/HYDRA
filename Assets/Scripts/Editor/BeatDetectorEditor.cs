using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Lasp.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BeatDetector))]
    sealed class BeatDetectorEditor : UnityEditor.Editor
    {
        static Vector3[] _vertices = new Vector3[512];

        public override bool RequiresConstantRepaint()
        {
            // Keep updated while playing.
            return EditorApplication.isPlaying && targets.Length == 1;
        }

        public static void DrawGraph(float[] data, float[] bars = null, float[] redBars = null)
        {
            EditorGUILayout.Space();

            // Graph area
            var rect = GUILayoutUtility.GetRect(128, 64);

            // Background
            Handles.DrawSolidRectangleWithOutline
              (rect, new Color(0.1f, 0.1f, 0.1f, 1), Color.clear);

            // Don't draw the actual graph if it isn't a repaint event.
            if (Event.current.type != EventType.Repaint) return;

            if (bars != null)
            {
                for (var i = 0; i < bars.Length; i++)
                {
                    var x = bars[i] / data.Length;

                    x = x * rect.width + rect.xMin;

                    Vector3 p0 = new Vector3(x, rect.yMax, 0);
                    Vector3 p1 = new Vector3(x, rect.yMin, 0);

                    Handles.color = Color.green;
                    Handles.DrawLine(p0, p1);
                }
            }

            if (redBars != null)
            {
                for (var i = 0; i < redBars.Length; i++)
                {
                    var x = redBars[i] / data.Length;

                    x = x * rect.width + rect.xMin;

                    Vector3 p0 = new Vector3(x, rect.yMax, 0);
                    Vector3 p1 = new Vector3(x, rect.yMin, 0);

                    Handles.color = Color.red;
                    Handles.DrawLine(p0, p1);
                }

            }

            float maxVal = data.Max();
            // Spectrum curve construction
            for (var i = 0; i < data.Length; i++)
            {
                var x = (float)i / data.Length;
                var y = data[i * data.Length / data.Length] / maxVal;

                x = x * rect.width + rect.xMin;
                y = rect.yMax - y * rect.height;

                _vertices[i] = new Vector3(x, y, 0);
            }

            // Curve
            Handles.color = Color.white;
            Handles.DrawAAPolyLine(3f, data.Length, _vertices);
        }

        public override void OnInspectorGUI()
        {
            var targetComponent = (BeatDetector)target;
            DrawDefaultInspector();

            // Spectrum graph
            if (EditorApplication.isPlaying)
            {
                DrawGraph(targetComponent.m_currentLevels);
                DrawGraph(targetComponent.m_weights, new float[3] {
                    targetComponent.CurrentOffset ,
                    targetComponent.CurrentOffset* 2f,
                    targetComponent.CurrentOffset/2f }, new float[1] { targetComponent.CurrentOffsetLagrange });

            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
