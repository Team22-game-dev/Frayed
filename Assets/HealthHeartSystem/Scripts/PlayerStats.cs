/*
 *  Author: ariel oliveira [o.arielg@gmail.com]
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public delegate void OnHealthChangedDelegate();
    public OnHealthChangedDelegate onHealthChangedCallback;

    #region Sigleton
    private static PlayerStats instance;
    public static PlayerStats Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PlayerStats>();
            return instance;
        }
    }
    #endregion

    [SerializeField]
    private float health;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private float maxTotalHealth;
    [SerializeField]
    public float _skill = 1;

    private GameOverScreen gameOverScreen;

    public float Health { get { return health; } }
    public float MaxHealth { get { return maxHealth; } }
    public float MaxTotalHealth { get { return maxTotalHealth; } }

    public void Start()
    {
        gameOverScreen = GameOverScreen.Instance;
    }

    public void Heal(float health)
    {
        this.health += health;
        ClampHealth();
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        ClampHealth();

        if (health <= 0 && !gameOverScreen.gameOverTriggered)
        {
            StartCoroutine(GameOver());
        }
    }

    public void AddHealth()
    {
        if (maxHealth < maxTotalHealth)
        {
            maxHealth += 1;
            health = maxHealth;

            if (onHealthChangedCallback != null)
                onHealthChangedCallback.Invoke();
        }
    }

    void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth);

        if (onHealthChangedCallback != null)
            onHealthChangedCallback.Invoke();
    }

    void IncreaseSkill()
    {
        _skill += 0.1f;
    }

    private IEnumerator GameOver()
    {
        GameObject _mainCharacter = GameObject.FindGameObjectWithTag("Player");

        OnDeathController onDeathcontroller = _mainCharacter.GetComponent<OnDeathController>();

        yield return StartCoroutine(onDeathcontroller.ActivateRagdoll(Vector3.back, 1f));

        //yield return new WaitForSeconds(3f);

        if (gameOverScreen != null)
        {
            gameOverScreen.ShowGameOver();
        }
        else
        {
            UnityEngine.Debug.LogError("GameOverScreen not found!");
        }
    }
}