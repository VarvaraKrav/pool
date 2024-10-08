using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneManagerController : MonoBehaviour
{
    [System.Serializable]
    public class PointLightSettings
    {
        public Color lightColor = Color.white; // Default light color
        public float intensity = 1f; // Default light intensity
    }

    [System.Serializable]
    public class MaterialSwapSetting
    {
        public GameObject gameObject; // The game object whose material will be swapped
        public Material newMaterial; // The new material to apply to the game object
    }

    [System.Serializable]
    public class MaterialSettings
    {
        public MaterialSwapSetting[] materialsToSwap; // List of game objects and their new materials
        public TextMeshProUGUI[] textMeshProUGUIObjects; // List of TextMeshProUGUI objects to change vertex colors
        [HideInInspector] public Color originalTextVertexColor; // Original vertex color for TextMeshProUGUI objects
        public Color textVertexColor = Color.white; // Vertex color to change to for TextMeshProUGUI objects
    }

    [System.Serializable]
    public class SceneInfo
    {
        public string sceneName; // Name of the scene to load
        public float timeDuration; // Time duration for this specific scene
        public PointLightSettings pointLightSettings; // Light settings for the car's light
        public MaterialSettings materialSettings; // Material and text settings for the scene
    }

    public static SceneManagerController Instance { get; private set; } // Singleton instance
    public GameObject fadeObject; // Object with the material to fade
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

    private Material fadeMaterial; // Material to fade
    private bool isFading = false; // Prevent multiple fades from happening at once

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
        if (fadeObject != null)
        {
            fadeMaterial = fadeObject.GetComponent<Renderer>().material;
            // Make sure the fade starts fully transparent
            Color color = fadeMaterial.color;
            color.a = 0f;
            fadeMaterial.color = color;
        }
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

            StartCoroutine(DelayedActions(sceneInfo));

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

            // Destroy the current portal FX if it exists
            if (currentPortalFX != null)
            {
                Destroy(currentPortalFX);
            }

            // Move to the next scene
            currentSceneIndex++;
            StartCoroutine(LoadSceneSequence()); // Load the next scene
        }
    }

    // Call this method to start the delay
    public void StartDelayedActions(SceneInfo sceneInfo)
    {
        StartCoroutine(DelayedActions(sceneInfo));
    }

    private IEnumerator DelayedActions(SceneInfo sceneInfo)
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(1f);

        // Set the point light intensity and color for the current scene
        if (carPointLight != null)
        {
            carPointLight.color = sceneInfo.pointLightSettings.lightColor;
            carPointLight.intensity = sceneInfo.pointLightSettings.intensity;
        }

        // Swap materials for the specified game objects in the current scene
        SwapMaterials(sceneInfo.materialSettings);
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        yield return StartCoroutine(FadeOut()); // Fade out before loading
        SceneManager.LoadScene(sceneName); // Load the next scene
        yield return StartCoroutine(FadeIn()); // Fade in after loading
    }

    private IEnumerator FadeOutAndQuit()
    {
        yield return StartCoroutine(FadeOut()); // Fade out before quitting
        Application.Quit(); // Quit the application
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

    private void SwapMaterials(MaterialSettings materialSettings)
    {
        if (materialSettings.materialsToSwap != null)
        {
            foreach (MaterialSwapSetting swapSetting in materialSettings.materialsToSwap)
            {
                if (swapSetting.gameObject != null && swapSetting.newMaterial != null)
                {
                    Renderer renderer = swapSetting.gameObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        // Swap the material
                        Material[] materials = renderer.materials;
                        for (int i = 0; i < materials.Length; i++)
                        {
                            // Replace the material at index i with the new material
                            materials[i] = swapSetting.newMaterial;
                        }
                        renderer.materials = materials; // Update the materials array
                    }
                }
            }
        }

        // Change vertex colors for TextMeshProUGUI objects
        if (materialSettings.textMeshProUGUIObjects != null)
        {
            foreach (TextMeshProUGUI tmpUGUI in materialSettings.textMeshProUGUIObjects)
            {
                if (tmpUGUI != null)
                {
                    tmpUGUI.color = materialSettings.textVertexColor; // Set vertex color for TextMeshProUGUI objects
                }
            }
        }
    }
}
