using System.Collections;
using UnityEngine;
using TMPro;
using Liminal.SDK.Core;
using Liminal.Core.Fader;

public class SceneManagerController : MonoBehaviour
{
    [System.Serializable]
    public class PointLightSettings
    {
        public Color lightColor = Color.white;
        public float intensity = 1f;
    }

    [System.Serializable]
    public class MaterialSwapSetting
    {
        public GameObject gameObject;
        public Material newMaterial;
    }

    [System.Serializable]
    public class MaterialSettings
    {
        public MaterialSwapSetting[] materialsToSwap;
        public TextMeshProUGUI[] textMeshProUGUIObjects;
        public Color textVertexColor = Color.white;
    }

    [System.Serializable]
    public class SceneSettings
    {
        public GameObject scenePrefab;
        public float sceneDuration = 5f;
        public Material skyboxMaterial;
        public Color ambientLightColor = Color.white;
        public Color fogColor = Color.gray;
        public float fogDensity = 0.01f; // Add fogDensity field
        public PointLightSettings pointLightSettings;
        public MaterialSettings materialSettings;
    }

    public UIManager uiManager; // Reference to UIManager
    public GameObject fadeObject;
    public float fadeDuration = 1f;
    public SceneSettings[] sceneSettings; // Array to manage scenes as prefabs
    public SceneSettings finalSceneSettings; // Add a SceneSettings object for the final scene
    public GameObject finalScenePrefab; // Final scene prefab for application quit
    public float finalSceneDuration = 5f; // Duration for the final scene
    public GameObject audioSwitchController;
    public GameObject experienceAppPlayer;
    public GameObject portalFXPrefab; // Add a reference to the portalFX prefab
    public GameObject menuPrefab; // Add a reference to the Menu prefab
    public GameObject carGameObject; // Reference to the Car GameObject

    public Light carPointLight;
    private Material fadeMaterial;
    private bool isFading = false;
    private int currentSceneIndex = 0;
    private GameObject currentScenePrefab;
    private GameObject portalFXInstance; // Reference to the instantiated portalFX
    private AudioSource audioSource; // Reference to the AudioSource component

    private void Start()
    {
        if (fadeObject != null)
        {
            fadeMaterial = fadeObject.GetComponent<Renderer>().material;
            Color color = fadeMaterial.color;
            color.a = 0f;
            fadeMaterial.color = color;
        }

        // Get the AudioSource from the audioSwitchController
        if (audioSwitchController != null)
        {
            audioSource = audioSwitchController.GetComponent<AudioSource>();
        }

        // Load the menu scene first
        StartCoroutine(LoadMenuScene());
    }

    private IEnumerator LoadMenuScene()
    {
        // Ensure we are using the already-existing Menu GameObject in the scene
        if (menuPrefab != null)
        {
            // No need to instantiate the menuPrefab, use the existing menuGameObject instead

            // Turn off the Car GameObject
            if (carGameObject != null)
            {
                carGameObject.SetActive(false);
            }

            // Fade in after the menu is loaded
            yield return StartCoroutine(FadeIn());

            // Wait for user input or specific duration (can be modified here)
            // Example: Wait for a button click (you need to handle this elsewhere in your UI)
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space)); // Placeholder for user input

            // Fade out when leaving the menu
            yield return StartCoroutine(FadeOut());

            // Deactivate the Menu GameObject instead of destroying it
            if (menuPrefab != null)
            {
                menuPrefab.SetActive(false); // Deactivate the existing Menu GameObject
            }

            // Turn the Car GameObject back on when leaving the menu
            if (carGameObject != null)
            {
                carGameObject.SetActive(true);
            }

