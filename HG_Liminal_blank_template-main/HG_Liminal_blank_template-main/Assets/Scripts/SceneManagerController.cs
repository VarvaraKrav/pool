using System.Collections;
using UnityEngine;
using TMPro;

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
        public GameObject scenePrefab; // Prefab to load for this scene
        public float sceneDuration = 5f; // How long this scene will last
        public Material skyboxMaterial; // Skybox for the scene
        public Color ambientLightColor = Color.white; // Ambient lighting color for the scene
        public Color fogColor = Color.gray; // Fog color for the scene
        public PointLightSettings pointLightSettings;
        public MaterialSettings materialSettings;
    }

    public GameObject fadeObject;
    public float fadeDuration = 1f;
    public SceneSettings[] sceneSettings; // Array to manage scenes as prefabs
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
        StartCoroutine(LoadNextScene());
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
