using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    public Text timerText;
    public Text bestScoreText;

    private float timer;
    private float bestScore;

    private bool gameActive;

    void Start()
    {
        // Load the best score from PlayerPrefs if it exists
        bestScore = PlayerPrefs.GetFloat("BestScore", float.MaxValue);

        // Initialize timer
        timer = 0;
        gameActive = true;

        // Update best score text
        UpdateBestScoreText();
    }

    void Update()
    {
        if (gameActive)
        {
            timer += Time.deltaTime;
            timerText.text = "Time: " + timer.ToString("F2") + "s";
        }
    }

    public void EndGame()
    {
        gameActive = false;

        // Check if this is the best score
        if (timer < bestScore)
        {
            bestScore = timer;
            PlayerPrefs.SetFloat("BestScore", bestScore);
        }

        // Update the best score text
        UpdateBestScoreText();

        // Reload the scene after a delay
        Invoke("ReloadScene", 2f); // 2-second delay before restarting
    }

    private void UpdateBestScoreText()
    {
        if (bestScore == float.MaxValue)
        {
            bestScoreText.text = "Best Time: --";
        }
        else
        {
            bestScoreText.text = "Best Time: " + bestScore.ToString("F2") + "s";
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
