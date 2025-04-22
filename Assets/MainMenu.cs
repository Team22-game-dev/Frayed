using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame ()
    {
        // Starting Scene
        SceneManager.LoadScene("TownTest");
    }

    public void QuitGame ()
    {
        UnityEngine.Debug.Log("QUIT!");
        Application.Quit();
    }
}
