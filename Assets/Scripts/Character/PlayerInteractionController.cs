using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    private Checkpoint currentCheckpoint;

    private void Start()
    {
        if (this.gameObject.CompareTag("Player1"))
        {
            GameManager.Instance.SetPlayer1IC(this);
        }
        else if (this.gameObject.CompareTag("Player2"))
        {
            GameManager.Instance.SetPlayer2IC(this);
            currentCheckpoint = GameManager.Instance.Player1IC.GetCurrentCheckpoint();
        }
    }

    public void Respawn()
    {
        this.transform.position = currentCheckpoint.GetRespawnPoint().position;
        Physics.SyncTransforms();
    }

    public void SetCurrentCheckpoint(Checkpoint checkpoint)
    {
        currentCheckpoint = checkpoint;
        Checkpoint[] sceneCheckpoints = GameManager.Instance.GetSceneCheckpoints();

        for (int i = 0; i < sceneCheckpoints.Length; i++)
            if (sceneCheckpoints[i] != currentCheckpoint)
                sceneCheckpoints[i].SetInactive();
    }

    public Checkpoint GetCurrentCheckpoint()
    {
        return currentCheckpoint;
    }
}
