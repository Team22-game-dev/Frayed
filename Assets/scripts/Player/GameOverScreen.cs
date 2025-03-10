using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using Frayed.Input;

public class GameOverScreen : MonoBehaviour
{
    public GameObject GameOverBackground; // Reference to the background
    public UnityEngine.UI.Text GameOverText; // Explicit namespace
    public Button RestartButton, MainMenuButton;

    private RectTransform gameOverTextRect;
    private bool gameOverTriggered = false; // Prevent multiple calls
    private OptionsMenu optionsMenu;
    private InputManager inputManager;

    void Start()
    {
        gameOverTextRect = GameOverText.GetComponent<RectTransform>();
        optionsMenu = OptionsMenu.Instance;
        inputManager = InputManager.Instance;

        // Hide all Game Over UI elements at start
        HideGameOver();
    }

    void HideGameOver()
    {
        GameOverBackground.SetActive(false);
        GameOverText.gameObject.SetActive(false);
        RestartButton.gameObject.SetActive(false);
        MainMenuButton.gameObject.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverTriggered) return; // Prevent multiple activations
        gameOverTriggered = true;

        // Unlock mouse and lock movement.
        inputManager.UnlockMouse();
        inputManager.LockMovement();

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

        // Pause game.
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        UnityEngine.Debug.Log("HERE");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Lock mouse and unlock movement and unpause game.
        inputManager.LockMouse();
        inputManager.UnlockMovement();
        Time.timeScale = 1f;
        // TODO: Improve logic, respawn.
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = false;
        GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(504.94f, 0f, 106.3f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = true;
    }

    public void MainMenu()
    {
        UnityEngine.Debug.Log("HERE");
        HideGameOver();
        optionsMenu.Toggle(true);
    }
}
