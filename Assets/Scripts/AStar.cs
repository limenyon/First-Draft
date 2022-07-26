using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public ShowNavmesh navmeshScript;
    public FindNearestUnobstructed findNearestUnobstructed;
    public GameObject player;
    public GetNeighbour getNeighbours;
    Vector3 startNode;
    Vector3 endNode;
    float[,] adjacencyMatrix;
    List<Vector3> vertices;
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            adjacencyMatrix = navmeshScript.adjacencyMatrix.GetMatrix();
            vertices = navmeshScript.vertices;
            startNode = player.transform.position;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit);
            if (hasHit)
            {
                endNode = hit.point;
            }
            RecalculatePath();
        }
    }
    private List<int> RecalculatePath()
    {
        List<int> path = new List<int>();
        var start = findNearestUnobstructed.FindNearestIndexUnobstructed(navmeshScript.meshData.vertices, startNode);
        var end = findNearestUnobstructed.FindNearestIndexUnobstructed(navmeshScript.meshData.vertices, endNode);
        path = AStarAlgorith(adjacencyMatrix, start, end);
        return path;
    }
    List<int> AStarAlgorith(float[,] adjacencyMatrix, int start, int end)
    {
        int[] parent = new int[(int)Mathf.Sqrt(adjacencyMatrix.Length)];
        List<int> openSet = new List<int>();
        HashSet<int> closedSet = new HashSet<int>();
        openSet.Add(start);
        int test = 0;
        while(openSet.Count > 0)
        {
            int currentVertex = openSet[0];
            for(int i = 0; i < openSet.Count; i++)
            {
                float setNodeCost = GetFCost(GetGCost(adjacencyMatrix, openSet[i], start), GetHCost(vertices, openSet[i], end));
                float currentNodeCost = GetFCost(GetGCost(adjacencyMatrix, currentVertex, start), GetHCost(vertices, currentVertex, end));
                if (setNodeCost < currentNodeCost || setNodeCost == currentNodeCost && GetHCost(vertices, openSet[i], end) < GetHCost(vertices, currentVertex, end))
                {
                    currentVertex = openSet[i];
                    Debug.Log("Current vertex" + currentVertex);
                }
            }
            openSet.Remove(currentVertex);
            closedSet.Add(currentVertex);

            if(currentVertex == end)
            {
                break;
            }
            var neighbours = getNeighbours.GetNeighbours(adjacencyMatrix, currentVertex);
            foreach(int neighbour in neighbours)
            {
                float costToNeighbour = GetGCost(adjacencyMatrix, currentVertex, start) + adjacencyMatrix[currentVertex, neighbour];
                if (costToNeighbour < GetGCost(adjacencyMatrix, neighbour, start) || !openSet.Contains(neighbour))
                {
                    parent[neighbour] = currentVertex;
                    if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
            Debug.Log(test);
            test++;
        }
        List<int> path = new List<int>();
        int currentNode = end;
        while(currentNode != start)
        {
            path.Add(currentNode);
            currentNode = parent[currentNode];
        }
        path.Reverse();
        Debug.Log(test);
        return path;
    }
    private float GetHCost(List<Vector3> vertices, int currentVertex, int endVertex)
    {
        float x = Mathf.Abs(vertices[currentVertex].x - vertices[endVertex].x);
        float y = Mathf.Abs(vertices[currentVertex].y - vertices[endVertex].y);
        float z = Mathf.Abs(vertices[currentVertex].z - vertices[endVertex].z);
        Debug.Log("h");
        return Mathf.Sqrt(x * x + y * y + z * z);
    }
    private float GetGCost(float[,] adjacencyMatrix, int currentVertex, int startVertex)
    {
        float g = adjacencyMatrix[currentVertex, startVertex];
        return g;
    }
    private float GetFCost(float h, float g)
    {
        float f = h + g;
        return f;
    }
}