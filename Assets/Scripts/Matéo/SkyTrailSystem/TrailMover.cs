using UnityEngine;

public class TrailMover : MonoBehaviour
{
    public Vector3 direction;
    public float speed = 60f;
    public float lifetime = 3f;
    public int trailPoints = 20;
    public float trailLength = 5f;

    private LineRenderer lr;
    private float elapsed = 0f;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        transform.position += direction * speed * Time.deltaTime;

        Vector3 startPos = transform.position;
        for (int i = 0; i < trailPoints; i++)
        {
            float t = i / (float)(trailPoints - 1);
            lr.SetPosition(i, startPos - direction * trailLength * t);
        }

        float alpha = Mathf.Clamp01(1f - (elapsed / lifetime));
        Color startColor = lr.startColor;
        Color endColor = lr.endColor;

        lr.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
        lr.endColor = new Color(endColor.r, endColor.g, endColor.b, alpha);

        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}