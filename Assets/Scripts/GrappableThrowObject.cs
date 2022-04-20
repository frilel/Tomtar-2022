using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappableThrowObject : MonoBehaviour
{
    [SerializeField] private float translationSpeed = 0.01f;
    [SerializeField] LayerMask grappleObjectLayer;
    [SerializeField] LayerMask ignoreCollisionLayer;

    private Collider myCollider;
    private Vector3 targetPosition = Vector3.zero;
    private Quaternion targetRotation = Quaternion.identity;
    private Vector3 targetScale = Vector3.zero;
    private Transform targetParent = null;
    private bool travelingOut = false;
    private bool travelingBack = false;

    private void Start()
    {
        myCollider = GetComponent<Collider>();
        myCollider.enabled = false;
    }
    void Update()
    {
        if (!travelingOut && !travelingBack)
            return;

        if (travelingOut)
        {
            this.transform.SetPositionAndRotation(
                Vector3.Lerp(this.transform.position, targetPosition, translationSpeed * Time.deltaTime),
                Quaternion.Lerp(this.transform.rotation, targetRotation, translationSpeed * 2.0f * Time.deltaTime)
                );
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, targetScale, translationSpeed * 2.0f * Time.deltaTime);
            
            if ((this.transform.position - targetPosition).sqrMagnitude < 2.0f)
            {
                myCollider.enabled = true;
                this.gameObject.layer = LayerMask.NameToLayer("Grappable");
                travelingOut = false;
            }
        }
        if (travelingBack)
        {
            this.transform.SetPositionAndRotation(
                Vector3.Lerp(this.transform.position, targetParent.position, translationSpeed * Time.deltaTime),
                Quaternion.Lerp(this.transform.rotation, targetParent.rotation, translationSpeed * 2.0f * Time.deltaTime)
                );
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, targetScale, translationSpeed * 2.0f * Time.deltaTime);
            
            if ((this.transform.position - targetParent.position).sqrMagnitude < 2.0f)
            {
                this.transform.position = targetParent.position;
                this.transform.rotation = targetParent.rotation;
                this.transform.localScale = targetScale;
                this.transform.parent = targetParent;
                //this.gameObject.layer = LayerMask.NameToLayer("Grappable");
                travelingBack = false;
            }
        }
    }

    public void Throw(Vector3 targetPos, Quaternion targetRot, Vector3 targetSca)
    {
        travelingOut = true;
        travelingBack = false;
        //this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        this.transform.parent = null;
        this.targetPosition = targetPos;
        this.targetRotation = targetRot;
        this.targetScale = targetSca;
    }

    public void Return(Vector3 targetPos, Quaternion targetRot, Vector3 targetSca, Transform parent)
    {
        travelingOut = false;
        travelingBack = true;
        this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        this.targetParent = parent;
        this.targetPosition = targetPos;
        this.targetRotation = targetRot;
        this.targetScale = targetSca;
        myCollider.enabled = false;
    }
}
