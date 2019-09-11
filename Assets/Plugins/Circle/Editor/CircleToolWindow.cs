using UnityEngine;
using UnityEditor;
using System.Collections;


public class CircleToolWindow : EditorWindow
{

    [SerializeField] int sides;
    [SerializeField] float radius;
    [SerializeField] Object lineRend;
    [SerializeField] bool flip;





    [MenuItem("Window/Circle")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CircleToolWindow));
    }

    private void OnGUI()
    {
        GUILayout.Label("Settings: ", EditorStyles.boldLabel);
        sides = EditorGUILayout.IntField("Sides", sides);
        radius = EditorGUILayout.Slider("Radius", radius, 0f, 100f);
        flip = EditorGUILayout.Toggle("Flip", flip);
        lineRend = EditorGUILayout.ObjectField(lineRend, typeof(LineRenderer), true);


        if (lineRend)
        {
            CreatePoints();
        }


    }

    void CreatePoints()
    {
        {
            float x = 0;
            float y = 0;
            float z = 0;        

            float angle = 20f;

            for (int i = 0; i < (sides + 1); i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                if(flip == false)
                {
                    z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                }
                else
                {
                    y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                }
                

                LineRenderer temp = lineRend as LineRenderer;
                temp.useWorldSpace = false;
                temp.positionCount = (sides + 1);
                temp.SetPosition(i, new Vector3(x, y, z));

                angle += (360f / sides);
            }
        }
    }

}
