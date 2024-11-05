using UnityEngine;
using TMPro;
using System.Collections;

public class TextFadeLoop : MonoBehaviour
{
    public TextMeshProUGUI textElement;  // The TextMeshPro UI element
    public float fadeDuration = 1.0f;    // Time it takes to fade in or out
    //public Light spotlight;              // The spotlight to sync with the text fade
    public float glowMultiplier = 1.0f;


    private Color originalColor;
    private Material textMaterial;
    private float originalIntensity;

    void Start()
    {
        // Store the original color of the text
        originalColor = textElement.color;

        // Instantiate a unique material for this text element
        textMaterial = new Material(textElement.fontMaterial);
        textElement.fontMaterial = textMaterial;  // Assign the new material to the text element

        // Set up the spotlight
       /* if (spotlight == null)
        {
            GameObject lightObj = new GameObject("Text Spotlight");
            spotlight = lightObj.AddComponent<Light>();
            spotlight.type = LightType.Spot;
            spotlight.spotAngle = 34f;
            spotlight.range = 1.14f;
            spotlight.color = Color.yellow;
            


            // Make the spotlight a child of the text element
            lightObj.transform.SetParent(textElement.transform);

            // Position the spotlight slightly in front of the text
            lightObj.transform.localPosition = new Vector3(0.84f, 0.18f, -0.422f);
        }
       */
       
        // Store the original intensity of the spotlight
        //originalIntensity = spotlight.intensity * glowMultiplier;

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

            // Adjust glow intensity based on alpha value
            textMaterial.SetFloat(ShaderUtilities.ID_GlowPower, alpha * glowMultiplier);  // Adjust multiplier as needed

            // Adjust spotlight intensity based on alpha
            //spotlight.intensity = originalIntensity * alpha;

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

            // Adjust glow intensity based on alpha value
            textMaterial.SetFloat(ShaderUtilities.ID_GlowPower, alpha * glowMultiplier);  // Adjust multiplier as needed

            // Adjust spotlight intensity based on alpha
            //spotlight.intensity = originalIntensity * alpha;

            yield return null;  // Wait for the next frame
        }

        StartCoroutine(FadeOut());
    }
}
