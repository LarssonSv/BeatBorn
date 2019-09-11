using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;

[System.Serializable]
public struct NpcSlot
{
    public Vector3 pos => partent ? partent.position + localPos : Vector3.zero;
    public Transform partent;
    public Vector3 localPos;
    public bool taken;
    public int nexusIndex;
    public int playerIndex;
}

public class NexusCrowdManager : MonoBehaviour
{
    public int Slots = 10;
    public float Radius = 3f;
    public NpcSlot[] NpcSlots = new NpcSlot[0];
    public int MaxHealth;
    private int currentHealt;

    public SphereCollider centerPoint;

    private void Start()
    {
        centerPoint = GetComponent<SphereCollider>();
    }

    public static NexusCrowdManager Instance
    {
        get
        {
            if (!_instanceHelper)
                _instanceHelper = FindObjectOfType<NexusCrowdManager>();

            if (!_instanceHelper)
                Debug.LogError("There is no Nexus crowd-manager in the scene");

            return _instanceHelper;
        }

        
    }
    private static NexusCrowdManager _instanceHelper;

    public NpcSlot RequestSlot(Vector3 npcPos)
    {
        if (NpcSlots.Length == 0)
            NpcSlots = CreatePoints(Radius, Slots);

        float dist = float.PositiveInfinity;
        float newDist = 0;
        int index = 0;

        for (int i = 0; i < NpcSlots.Length; i++)
        {
            if (NpcSlots[i].taken) continue;
            newDist = Vector3.Distance(NpcSlots[i].localPos, npcPos);
            if (newDist < dist)
            {
                index = i;
                dist = newDist;
            }
        }
        NpcSlots[index].playerIndex = 99;
        NpcSlots[index].nexusIndex = index;
        NpcSlots[index].taken = true;
        return NpcSlots[index];

    }

    private NpcSlot[] CreatePoints(float _Radius, int _Slots)
    {
        List<NpcSlot> listPoints = new List<NpcSlot>();

        float x = 0;
        float y = 0;
        float z = 0;

        float angle = 20f;

        for (int i = 0; i < _Slots; i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * _Radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * _Radius;

            NpcSlot temp = new NpcSlot();
            temp.localPos = new Vector3(x, y, z);
            temp.taken = false;
            temp.nexusIndex = i;
            temp.partent = transform;

            listPoints.Add(temp);

            angle += (360f / _Slots);
        }

        return listPoints.ToArray();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);

        foreach (NpcSlot x in NpcSlots)
        {
            if (x.taken)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawSphere(x.pos, 0.25f);
        }

    }

    public void SetSlotFree(int npcIndex)
    {
        if (npcIndex >= NpcSlots.Length || npcIndex == -1) return;

        NpcSlots[npcIndex].taken = false;
    }
}
