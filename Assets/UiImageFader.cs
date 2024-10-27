using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class UiImageFader : MonoBehaviour
{
    public Image uiImage;  // Drag your UI Image here in the Inspector
    public float minDuration = 0.5f;  // Minimum time for fading
    public float maxDuration = 2f;    // Maximum time for fading

    private void Start()
    {
        uiImage = this.GetComponent<Image>();
        // Start the fade loop when the script initializes
        StartCoroutine(FadeLoop());
    }

    private IEnumerator FadeLoop()
    {
        while (true)  // Infinite loop to keep fading
        {
            // Random duration for fading
            float fadeDuration = Random.Range(minDuration, maxDuration);

            // Random target alpha (0 or 1)
            float targetAlpha = Random.value > 0.5f ? 1f : 0f;

            // Start fading the image's alpha to the target value
            yield return StartCoroutine(FadeAlpha(targetAlpha, fadeDuration));
        }
    }

    private IEnumerator FadeAlpha(float targetAlpha, float duration)
    {
        float startAlpha = uiImage.color.a;  // Get the current alpha
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Lerp the alpha value between start and target
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            // Apply the new alpha to the image's color
            Color color = uiImage.color;
            color.a = newAlpha;
            uiImage.color = color;

            yield return null;  // Wait for the next frame
        }

        // Ensure the final alpha is set to the target
        Color finalColor = uiImage.color;
        finalColor.a = targetAlpha;
        uiImage.color = finalColor;
    }
}
