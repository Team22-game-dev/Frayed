using System.Collections;
using System.Collections.Generic;
using Frayed.Input;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    // Singleton Design
    private static DamageIndicator _instance;
    public static DamageIndicator Instance => _instance;


    [SerializeField]
    private GameObject textPrefab;
    [SerializeField]
    private Color defaultColor = Color.red;
    [SerializeField]
    [Tooltip("In seconds.")]
    private float displayTime = 0.2f;
    [SerializeField]
    private float startingFontSize = 36f;
    [SerializeField]
    private float endingFontSize = 18f;
    [SerializeField]
    private GameObject mainCamera;

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

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(textPrefab != null);

        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        Debug.Assert(mainCamera != null);
    }

    public void IndicateDamage(float damage, Vector3 hitPosition)
    {
        IndicateDamage(damage, hitPosition, defaultColor);
    }

    public void IndicateDamage(float damage, Vector3 hitPosition, Color color)
    {
        if (textPrefab != null)
        {
            GameObject textPrefabInstance = Instantiate(textPrefab, hitPosition, Quaternion.identity, transform);
            StartCoroutine(DamageTextSequence(textPrefabInstance, damage, hitPosition, color));
        }
    }

    public IEnumerator DamageTextSequence(GameObject textPrefabInstance, float damage, Vector3 hitPosition, Color color)
    {
        GameObject damageText = textPrefabInstance.transform.Find("DamageText").gameObject;
        RectTransform rectTransform = damageText.GetComponent<RectTransform>();
        rectTransform.position = hitPosition;
        TMP_Text tmpText = damageText.GetComponent<TMP_Text>();
        tmpText.fontSize = startingFontSize;
        tmpText.text = damage.ToString();
        tmpText.color = color;
        float elapsedTime = 0f;
        while (elapsedTime < displayTime)
        {
            float deltaTime = Time.deltaTime;
            tmpText.fontSize -= (startingFontSize - endingFontSize) * (deltaTime / displayTime);
            elapsedTime += deltaTime;

            Vector3 textDirection = (damageText.transform.position - mainCamera.transform.position).normalized;
            damageText.transform.rotation = Quaternion.LookRotation(textDirection);
            yield return null;
        }
        Destroy(textPrefabInstance);
        yield return null;
    }
}
