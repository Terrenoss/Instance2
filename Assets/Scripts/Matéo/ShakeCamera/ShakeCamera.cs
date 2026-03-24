using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    public static ShakeCamera Instance { get; private set; }

    [Header("Trauma")]
    [Range(0f, 2f)] public float traumaDecaySpeed = 0.8f;

    [Header("Shake Amplitudes")]
    public Vector2 maxTranslation = new Vector2(0.3f, 0.2f);

    [Tooltip("Max rotation offset in degrees")]
    public float maxRotation = 3f;

    [Header("Noise")]
    [Range(1f, 60f)] public float frequency = 15f;

    [Range(1f, 3f)] public float traumaExponent = 2f;

    private float _trauma = 0f;
    private float _seed;
    private Vector3 _originLocalPos;
    private Quaternion _originLocalRot;

    private void Start()
    {
        AddTrauma(2);
    }
    public void AddTrauma(float amount)
    {
        _trauma = Mathf.Clamp01(_trauma + amount);
    }

    public float Trauma => _trauma;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        _seed = Random.Range(0f, 100f);
        _originLocalPos = transform.localPosition;
        _originLocalRot = transform.localRotation;
    }

    private void LateUpdate()
    {
        _trauma = Mathf.Clamp01(_trauma - traumaDecaySpeed * Time.deltaTime);

        float shake = Mathf.Pow(_trauma, traumaExponent);

        if (shake < 0.0001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _originLocalPos, Time.deltaTime * 20f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, _originLocalRot, Time.deltaTime * 20f);
            return;
        }

        float t = Time.time * frequency + _seed;

        float offsetX = (Mathf.PerlinNoise(t, 0f) * 2f - 1f) * maxTranslation.x * shake;
        float offsetY = (Mathf.PerlinNoise(t + 32.7f, 0f) * 2f - 1f) * maxTranslation.y * shake;
        float roll = (Mathf.PerlinNoise(t + 78.3f, 0f) * 2f - 1f) * maxRotation * shake;

        transform.localPosition = _originLocalPos + new Vector3(offsetX, offsetY, 0f);
        transform.localRotation = _originLocalRot * Quaternion.Euler(0f, 0f, roll);
    }

    public void SetOrigin()
    {
        _originLocalPos = transform.localPosition;
        _originLocalRot = transform.localRotation;
    }
}