using UnityEngine;
using System.Collections;

public class SkyTrailSystem : MonoBehaviour
{
    [Header("Cible")]
    public Transform target;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Spawn")]
    public float spawnInterval = 0.2f;
    public float spawnRadius = 5f;

    [Header("Trail")]
    public Material trailMaterial;
    public float speed = 60f;
    public float trailTime = 2f;
    public float bandwidth = 0.2f;
    public float minTrailLength = 3f;
    public float maxTrailLength = 8f;
    public float lifetime = 3f;
    public int trailPoints = 20;  

    private GameObject _prefab;

    void OnEnable()
    {
        BuildPrefab();
        StartCoroutine(SpawnLoop());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        if (_prefab != null) Destroy(_prefab);
    }

    void BuildPrefab()
    {
        _prefab = new GameObject("TrailObject");
        _prefab.SetActive(false);

        LineRenderer lr = _prefab.AddComponent<LineRenderer>();
        lr.positionCount = trailPoints;
        lr.startWidth = bandwidth;
        lr.endWidth = bandwidth; 
        lr.useWorldSpace = true;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;

        if (trailMaterial != null)
            lr.material = trailMaterial;

        _prefab.AddComponent<TrailMover>();
        DontDestroyOnLoad(_prefab);
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void Spawn()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        Vector3 targetPos = target != null ? target.position : transform.position;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 origin = spawnPoint.position + Random.onUnitSphere * spawnRadius;
        Vector3 dir = (targetPos - origin).normalized;

        GameObject go = Instantiate(_prefab, origin, Quaternion.LookRotation(dir));
        go.SetActive(true);

        LineRenderer lr = go.GetComponent<LineRenderer>();
        lr.positionCount = trailPoints;
        lr.startWidth = bandwidth;
        lr.endWidth = bandwidth;

        if (trailMaterial != null)
            lr.material = trailMaterial;

        for (int i = 0; i < trailPoints; i++)
            lr.SetPosition(i, origin);

        TrailMover mv = go.GetComponent<TrailMover>();
        mv.direction = dir;
        mv.speed = speed;
        mv.lifetime = lifetime;
        mv.trailPoints = trailPoints;
        mv.trailLength = Random.Range(minTrailLength, maxTrailLength);

        StartCoroutine(DestroyOnArrival(go, targetPos));
    }

    IEnumerator DestroyOnArrival(GameObject go, Vector3 targetPos)
    {
        while (go != null)
        {
            if (Vector3.Distance(go.transform.position, targetPos) < 1f)
            {
                Destroy(go);
                yield break;
            }
            yield return null;
        }
    }
    
}