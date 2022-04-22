using UnityEngine;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ConjureGrappableController : MonoBehaviour
{
    [SerializeField] private List<Transform> throwOrigins;
    [SerializeField] private List<GrappableThrowObject> objectsInInventory;
    [SerializeField] private float maxDistance;
    [SerializeField] private float scaleAtOrigin = 0.1f;
    [SerializeField] private float scaleAtSurface = 1.5f;

    [SerializeField] private Transform mainCamera;
    [SerializeField] private LayerMask conjurableSurface;

    private StarterAssetsInputs input;
    private int counter = 0;
    //private float timerObject1 = 0f;
    //private float timerObject2 = 0f;
    private List<GrappableThrowObject> objectsThrown = new List<GrappableThrowObject>();

    /* TODO:
     * they get returned after x amount of time (handle in thrown object?)
     */

    private void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        input.FireEvent.AddListener(Conjure);
    }

    private void OnDestroy()
    {
        input.FireEvent.RemoveListener(Conjure);
    }

    private void Conjure(InputAction.CallbackContext context)
    {
        if (context.action.triggered && context.action.phase == InputActionPhase.Performed && objectsInInventory.Count > 0) // We can throw
        {
            ThrowGrappable();
        } 
        else if (context.action.triggered && context.action.phase == InputActionPhase.Performed && objectsInInventory.Count == 0) // We need to return thrown
        {
            ReturnFirstThrown();
        }
    }

    private void ReturnFirstThrown()
    {
        GrappableThrowObject thrownToReturn = objectsThrown[0];
        objectsThrown.RemoveAt(0);
		objectsInInventory.Add(thrownToReturn);

        //thrownToReturn.transform.localScale = new Vector3(scaleAtOrigin, scaleAtOrigin, scaleAtOrigin);
        //thrownToReturn.transform.position = throwOrigins[counter].transform.position;
        //thrownToReturn.transform.rotation = throwOrigins[counter].transform.rotation;
        //thrownToReturn.transform.parent = throwOrigins[counter];
        thrownToReturn.Return(throwOrigins[counter].transform.position, throwOrigins[counter].transform.rotation, new Vector3(scaleAtOrigin, scaleAtOrigin, scaleAtOrigin), throwOrigins[counter]);

        if (counter == 0)
            counter = 1;
        else
            counter = 0;
    }

    private void ThrowGrappable()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, maxDistance, conjurableSurface))
        {
            GrappableThrowObject thrown = objectsInInventory[0];
            objectsInInventory.RemoveAt(0);
            objectsThrown.Add(thrown);


            Vector3 translatePos = hit.point + (hit.normal * scaleAtSurface);
            //thrown.transform.parent = null;
            thrown.Throw(translatePos, Quaternion.identity, new Vector3(scaleAtSurface, scaleAtSurface, scaleAtSurface));
            //thrown.transform.position = translatePos;
            //thrown.transform.localScale = new Vector3(scaleAtSurface, scaleAtSurface, scaleAtSurface);
        }
    }

}
