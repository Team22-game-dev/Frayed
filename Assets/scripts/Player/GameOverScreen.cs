using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class GameOverScreen : MonoBehaviour
{
    public GameObject GameOverBackground; // Reference to the background
    public UnityEngine.UI.Text GameOverText; // Explicit namespace
    public Button RestartButton, MainMenuButton;

    private RectTransform gameOverTextRect;
    private bool gameOverTriggered = false; // Prevent multiple calls

    void Start()
    {
        gameOverTextRect = GameOverText.GetComponent<RectTransform>();

        // Hide all Game Over UI elements at start
        GameOverBackground.SetActive(false);
        GameOverText.gameObject.SetActive(false);
        RestartButton.gameObject.SetActive(false);
        MainMenuButton.gameObject.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverTriggered) return; // Prevent multiple activations
        gameOverTriggered = true;

        GameOverBackground.SetActive(true); // Show background
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        GameOverText.gameObject.SetActive(true);

        Vector2 startPosition = gameOverTextRect.anchoredPosition;
        Vector2 endPosition = new Vector2(startPosition.x, startPosition.y + 55);

        yield return new WaitForSeconds(2f);

        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            gameOverTextRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is exactly set and prevent further movement
        gameOverTextRect.anchoredPosition = endPosition;

        RestartButton.gameObject.SetActive(true);
        MainMenuButton.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        // SceneManager.LoadScene("MainMenu");
    }
}
