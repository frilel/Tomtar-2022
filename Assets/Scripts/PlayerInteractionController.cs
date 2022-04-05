using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    private Checkpoint currentCheckpoint;

    private void Start()
    {
        if (this.gameObject.CompareTag("Player1"))
        {
            GameManager.Instance.SetPlayer1(this);
        }
        else if (this.gameObject.CompareTag("Player2"))
        {
            GameManager.Instance.SetPlayer2(this);
        }
    }

    public void Respawn()
    {
        this.transform.position = currentCheckpoint.GetRespawnPoint().position;
        Physics.SyncTransforms();
    }

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        currentCheckpoint = checkpoint;
        Checkpoint[] sceneCheckpoints = GameManager.Instance.GetSceneCheckpoints();

        for (int i = 0; i < sceneCheckpoints.Length; i++)
            if (sceneCheckpoints[i] != currentCheckpoint)
                sceneCheckpoints[i].SetInactive();
    }
}
