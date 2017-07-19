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

        static GUIContent _labelDynamicRange = new GUIContent("Dyn. Range (dB)");
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

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_amplitudeType);
            EditorGUILayout.PropertyField(_filterType);
            EditorGUILayout.PropertyField(_dynamicRange, _labelDynamicRange);
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

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_outputEvent);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
