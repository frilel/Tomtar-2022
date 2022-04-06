using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerAimController : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject defaultCamera;
    public GameObject aimCamera;
    public Canvas reticleCanvas;
    public LayerMask aimMask;
    [SerializeField] private float characterLookRotationSpeed = 5f;

    private StarterAssetsInputs input;
    private ThirdPersonController moveController;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        moveController = GetComponent<ThirdPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (input.Aim) {
            if (!aimCamera.activeInHierarchy) {
                defaultCamera.SetActive(false);
                aimCamera.SetActive(true);
                moveController.SetRotateOnMove(false);
                reticleCanvas.gameObject.SetActive(true);
            }

            // Calculate aim location
            if (Physics.Raycast(AimRay(), out RaycastHit raycastHit, 999f, aimMask)) {
                  Vector3 target = raycastHit.point;
                
                // Look towards target
                Vector3 lookPos = target;
                lookPos.y = transform.position.y;
                Vector3 lookDir = (lookPos - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, lookDir, characterLookRotationSpeed * Time.deltaTime);
            }
        } else {
            if (!defaultCamera.activeInHierarchy){
                defaultCamera.SetActive(true);
                aimCamera.SetActive(false);
                moveController.SetRotateOnMove(true);
                reticleCanvas.gameObject.SetActive(true);
            }
        }
    }

    public Ray AimRay() {
        return new Ray(mainCamera.transform.position, mainCamera.transform.forward);
    }
}
