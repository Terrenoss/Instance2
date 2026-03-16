using UnityEngine;
using System.Collections.Generic;

public class PoolingSystem : MonoBehaviour
{
    public List<GameObject> cubeObjects = new();
    public List<GameObject> waveObjects = new();

    private Dictionary<ObstacleType, List<GameObject>> poolDict;

    void Awake()
    {
        poolDict = new Dictionary<ObstacleType, List<GameObject>>
        {
            { ObstacleType.cube, cubeObjects },
            { ObstacleType.wave, waveObjects }
        };
    }

    public GameObject GetAvailable(ObstacleType type)
    {
        if (!poolDict.ContainsKey(type)) return null;

        foreach (GameObject obj in poolDict[type])
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        return null;
    }

    public void Release(GameObject obj)
    {
        obj.SetActive(false);
    }
    
    public bool AreAllBlocksInactive()
    {
        foreach (GameObject obj in cubeObjects)
        {
            if (obj.activeInHierarchy)
                return false;
        }

        foreach (GameObject obj in waveObjects)
        {
            if (obj.activeInHierarchy)
                return false;
        }

        return true;
    }
}