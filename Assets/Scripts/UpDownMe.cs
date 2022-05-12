using UnityEngine;

public class UpDownMe : MonoBehaviour
{
    [SerializeField] private float frequency = 0.5f;
    [SerializeField] private float amplitude = 1.0f;
    [SerializeField] private Vector3 axisToMove;

    void Update()
    {
        this.transform.position += amplitude * Mathf.Sin(Mathf.PI * 2.0f * frequency * Time.time) * axisToMove;
    }
}
