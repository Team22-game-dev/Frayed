using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using Frayed.Input;

public class GameOverScreen : MonoBehaviour
{

    // Singleton Design
    private static GameOverScreen _instance;
    public static GameOverScreen Instance => _instance;

    public GameObject GameOverBackground; // Reference to the background
    public UnityEngine.UI.Text GameOverText; // Explicit namespace
    public Button RestartButton, MainMenuButton;
    public Vector3 gameOverTextStartPosition;

    private RectTransform gameOverTextRect;

    public bool gameOverTriggered { get { return GameOverBackground != null && GameOverBackground.activeSelf; } } // Prevent multiple calls

    private Menu menu;
    private InputManager inputManager;
    private MC_Inventory mcInventory;
    private void Awake()
    {
        // Singleton pattern with explicit null check
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        gameOverTextRect = GameOverText.GetComponent<RectTransform>();
        menu = Menu.Instance;
        inputManager = InputManager.Instance;
        gameOverTextStartPosition = gameOverTextRect.anchoredPosition;

        mcInventory = MC_Inventory.Instance;
        UnityEngine.Debug.Assert(mcInventory != null);

        // Hide all Game Over UI elements at start
        if (gameOverTriggered)
        {
            ++inputManager.disableInputCount;
        }
        HideGameOver();
    }

    void HideGameOver()
    {
        if (gameOverTriggered)
        {
            --inputManager.disableInputCount;
        }
        GameOverBackground.SetActive(false);
        GameOverText.gameObject.SetActive(false);
        RestartButton.gameObject.SetActive(false);
        MainMenuButton.gameObject.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverTriggered) return; // Prevent multiple activations
        gameOverTextRect.anchoredPosition = gameOverTextStartPosition;
        // Unlock mouse and lock movement.
        inputManager.UnlockMouse();
        inputManager.LockMovement();

        GameOverBackground.SetActive(true); // Show background
        ++inputManager.disableInputCount;
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        mcInventory.ClearInventory();
        GameObject.FindObjectOfType<MC_Data>().enemiesKilled = 0;
        if (CorruptionMeter.Instance != null)
        {
            CorruptionMeter.Instance.ResetCorruption();
        }

        GameOverText.gameObject.SetActive(true);

        Vector2 startPosition = gameOverTextStartPosition;
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
        HideGameOver();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // TODO: Use options menu to reload scene for now.
        //menu.LoadScene(SceneManager.GetActiveScene().name);
        menu.LoadScene("TownTest");
        // Lock mouse and unlock movement and unpause game.
        inputManager.LockMouse();
        inputManager.UnlockMovement();
        Time.timeScale = 1f;
        // TODO: Improve logic, respawn.
        //GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = false;
        //GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(504.94f, 0f, 106.3f);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = true;
    }

    public void MainMenu()
    {
        HideGameOver();
        menu.Toggle(true);
    }
}
