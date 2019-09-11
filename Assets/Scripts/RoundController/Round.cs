using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "RoundController/New Round")]
public class Round : ScriptableObject
{
    [SerializeField] public int ThreatLevel;

    [EventRef] public string Song; 
    public List<RoundController.Slot> slots = new List<RoundController.Slot>();
}