            // Start loading the first scene
            StartCoroutine(LoadFirstScene());
        }
    }

    private IEnumerator LoadFirstScene()
    {
        // Ensure we load Element 0 of the sceneSettings array
        if (sceneSettings.Length > 0)
        {
            // Load the first scene (Element 0)
            SceneSettings firstScene = sceneSettings[0];

            // Instantiate the first scene prefab
            currentScenePrefab = Instantiate(firstScene.scenePrefab);

            // Apply settings (Skybox, lighting, fog, etc.) for the first scene
            ApplySceneSettings(firstScene);

            // Fade in after the first scene is loaded
            yield return StartCoroutine(FadeIn());

            // Wait for the first scene's duration before moving to the next scene
            yield return new WaitForSeconds(firstScene.sceneDuration);

            // Start loading the next scenes after the first scene's duration
            currentSceneIndex = 1;  // Set index to 1 to load the second scene next
            StartCoroutine(LoadNextScene());
        }
    }

    // New Functions to set scene and final scene durations, and load the next scene with fade
    public void SetDuration3Minutes()
    {
        // Set to 3 minutes (180 seconds)
        finalSceneDuration = 75f;
        if (sceneSettings.Length > 0)
        {
            sceneSettings[0].sceneDuration = 75f;
        }
        Debug.Log("Set duration to 3 minutes.");

        // Set prompt delays for 3 minutes
        if (uiManager != null)
        {
            uiManager.promptDelays = new float[] { 5f, 10f, 15f }; // Example delay times for 3 minutes
        }

        // Start the transition to the next scene
        StartCoroutine(FadeOutAndLoadNextScene());
    }

    public void SetDuration5Minutes()
    {
        // Set to 5 minutes (300 seconds)
        finalSceneDuration = 132f;
        if (sceneSettings.Length > 0)
        {
            sceneSettings[0].sceneDuration = 132f;
        }
        Debug.Log("Set duration to 5 minutes.");

        // Set prompt delays for 5 minutes
        if (uiManager != null)
        {
            uiManager.promptDelays = new float[] { 10f, 20f, 30f }; // Example delay times for 5 minutes
        }

        // Start the transition to the next scene
        StartCoroutine(FadeOutAndLoadNextScene());
    }

    public void SetDuration10Minutes()
    {
        // Set to 10 minutes (600 seconds)
        finalSceneDuration = 282f;
        if (sceneSettings.Length > 0)
        {
            sceneSettings[0].sceneDuration = 282f;
        }
        Debug.Log("Set duration to 10 minutes.");

        // Set prompt delays for 10 minutes
        if (uiManager != null)
        {
            uiManager.promptDelays = new float[] { 15f, 30f, 45f }; // Example delay times for 10 minutes
        }

        // Start the transition to the next scene
        StartCoroutine(FadeOutAndLoadNextScene());
    }

    // Coroutine for fading out, loading the next scene, and fading back in
    private IEnumerator FadeOutAndLoadNextScene()
    {
        // Start the fade-out
        yield return StartCoroutine(FadeOut());

        // Turn off the Menu GameObject (Deactivate it)
        if (menuPrefab != null)
        {
            menuPrefab.SetActive(false); // Deactivate the existing Menu GameObject in the scene
        }

        // Turn the Car GameObject back on when leaving the menu
        if (carGameObject != null)
        {
            carGameObject.SetActive(true);
        }

        // Load the first scene
        StartCoroutine(LoadFirstScene());

        // Start the fade-in after loading the scene
        yield return StartCoroutine(FadeIn());
    }


    private IEnumerator LoadNextScene()
    {
        while (currentSceneIndex < sceneSettings.Length)
        {
            SceneSettings currentScene = sceneSettings[currentSceneIndex];

            // Instantiate and parent the portalFX to ExperienceApp 5 seconds before fading out
            if (portalFXPrefab != null)
            {
                portalFXInstance = Instantiate(portalFXPrefab, experienceAppPlayer.transform); // Parent to ExperienceApp
            }

            // Wait for 4 seconds before starting fade out
            yield return new WaitForSeconds(4f);

            yield return StartCoroutine(FadeOut()); // Fade out before loading the next scene

            // Destroy both the current scene prefab and the portalFXInstance
            if (currentScenePrefab != null) Destroy(currentScenePrefab); // Destroy the previous scene prefab
            if (portalFXInstance != null) Destroy(portalFXInstance); // Destroy the portalFX

            currentScenePrefab = Instantiate(currentScene.scenePrefab); // Instantiate new scene prefab

            ApplySceneSettings(currentScene); // Apply settings (Skybox, lighting, fog, etc.)

            yield return StartCoroutine(FadeIn()); // Fade in after loading

            yield return new WaitForSeconds(currentScene.sceneDuration); // Wait for scene duration

            currentSceneIndex++;
        }

        // Load final scene prefab
        if (finalScenePrefab != null)
        {
            // Instantiate and parent the portalFX for the final scene FadeOut
            if (portalFXPrefab != null)
            {
                portalFXInstance = Instantiate(portalFXPrefab, experienceAppPlayer.transform); // Parent to ExperienceApp
            }

            // Wait for 5 seconds before starting fade out
            yield return new WaitForSeconds(4f);

            yield return StartCoroutine(FadeOut());

            if (currentScenePrefab != null) Destroy(currentScenePrefab); // Destroy the last scene prefab
            if (portalFXInstance != null) Destroy(portalFXInstance); // Destroy the portalFX

            currentScenePrefab = Instantiate(finalScenePrefab); // Instantiate final scene prefab

            ApplyFinalSceneSettings(); // Apply any settings you want for the final scene

            yield return StartCoroutine(FadeIn());

            yield return new WaitForSeconds(finalSceneDuration); // Wait for final scene duration

            // Instantiate and parent the portalFX to ExperienceApp 5 seconds before fading out for the final time
            if (portalFXPrefab != null)
            {
                portalFXInstance = Instantiate(portalFXPrefab, experienceAppPlayer.transform); // Parent to ExperienceApp
            }

            // Wait for 5 seconds before starting fade out
            yield return new WaitForSeconds(5f);

            // Fade out and load the Menu scene again instead of quitting
            yield return StartCoroutine(FadeOut());

            // Destroy both the current scene prefab and the portalFXInstance
            if (currentScenePrefab != null) Destroy(currentScenePrefab); // Destroy the previous scene prefab
            if (portalFXInstance != null) Destroy(portalFXInstance); // Destroy the portalFX

            // If Liminal Experience App, end scene here.
            ExperienceApp.End();

        }
    }


    private IEnumerator DestroyCurrentScenePrefab()
    {
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        Destroy(currentScenePrefab); // Destroy the last scene prefab
                                     // Stop the AudioSource when ExperienceApp.End() is called
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    private void ApplySceneSettings(SceneSettings scene)
    {
        if (scene.skyboxMaterial != null)
        {
            RenderSettings.skybox = scene.skyboxMaterial;
        }

        RenderSettings.ambientLight = scene.ambientLightColor;
        RenderSettings.fogColor = scene.fogColor;
        RenderSettings.fogDensity = scene.fogDensity; // Apply fog density

        if (carPointLight != null)
        {
            carPointLight.color = scene.pointLightSettings.lightColor;
            carPointLight.intensity = scene.pointLightSettings.intensity;
        }

        SwapMaterials(scene.materialSettings);
    }

    private void ApplyFinalSceneSettings()
    {
        if (finalSceneSettings != null)
        {
            if (finalSceneSettings.skyboxMaterial != null)
            {
                RenderSettings.skybox = finalSceneSettings.skyboxMaterial;
            }

            RenderSettings.ambientLight = finalSceneSettings.ambientLightColor;
            RenderSettings.fogColor = finalSceneSettings.fogColor;
            RenderSettings.fogDensity = finalSceneSettings.fogDensity;

            if (carPointLight != null)
            {
                carPointLight.color = finalSceneSettings.pointLightSettings.lightColor;
                carPointLight.intensity = finalSceneSettings.pointLightSettings.intensity;
            }

            SwapMaterials(finalSceneSettings.materialSettings);
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
                        Material[] materials = renderer.materials;
                        for (int i = 0; i < materials.Length; i++)
                        {
                            materials[i] = swapSetting.newMaterial;
                        }
                        renderer.materials = materials;
                    }
                }
            }
        }

        if (materialSettings.textMeshProUGUIObjects != null)
        {
            foreach (TextMeshProUGUI tmpUGUI in materialSettings.textMeshProUGUIObjects)
            {
                if (tmpUGUI != null)
                {
                    tmpUGUI.color = materialSettings.textVertexColor;
                }
            }
        }
    }

    private IEnumerator FadeOut()
    {
        if (fadeMaterial != null && !isFading)
        {
            isFading = true;
            float timer = 0f;
            Color color = fadeMaterial.color;
            color.a = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                fadeMaterial.color = color;
                yield return null;
            }

            color.a = 1f;
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
            color.a = 1f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                fadeMaterial.color = color;
                yield return null;
            }

            color.a = 0f;
            fadeMaterial.color = color;
            isFading = false;
        }
    }
}
