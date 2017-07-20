using UnityEngine;
using UnityEditor;

namespace Klak.Audio
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AudioInput))]
    public class AudioInputEditor : Editor
    {
        SerializedProperty _amplitudeType;
        SerializedProperty _filterType;
        SerializedProperty _dynamicRange;
        SerializedProperty _autoGainControl;
        SerializedProperty _internalGain;
        SerializedProperty _holdAndFallDown;
        SerializedProperty _fallDownSpeed;
        SerializedProperty _outputEvent;

        static GUIContent _labelDynamicRange = new GUIContent("Dynamic Range");
        static GUIContent _labelDynamicRangeWide = new GUIContent("Dynamic Range (dB)");
        static GUIContent _labelGain = new GUIContent("Gain (dB)");
        static GUIContent _labelSpeed = new GUIContent("Speed");

        void OnEnable()
        {
            _amplitudeType = serializedObject.FindProperty("_amplitudeType");
            _filterType = serializedObject.FindProperty("_filterType");
            _dynamicRange = serializedObject.FindProperty("_dynamicRange");
            _autoGainControl = serializedObject.FindProperty("_autoGainControl");
            _internalGain = serializedObject.FindProperty("_internalGain");
            _holdAndFallDown = serializedObject.FindProperty("_holdAndFallDown");
            _fallDownSpeed = serializedObject.FindProperty("_fallDownSpeed");
            _outputEvent = serializedObject.FindProperty("_outputEvent");
        }

        public override bool RequiresConstantRepaint()
        {
            // Keep updated while playing.
            return Application.isPlaying && targets.Length == 1;
        }

        public override void OnInspectorGUI()
        {
            var wide = EditorGUIUtility.labelWidth > 132;

            serializedObject.Update();

            EditorGUILayout.PropertyField(_amplitudeType);
            EditorGUILayout.PropertyField(_filterType);
            EditorGUILayout.PropertyField(_dynamicRange, wide ? _labelDynamicRangeWide : _labelDynamicRange);
            EditorGUILayout.PropertyField(_autoGainControl);

            if (_autoGainControl.hasMultipleDifferentValues || !_autoGainControl.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_internalGain, _labelGain);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(_holdAndFallDown);

            if (_holdAndFallDown.hasMultipleDifferentValues || _holdAndFallDown.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_fallDownSpeed, _labelSpeed);
                EditorGUI.indentLevel--;
            }

            if (RequiresConstantRepaint())
            {
                EditorGUILayout.Space();
                DrawMeter((AudioInput)target);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_outputEvent);

            serializedObject.ApplyModifiedProperties();
        }

        // Draw a VU meter with a given AudioInput instance.
        void DrawMeter(AudioInput input)
        {
            var rect = GUILayoutUtility.GetRect(128, 9);

            const float kSilence = -60;            // -60dB = silence
            const float kFullRange = 3 - kSilence; // Add +3db for RMS
            var gray = new Color(0.8f, 0.8f, 0.8f, 1);

            var amp  = 1 + (input.inputAmplitude - 3) / kFullRange;
            var peak = 1 - (input.calculatedGain + 3) / kFullRange;
            var dr = input.dynamicRange / kFullRange;

            // Background bar
            var x0db = -kSilence / kFullRange;
            DrawRect(0, 0, x0db, 1, rect, new Color(0, 0.15f, 0, 1)); // <0dB (green)
            DrawRect(x0db, 0, 1, 1, rect, new Color(0.15f, 0, 0, 1)); // >0dB (red)

            // Amplitude bar
            var x1 = Mathf.Min(amp, peak - dr);
            var x2 = Mathf.Min(peak, amp);
            DrawRect(0, 0, x1, 1, rect, new Color(0, 0.4f, 0, 1)); // under the range
            DrawRect(x1, 0, x2, 1, rect, Color.green); // inside the range
            DrawRect(x2, 0, amp, 1, rect, Color.red);  // over the range

            // Dynamic range indicator
            DrawRect(peak - dr, 0.75f, peak, 1, rect, gray);

            var x3 = peak + dr * (input.outputAmplitude - 1);
            DrawRect(x3 - 3.0f / rect.width, 0, x3, 1, rect, gray);

            // Label: -60dB
            Handles.Label(
                new Vector2(rect.xMin + 1, rect.yMax - 10),
                "-60dB", EditorStyles.miniLabel
            );

            // Label: 0dB
            Handles.Label(
                new Vector2(rect.xMin + rect.width * x0db - 20, rect.yMax - 10),
                "0dB", EditorStyles.miniLabel
            );
        }

        // Vertex array for drawing rectangles: Reused to avoid GC allocation.
        Vector3 [] _rectVertices = new Vector3 [4];

        // Draw a rectangle with normalized coordinates.
        void DrawRect(float x1, float y1, float x2, float y2, Rect area, Color color)
        {
            x1 = area.xMin + area.width  * Mathf.Clamp01(x1);
            x2 = area.xMin + area.width  * Mathf.Clamp01(x2);
            y1 = area.yMin + area.height * Mathf.Clamp01(y1);
            y2 = area.yMin + area.height * Mathf.Clamp01(y2);

            _rectVertices[0] = new Vector2(x1, y1);
            _rectVertices[1] = new Vector2(x1, y2);
            _rectVertices[2] = new Vector2(x2, y2);
            _rectVertices[3] = new Vector2(x2, y1);

            Handles.DrawSolidRectangleWithOutline(_rectVertices, color, Color.clear);
        }
    }
}
