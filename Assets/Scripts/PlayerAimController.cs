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

        public const float fireTimeout = 1f;
        private float fireTimeoutDelta = 0f;

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
            // Fire timeout calculations
            if (fireTimeoutDelta > 0f) {
                fireTimeoutDelta -= Time.deltaTime;
                if (fireTimeoutDelta <= 0f) {
                    // Fire lockout is over
                    aimTarget.SetActive(false);
                }
            }

            if (input.Aim) {
                if (!aimCamera.activeInHierarchy) {
                    mainCamera.SetActive(false);
                    aimCamera.SetActive(true);
                    moveController.SetRotateOnMove(false);
                }

                // Calculate aim location
                Vector3 target = AimTarget();
                if (target != Vector3.positiveInfinity){
                    // Look towards target
                    Vector3 lookPos = target;
                    lookPos.y = transform.position.y;
                    Vector3 lookDir = (lookPos - transform.position).normalized;
                    transform.forward = Vector3.Lerp(transform.forward, lookDir, 0.5f);

                    // Check for fire
                    if (input.Fire && fireTimeoutDelta <= 0f) {
                        fireTimeoutDelta = fireTimeout;

                        // Show target in game
                        aimTarget.SetActive(true);
                        aimTarget.transform.position = target;
                    }
                }
            } else {
                if (!mainCamera.activeInHierarchy){
                    mainCamera.SetActive(true);
                    aimCamera.SetActive(false);
                    moveController.SetRotateOnMove(true);
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
