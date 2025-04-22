using System.Collections;
using System.Collections.Generic;
using Frayed.Input;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Composites;
using TMPro;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Menu : MonoBehaviour
{

    // Singleton Design
    private static Menu _instance;
    public static Menu Instance => _instance;

    public bool toggled { get { return _toggled; } private set { _toggled = value; } }

    [SerializeField]
    private bool _toggled = false;

    private InputManager inputManager;
    private GameObject canvas;
    private GameObject optionsPanel;
    private GameObject keybindsPanel;
    private GameObject keybindsScrollContent;
    private GameObject keybindTextTemplate;

    private GameOverScreen gameOverScreen;
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

    public void Start()
    {
        inputManager = InputManager.Instance;
        Debug.Assert(inputManager != null);

        canvas = this.transform.Find("Canvas").gameObject;
        Debug.Assert(canvas != null);

        optionsPanel = canvas.transform.Find("OptionsPanel").gameObject;
        Debug.Assert(optionsPanel != null);

        keybindsPanel = canvas.transform.Find("KeybindsPanel").gameObject;
        Debug.Assert(keybindsPanel != null);

        keybindsScrollContent = keybindsPanel.transform.Find("ScrollContent").gameObject;
        Debug.Assert(keybindsScrollContent != null);

        keybindTextTemplate = keybindsScrollContent.transform.Find("Template").gameObject;
        Debug.Assert(keybindTextTemplate != null);

        gameOverScreen = GameOverScreen.Instance;
        Debug.Assert(gameOverScreen != null);

        if (_toggled)
        {
            ++inputManager.disableInputCount;
        }
        Toggle(_toggled);
    }

    public void Toggle(bool state)
    {
        if (state)
        {
            canvas.SetActive(true);
            optionsPanel.SetActive(true);
            keybindsPanel.SetActive(false);
            // Pause game.
            Time.timeScale = 0f;
            inputManager.UnlockMouse();
            inputManager.LockMovement();
            if (state != _toggled)
            {
                ++inputManager.disableInputCount;
            }
        }
        else
        {
            canvas.SetActive(false);
            // Unpause game.
            Time.timeScale = 1f;
            inputManager.LockMouse();
            inputManager.UnlockMovement();
            if (state != _toggled)
            {
                --inputManager.disableInputCount;
            }
        }
        _toggled = state;
    }

    public void Update()
    {
        if (inputManager.toggleMenu)
        {
            if (!gameOverScreen.gameOverTriggered)
            {
                Toggle(!_toggled);
            }
            // Finished with the button input, set back to false.
            inputManager.toggleMenu = false;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        // Just for debug purposes for now. // TODO
        if (sceneName == "Playground")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = false;
            GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(0f, 0f, 0f);
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = true;
        }
        else if (sceneName == "VillageTest")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = false;
            GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(328.58f, 8.75f, 238.33f);
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = true;
        }
        else if (sceneName == "TownTest")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = false;
            GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(12.26f, 2.575f, -13.538f);
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = true;
        }
        Toggle(false);
    }

    public void ShowKeybinds()
    {
        optionsPanel.SetActive(false);
        keybindsPanel.SetActive(true);
        foreach (Transform child in keybindsScrollContent.transform)
        {
            if (child.gameObject != keybindTextTemplate)
            {
                Destroy(child.gameObject);
            }
        }
#if ENABLE_INPUT_SYSTEM
        foreach (InputAction inputAction in inputManager.playerInput.actions)
        {
            foreach(InputBinding binding in inputAction.bindings)
            {
                if (binding.groups.Contains(inputManager.playerInput.currentControlScheme))
                {
                    //string keybind = binding.path.Substring(binding.path.IndexOf("/") + 1).ToUpper();
                    string keybind = binding.path.ToUpper();
                    string text;
                    if (!string.IsNullOrWhiteSpace(binding.name))
                    {
                        text = $"{inputAction.name} {binding.name}: {keybind}";
                    }
                    else
                    {
                        text = $"{inputAction.name}: {keybind}";
                    }
                    GameObject textGameObject = GameObject.Instantiate(keybindTextTemplate);
                    textGameObject.SetActive(true);
                    textGameObject.transform.SetParent(keybindsScrollContent.transform);
                    textGameObject.GetComponent<TMP_Text>().text = text;
                }
            }
        }
#endif
    }

    public void HideKeybinds()
    {
        keybindsPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }
}
