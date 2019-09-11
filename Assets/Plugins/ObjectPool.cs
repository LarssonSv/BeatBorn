using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool {

    private static Dictionary<GameObject, List<GameObject>> poolAvailable = new Dictionary<GameObject, List<GameObject>>();
    private static Dictionary<GameObject, GameObject> PrefabMapper = new Dictionary<GameObject, GameObject>();
    private static int SizeToAddAtFull = 5;
    private static Transform _transformInstance;
    private static Transform transform {
        get {
            if (_transformInstance == null)
                _transformInstance = new GameObject("ObjectPool").transform;

            return _transformInstance;
        }
    }

    private static  GameObject GetObjectFromPool(GameObject objKey)
    {
        if (poolAvailable[objKey].Count <= 0) return null;
        GameObject obj = poolAvailable[objKey][0];

        PrefabMapper[obj] = objKey;
        poolAvailable[objKey].Remove(obj);
        obj.transform.localScale = objKey.transform.localScale;
        obj.transform.rotation = objKey.transform.rotation;
        obj.transform.position = objKey.transform.position;
        obj.SetActive(true);
        return obj;
    }
    //Public facing methods
    public static void Destroy(GameObject Object)
    {
        if (PrefabMapper.ContainsKey(Object))
        {
            poolAvailable[PrefabMapper[Object]].Add(Object);
            Object.transform.parent = transform;
            Object.SetActive(false);
        }
        else
            UnityEngine.Object.Destroy(Object);
    }
    public static GameObject Instantiate(GameObject prefab, Vector3 posistion, Quaternion rotation, Transform parent = null)
    {
        GameObject temp = Instantiate(prefab);
        temp.transform.position = posistion;
        temp.transform.rotation = rotation;
        temp.transform.parent =  parent == null ? transform : parent;
        return temp;
    }
    public static GameObject Instantiate(GameObject prefab)
    {
        if (!poolAvailable.ContainsKey(prefab) || poolAvailable[prefab][0] == null) poolAvailable[prefab] = new List<GameObject>();
        if (poolAvailable[prefab].Count <= 0)
        {
            //Add object to fill out pool
            for (int i = 0; i < SizeToAddAtFull; i++)
            {
                GameObject temp = GameObject.Instantiate(prefab);
                temp.transform.parent = transform;
                temp.name = prefab.name + "(" + i + ")";
                temp.SetActive(false);
                poolAvailable[prefab].Add(temp);
            }
        }
    
        return GetObjectFromPool(prefab);
    }

    public static void Clear()
    {
        poolAvailable = new Dictionary<GameObject, List<GameObject>>();
        PrefabMapper = new Dictionary<GameObject, GameObject>();
        _transformInstance = null;
    }
}
