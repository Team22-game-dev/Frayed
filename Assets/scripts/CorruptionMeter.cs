using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class CorruptionMeter : MonoBehaviour
{
    public static CorruptionMeter Instance;

    public Text meterNumber;
    public Image corruptionMeter;
    public static float corruption = 0;

    float maxCorruption = 100;
    float lerpSpeed;
    [SerializeField] private float corruptionDrainRate = 1f;

    public float corruptionDecayRate = 5f;

    /*private void Start()
    {
        corruption = 0;
    }*/

    private void Awake()
    {
        Instance = this;
    }


    private void Update()
    {
        meterNumber.text = Mathf.RoundToInt(corruption) + "%";

        if (corruption >= maxCorruption)
        {
            corruption = maxCorruption;
            if (!GameOverScreen.Instance.gameOverTriggered)
            {
                GameOverScreen.Instance.ShowGameOver();
            }
        }

        else if (corruption > 0)
        {
            corruption -= Time.deltaTime * corruptionDrainRate;
            corruption = Mathf.Max(0, corruption);
        }


        lerpSpeed = 3f * Time.deltaTime;

        MeterFiller();
        ColorChanger();
    }

    void MeterFiller()
    {
        corruptionMeter.fillAmount = Mathf.Lerp(corruptionMeter.fillAmount, corruption / maxCorruption, lerpSpeed);
    }

    void ColorChanger()
    {
        Color corruptionColor = Color.Lerp(Color.green, Color.red, (corruption / maxCorruption));
        corruptionMeter.color = corruptionColor;
        meterNumber.color = corruptionColor;
    }

    public void Decrease(float decrease)
    {
        if (corruption > 0)
        {
            corruption -= decrease;
        }
    }

    public void Increase(float increase)
    {
        if (corruption < maxCorruption)
        {
            corruption += increase;
        }
    }

    public void ResetCorruption()
    {
        corruption = 0;
    }

    void GameOver()
    {
        UnityEngine.Debug.Log("GAME OVER");
    }
}
