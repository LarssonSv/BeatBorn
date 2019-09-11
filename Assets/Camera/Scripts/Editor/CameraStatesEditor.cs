using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraDefault), true)]
public class CameraStatesEditor : Editor{
    private CameraDefault camera;

    private void OnEnable() {
        camera = target as CameraDefault;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if(GUILayout.Button("Apply Changes")) {
            camera.ApplyChanges();
        }
    }
}