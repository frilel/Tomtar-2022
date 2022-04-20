using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class MagicWandController : MonoBehaviour
{
    public LayerMask layerMask;
    public float minDistance = 5f;

    private PlayerAimController aimController;
    private StarterAssetsInputs input;
    private CharacterController characterController;
    private GameObject target;
    private float targetLatchDistance;

    // Start is called before the first frame update
    void Start()
    {
        aimController = GetComponent<PlayerAimController>();
        characterController = GetComponent<CharacterController>();
        input = GetComponent<StarterAssetsInputs>();
        input.FireEvent.AddListener(FireEventListener);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate ()
    {
        if (target != null) {
            Ray aimRay = aimController.AimRay();
            ConstrainedPathObject constrainedPath = target.GetComponentInParent<ConstrainedPathObject>();
            if (constrainedPath != null){
                // TODO: cache old stopped value to it can be restored in UnLatch
                constrainedPath.stopped = true;
                constrainedPath.MoveByAim(aimRay);
            } else {
                Vector3 newPosition = aimRay.GetPoint(targetLatchDistance);

                Vector3 delta = newPosition - target.transform.position;
                target.GetComponent<CharacterController>().Move(delta);

                // targetLatchDistance = (target.transform.position - aimRay.origin).magnitude;
            }            
        }
    }

    void FireEventListener(InputAction.CallbackContext value)
    {
        if (!value.action.IsPressed()){
            // Just released, unlock target
            UnLatch();
        } else {
            if (Physics.Raycast(aimController.AimRay(), out RaycastHit raycastHit, 999f, layerMask)) {

                // Just pressed, lock target
                if (raycastHit.collider.gameObject.CompareTag("MagicMoveable")){
                    target = raycastHit.collider.gameObject;

                    Outline outline = target.GetComponent<Outline>();
                    if (outline != null){
                        outline.OutlineWidth = 5;
                        outline.OutlineMode = Outline.Mode.OutlineAll;
                    }


                    targetLatchDistance = raycastHit.distance;
                    Physics.IgnoreCollision(raycastHit.collider, GetComponent<Collider>(), true);
                    if (target.GetComponent<Rigidbody>() != null){
                        target.GetComponent<Rigidbody>().isKinematic = true;
                    }
                }
            }
        }
    }

    private void UnLatch(){
        if (target != null){
            Physics.IgnoreCollision(target.GetComponent<Collider>(), GetComponent<Collider>(), false);
            if (target.GetComponent<Rigidbody>() != null){
                target.GetComponent<Rigidbody>().isKinematic = false;
            }
            
            Outline outline = target.GetComponent<Outline>();
            if (outline != null){
                outline.OutlineWidth = 3;
                outline.OutlineMode = Outline.Mode.OutlineVisible;
            }

            target = null;
        }
    }
}
