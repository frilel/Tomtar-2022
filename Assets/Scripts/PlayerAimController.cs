using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets {
    public class PlayerAimController : MonoBehaviour
    {
        public GameObject mainCamera;
        public GameObject aimCamera;
        public LayerMask aimMask;

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
                    mainCamera.SetActive(false);
                    aimCamera.SetActive(true);
                    moveController.SetRotateOnMove(false);
                }

                // Calculate aim location
                if (Physics.Raycast(AimRay(), out RaycastHit raycastHit, 999f, aimMask)) {
                    Vector3 target = raycastHit.point;
                    
                    // Look towards target
                    Vector3 lookPos = target;
                    lookPos.y = transform.position.y;
                    Vector3 lookDir = (lookPos - transform.position).normalized;
                    transform.forward = Vector3.Lerp(transform.forward, lookDir, 0.5f);
                }
            } else {
                if (!mainCamera.activeInHierarchy){
                    mainCamera.SetActive(true);
                    aimCamera.SetActive(false);
                    moveController.SetRotateOnMove(true);
                }
            }
        }

        public Ray AimRay() {
            // TODO: ray origin should probably be on the plane of the character instead of the camera
            return new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        }
    }
}
