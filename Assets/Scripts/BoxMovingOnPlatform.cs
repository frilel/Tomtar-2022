using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Put this script on an object with a BoxCollider in order to have it follow along
/// moving platforms.
/// </summary>
public class BoxMovingOnPlatform : MonoBehaviour
{
    // TODO: Make class, or subclasses, work with other types of colliders

    [SerializeField] private LayerMask groundLayers;

    private BoxCollider boxCollider;
    private GameObject currentPlatform = null;
    private Vector3 prevPlatformPos = Vector3.zero;
    private Vector3 platformVelocity = Vector3.zero;
    private readonly RaycastHit[] hitInfos = new RaycastHit[12];

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void LateUpdate()
    {
        Vector3 checkBoxPosition = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z); // check underneath
        
        // check if grounded
        if (Physics.CheckBox(checkBoxPosition, boxCollider.bounds.extents, transform.rotation, groundLayers, QueryTriggerInteraction.Ignore) ||
            Physics.CheckBox(transform.position, boxCollider.bounds.extents, transform.rotation, groundLayers, QueryTriggerInteraction.Ignore))
        {
            Vector3 startPos = new Vector3(transform.position.x, transform.position.y + boxCollider.bounds.extents.magnitude, transform.position.z); // start a bit above
            Physics.BoxCastNonAlloc(startPos, boxCollider.bounds.extents, Vector3.down, hitInfos, transform.rotation, boxCollider.bounds.extents.magnitude * 2.0f, groundLayers, QueryTriggerInteraction.Ignore);
            
            // check if ground is platform, add platform velocity to movement if so
            // NOTE (christian): atm the code assumes only one platform is going to affect the box
            for (int i = 0; i < hitInfos.Length; i++)
            {
                if (hitInfos[i].collider == null || !hitInfos[i].collider.gameObject.CompareTag("MagicMoveable"))
                    continue; // next

                if (currentPlatform == null) // We have just stepped on to the platform
                {
                    currentPlatform = hitInfos[i].collider.gameObject;
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
