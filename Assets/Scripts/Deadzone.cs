using UnityEngine;

public class Deadzone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player1"))
        {
            GameManager.Instance.Player1IC.Respawn();
        }
        else if (other.gameObject.CompareTag("Player2"))
        {
            GameManager.Instance.Player2IC.Respawn();
        }
    }

}
