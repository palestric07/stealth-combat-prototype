using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider noiseSlider;

    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private AudioSource alertAudioSource;

    private Coroutine blinkCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateHealth(float current, float max)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }
    }

    public void UpdateStamina(float current, float max)
    {
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = max;
            staminaSlider.value = current;
        }
    }

    public void UpdateNoiseLevel(float level)
    {
        if (noiseSlider != null)
        {
            noiseSlider.value = Mathf.Clamp(level, 0f, 100f);
        }
    }

    public void TriggerAlert(bool isDetected)
    {
        if (isDetected)
        {
            if (blinkCoroutine == null)
            {
                blinkCoroutine = StartCoroutine(BlinkAlertUI());
            }

            if (alertAudioSource != null && !alertAudioSource.isPlaying)
            {
                alertAudioSource.Play();
            }
        }
        else
        {
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }

            if (alertText != null)
            {
                alertText.gameObject.SetActive(false);
            }

            if (alertAudioSource != null && alertAudioSource.isPlaying)
            {
                alertAudioSource.Stop();
            }
        }
    }

    private IEnumerator BlinkAlertUI()
    {
        while (true)
        {
            if (alertText != null)
            {
                alertText.gameObject.SetActive(!alertText.gameObject.activeSelf);
            }
            yield return new WaitForSeconds(0.4f);
        }
    }
}