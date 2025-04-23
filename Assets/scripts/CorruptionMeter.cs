using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class CorruptionMeter : MonoBehaviour
{
    public Text meterNumber;
    public Image corruptionMeter;

    float corruption, maxCorruption = 100;
    float lerpSpeed;

    private void Start()
    {
        corruption = 0;
    }

    private void Update()
    {
        meterNumber.text = corruption + "%";
        if (corruption > maxCorruption)
        {
            corruption = maxCorruption;
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
}
