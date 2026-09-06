using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void OnObjectSpawn();
}

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string poolKey;
        public GameObject prefab;
        public int initialSize = 5;
    }

    public List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> prefabDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        prefabDictionary = new Dictionary<string, GameObject>();

        foreach (Pool pool in pools)
        {
            if (prefabDictionary.ContainsKey(pool.poolKey)) continue;

            Queue<GameObject> objectPool = new Queue<GameObject>();
            prefabDictionary.Add(pool.poolKey, pool.prefab);

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = CreateNewObject(pool.prefab);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.poolKey, objectPool);
        }
    }

    public GameObject SpawnFromPool(string poolKey, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(poolKey))
        {
            Debug.LogWarning($"Pool key not found: {poolKey}");
            return null;
        }

        GameObject objToSpawn;

        if (poolDictionary[poolKey].Count > 0)
        {
            objToSpawn = poolDictionary[poolKey].Dequeue();
        }
        else
        {
            objToSpawn = CreateNewObject(prefabDictionary[poolKey]);
        }

        objToSpawn.transform.SetPositionAndRotation(position, rotation);
        objToSpawn.SetActive(true);

        if (objToSpawn.TryGetComponent(out IPoolable poolable))
        {
            poolable.OnObjectSpawn();
        }

        return objToSpawn;
    }

    public void ReturnToPool(string poolKey, GameObject obj)
    {
        if (obj == null || !obj.activeSelf) return;

        obj.SetActive(false);
        obj.transform.SetParent(transform);

        if (poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary[poolKey].Enqueue(obj);
        }
    }

    private GameObject CreateNewObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        return obj;
    }
}