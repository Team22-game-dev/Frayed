using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VillageHelp : MonoBehaviour
{
    [SerializeField]
    private float timeElapsed;
    [SerializeField]
    private TMP_Text tmpText;
    void Start()
    {
        if (tmpText == null)
        {
            tmpText = transform.Find("Text").GetComponent<TMP_Text>();
        }
        Debug.Assert(tmpText != null);

        timeElapsed = 0.0f;
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameOverScreen.Instance.gameOverTriggered)
        {
            gameObject.SetActive(false);
            return;
        }
        timeElapsed += Time.deltaTime;
        if (timeElapsed <= 10.0f)
        {
            tmpText.text = "Welcome to the Village! Here, you can free play and try out new weapons and power ups!";
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
