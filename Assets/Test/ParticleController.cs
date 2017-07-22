using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public float throttle { get; set; }

    ParticleSystem.EmissionModule _emission;
    ParticleSystem.ShapeModule _shape;
    ParticleSystem.NoiseModule _noise;
    ParticleSystem.ExternalForcesModule _force;

    float _originalEmission;
    float _originalRadius;
    float _originalNoise;

    void Start()
    {
        var ps = GetComponent<ParticleSystem>();

        _emission = ps.emission;
        _originalEmission = _emission.rateOverTime.constant;

        _shape = ps.shape;
        _originalRadius = _shape.radius;

        _noise = ps.noise;
        _originalNoise = _noise.strength.constant;

        _force = ps.externalForces;
    }

    void Update()
    {
        _emission.rateOverTime = Mathf.Clamp01(throttle * 5) * _originalEmission;
        _shape.radius = throttle * _originalRadius;
        _noise.strength = Mathf.Clamp01(throttle + 0.5f) * _originalNoise;
        _force.multiplier = throttle * 2 - 1;
    }
}
