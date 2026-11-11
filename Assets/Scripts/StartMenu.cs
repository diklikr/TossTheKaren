using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    public void RestartGame()
    {
        gameManager.ResetRound();
    }

public void QuitGame()
    {
        Application.Quit();
    }
}
