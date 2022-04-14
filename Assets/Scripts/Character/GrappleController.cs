using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

public class GrappleController : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform grappleOrigin;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private LayerMask grappableLayer;
    [SerializeField] private float maxDistance;

    [SerializeField] private ThirdPersonController player;
    [SerializeField] private Transform grappleTransform;

    private StarterAssetsInputs input;
    private RaycastHit raycastHit;

    private void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        input.FireEvent.AddListener(Grapple);

        // Linerenderer screwing with stuff in edit mode, must be kept inactive and set active here
        grappleTransform.gameObject.SetActive(true);
    }
    private void LateUpdate()
    {
        DrawGrappleRope();
    }

    private void OnDestroy()
    {
        input.FireEvent.RemoveListener(Grapple);
    }

    private void Grapple(InputAction.CallbackContext context)
    {
        if (context.action.triggered && context.action.phase == InputActionPhase.Performed)
        {
            StartGrapple();
        }
        else if (!context.action.triggered && context.action.phase == InputActionPhase.Canceled)
        {
            StopGrapple();
        }
    }

    private void StartGrapple()
    {
        if(Physics.Raycast(mainCamera.position, mainCamera.forward, out raycastHit, maxDistance, grappableLayer))
        {
            player.IsGrappling = true;
            player.GrapplePoint = raycastHit.point;

            grappleTransform.parent = null;
            grappleTransform.position = raycastHit.point;

            lineRenderer.positionCount = 2;
        }
    }

    private void StopGrapple()
    {
        player.IsGrappling = false;
        player.GrapplePoint = Vector3.zero;

        grappleTransform.parent = grappleOrigin;
        grappleTransform.position = grappleOrigin.position;

        lineRenderer.positionCount = 0;
    }

    private void DrawGrappleRope()
    {
        if (!player.IsGrappling)
            return;

        lineRenderer.SetPosition(0, grappleOrigin.position);
        lineRenderer.SetPosition(1, player.GrapplePoint);
    }
}
