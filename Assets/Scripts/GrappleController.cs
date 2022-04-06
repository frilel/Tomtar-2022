using UnityEngine;
using StarterAssets;

public class GrappleController : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform grappleOrigin;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private LayerMask grappableLayer;
    [SerializeField] private float maxDistance;

    [SerializeField] private ThirdPersonController player;
    [SerializeField] private Transform grappleTransform;

    void Update()
    {
        if (GameManager.Instance.Input.Fire)
        {
            StartGrapple();
        }
        else if (!GameManager.Instance.Input.Fire)
        {
            StopGrapple();
        }
    }

    private void LateUpdate()
    {
        DrawGrappleRope();
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, maxDistance, grappableLayer))
        {
            player.IsGrappling = true;
            player.GrapplePoint = hit.point;

            grappleTransform.parent = null;
            grappleTransform.position = hit.point;

            lineRenderer.positionCount = 2;
        }
    }

    void StopGrapple()
    {
        player.IsGrappling = false;
        player.GrapplePoint = Vector3.zero;

        grappleTransform.parent = grappleOrigin;
        grappleTransform.position = grappleOrigin.position;

        lineRenderer.positionCount = 0;
    }

    void DrawGrappleRope()
    {
        if (!player.IsGrappling)
            return;

        lineRenderer.SetPosition(0, grappleOrigin.position);
        lineRenderer.SetPosition(1, player.GrapplePoint);
    }
}
