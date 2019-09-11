using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CircleTool)), CanEditMultipleObjects]
public class CircleToolScene : Editor
{
    LineRenderer lineRend;


    private void OnSceneGUI()
    {
        CircleTool tool = (CircleTool)target;

        lineRend = tool.GetLineRender();

        if (lineRend)
        {
            EditorGUI.BeginChangeCheck();
            tool.t = Handles.ScaleHandle(tool.t, tool.transform.position, Quaternion.identity, 2f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Adjusted Circle");
            }

            CreatePoints(tool.t.x, Mathf.RoundToInt(tool.t.y));
        }
       
    }

    void CreatePoints(float radius, int sides)
    {
        {
            float x = 0;
            float y = 0;
            float z = 0;

            float angle = 20f;

            for (int i = 0; i < (sides + 1); i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

                lineRend.useWorldSpace = false;
                lineRend.positionCount = (sides + 1);
                lineRend.SetPosition(i, new Vector3(x, y, z));

                angle += (360f / sides);
            }
        }
    }


}
