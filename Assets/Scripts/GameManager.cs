using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerInteractionController Player1IC { get; private set; }
    public PlayerInteractionController Player2IC { get; private set; }

    private Checkpoint[] sceneCheckpoints;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void Start()
    {
        sceneCheckpoints = FindObjectsOfType<Checkpoint>();
    }

    public Checkpoint[] GetSceneCheckpoints() => sceneCheckpoints;

    public void SetPlayer1(PlayerInteractionController pic) => Player1IC = pic;
    public void SetPlayer2(PlayerInteractionController pic) => Player2IC = pic;

}
