using UnityEngine;
using Klak.Wiring;

namespace Klak.Audio
{
    [AddComponentMenu("Klak/Wiring/Input/Audio Input")]
    public class AudioInput : NodeBase
    {
        #region Editable attributes

        [SerializeField]
        Lasp.FilterType _filterType = Lasp.FilterType.LowPass;

        [SerializeField]
        bool _autoGain = true;

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
        public float gain {
            get { return _externalGain; }
            set { _externalGain = value; }
        }

        [Inlet]
        public void ResetAutoGain()
        {
            _peak = kSilence;
        }

        [SerializeField, Outlet]
        FloatEvent _outputEvent = new FloatEvent();

        #endregion

        #region Public properties

        public Lasp.FilterType filterType {
            get { return _filterType; }
            set { _filterType = value; }
        }

        public bool autoGain {
            get { return _autoGain; }
            set { _autoGain = value; }
        }

        public float dynamicRange {
            get { return _dynamicRange; }
            set { _dynamicRange = value; }
        }

        public bool holdAndFallDown {
            get { return _holdAndFallDown; }
            set { _holdAndFallDown = value; }
        }

        public float fallDownSpeed {
            get { return _fallDownSpeed; }
            set { _fallDownSpeed = value; }
        }

        public float calculatedGain {
            get { return _autoGain ? -_peak : _internalGain + _externalGain; }
        }

        public float inputAmplitude {
            get { return Lasp.AudioInput.CalculateRMSDecibel(_filterType); }
        }

        public float outputAmplitude {
            get { return _amplitude; }
        }

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
            var input = inputAmplitude;
            var dt = Time.deltaTime;

            // Automatic gain control
            if (_autoGain)
            {
                // Gradually falls down to the minimum amplitude.
                const float peakFallSpeed = 0.6f;
                _peak = Mathf.Max(_peak - peakFallSpeed * dt, kSilence);

                // Pull up by input with allowing a small amount of clipping.
                var clip = _dynamicRange * 0.05f;
                _peak = Mathf.Clamp(input - clip, _peak, 0);
            }

            // Normalize the input value.
            input = Mathf.Clamp01((input + calculatedGain) / _dynamicRange + 1);

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
