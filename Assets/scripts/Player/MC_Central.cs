using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MC_Central : MonoBehaviour
{
    private static MC_Central _instance;
    public static MC_Central Instance => _instance;


    public GameObject MC_Prefab; // Assign the main character prefab in the Inspector

    public AnimationManager mcAnimationManagerScript;

    private GameObject mainCharacter;

    [SerializeField] private Transform characterRoot;



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
        // Initialize the main character
        InitializeMainCharacter();

        // Load the first game scene
        //SceneManager.LoadScene("Test_Scene");
    }

    void Update()
    {
        // Update logic (if needed)
    }

    private void InitializeMainCharacter()
    {
        if (MC_Prefab == null)
        {
            Debug.LogError("MC_Prefab is not assigned!");
            return;
        }

        // Instantiate the main character from the prefab
        mainCharacter = Instantiate(MC_Prefab, transform); // Make it a child of MC_Central
        DontDestroyOnLoad(mainCharacter); // Ensure it persists across scenes

    }



}
