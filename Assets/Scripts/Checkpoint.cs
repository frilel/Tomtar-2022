using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("Whether this checkpoint will be set for each player individually or not.")]
    [SerializeField] private bool affectBothPlayers = false;
    [Tooltip("Assign the material for displaying inactive checkpoint")]
    [SerializeField] private Material inactiveMaterial;
    [Tooltip("Assign the material for displaying active checkpoint")]
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Transform respawnPoint;

    private Renderer rend;

    private void Start()
    {
        rend = this.transform.gameObject.GetComponent<Renderer>();
        rend.material = inactiveMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player1") && !other.gameObject.CompareTag("Player2"))
            return;

        rend.material = activeMaterial;

        if (affectBothPlayers)
        {
            GameManager.Instance.Player1IC?.SetCurrentCheckpoint(this);
            GameManager.Instance.Player2IC?.SetCurrentCheckpoint(this);
        }
        else if (other.gameObject.CompareTag("Player1"))
            GameManager.Instance.Player1IC.SetCurrentCheckpoint(this);
        else if (other.gameObject.CompareTag("Player2"))
            GameManager.Instance.Player2IC.SetCurrentCheckpoint(this);
    }

    public Transform GetRespawnPoint() => respawnPoint;

    public void SetInactive()
    {
        rend.material = inactiveMaterial;
    }
}
