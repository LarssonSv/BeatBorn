using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoundController))]
public class RoundControllerEdit : Editor
{
    RoundController controller;

    void OnEnable()
    {
        controller = (RoundController)target;

    }

    public override void OnInspectorGUI()
    {

        if (GUILayout.Button("Reset Rounds:"))
        {
            Debug.Log("Resetting Rounds!");
            controller.ResetRound();
        }


        base.OnInspectorGUI();



    }
}
