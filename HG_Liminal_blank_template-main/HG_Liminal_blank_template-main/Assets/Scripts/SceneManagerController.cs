using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerController : MonoBehaviour
{
    [System.Serializable]
    public class PointLightSettings
    {
        public Color lightColor = Color.white; // Default light color
        public float intensity = 1f; // Default light intensity
    }

    [System.Serializable]
    public class SceneInfo
    {
        public string sceneName; // Name of the scene to load
        public float timeDuration; // Time duration for this specific scene
        public PointLightSettings pointLightSettings; // Light settings for the car's light
    }

    public static SceneManagerController Instance { get; private set; } // Singleton instance
    public Image fadeImage; // UI Image for fade effect
    public float fadeDuration = 1f; // Duration of the fade effect
    public SceneInfo[] scenesToLoad; // Array to hold scenes and their time durations
    public GameObject audioSwitchController; // Audio switch controller to carry over
    public GameObject experienceAppPlayer; // Experience app player to carry over
    public string finalSceneName; // The name of the final scene
    public float finalSceneExitTime = 5f; // Time duration before exiting on the final scene
    public float initialSceneDuration = 3f; // Time duration for the initial scene before transition

    public GameObject portalFXPrefab; // Reference to the portal FX prefab
    private GameObject currentPortalFX; // Keep a reference to the current portal FX instance

    private int currentSceneIndex = 0;

    // New references for the car's point light
    public Light carPointLight; // Assign the car's point light here in the Inspector or via code

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set the singleton instance
            DontDestroyOnLoad(audioSwitchController);
            DontDestroyOnLoad(experienceAppPlayer);
            DontDestroyOnLoad(gameObject); // Ensure this manager persists across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy any duplicate instances
        }
    }

    private void Start()
    {
        StartCoroutine(InitialSceneWait());
    }

    private IEnumerator InitialSceneWait()
    {
        yield return new WaitForSeconds(initialSceneDuration); // Wait for initial scene duration
        yield return ActivatePortalFXAndWait(); // Activate the portal FX and wait for it to finish
    }

    private IEnumerator ActivatePortalFXAndWait()
    {
        if (portalFXPrefab != null)
        {
            currentPortalFX = Instantiate(portalFXPrefab); // Instantiate the portal FX
            currentPortalFX.transform.SetParent(experienceAppPlayer.transform); // Make portal a child of ExperienceAppPlayer
            Animator portalAnimator = currentPortalFX.GetComponent<Animator>();

            if (portalAnimator != null)
            {
                // Wait for the portal animation to complete
                yield return StartCoroutine(WaitForPortalAnimation(portalAnimator));
            }
            else
            {
                Debug.LogError("No Animator component found on Portal FX Prefab.");
            }
        }
        else
        {
            Debug.LogError("Portal FX Prefab is not assigned.");
        }

        StartCoroutine(LoadSceneSequence()); // Start loading the next scene in sequence
    }

    private IEnumerator ActivatePortalFXBeforeFinalSceneAndWait()
    {
        if (portalFXPrefab != null)
        {
            currentPortalFX = Instantiate(portalFXPrefab); // Instantiate the portal FX
            currentPortalFX.transform.SetParent(experienceAppPlayer.transform); // Make portal a child of ExperienceAppPlayer
            Animator portalAnimator = currentPortalFX.GetComponent<Animator>();

            if (portalAnimator != null)
            {
                // Wait for the portal animation to complete
                yield return StartCoroutine(WaitForPortalAnimation(portalAnimator));
            }
            else
            {
                Debug.LogError("No Animator component found on Portal FX Prefab.");
            }
        }
        else
        {
            Debug.LogError("Portal FX Prefab is not assigned.");
        }
    }

    private IEnumerator WaitForPortalAnimation(Animator animator)
    {
        // Wait for the animation to finish
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
    }

    private IEnumerator LoadSceneSequence()
    {
        // Start loading scenes only if currentSceneIndex is less than the number of scenes
        if (currentSceneIndex < scenesToLoad.Length)
        {
            SceneInfo sceneInfo = scenesToLoad[currentSceneIndex];

            // Set the point light intensity and color for the current scene
            if (carPointLight != null)
            {
                carPointLight.color = sceneInfo.pointLightSettings.lightColor;
                carPointLight.intensity = sceneInfo.pointLightSettings.intensity;
            }

            // Start scene fade and wait for the scene's specific duration
            yield return StartCoroutine(FadeAndLoadScene(sceneInfo.sceneName));
            Debug.Log($"Scene '{sceneInfo.sceneName}' duration: {sceneInfo.timeDuration} seconds");




            // Wait for the duration of the scene
            yield return new WaitForSeconds(sceneInfo.timeDuration);

            // Check if this scene is the final one
            if (sceneInfo.sceneName == finalSceneName)
            {
                Debug.Log("Reached final scene: " + sceneInfo.sceneName);
                yield return new WaitForSeconds(finalSceneExitTime); // Wait for final scene exit time
                StartCoroutine(FadeOutAndQuit()); // Quit after fade-out
                yield break; // End the coroutine since it's the final scene
            }

            // Destroy the current portal FX instance before moving to the next scene
            Destroy(currentPortalFX);

            yield return ActivatePortalFXBeforeFinalSceneAndWait(); // This ensures the portal FX plays before moving to the next scene

            currentSceneIndex++; // Move to the next scene
            yield return StartCoroutine(LoadSceneSequence()); // Recursive call to load the next scene
        }
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        yield return StartCoroutine(FadeOut()); // Fade out to black
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName); // Load the scene asynchronously
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return StartCoroutine(FadeIn()); // Fade back in after loading the scene
    }

    private IEnumerator FadeOutAndQuit()
    {
        yield return StartCoroutine(FadeOut()); // Fade out to black
        Application.Quit(); // Quit the application

        // For editor testing, since Application.Quit() doesn't work in the editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        Color color = fadeImage.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }
}
