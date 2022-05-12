using UnityEngine;

/// <summary>
/// Script for making obstacles, doors, platforms etc. move when stepping on this collider
/// </summary>
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
    [SerializeField] private bool linearSpeed = true;

    private Vector3 startPos;
    private Renderer rend;
    private bool activated = false;
    private GameObject entityInteracting = null;

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
            if (activated)
                MoveToEndpoint();
            else
            {
                entityInteracting = other.gameObject;
                activated = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == entityInteracting)
        {
            entityInteracting = null;
            activated = false;
        }
    }

    private void MoveToEndpoint()
    {
        if (HasReachedTarget(endPoint.transform.position))
            return;

        if (rend.material != activeMaterial)
            rend.material = activeMaterial;

        if (linearSpeed)
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, endPoint.position, translationSpeed * Time.deltaTime);
        else
            objectToMove.transform.position = Vector3.Lerp(objectToMove.transform.position, endPoint.position, translationSpeed * Time.deltaTime);
    }

    private void MoveToStartpos()
    {
        if (HasReachedTarget(startPos))
            return;

        if (rend.material != inactiveMaterial)
            rend.material = inactiveMaterial;

        if (linearSpeed)
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, startPos, translationSpeed * Time.deltaTime);
        else
            objectToMove.transform.position = Vector3.Lerp(objectToMove.transform.position, startPos, translationSpeed * Time.deltaTime);
    }

    private bool HasReachedTarget(Vector3 target) => (target - objectToMove.transform.position).sqrMagnitude < 0.001f;
}
