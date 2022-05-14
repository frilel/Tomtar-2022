using UnityEngine;
using UnityEngine.Events;
using StarterAssets;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerInteractionController Player1IC { get; private set; }
    public PlayerInteractionController Player2IC { get; private set; }
    public ThirdPersonController Player1TPC { get; private set; }
    public ThirdPersonController Player2TPC { get; private set; }
    public ObjectRespawner CurrentObjectRespawner { get; private set; }
    public bool GameIsPaused { get; private set; } = false;

    [SerializeField] GameObject inGameMenu;
    private Checkpoint[] sceneCheckpoints;
    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        sceneCheckpoints = FindObjectsOfType<Checkpoint>();
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerInputManager.onPlayerJoined += OnPlayerJoined;

        UnpauseGame();
    }

    private void OnDestroy()
    {
        playerInputManager.onPlayerJoined -= OnPlayerJoined;
    }

    public void TogglePause()
    {
        GameIsPaused = !GameIsPaused;

        if (GameIsPaused)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            inGameMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            inGameMenu.SetActive(false);
        }
    }

    private void UnpauseGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inGameMenu.SetActive(false);
    }

    public Checkpoint[] GetSceneCheckpoints() => sceneCheckpoints;

    public void SetPlayer1IC(PlayerInteractionController pic) => Player1IC = pic;
    public void SetPlayer2IC(PlayerInteractionController pic) => Player2IC = pic;
    public void SetPlayer1TPC(ThirdPersonController tpc) => Player1TPC = tpc;
    public void SetPlayer2TPC(ThirdPersonController tpc) => Player2TPC = tpc;
    public void SetCurrentObjectRespawner(ObjectRespawner or) => CurrentObjectRespawner = or;
    private void OnPlayerJoined(PlayerInput obj)
    {
        if (obj.gameObject.CompareTag("Player2"))
        {
            obj.transform.position = GameManager.Instance.Player1TPC.transform.position + Vector3.up * 1f;
            Physics.SyncTransforms();
        }
    }
}
