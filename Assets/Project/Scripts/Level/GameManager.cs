using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject gameWinPanel;
    [SerializeField] PlayerController playerController;
    [SerializeField] TextMeshProUGUI scoreText;
    private static int score;

    private void Start()
    {
        score = 0;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gameWinPanel.SetActive(false);
        scoreText.text = "Score: 0";
    }

    public static void UpdateScore(int amount)
    {
        score += amount;
    }

    private void SetScore()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    private void GamePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    private void GameWin()
    {
        gameWinPanel.SetActive(true);
    }

    private void Update()
    {
        GamePause();
        if (playerController.currentHealth <= 0)
        {
            GameOver();
        }
        SetScore();
        if (score >= 350)
        {
            GameWin();
        }
    }
}
