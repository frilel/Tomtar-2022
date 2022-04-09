using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstrainedPathObject : MonoBehaviour
{
    public enum StopTypeOptions {
        NONE,
        StopAtStart,
        StopAtEnd,
        StopAtStartEnd,
    };
    public StopTypeOptions StopType;
    public float speed;
    public bool stopped;

    public LineRenderer lineRenderer;
    public Transform path;
    public Transform obj;

    private float pointInterpolator = 0f;
    private int maxIndex;
    private int direction = 1; // 1 is forwards, -1 is backwards
    private float destination = -1f;


    /* MagicMoveable parameters */
    // (hardcoded b/c they should be the same for all objects)
    private const float aimThreshold = 0.2f; // Aiming within this distance of the path will latch to it
    private const float magicSpeed = 5f;
    

    // Start is called before the first frame update
    void Start()
    {
        if (path.childCount == 1) {
            Debug.LogWarning("ConstrainedPathObject with insufficient Path nodes: " + this.name);
        }

        maxIndex = path.childCount - 1;
        lineRenderer = GetComponentInChildren<LineRenderer>();
        ResetLineRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        if (destination >= 0){
            // We have a target destination, move towards it
            int pointIndex = Mathf.FloorToInt(pointInterpolator) % maxIndex;

            Vector3 start = GetPosition(pointIndex);
            Vector3 end = GetPosition(pointIndex+1);

            // Increase lerp value relative to the distance between points to keep the speed consistent.
            pointInterpolator += direction * magicSpeed * Time.deltaTime / Vector3.Distance(start, end);
            
            // Check if reached destination
            if (direction == 1 && pointInterpolator >= destination){
                pointInterpolator = destination;
                destination = -1;
            } else if (direction == -1 && pointInterpolator <= destination) {
                pointInterpolator = destination;
                destination = -1;
            }
            
            // Move smoothly to next point
            obj.position = Vector3.Lerp(start, end, pointInterpolator-pointIndex);
        }
        else if (!stopped) {
            if (pointInterpolator > maxIndex || pointInterpolator < 0){
                pointInterpolator = Mathf.Clamp(pointInterpolator, 0, maxIndex);
                switch (StopType){
                    case StopTypeOptions.StopAtEnd:
                        if (direction == 1) stopped = true;
                        break;
                    case StopTypeOptions.StopAtStart:
                        if (direction == -1) stopped = true;
                        break;
                    case StopTypeOptions.StopAtStartEnd:
                        stopped = true;
                        break;
                }
                direction = -1*direction; // Reflect at ends
            }
            int pointIndex = Mathf.FloorToInt(pointInterpolator) % maxIndex;

            Vector3 start = GetPosition(pointIndex);
            Vector3 end = GetPosition(pointIndex+1);

            // Increase lerp value relative to the distance between points to keep the speed consistent.
            pointInterpolator += direction * speed * Time.deltaTime / Vector3.Distance(start, end);
            
            // Move smoothly to next point
            obj.position = Vector3.Lerp(start, end, pointInterpolator-pointIndex);
        }
    }

    public void MoveByAim(Ray ray)
    {       
        // Get point closest to ray segment-by-segment and choose the best
        float minError = float.PositiveInfinity;
        float minDepth = float.PositiveInfinity;
        bool usingDepth = false;
        for(int i = 0; i < path.childCount - 1; i++){
            Vector3 start = GetPosition(i);
            Vector3 end = GetPosition(i+1);
            if (ClosestPoint(ray, start, end, out Vector3 point, out float tAlongSegment, out float error, out float depth)) {
                // TODO: Should aimThreshold be in screen space and not world space so that
                //  the threshold isn't effectively larger for closer objects? Screen space might
                //  be more intuitive, but current setup does further aid the idea that you'll tend
                //  to be aiming for the closer thing
                
                // Within threshold, choose based on depth
                bool betterDepth = error < aimThreshold*aimThreshold && depth < minDepth;
                if (betterDepth) usingDepth = true;

                // If we haven't had points within threshold, keep using distance
                bool betterDistance = !usingDepth && error < minError;
                
                if (betterDistance) minError = error;
                if (betterDepth) minDepth = depth;
                if (betterDistance || betterDepth){
                    destination = i + tAlongSegment;
                    direction = (int)Mathf.Sign(destination - pointInterpolator);
                    Vector3 intersection = ray.GetPoint(depth);

                    // Debug lines to show aim decision making
                    // Debug.DrawLine(intersection, point, Color.red, Time.deltaTime);
                    // Debug.DrawRay(point, (intersection - point).normalized * aimThreshold, Color.green, Time.deltaTime);
                }
            }
        }
    }

    /**
     * bool ClosestPoint(Ray ray, Vector3 segStart, Vector3 segEnd, out Vector3 closestPoint, out float missSqrDistance)
     * DESCRIPTION: Clamps an aim ray onto a line segment as close as possible
     * RETURNS: Boolean representing whether a clamp was possible. (Would be impossible if parallel)
     * OUTPUT: Also outputs the actual closest point along the line segment as well as the distance squared between the ray
     *      and the end result of closestPoint
     * USAGE: Used internally to position the element along it's constrained path via an aim ray. missSqrDistance is used to
     *      rate the quality of this closest point when compared against other line segments which are part of the path
     */
    bool ClosestPoint(Ray ray, Vector3 segStart, Vector3 segEnd, out Vector3 closestPoint, out float tAlongSegment, out float missSqrDistance, out float rayLength)
    {
        // Create a plane which contains the segment and is perpendicular to ray.direction
        Vector3 segDir = segEnd - segStart;
        Vector3 cross = Vector3.Cross(ray.direction, segDir);
        Vector3 planeNormal = Vector3.Cross(cross, segDir);
        Plane plane = new Plane(planeNormal, segStart);

        if(plane.Raycast(ray, out float t)){
            Vector3 intersection = ray.GetPoint(t);
            Vector3 offset = Vector3.Project(intersection - segStart, segDir);
            if (Vector3.Dot(offset, segDir) < 0){
                closestPoint = segStart;
                tAlongSegment = 0;
            } else if (offset.sqrMagnitude > segDir.sqrMagnitude) {
                closestPoint = segEnd;
                tAlongSegment = 1;
            } else {
                closestPoint = segStart + offset;
                tAlongSegment = offset.magnitude / Vector3.Distance(segStart, segEnd);
            }

            missSqrDistance = (intersection - closestPoint).sqrMagnitude;
            rayLength = t;
            return true;
        }

        closestPoint = Vector3.zero;
        tAlongSegment = -1;
        missSqrDistance = float.PositiveInfinity;
        rayLength = float.PositiveInfinity;
        return false; 
    }


    public void ResetLineRenderer(){
        int count = path.childCount;
        lineRenderer.positionCount = count;
        Vector3[] points = new Vector3[count];
        for(int i = 0; i < count; i++){
            points[i] = path.GetChild(i).position;
        }
        lineRenderer.SetPositions(points);
    }


    private Vector3 GetPosition(int i){
        return path.GetChild(i).transform.position;
    }
}
