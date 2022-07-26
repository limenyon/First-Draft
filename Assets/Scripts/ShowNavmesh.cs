using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEditor;

public class ShowNavmesh : MonoBehaviour
{
	private LineRenderer lineRenderer;
	public Material myMaterial;
	public List<TriangleData> triangleList = new List<TriangleData>();
	public AdjacencyMatrix adjacencyMatrix;
	public NavMeshTriangulation meshData;
	public List<int> indices;
	public List<Vector3> vertices;
	public int limiter = 0;
	public List<GameObject> gameObjects;

	void Start()
	{
		gameObjects = new List<GameObject>();
		lineRenderer = GetComponent<LineRenderer>();
		meshData = NavMesh.CalculateTriangulation();
		indices = new List<int>();
		vertices = new List<Vector3>();
		FixUnityGarbage();
		CreateTriangles();
		CreateLines();
		CreateMeshes();
		adjacencyMatrix = CreateAdjacencyMatrix();
	}
	void FixUnityGarbage()
    {
		int i = 0;
		foreach (int index in meshData.indices)
		{
			indices.Add(meshData.indices[i]);
			i++;
		}
		i = 0;
		foreach (Vector3 vertex in meshData.vertices)
		{
			vertices.Add(meshData.vertices[i]);
			i++;
		}
		List<int> verticesToRemove = new List<int>();
		List<int> numbersToRemoveWith = new List<int>();
		for (int j = 0; j < vertices.Count; j++)
		{
			for (int k = 0; k < j; k++)
			{
				if (Vector3.Distance(vertices[j], vertices[k]) < Mathf.Epsilon)
				{
					verticesToRemove.Add(j);
					numbersToRemoveWith.Add(k);
					break;
				}
			}
		}
		for (int x = 0; x < verticesToRemove.Count; x++)
		{
			for (int y = 0; y < indices.Count; y++)
			{
				if (indices[y] == verticesToRemove[x])
				{
					indices[y] = numbersToRemoveWith[x];
				}
			}
		}
		for (int z = verticesToRemove.Count; z > 0; z--)
		{
			Vector3 temp = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			vertices[verticesToRemove[z - 1]] = temp;
		}
	}
	void CreateTriangles()
    {
		for(int i = 0; i < meshData.indices.Length; i += 3)
        {
			float randomNumber = Random.Range(0f, 1f);
			Color materialColor = new Color(randomNumber, 0.5f, randomNumber);
			Material tempMaterial = new Material(myMaterial);
			tempMaterial.SetColor("_Color", materialColor);
			TriangleData triangle = new TriangleData(vertices[indices[i]], vertices[indices[i+1]], vertices[indices[i+2]], tempMaterial);
			triangleList.Add(triangle);
        }
    }
	void CreateLines()
    {
		foreach (TriangleData triangle in triangleList)
        {
			GameObject triangleObject = new GameObject();
			triangleObject.AddComponent<LineRenderer>();
			triangleObject.GetComponent<LineRenderer>().positionCount = 3;
			triangleObject.GetComponent<LineRenderer>().SetPositions(triangle.GetVertices());
			triangleObject.GetComponent<LineRenderer>().endWidth = 0.05f;
			triangleObject.GetComponent<LineRenderer>().startWidth = 0.05f;
			triangleObject.GetComponent<LineRenderer>().loop = true;
			triangleObject.GetComponent<LineRenderer>().material = myMaterial;
		}
	}
	void CreateMeshes()
    {
		foreach(TriangleData triangle in triangleList)
        {
			GameObject triangleObject = new GameObject();
			triangleObject.AddComponent<MeshRenderer>();
			triangleObject.AddComponent<MeshFilter>();
			Mesh triangleMesh = new Mesh();
			triangleObject.GetComponent<MeshFilter>().mesh = triangleMesh;
			triangleMesh.Clear();
			triangleMesh.vertices = triangle.GetVertices();
			triangleMesh.triangles = triangle.GetIndices();
			triangleObject.GetComponent<MeshRenderer>().material = triangle.GetMaterial();
			triangleObject.transform.parent = transform;
			gameObjects.Add(triangleObject);
        }
    }

	private void DeleteMeshes()
    {
		for(int i = gameObjects.Count; i > 0; i--)
        {
			Destroy(gameObjects[i - 1]);
			gameObjects.Remove(gameObjects[i - 1]);
        }
		for(int j = triangleList.Count; j > 0; j--)
        {
			triangleList.Remove(triangleList[j - 1]);
        }
    }
	private AdjacencyMatrix CreateAdjacencyMatrix()
    {
		AdjacencyMatrix adjacencyMatrix = new AdjacencyMatrix(meshData.vertices.Length);
		for (int i = 0; i < meshData.indices.Length; i += 3)
        {
			int index1 = indices[i];
			int index2 = indices[i + 1];
			int index3 = indices[i + 2];
			Vector3 vertex1 = vertices[index1];
			Vector3 vertex2 = vertices[index2];
			Vector3 vertex3 = vertices[index3];
			float distance1 = Vector3.Distance(vertex1, vertex2);
			float distance2 = Vector3.Distance(vertex2, vertex3);
			float distance3 = Vector3.Distance(vertex1, vertex3);
			adjacencyMatrix.SetAdjacency(index1, index2, distance1);
			adjacencyMatrix.SetAdjacency(index2, index3, distance2);
			adjacencyMatrix.SetAdjacency(index1, index3, distance3);
        }
		List<float> sexo = new List<float>();
		for(int pee = 0; pee < adjacencyMatrix.length(); pee++)
        {
			sexo.Add(adjacencyMatrix.GetMatrix()[37, pee]);
        }
		return adjacencyMatrix;
    }
}

[CustomEditor(typeof(ShowNavmesh))]
public class MeshEditor : Editor
{
	void OnSceneGUI()
	{
		var t = target as ShowNavmesh;

		foreach (int i in t.indices)
		{
			var v = t.vertices[i];
			Handles.Label(v + Vector3.up * 0.15f, i.ToString());
        }
	}
}