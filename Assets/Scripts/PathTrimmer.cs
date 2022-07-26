using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathTrimmer : MonoBehaviour
{
    private NavMeshHit hit;
    public List<int> TrimPath(Vector3 start, Vector3 end, List<int> path, Vector3[] vertices)
    {
        List<int> nodesToRemove = new List<int>();
        for(int i = 0; i < path.Count; i++)
        {
            //!Physics.Linecast(start, vertices[path[i]])
            if (!NavMesh.Raycast(start, vertices[path[i]], out hit, NavMesh.AllAreas))
            {
                nodesToRemove.Add(path[i]);
            }
            else if(nodesToRemove.Count > 0)
            {
                nodesToRemove.Remove(nodesToRemove[nodesToRemove.Count - 1]);
                break;
            }
            else
            {
                break;
            }
        }
        List<int> nodesToRemoveBack = new List<int>();
        if(path.Count > 0)
        {
            for(int j = path.Count; j > 0; j--)
            {
                if (!NavMesh.Raycast(end, vertices[path[j - 1]], out hit, NavMesh.AllAreas))
                {
                    nodesToRemoveBack.Add(path[j - 1]);
                }
                else if(nodesToRemoveBack.Count > 0)
                {
                    nodesToRemoveBack.Remove(nodesToRemoveBack[nodesToRemoveBack.Count - 1]);
                    break;
                }
                else
                {
                    break;
                }
            }
            //These have to happen after the checks otherwise there are possible scenarios where the path goes through walls
            foreach(int pathToRemove in nodesToRemove)
            {
                path.Remove(pathToRemove);
            }
            foreach(int pathToRemove in nodesToRemoveBack)
            {
                path.Remove(pathToRemove);
            }
        }
        return path;
    }
}
