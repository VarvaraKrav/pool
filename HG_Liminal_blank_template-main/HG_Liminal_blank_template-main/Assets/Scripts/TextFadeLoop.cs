using UnityEngine;
using TMPro;  // Make sure to include this to work with TextMeshPro
using System.Collections;  // For coroutines

public class TextFadeLoop : MonoBehaviour
{
    public TextMeshProUGUI textElement;  // The TextMeshPro UI element
    public float fadeDuration = 1.0f;    // Time it takes to fade in or out

    private Color originalColor;

    void Start()
    {
        // Store the original color of the text
        originalColor = textElement.color;

        // Start the fade-out loop
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color currentColor = textElement.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);  // Interpolate from 1 to 0
            currentColor.a = alpha;
            textElement.color = currentColor;

            yield return null;  // Wait for the next frame
        }

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color currentColor = textElement.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);  // Interpolate from 0 to 1
            currentColor.a = alpha;
            textElement.color = currentColor;

            yield return null;  // Wait for the next frame
        }

        StartCoroutine(FadeOut());
    }
}
