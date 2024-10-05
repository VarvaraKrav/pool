using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerController : MonoBehaviour
{
    [System.Serializable]
    public class SceneInfo
    {
        public string sceneName; // Name of the scene to load
        public float timeDuration; // Time duration for this specific scene
    }

    public Image fadeImage; // UI Image for fade effect
    public float fadeDuration = 1f; // Duration of the fade effect
    public SceneInfo[] scenesToLoad; // Array to hold scenes and their time durations
    public GameObject audioSwitchController; // Audio switch controller to carry over
    public GameObject experienceAppPlayer; // Experience app player to carry over
    public string finalSceneName; // The name of the final scene
    public float finalSceneExitTime = 5f; // Time duration before exiting on the final scene
    public float initialSceneDuration = 3f; // Time duration for the initial scene before transition

    private int currentSceneIndex = 0;

    private void Start()
    {
        DontDestroyOnLoad(audioSwitchController);
        DontDestroyOnLoad(experienceAppPlayer);
        DontDestroyOnLoad(gameObject); // Ensure this manager persists across scenes

        StartCoroutine(InitialSceneWait());
    }

    private IEnumerator InitialSceneWait()
    {
        yield return new WaitForSeconds(initialSceneDuration); // Wait for initial scene duration
        StartCoroutine(LoadSceneSequence()); // Start loading the next scene in sequence
    }

    private IEnumerator LoadSceneSequence()
    {
        while (currentSceneIndex < scenesToLoad.Length)
        {
            SceneInfo sceneInfo = scenesToLoad[currentSceneIndex];

            // Start scene fade and wait for the scene's specific duration
            yield return StartCoroutine(FadeAndLoadScene(sceneInfo.sceneName));
            yield return new WaitForSeconds(sceneInfo.timeDuration);

            // Check if this scene is the final one
            if (sceneInfo.sceneName == finalSceneName)
            {
                yield return new WaitForSeconds(finalSceneExitTime); // Wait for final scene exit time
                StartCoroutine(FadeOutAndQuit()); // Quit after fade-out
                yield break; // End the coroutine since it's the final scene
            }

            currentSceneIndex++;
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
