using UnityEngine;

public class RotateMe : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private Vector3 axisToRotateAround;

    void Update()
    {
        this.transform.Rotate(axisToRotateAround, speed * Time.deltaTime);
    }
}
