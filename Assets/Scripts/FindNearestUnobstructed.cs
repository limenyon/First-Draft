using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FindNearestUnobstructed : MonoBehaviour
{
    public int FindNearestIndexUnobstructed(Vector3[] vertices, Vector3 position)
    {
        int nearestIndex = -1;
        float nearestDistance = float.MaxValue;
        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = Vector3.Distance(vertices[i], position);
            if (distance < nearestDistance)
            {
                if (!NavMesh.Raycast(vertices[i], position, out NavMeshHit hit, NavMesh.AllAreas))
                {
                    nearestIndex = i;
                    nearestDistance = distance;
                }
            }
        }
        return nearestIndex;
    }
}