using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets {
    public class PlayerAimController : MonoBehaviour
    {
        public GameObject mainCamera;
        public GameObject aimCamera;
        public LayerMask aimMask;
        public GameObject aimTarget;

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
                Vector3 target = AimTarget();
                if (target != Vector3.positiveInfinity){
                    // Show target in game
                    aimTarget.SetActive(true);
                    aimTarget.transform.position = target;
                    
                    // Look towards target
                    Vector3 lookPos = target;
                    lookPos.y = transform.position.y;
                    Vector3 lookDir = (lookPos - transform.position).normalized;
                    transform.forward = Vector3.Lerp(transform.forward, lookDir, 0.5f);
                } else {
                    aimTarget.SetActive(false);
                }


            } else if (!input.Aim) {
                if (!mainCamera.activeInHierarchy){
                    mainCamera.SetActive(true);
                    aimCamera.SetActive(false);
                    moveController.SetRotateOnMove(true);
                    aimTarget.SetActive(false);
                }

            }
        }

        public Vector3 AimTarget() {
            Vector3 target = Vector3.zero;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit raycastHit, 999f, aimMask)) {
                return raycastHit.point;
            }

            return Vector3.positiveInfinity;
        }
    }
}
