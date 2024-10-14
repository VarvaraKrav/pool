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
        public PointLightSettings pointLightSettings;
        public MaterialSettings materialSettings;
    }

    public GameObject fadeObject;
    public float fadeDuration = 1f;
    public SceneSettings[] sceneSettings; // Array to manage scenes as prefabs
    public GameObject finalScenePrefab; // Final scene prefab for application quit
    public float finalSceneDuration = 5f; // Duration for the final scene
    public GameObject audioSwitchController;
    public GameObject experienceAppPlayer;

    public Light carPointLight;
    private Material fadeMaterial;
    private bool isFading = false;
    private int currentSceneIndex = 0;
    private GameObject currentScenePrefab;

    private void Start()
    {
        if (fadeObject != null)
        {
            fadeMaterial = fadeObject.GetComponent<Renderer>().material;
            Color color = fadeMaterial.color;
            color.a = 0f;
            fadeMaterial.color = color;
        }

        // Load the first scene (Element 0) immediately and display it for its duration
        StartCoroutine(LoadFirstScene());
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



    private IEnumerator LoadNextScene()
    {
        while (currentSceneIndex < sceneSettings.Length)
        {
            SceneSettings currentScene = sceneSettings[currentSceneIndex];

            yield return StartCoroutine(FadeOut()); // Fade out before loading the next scene

            if (currentScenePrefab != null) Destroy(currentScenePrefab); // Destroy the previous scene prefab
            currentScenePrefab = Instantiate(currentScene.scenePrefab); // Instantiate new scene prefab

            ApplySceneSettings(currentScene); // Apply settings (Skybox, lighting, fog, etc.)

            yield return StartCoroutine(FadeIn()); // Fade in after loading

            yield return new WaitForSeconds(currentScene.sceneDuration); // Wait for scene duration

            currentSceneIndex++;
        }

        // Load final scene prefab
        if (finalScenePrefab != null)
        {
            yield return StartCoroutine(FadeOut());

            if (currentScenePrefab != null) Destroy(currentScenePrefab); // Destroy the last scene prefab
            currentScenePrefab = Instantiate(finalScenePrefab); // Instantiate final scene prefab

            ApplyFinalSceneSettings(); // Apply any settings you want for the final scene

            yield return StartCoroutine(FadeIn());

            yield return new WaitForSeconds(finalSceneDuration); // Wait for final scene duration

            // Quit the application after the final scene
            Debug.Log("Application Quit after Final Scene");
            ExperienceApp.End();
            var fader = ScreenFader.Instance;
            fader.FadeToBlack();
            StartCoroutine(DestroyCurrentScenePrefab());
        }
    }
    private IEnumerator DestroyCurrentScenePrefab()
    {
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        Destroy(currentScenePrefab); // Destroy the last scene prefab
    }



    private void ApplySceneSettings(SceneSettings scene)
    {
        // Apply Skybox material
        if (scene.skyboxMaterial != null)
        {
            RenderSettings.skybox = scene.skyboxMaterial;
        }

        // Apply ambient light
        RenderSettings.ambientLight = scene.ambientLightColor;

        // Apply fog color
        RenderSettings.fogColor = scene.fogColor;

        // Apply point light settings
        if (carPointLight != null)
        {
            carPointLight.color = scene.pointLightSettings.lightColor;
            carPointLight.intensity = scene.pointLightSettings.intensity;
        }

        // Apply material swaps and text changes
        SwapMaterials(scene.materialSettings);
    }

    private void ApplyFinalSceneSettings()
    {
        // You can add any specific logic for final scene settings here
        // For example, a different Skybox, lighting, or effects.
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
