using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircuitTrailManager : MonoBehaviour
{
    [Header("Référence Agent")]
    public Transform agent;

    [Header("Propagation")]
    public int maxBranches = 5;
    public float spawnInterval = 0.15f;
    public float branchSpeed = 6f;
    public float minSegmentLength = 1.5f;
    public float maxSegmentLength = 4f;
    public int maxSegmentsPerBranch = 4;

    [Header("Piste (bande statique)")]
    public Material trackMaterial;      
    public float trackWidth = 0.18f;     

    [Header("Signal (trait lumineux)")]
    public Material signalMaterial;      
    public float signalWidth = 0.04f;    
    public float trailTime = 0.25f;      

    [Header("Durée d'affichage de la piste")]
    public float trackFadeDelay = 3f;   

    [Header("Optimisation")]
    public int maxActiveTrails = 10;

    [Header("Dispersion des branches")]
    public float spawnAngleSpread = 45f;

    private Queue<LineRenderer> trackPool = new Queue<LineRenderer>();
    private Queue<TrailRenderer> signalPool = new Queue<TrailRenderer>();
    private int activeCount = 0;

    private float timer = 0f;
    private Material sharedTrack;
    private Material sharedSignal;

    void Start()
    {
        sharedTrack = new Material(trackMaterial);
        sharedSignal = new Material(signalMaterial);
        PrewarmPools(maxActiveTrails);
    }

    void PrewarmPools(int count)
    {
        for (int i = 0; i < count; i++)
        {
            LineRenderer lr = CreateTrackObject();
            lr.gameObject.SetActive(false);
            trackPool.Enqueue(lr);

            TrailRenderer tr = CreateSignalObject();
            tr.gameObject.SetActive(false);
            signalPool.Enqueue(tr);
        }
    }

    LineRenderer CreateTrackObject()
    {
        GameObject go = new GameObject("CircuitTrack");
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.material = sharedTrack;
        lr.startWidth = trackWidth;
        lr.endWidth = trackWidth;
        lr.numCapVertices = 4;
        lr.numCornerVertices = 4;
        lr.textureMode = LineTextureMode.Tile;
        lr.alignment = LineAlignment.View;
        lr.positionCount = 0;
        return lr;
    }

    TrailRenderer CreateSignalObject()
    {
        GameObject go = new GameObject("CircuitSignal");
        TrailRenderer tr = go.AddComponent<TrailRenderer>();
        tr.material = sharedSignal;
        tr.time = trailTime;
        tr.startWidth = signalWidth;
        tr.endWidth = 0f;         
        tr.textureMode = LineTextureMode.Tile;
        tr.alignment = LineAlignment.View;
        tr.minVertexDistance = 0.02f;
        tr.numCapVertices = 2;

        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        tr.colorGradient = g;
        return tr;
    }

    LineRenderer GetTrack()
    {
        if (trackPool.Count > 0)
        {
            LineRenderer lr = trackPool.Dequeue();
            lr.positionCount = 0;
            lr.gameObject.SetActive(true);
            return lr;
        }
        return CreateTrackObject();
    }

    TrailRenderer GetSignal()
    {
        if (signalPool.Count > 0)
        {
            TrailRenderer tr = signalPool.Dequeue();
            tr.Clear();
            tr.gameObject.SetActive(true);
            return tr;
        }
        return CreateSignalObject();
    }

    void ReturnTrack(LineRenderer lr)
    {
        lr.positionCount = 0;
        lr.gameObject.SetActive(false);
        trackPool.Enqueue(lr);
    }

    void ReturnSignal(TrailRenderer tr)
    {
        tr.Clear();
        tr.gameObject.SetActive(false);
        signalPool.Enqueue(tr);
        activeCount--;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            if (activeCount < maxActiveTrails)
            {
                int count = Mathf.Min(
                    Random.Range(1, maxBranches + 1),
                    maxActiveTrails - activeCount
                );
                for (int i = 0; i < count; i++)
                    StartCoroutine(SpawnBranch(i * 0.04f));
            }
        }
    }

    IEnumerator SpawnBranch(float delay)
    {
        Vector3 origin = agent.position;
        float fixedY = origin.y;

        if (delay > 0f) yield return new WaitForSeconds(delay);
        if (activeCount >= maxActiveTrails) yield break;
        activeCount++;

        LineRenderer track = GetTrack();
        TrailRenderer signal = GetSignal();

        float angle = Random.Range(-spawnAngleSpread, spawnAngleSpread);
        Vector3 initialDir = Quaternion.Euler(0f, angle, 0f) * Vector3.back;
        initialDir.y = 0f;
        initialDir.Normalize();

        List<Vector3> waypoints = new List<Vector3>();
        waypoints.Add(origin);

        Vector3 cursor = origin;
        Vector3 dir = initialDir;
        int segments = Random.Range(2, maxSegmentsPerBranch + 1);

        float tanAngle = Mathf.Tan(spawnAngleSpread * Mathf.Deg2Rad);

        for (int s = 0; s < segments; s++)
        {
            if (s == segments - 1) dir = Vector3.back;

            float segLen = Random.Range(minSegmentLength, maxSegmentLength);

            float newX = cursor.x + dir.x * segLen;
            float newZ = cursor.z + dir.z * segLen;
            
            float zDist = Mathf.Abs(newZ - origin.z);
            float xLimit = tanAngle * zDist;
            newX = Mathf.Clamp(newX, origin.x - xLimit, origin.x + xLimit);

            cursor.x = newX;
            cursor.z = newZ;
            cursor.y = fixedY;
            waypoints.Add(cursor);

            if (s < segments - 2)
                dir = GetPerpendicularDir(dir);
        }

        track.positionCount = waypoints.Count;
        track.SetPositions(waypoints.ToArray());

        signal.transform.position = waypoints[0];

        for (int i = 1; i < waypoints.Count; i++)
        {
            Vector3 from = waypoints[i - 1];
            Vector3 to = waypoints[i];
            float dist = Vector3.Distance(from, to);
            float t = 0f;

            while (t < 1f)
            {
                t += (branchSpeed * Time.deltaTime) / dist;
                signal.transform.position = Vector3.Lerp(from, to, Mathf.Clamp01(t));
                yield return null;
            }
        }

        signal.transform.position = waypoints[waypoints.Count - 1];
        ReturnSignal(signal);

        yield return new WaitForSeconds(trackFadeDelay);
        ReturnTrack(track);
    }
    
    static Vector3 GetPerpendicularDir(Vector3 current)
    {
        if (Mathf.Abs(current.z) >= Mathf.Abs(current.x))
            return Random.value > 0.5f ? Vector3.left : Vector3.right;

        return Random.value > 0.5f ? Vector3.forward : Vector3.back;
    }

    void OnDestroy()
    {
        if (sharedTrack != null) Destroy(sharedTrack);
        if (sharedSignal != null) Destroy(sharedSignal);
    }
}