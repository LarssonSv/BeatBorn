using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(Round))]
public class RoundScriptableEdit : Editor
{
    Round round;
    SerializedObject GetTarget;
    SerializedProperty list;

    RoundController.slotFunction SlotFunction;

    int listSize = 0;

    GUIStyle textStyle = new GUIStyle();
    GUIStyle spawnerStyle = new GUIStyle();
    GUIStyle soundStyle = new GUIStyle();
    GUIStyle waitStyle = new GUIStyle();
    GUIStyle waveStyle = new GUIStyle();
    GUIStyle OpenUIStyle = new GUIStyle();
    GUIStyle ShowScoreStyle = new GUIStyle();

    void OnEnable()
    {
        round = (Round)target;
        GetTarget = new SerializedObject(round);
        list = GetTarget.FindProperty("slots");

        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.normal.textColor = Color.white;

        spawnerStyle.alignment = TextAnchor.MiddleCenter;
        spawnerStyle.normal.textColor = Color.cyan;

        soundStyle.alignment = TextAnchor.MiddleCenter;
        soundStyle.normal.textColor = Color.yellow;

        waitStyle.alignment = TextAnchor.MiddleCenter;
        waitStyle.normal.textColor = Color.green;

        waveStyle.alignment = TextAnchor.MiddleCenter;
        waveStyle.normal.textColor = Color.red;

        OpenUIStyle.alignment = TextAnchor.MiddleCenter;
        OpenUIStyle.normal.textColor = Color.magenta;

        ShowScoreStyle.alignment = TextAnchor.MiddleCenter;
        ShowScoreStyle.normal.textColor = Color.blue;

    }

    public override void OnInspectorGUI()
    {
        GetTarget.Update();




        EditorGUILayout.Space();
        listSize = list.arraySize;

        EditorGUILayout.LabelField("Round Music", textStyle);
        SerializedProperty MySong = GetTarget.FindProperty("Song");
        EditorGUILayout.PropertyField(MySong);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.LabelField("ThreatLevel", textStyle);
        SerializedProperty MyThreat = GetTarget.FindProperty("ThreatLevel");
        EditorGUILayout.PropertyField(MyThreat);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);


        listSize = EditorGUILayout.IntField("Round Events:", listSize);

        if (listSize < 1)
        {
            listSize = 1;
        }

        if (listSize != list.arraySize)
        {
            while (listSize > list.arraySize)
            {
                list.InsertArrayElementAtIndex(list.arraySize);
            }
            while (listSize < list.arraySize)
            {
                list.DeleteArrayElementAtIndex(list.arraySize - 1);
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);


        for (int i = 0; i < list.arraySize; i++)
        {
            SerializedProperty MyListRef = list.GetArrayElementAtIndex(i);
            SerializedProperty MyFunction = MyListRef.FindPropertyRelative("function");
            //Spawner
            SerializedProperty MyEnemyType = MyListRef.FindPropertyRelative("EnemyType");
            SerializedProperty MySpawnPoint = MyListRef.FindPropertyRelative("SpawnPosition");
            SerializedProperty MySpawnAmount = MyListRef.FindPropertyRelative("SpawnAmount");
            SerializedProperty MySpawnTime = MyListRef.FindPropertyRelative("SpawnTime");
            //Waiter
            SerializedProperty MyWaitTime = MyListRef.FindPropertyRelative("WaitFor");
            //Sound
            SerializedProperty MySound = MyListRef.FindPropertyRelative("EventRef");
            SerializedProperty MyMessage = MyListRef.FindPropertyRelative("message");
            SerializedProperty MySprite = MyListRef.FindPropertyRelative("sprite");
            SerializedProperty MyType = MyListRef.FindPropertyRelative("type");



            if (MyListRef.FindPropertyRelative("function").enumValueIndex == (int)RoundController.slotFunction.Spawn)
            {

                EditorGUILayout.LabelField("Spawner", spawnerStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(MyFunction);
                EditorGUILayout.PropertyField(MyEnemyType);
                EditorGUILayout.PropertyField(MySpawnPoint);
                EditorGUILayout.PropertyField(MySpawnAmount);
                if (MySpawnAmount.intValue < 1)
                {
                    MySpawnAmount.intValue = 1;
                }
                EditorGUILayout.PropertyField(MySpawnTime);
                if (MySpawnTime.floatValue < i)
                {
                    MySpawnTime.floatValue = i;
                }

            }
            else if (MyListRef.FindPropertyRelative("function").enumValueIndex == (int)RoundController.slotFunction.Wait)
            {
                EditorGUILayout.LabelField("Waiter", waitStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(MyFunction);
                EditorGUILayout.PropertyField(MyWaitTime);
                EditorGUILayout.PropertyField(MySpawnTime);
                if (MySpawnTime.floatValue < i)
                {
                    MySpawnTime.floatValue = i;
                }

            }
            else if (MyListRef.FindPropertyRelative("function").enumValueIndex == (int)RoundController.slotFunction.WaveClear)
            {
                EditorGUILayout.LabelField("Wave Clear", waveStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(MyFunction);
                EditorGUILayout.PropertyField(MySpawnTime);
                if (MySpawnTime.floatValue < i)
                {
                    MySpawnTime.floatValue = i;
                }

            }
            else if (MyListRef.FindPropertyRelative("function").enumValueIndex == (int)RoundController.slotFunction.PopUp)
            {
                EditorGUILayout.LabelField("Popup", OpenUIStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(MyFunction);
                EditorGUILayout.PropertyField(MyMessage);
                EditorGUILayout.PropertyField(MySprite);
                EditorGUILayout.PropertyField(MyType);
                EditorGUILayout.PropertyField(MySpawnTime);

                if (MySpawnTime.floatValue < i)
                {
                    MySpawnTime.floatValue = i;
                }
            }
            else if (MyListRef.FindPropertyRelative("function").enumValueIndex == (int)RoundController.slotFunction.ShowScore)
            {
                EditorGUILayout.LabelField("ShowScore", ShowScoreStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(MySpawnTime);

                if (MySpawnTime.floatValue < i)
                {
                    MySpawnTime.floatValue = i;
                }
            }
            else
            {
                EditorGUILayout.LabelField("Sound", soundStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(MyFunction);
                EditorGUILayout.PropertyField(MySound);
                EditorGUILayout.PropertyField(MySpawnTime);
                if (MySpawnTime.floatValue < i)
                {
                    MySpawnTime.floatValue = i;
                }

            }
            if (GUILayout.Button("Remove"))
            {                list.DeleteArrayElementAtIndex(i);
            }



            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();

        round.slots = round.slots.OrderBy(go => go.SpawnTime).ToList();

    }

}