using System.Collections;
using System.Collections.Generic;
using Frayed.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{

    // Singleton Design
    private static OptionsMenu _instance;
    public static OptionsMenu Instance => _instance;

    public bool toggled { get { return _toggled; } private set { _toggled = value; } }

    [SerializeField]
    private bool _toggled = false;

    private InputManager inputManager;
    private GameObject canvas;
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
        canvas = this.transform.Find("Canvas").gameObject;
        Debug.Assert(canvas != null);

        Toggle(_toggled);
    }

    public void Toggle(bool state)
    {
        _toggled = state;
        if (_toggled)
        {
            canvas.SetActive(true);
            // Pause game.
            Time.timeScale = 0f;
            inputManager.UnlockMouse();
        }
        else
        {
            canvas.SetActive(false);
            // Unpause game.
            Time.timeScale = 1f;
            inputManager.LockMouse();
        }
    }

    public void Update()
    {
        if (inputManager.optionsMenu)
        {
            Toggle(!_toggled);
            // Finished with the button input, set back to false.
            inputManager.optionsMenu = false;
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
            GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(504.94f, 0f, 106.3f);
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = true;
        }
    }
}
