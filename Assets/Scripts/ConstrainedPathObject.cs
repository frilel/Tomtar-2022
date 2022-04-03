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
    public bool loop;
    public float speed;

    public LineRenderer lineRenderer;
    public GameObject obj;
    private float pointInterpolator = 0f;
    private int maxIndex;
    private int direction = 1; // 1 is forwards, -1 is backwards
    public bool stopped {get; set;} = false;
    

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        maxIndex = lineRenderer.positionCount - 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopped) {
            if (pointInterpolator > maxIndex || pointInterpolator < 0){
                pointInterpolator = loop ? Mathf.Clamp(pointInterpolator, 0, maxIndex) : 0;
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

            Vector3 start = lineRenderer.GetPosition(pointIndex);
            Vector3 end = lineRenderer.GetPosition(pointIndex+1);

            // Increase lerp value relative to the distance between points to keep the speed consistent.
            pointInterpolator += direction * speed * Time.deltaTime / Vector3.Distance(start, end);
            
            // Move smoothly to next point
            obj.transform.position = Vector3.Lerp(start, end, pointInterpolator-pointIndex);
        }
    }
}
