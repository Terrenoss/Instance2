using UnityEngine;
using System.Collections;

public class BuildObjectParticles : MonoBehaviour
{
    [Header("References")]
    public Material cubeMaterial;
    public MeshFilter targetMeshFilter;

    [Header("Final Object")]
    public GameObject finalPrefab;
    public bool destroyParticles = true;

    [Header("Particles")]
    public int particleCount = 1000;
    public float spawnRadius = 5f;
    [Range(0.5f, 30f)]
    public float speed = 5f;
    public float cubeSize = 0.1f;

    [Header("Reveal")]
    [Range(0f, 1f)]
    public float revealThreshold = 0.95f;
    public float arrivalThreshold = 0.005f;

    private Vector3[] currentPos;
    private Vector3[] targetPos;
    private bool[] arrived;
    private int arrivedCount = 0;
    private bool finished = false;

    private Mesh cubeMesh;
    private Matrix4x4[] matrices;
    private Matrix4x4[] batchBuffer;
    private MaterialPropertyBlock mpb;

    const int BATCH = 1023;

    void Start()
    {

        HideTarget();
        cubeMesh = BuildCubeMesh();
        mpb = new MaterialPropertyBlock();
        batchBuffer = new Matrix4x4[BATCH];

        SpawnParticles();
    }
    void HideTarget()
    {
        if (targetMeshFilter == null) return;

        foreach (Renderer r in targetMeshFilter.GetComponentsInChildren<Renderer>())
            r.enabled = false;
    }
    static Mesh BuildCubeMesh()
    {
        Mesh mesh = new Mesh { name = "PCube" };
        float h = 0.5f;

        Vector3[] v = {
            new(-h,-h,-h), new( h,-h,-h), new( h, h,-h), new(-h, h,-h),
            new( h,-h, h), new(-h,-h, h), new(-h, h, h), new( h, h, h),
            new(-h,-h, h), new(-h,-h,-h), new(-h, h,-h), new(-h, h, h),
            new( h,-h,-h), new( h,-h, h), new( h, h, h), new( h, h,-h),
            new(-h,-h, h), new( h,-h, h), new( h,-h,-h), new(-h,-h,-h),
            new(-h, h,-h), new( h, h,-h), new( h, h, h), new(-h, h, h),
        };

        int[] t = new int[36];
        for (int f = 0; f < 6; f++)
        {
            int b = f * 4, o = f * 6;
            t[o] = b; t[o + 1] = b + 1; t[o + 2] = b + 2;
            t[o + 3] = b; t[o + 4] = b + 2; t[o + 5] = b + 3;
        }

        mesh.vertices = v;
        mesh.triangles = t;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
    Vector3[] SampleSurface(int count)
    {
        Mesh m = targetMeshFilter.sharedMesh;
        Vector3[] verts = m.vertices;
        int[] tris = m.triangles;
        Transform tf = targetMeshFilter.transform;

        int triCnt = tris.Length / 3;
        float[] areas = new float[triCnt];
        float total = 0f;

        for (int i = 0; i < triCnt; i++)
        {
            Vector3 a = tf.TransformPoint(verts[tris[i * 3]]);
            Vector3 b = tf.TransformPoint(verts[tris[i * 3 + 1]]);
            Vector3 c = tf.TransformPoint(verts[tris[i * 3 + 2]]);
            float area = Vector3.Cross(b - a, c - a).magnitude * 0.5f;
            areas[i] = area;
            total += area;
        }

        float[] cdf = new float[triCnt];
        float acc = 0f;
        for (int i = 0; i < triCnt; i++)
        {
            acc += areas[i] / total;
            cdf[i] = acc;
        }

        Vector3[] pts = new Vector3[count];

        for (int p = 0; p < count; p++)
        {
            int tri = System.Array.BinarySearch(cdf, Random.value);
            if (tri < 0) tri = ~tri;
            tri = Mathf.Clamp(tri, 0, triCnt - 1);

            Vector3 A = tf.TransformPoint(verts[tris[tri * 3]]);
            Vector3 B = tf.TransformPoint(verts[tris[tri * 3 + 1]]);
            Vector3 C = tf.TransformPoint(verts[tris[tri * 3 + 2]]);

            float u = Random.value;
            float v2 = Random.value;

            if (u + v2 > 1f)
            {
                u = 1f - u;
                v2 = 1f - v2;
            }

            pts[p] = A + u * (B - A) + v2 * (C - A);
        }

        return pts;
    }
    void SpawnParticles()
    {
        arrivedCount = 0;
        finished = false;

        targetPos = SampleSurface(particleCount);
        currentPos = new Vector3[particleCount];
        arrived = new bool[particleCount];
        matrices = new Matrix4x4[particleCount];

        Vector3 origin = transform.position;
        Vector3 scale = Vector3.one * cubeSize;

        for (int i = 0; i < particleCount; i++)
        {
            currentPos[i] = origin + Random.insideUnitSphere * spawnRadius;
            matrices[i] = Matrix4x4.TRS(currentPos[i], Quaternion.identity, scale);
        }
    }
    void Update()
    {
        if (finished || currentPos == null) return;

        float dt = Time.deltaTime * speed;
        float threshold = arrivalThreshold * arrivalThreshold;
        Vector3 scale = Vector3.one * cubeSize;

        for (int i = 0; i < particleCount; i++)
        {
            if (!arrived[i])
            {
                Vector3 dir = targetPos[i] - currentPos[i];

                if (dir.sqrMagnitude <= threshold)
                {
                    currentPos[i] = targetPos[i];
                    arrived[i] = true;
                    arrivedCount++;
                }
                else
                {
                    currentPos[i] += dir * dt;
                }
            }

            matrices[i] = Matrix4x4.TRS(currentPos[i], Quaternion.identity, scale);
        }

        DrawBatches();

        if ((float)arrivedCount / particleCount >= revealThreshold)
        {
            finished = true;
            StartCoroutine(FinishEffect());
        }
    }

    void DrawBatches()
    {
        int index = 0;

        while (index < particleCount)
        {
            int count = Mathf.Min(BATCH, particleCount - index);

            System.Array.Copy(matrices, index, batchBuffer, 0, count);

            Graphics.DrawMeshInstanced(
                cubeMesh,
                0,
                cubeMaterial,
                batchBuffer,
                count,
                mpb
            );

            index += count;
        }
    }

    IEnumerator FinishEffect()
    {
        yield return new WaitForSeconds(0.2f);
        if (finalPrefab != null)
        {
            Instantiate(
                finalPrefab,
                targetMeshFilter.transform.position,
                targetMeshFilter.transform.rotation
            );
        }
        if (destroyParticles)
        {
            currentPos = null;
            matrices = null;
        }
        //Destroy(targetMeshFilter.gameObject);
        //Destroy(gameObject);
    }
    public void ResetParticles()
    {
        SpawnParticles();
    }
}