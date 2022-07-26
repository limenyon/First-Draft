using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjacencyMatrix
{
    float[,] adjacencyMatrix;
    public AdjacencyMatrix(int size)
    {
        adjacencyMatrix = new float[size, size];
    }
    public float[,] GetMatrix()
    {
        return adjacencyMatrix;
    }
    public void SetAdjacency(int posX, int posY, float distance)
    {
        adjacencyMatrix[posX, posY] = distance;
        adjacencyMatrix[posY, posX] = distance;
    }
    public float length()
    {
        return Mathf.Sqrt(adjacencyMatrix.Length);
    }
}