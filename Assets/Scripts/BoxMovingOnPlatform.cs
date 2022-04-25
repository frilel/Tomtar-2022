using UnityEngine;

/// <summary>
/// Put this script on an object with a BoxCollider in order to have it follow along
/// moving platforms.
/// </summary>
public class BoxMovingOnPlatform : MonoBehaviour
{
    // TODO: Make class work with other types of colliders

    [SerializeField] private LayerMask groundLayers;

    private BoxCollider boxCollider;
    private GameObject currentPlatform = null;
    private Vector3 prevPlatformPos = Vector3.zero;
    private Vector3 platformVelocity = Vector3.zero;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        Vector3 checkBoxPosition = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
        
        // check if ground is platform, add platform velocity to movement if so
        if (Physics.CheckBox(checkBoxPosition, boxCollider.bounds.extents, this.transform.rotation, groundLayers, QueryTriggerInteraction.Ignore))
        {
            Vector3 startPos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            Physics.BoxCast(startPos, boxCollider.bounds.extents, Vector3.down, out RaycastHit hitInfo, this.transform.rotation, 0.2f, groundLayers, QueryTriggerInteraction.Ignore);

            if (hitInfo.collider != null && hitInfo.collider.gameObject.CompareTag("MagicMoveable"))
            {
                if (currentPlatform == null) // We have just stepped on to the platform
                {
                    currentPlatform = hitInfo.collider.gameObject;
                    prevPlatformPos = currentPlatform.transform.position;
                }
                else // earliest second frame on platform
                {
                    platformVelocity = currentPlatform.transform.position - prevPlatformPos;
                    this.transform.position += platformVelocity;
                    Physics.SyncTransforms();

                    prevPlatformPos = currentPlatform.transform.position;
                }
            }
        }
        else
        {
            // remove data if we're no longer grounded
            if (currentPlatform != null)
            {
                currentPlatform = null;
                prevPlatformPos = Vector3.zero;
            }
        }
    }
}
