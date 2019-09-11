using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelpers 
{
    public static List<RaycastHit> PreventCollision(Func<RaycastHit> raycastFunction, ref Vector3 velocity, Transform transform, float DeltaTime, float skinWidth = 0.0f, float bounciness = 0.0f)
    {
        RaycastHit hit;
        List<RaycastHit> raycastHits = new List<RaycastHit>();
        int saftyCounter = 0;
        while ((hit = raycastFunction()).collider != null && saftyCounter++ < 100)
        {
            float distanceToCorrect = skinWidth / Vector3.Dot(velocity.normalized, hit.normal);
            float distanceToMove = hit.distance + distanceToCorrect;

            if (distanceToMove <= velocity.magnitude * DeltaTime)
            {
                raycastHits.Add(hit);
                if (distanceToMove > 0.0f)
                    transform.position += velocity.normalized * distanceToMove;
                velocity += CalculateNormalForce(hit.normal, velocity) * (1.0f + bounciness);
            }
            else
                break;
        }
        return raycastHits;
    }

    public static Vector3 CalculateNormalForce(Vector3 normal, Vector3 velocity)
    {
        float dot = Vector3.Dot(velocity, normal.normalized);
        return -normal.normalized * (dot > 0.0f ? 0 : dot);
    }
}
