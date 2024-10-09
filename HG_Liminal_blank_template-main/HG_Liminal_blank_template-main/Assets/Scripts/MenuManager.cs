using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject fadeObject; // Object with the material to fade
    public float fadeDuration = 1f; // Duration of the fade effect
    public string startScene; // The name of the scene to load when starting the game

    private Material fadeMaterial; // Material to fade
    private bool isFading = false; // Prevent multiple fades from happening at once

    private void Start()
    {
        if (fadeObject != null)
        {
            fadeMaterial = fadeObject.GetComponent<Renderer>().material;
            // Make sure the fade starts fully transparent
            Color color = fadeMaterial.color;
            color.a = 0f;
            fadeMaterial.color = color;
        }
    }

    // This function can be linked to a play button in the Unity Inspector
    public void OnPlayButtonPressed()
    {
        StartCoroutine(PlayGame());
    }

    // This function can be linked to an exit button in the Unity Inspector
    public void OnExitButtonPressed()
    {
        StartCoroutine(ExitGame());
    }

    // Coroutine to start the game by loading the specified scene
    private IEnumerator PlayGame()
    {
        yield return StartCoroutine(FadeOut()); // Fade out before loading
        // Load the scene specified in the 'startScene' variable
        if (!string.IsNullOrEmpty(startScene))
        {
            SceneManager.LoadScene(startScene);
        }
        else
        {
            Debug.LogError("StartScene is not set. Please specify a valid scene name.");
        }
    }

    // Coroutine to exit the game or the editor
    private IEnumerator ExitGame()
    {
        yield return StartCoroutine(FadeOut()); // Fade out before quitting
#if UNITY_EDITOR
        // If running in the editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running a built application, quit the application
        Application.Quit();
#endif
    }

    private IEnumerator FadeOut()
    {
        if (fadeMaterial != null && !isFading)
        {
            isFading = true;
            float timer = 0f;
            Color color = fadeMaterial.color;
            color.a = 0f; // Ensure starting alpha is 0

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration); // Fade alpha from 0 to 1
                fadeMaterial.color = color;
                yield return null;
            }

            color.a = 1f; // Ensure alpha is fully opaque at the end
            fadeMaterial.color = color;
            isFading = false;
        }
    }

    private IEnumerator FadeIn()
    {
        if (fadeMaterial != null && !isFading)
        {
            isFading = true;
            float timer = 0f;
            Color color = fadeMaterial.color;
            color.a = 1f; // Ensure starting alpha is 1

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration); // Fade alpha from 1 to 0
                fadeMaterial.color = color;
                yield return null;
            }

            color.a = 0f; // Ensure alpha is fully transparent at the end
            fadeMaterial.color = color;
            isFading = false;
        }
    }
}
