﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;

    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public static ObjectPooler Instance;

    private void Awake() {
        Instance = this;
    }


    void Start () {

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in pools) {

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab);
                obj.name = pool.prefab.name + $"({i})";
                obj.SetActive(false);
                objectPool.Enqueue(obj);

            }

            poolDictionary.Add(pool.tag, objectPool);

        }
	}

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null) {

        if (!poolDictionary.ContainsKey(tag)) {
            Debug.LogWarning("Key does not exist");
            return null;

        }


        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        if (parent != null)
        {
            objectToSpawn.transform.parent = parent;
        }


        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();

        if (pooledObj != null) {
            pooledObj.OnObjectSpawn();

        }

     

        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;

    }
}
