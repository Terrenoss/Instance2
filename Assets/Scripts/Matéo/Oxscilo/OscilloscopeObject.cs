using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OscilloscopeObjects : MonoBehaviour
{
    public int sampleSize = 100;
    public float width = 10f;
    public float height = 2f;
    public float speed = 2f;
    public float frequency = 3f;
    public float lineWidth = 0.05f;

    private LineRenderer lr;
    private Vector3[] positions;

    public int updateEveryNFrames = 2;
    private int frameOffset;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = sampleSize;
        lr.useWorldSpace = false;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        positions = new Vector3[sampleSize]; 

        frameOffset = GetInstanceID() % updateEveryNFrames;
    }

    void Update()
    {
        if (Time.frameCount % updateEveryNFrames != frameOffset) return;

        float time = Time.time * speed;

        for (int i = 0; i < sampleSize; i++)
        {
            float t = (float)i / sampleSize;
            float x = (t - 0.5f) * width;
            float y =
                Mathf.Sin(t * frequency * 10f + time) +
                Mathf.Sin(t * frequency * 3f + time * 1.5f) * 0.5f;
            y *= height;
            positions[i] = new Vector3(x, y, 0);
        }

        lr.SetPositions(positions);
    }
}