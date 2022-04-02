using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on https://www.youtube.com/watch?v=RnEO3MRPr5Y

public class TrajectoryRenderer : MonoBehaviour
{
    public int numPoints = 10;
    public float timeBetweenPoints = 0.5f;

    public LayerMask stopLayers;

    private ProjectileController projectileController;
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Load componenets
        projectileController = GetComponent<ProjectileController>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.positionCount = (int)numPoints;
        List<Vector3> points = new List<Vector3>();
        Vector3 initialPosition = projectileController.shotPoint.position;
        Vector3 initialVelocity = projectileController.shotPoint.forward * projectileController.initialSpeed;
        
        for (float i = 0; i < numPoints; i++) {
            float t = i * timeBetweenPoints;

            // Use kinematic equations to find new position
            Vector3 newPoint = initialPosition + initialVelocity * t;
            newPoint.y = initialPosition.y + initialVelocity.y * t + Physics.gravity.y/2f * t * t;
            points.Add(newPoint);

            // Once we collide with something, stop!
            if (Physics.OverlapSphere(newPoint, 2, stopLayers).Length > 0) {
                lineRenderer.positionCount = points.Count;
                break;
            }
        }

        lineRenderer.SetPositions(points.ToArray());
    }
}
