using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public float throttle { get; set; }

    ParticleSystem.EmissionModule _emission;
    ParticleSystem.ShapeModule _shape;

    float _originalEmission;
    float _originalRadius;

    void Start()
    {
        var ps = GetComponent<ParticleSystem>();

        _emission = ps.emission;
        _originalEmission = _emission.rateOverTime.constant;

        _shape = ps.shape;
        _originalRadius = _shape.radius;
    }

    void Update()
    {
        _emission.rateOverTime = Mathf.Clamp01(throttle * 5) * _originalEmission;
        _shape.radius = throttle * _originalRadius;
    }
}
