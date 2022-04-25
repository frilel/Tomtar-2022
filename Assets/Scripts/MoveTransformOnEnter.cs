using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MoveTransformOnEnter : MonoBehaviour
{
    [SerializeField] private bool affectPlayer1 = true;
    [SerializeField] private bool affectPlayer2 = true;
    [SerializeField] private bool affectOtherColliders = false;
    [Tooltip("Assign the material for displaying inactive checkpoint. Default will be current material")]
    [SerializeField] private Material inactiveMaterial;
    [Tooltip("Assign the material for displaying active checkpoint")]
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Transform endPoint;
    [SerializeField] private GameObject objectToMove;
    [SerializeField] private float translationSpeed = 1.0f;
    //[SerializeField] private bool linearSpeed = true; // TODO

    // Lerp
    //private float lerpDuration = 3;
    //private float startValue = 0;
    //private float endValue = 10;
    //float valueToLerp;

    private Vector3 startPos;
    private Renderer rend;
    private bool activated = false;
    private GameObject activator = null;

    private void Start()
    {
        rend = this.transform.gameObject.GetComponent<Renderer>();

        inactiveMaterial = rend.material;
        if(activeMaterial == null)
            activeMaterial = inactiveMaterial;

        startPos = objectToMove.transform.position;
    }

    private void Update()
    {
        if (!activated)
            MoveToStartpos();
    }

    private void OnTriggerStay(Collider other)
    {
        if ((affectPlayer1 && other.gameObject.CompareTag("Player1")) || (affectPlayer2 && other.gameObject.CompareTag("Player2")) || affectOtherColliders)
        {
            MoveToEndpoint();
            activator = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == activator)
        {
            activated = false;
            activator = null;
        }
    }

    private void MoveToEndpoint()
    {
        if (!activated)
        {
            rend.material = activeMaterial;
            activated = true;
        }
        objectToMove.transform.position = Vector3.Lerp(objectToMove.transform.position, endPoint.position, translationSpeed * Time.deltaTime);
    }

    private void MoveToStartpos()
    {
        rend.material = inactiveMaterial;
        objectToMove.transform.position = Vector3.Lerp(objectToMove.transform.position, startPos, translationSpeed * Time.deltaTime);
    }

    //IEnumerator Lerp()
    //{
    //    float timeElapsed = 0;
    //    while (timeElapsed < lerpDuration)
    //    {
    //        valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
    //        timeElapsed += Time.deltaTime;
    //        yield return null;
    //    }
    //    valueToLerp = endValue;
    //}
}
