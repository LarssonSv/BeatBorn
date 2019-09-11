using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StateKraft;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(State), true)]
public class StateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (!GUILayout.Button("Update Values")) return;

        //Find all created states in play
        State[] instantiatedStates = FindObjectsOfType<State>();
        //Find those that are instances of this object
        List<State> states = instantiatedStates.Where(state => state.GetType() == GetType()).ToList();
        //Reinitialize the states in their statemachines with the new variables
        foreach (State state in states)
        {
            state.StateMachine.ReinitializeState((State)target);
        }
    }
}
