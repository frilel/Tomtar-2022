using UnityEngine;

public class ObjectRespawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint = null;

    public void Respawn(GameObject respawnable)
    {
        respawnable.transform.position = spawnPoint.position;
        respawnable.transform.rotation = spawnPoint.rotation;
        Physics.SyncTransforms();

        respawnable.GetComponent<Rigidbody>().AddRelativeForce(0.0f, 10.0f, 10.0f, ForceMode.Impulse);
    }
}
