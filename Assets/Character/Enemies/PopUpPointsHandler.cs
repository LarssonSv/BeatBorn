using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpPointsHandler : MonoBehaviour
{
    [SerializeField] private Vector3 spawnLocationOffeset = new Vector3(0f,2f,0f);


    private ObjectPooler OP;
    readonly private string spawnTag = "ComboEffect";
    private Vector3 heading = new Vector3();

    private GameObject cameraObject;

    public static PopUpPointsHandler Instance;
    private void Start()
    {
        cameraObject = GameObject.FindObjectOfType<Camera>().gameObject;
        OP = ObjectPooler.Instance;
        Instance = this;
    }


    public void OnEnemyHitSpawn(Vector3 pos, RhythmGrade grade)
    {  
        GameObject temp = Instance.OP.SpawnFromPool(spawnTag, pos + spawnLocationOffeset, Quaternion.identity);
        heading = cameraObject.transform.position - temp.transform.position;
        temp.transform.rotation = Quaternion.LookRotation(heading, Vector3.up);
        temp.GetComponent<ComboEffectManager>().grade = (int)grade;

    }


}
