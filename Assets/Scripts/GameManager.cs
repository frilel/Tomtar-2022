using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField] private GameObject inGameMenu;
    [SerializeField] private GameObject player2JoinUI;
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

        if (player2JoinUI != null && Player2TPC == null)
            player2JoinUI.SetActive(true);

        UnpauseGame();
    }

    private void OnDestroy()
    {
        playerInputManager.onPlayerJoined -= OnPlayerJoined;
    }

    public void TogglePause()
    {
        GameIsPaused = !GameIsPaused;

        Time.timeScale = GameIsPaused ? 0f : 1f;
        AudioListener.pause = GameIsPaused;
        Cursor.lockState = GameIsPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = GameIsPaused;

        if (Player1TPC != null) Player1TPC.LockCameraPosition = GameIsPaused;
        if (Player2TPC != null) Player2TPC.LockCameraPosition = GameIsPaused;

        if(inGameMenu != null) inGameMenu.SetActive(GameIsPaused);
    }

    private void UnpauseGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (inGameMenu != null) inGameMenu.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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

            if (player2JoinUI != null)
                player2JoinUI.SetActive(false);
        }
    }
}
