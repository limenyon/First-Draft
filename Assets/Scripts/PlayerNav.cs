using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerNav : MonoBehaviour
{
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;
    [SerializeField] private Transform visualObject;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Move();
        }
        else if(agent.hasPath)
        {
            DrawPath();
        }
    }
    private void Move()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit);
        if(hasHit)
        {
            SetDestination(hit.point);
            NavMeshPath navPath = new NavMeshPath();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            NavMesh.CalculatePath(transform.position, hit.point, NavMesh.AllAreas, navPath);
            stopwatch.Stop();
            UnityEngine.Debug.Log(stopwatch.Elapsed);
        }
    }
    private void SetDestination(Vector3 target)
    {
        agent.SetDestination(target);
    }

    private void DrawPath()
    {
        lineRenderer.positionCount = agent.path.corners.Length;
        lineRenderer.SetPosition(0, transform.position);
        if(agent.path.corners.Length < 2)
        {
            return;
        }
        for(int i = 1; i < agent.path.corners.Length; i++)
        {
            Vector3 position = new Vector3(agent.path.corners[i].x, agent.path.corners[i].y + 0.1f, agent.path.corners[i].z);
            lineRenderer.SetPosition(i, position);
        }
    }
}
