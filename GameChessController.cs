using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameChessController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject gameOverPanel;

    [Header("UI Elements")]
    public TMP_Text winnerTMP;
    public Button startButton;
    public Button restartButton;

    private void Start()
    {
        if (startPanel != null)
            startPanel.SetActive(true); 

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); 

        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    private void StartGame()
    {
        if (startPanel != null)
            startPanel.SetActive(false);
    }

    public void ShowGameOver(string result)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (winnerTMP != null)
            {
                winnerTMP.text = (result == "DRAW") ? "DRAW" : result + " wins!";
            }
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("Chess");
    }

    public void GoMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}