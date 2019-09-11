using UnityEngine;
//Author: Simon

[RequireComponent(typeof(LineRenderer))]
public class CircleTool : MonoBehaviour
{

    public Vector3 t = Vector3.one;
    public LineRenderer GetLineRender()
    {
        return GetComponent<LineRenderer>();
    }

}
