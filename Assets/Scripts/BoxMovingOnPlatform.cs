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
    private RaycastHit[] hitInfosBuffer = new RaycastHit[12];
    private RaycastHit emptyHit = new RaycastHit();

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void LateUpdate()
    {
        Vector3 checkBoxPosition = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z); // check underneath
        bool isOnPlatform = false;

        // check if grounded
        if (Physics.CheckBox(checkBoxPosition, boxCollider.bounds.extents, transform.rotation, groundLayers, QueryTriggerInteraction.Ignore) ||
            Physics.CheckBox(transform.position, boxCollider.bounds.extents, transform.rotation, groundLayers, QueryTriggerInteraction.Ignore))
        {
            // reset hitInfosBuffer
            for (int i = 0; i < hitInfosBuffer.Length; i++)
                hitInfosBuffer[i] = emptyHit;

            Vector3 startPos = new Vector3(transform.position.x, transform.position.y + boxCollider.bounds.extents.magnitude, transform.position.z); // start a bit above
            Physics.BoxCastNonAlloc(startPos, boxCollider.bounds.extents, Vector3.down, hitInfosBuffer, transform.rotation, boxCollider.bounds.extents.magnitude * 2.0f, groundLayers, QueryTriggerInteraction.Ignore);
            
            // check if ground is platform, add platform velocity to movement if so
            // NOTE (christian): atm the code assumes only one platform is going to affect the box
            for (int i = 0; i < hitInfosBuffer.Length; i++)
            {
                if (hitInfosBuffer[i].collider == null || 
                    hitInfosBuffer[i].distance < float.Epsilon || 
                    hitInfosBuffer[i].point == Vector3.zero || 
                    !hitInfosBuffer[i].collider.gameObject.CompareTag("MagicMoveable"))
                    continue;

                isOnPlatform = true;

                if (currentPlatform == null) // first frame on platform
                {
                    currentPlatform = hitInfosBuffer[i].collider.gameObject;
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
            // iterated thru all but is not on platform
            if (!isOnPlatform && currentPlatform != null)
            {
                currentPlatform = null;
                prevPlatformPos = Vector3.zero;
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
