using UnityEngine;
using Klak.Wiring;

namespace Klak.Audio
{
    [AddComponentMenu("Klak/Wiring/Input/Audio Input")]
    public class AudioInput : NodeBase
    {
        #region Editable properties

        public enum AmplitudeType { Peak, RMS }

        [SerializeField]
        AmplitudeType _amplitudeType = AmplitudeType.RMS;

        [SerializeField]
        Lasp.FilterType _filterType = Lasp.FilterType.LowPass;

        [SerializeField]
        bool _autoGainControl = true;

        [SerializeField, Range(-10, 40)]
        float _internalGain = 6;

        [SerializeField, Range(1, 40)]
        float _dynamicRange = 12;

        [SerializeField]
        bool _holdAndFallDown = true;

        [SerializeField, Range(0, 1)]
        float _fallDownSpeed = 0.3f;

        #endregion

        #region Node I/O

        [Inlet]
        public float gain { set { _externalGain = value; } }

        [SerializeField, Outlet]
        FloatEvent _outputEvent = new FloatEvent();

        #endregion

        #region Private members

        // Silence: Minimum amplitude value
        const float kSilence = -60;

        // Gain by external control.
        float _externalGain = 0;

        // Current amplitude value.
        float _amplitude = kSilence;

        // Variables for automatic gain control.
        float _peak = kSilence;
        float _fall = 0;

        #endregion

        #region MonoBehaviour functions

        void Update()
        {
            var dt = Time.deltaTime;

            // Get the input value.
            float input;

            if (_amplitudeType == AmplitudeType.Peak)
                input = Lasp.AudioInput.GetPeakLevelDecibel(_filterType);
            else
                input = Lasp.AudioInput.CalculateRMSDecibel(_filterType);

            // Calculate the gain value.
            float gain;

            if (_autoGainControl)
            {
                // Automatic gain control

                // Gradually falls down to the minimum amplitude.
                const float peakFallSpeed = 1.0f;
                _peak = Mathf.Max(_peak - peakFallSpeed * dt, kSilence);

                // Pull up by input with a small headroom.
                var headroom = _dynamicRange * 0.2f;
                _peak = Mathf.Max(_peak, input - headroom);

                // Simply apply the peak level as gain.
                gain = -_peak;
            }
            else
            {
                // Simply apply the sum of internal/external gains.
                gain = _internalGain + _externalGain;
            }

            // Normalize the input value.
            input = Mathf.Clamp01((input + gain) / _dynamicRange + 1);

            if (_holdAndFallDown)
            {
                // Hold-and-fall-down animation.
                _fall += Mathf.Pow(10, 1 + _fallDownSpeed * 2) * dt;
                _amplitude -= _fall * dt;

                // Pull up by input.
                if (_amplitude < input)
                {
                    _amplitude = input;
                    _fall = 0;
                }
            }
            else
            {
                _amplitude = input;
            }

            // Output
            _outputEvent.Invoke(_amplitude);
        }

        #endregion
    }
}
