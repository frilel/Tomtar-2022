using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class MagicWandController : MonoBehaviour
{
    public LayerMask layerMask;

    private PlayerAimController aimController;
    private StarterAssetsInputs input;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        aimController = GetComponent<PlayerAimController>();
        input = GetComponent<StarterAssetsInputs>();
        input.FireEvent.AddListener(FireEventListener);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FireEventListener(InputValue value)
    {
        if (!value.isPressed){
            // Just released, unlock target
            target = null;
        } else if (Physics.Raycast(aimController.AimRay(), out RaycastHit raycastHit, 999f, layerMask)) {
            // Just pressed, lock target
            if (raycastHit.collider.gameObject.CompareTag("MagicMoveable")){
                target = raycastHit.collider.gameObject;
            }
        }
    }
}
