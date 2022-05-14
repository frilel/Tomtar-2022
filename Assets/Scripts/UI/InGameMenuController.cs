using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenuController : MonoBehaviour
{
    void Start()
    {
        //this.gameObject.SetActive(false);
    }

    public void OnExitToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
