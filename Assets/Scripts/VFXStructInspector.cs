using System.Collections;
using System.Collections.Generic;
#if UNITY_ENGINE
    using UnityEditor;

using UnityEngine;
[CustomEditor(typeof(VFXStruct))]
public class VFXStructInspector : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedProperty graphObject = serializedObject.FindProperty("VFXGraph");
        graphObject.objectReferenceValue = EditorGUILayout.ObjectField(graphObject.objectReferenceValue,null);
    }
}
#endif