using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System.Linq;

public class Dijkstra : MonoBehaviour
{
    public GameObject player;
    public ShowNavmesh navmeshScript;
    public LineRenderer lr;
    public PathTrimmer pathTrimmer;
    Vector3 startNode;
    Vector3 endNode;
    public FindNearestUnobstructed findNearestUnobstructed;
    public GetNeighbour getNeighbours;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startNode = player.transform.position;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit);
            if (hasHit)
            {
                endNode = hit.point;
            }
            if(hasHit)
            {
                List<int> path = RecalculatePath();
                if (path != null)
                {
                    var verticesInPath = path.Select(i => navmeshScript.meshData.vertices[i]).ToList();
                    verticesInPath.Insert(0, startNode);
                    verticesInPath.Add(endNode);
                    lr.endWidth = 0.2f;
                    lr.startWidth = 0.2f;
                    Vector3[] finalPath = verticesInPath.ToArray();
                    lr.positionCount = finalPath.Length;
                    lr.SetPositions(finalPath);
                }
            }
        }
    }

    private List<int> RecalculatePath()
    {
            var start = findNearestUnobstructed.FindNearestIndexUnobstructed(navmeshScript.meshData.vertices, player.transform.position);
            var end = findNearestUnobstructed.FindNearestIndexUnobstructed(navmeshScript.meshData.vertices, endNode);
            if (end == -1)
            {
                return null;
            }
            List<int> path = DijkstraAlgorithm(navmeshScript.adjacencyMatrix.GetMatrix(), start, end);
            if (path.Count > 0)
            {
                path = pathTrimmer.TrimPath(player.transform.position, endNode, path, navmeshScript.meshData.vertices);
                return path;
            }
            return null;
    }

    private List<int> DijkstraAlgorithm(float[,] edges, int start, int goal)
    {
        int unreachableSafety = 0;
        int n = edges.GetLength(0);
        float[] distance = new float[n];
        int[] previous = new int[n];

        HashSet<int> unvisited = new HashSet<int>();

        for (int i = 0; i < n; i++)
        {
            distance[i] = float.MaxValue;
            previous[i] = -1;
            unvisited.Add(i);
        }
        distance[start] = 0;
        while (unvisited.Count > 0)
        {
            if (unreachableSafety > n)
            {
                break;
            }
            int u = MinimumDistance(distance, unvisited, n);
            if (u == goal)
            {
                break;
            }
            unvisited.Remove(u);

            var neighbours = getNeighbours.GetNeighbours(edges, u);
            var unvisitedNeighbours = neighbours.Where(v => unvisited.Contains(v));
            foreach (var v in neighbours)
            {
                var alt = distance[u] + edges[u, v];
                if (unvisited.Contains(v) && alt < distance[v] && distance[u] != float.MaxValue)
                {
                    distance[v] = alt;
                    previous[v] = u;
                }
            }
            unreachableSafety++;
        }
        List<int> path = new List<int>();
        int current = goal;

        bool reachable = previous[current] != -1;
        if (reachable || current == start)
        {
            while (current != -1)
            {
                path.Insert(0, current);
                current = previous[current];
            }
        }
        return path;
    }
    private static int MinimumDistance(float[] distance, HashSet<int> unvisited, int n)
    {
        float min = int.MaxValue;
        int minIndex = 0;

        for (int v = 0; v < n; ++v)
        {
            if (unvisited.Contains(v) && distance[v] <= min)
            {
                min = distance[v];
                minIndex = v;
            }
        }
        return minIndex;
    }
}