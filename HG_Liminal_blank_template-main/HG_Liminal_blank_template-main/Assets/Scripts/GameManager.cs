using System.Collections;
using UnityEngine;
using Liminal.SDK.Core;
using Liminal.Core.Fader;

public class GameManager : MonoBehaviour
{
    [Header("Fade Settings")]
    public Material fadeMaterial; // Material to use for fading
    public float fadeDuration = 1.0f;

    [Header("Game Settings")]
    public float gameDuration = 10.0f; // Default game duration in minutes

    [SerializeField, Tooltip("Time remaining in seconds, visible in the Inspector.")]
    private float timeRemaining;

    private bool isFading = false;
    private bool gameStarted = false;

    public GameObject menuScene;
    public GameObject gameScene;
    public GameObject carGroup;


    [Header("VR Hands")]
    public GameObject vrLeftHand;  // Reference to the VR Left Hand GameObject
    public GameObject vrRightHand; // Reference to the VR Right Hand GameObject

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;

    //public GameObject carGameObject;
    public UIManager uiManager;

    private void Start()
    {
        menuScene.SetActive(true);
        gameScene.SetActive(false);

        

        if (carGroup == null)
        {
            // Attempt to find CarGroup by name if not assigned in Inspector
            carGroup = GameObject.Find("carGroup");  // Make sure the name matches exactly
        }

        if (carGroup != null)
        {
            carGroup.SetActive(false);  // Disable CarGroup
            Debug.Log("CarGroup SetActive = False.");
        }
        else
        {
            Debug.LogError("CarGroup is not assigned and could not be found!");
        }
    }

    private void Update()
    {
        if (gameStarted)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                StartCoroutine(FadeAndReturnToMenu());
                gameStarted = false;
            }
        }
    }

    public void SetGameDurationAndStart(float durationInMinutes)
    {
        gameDuration = durationInMinutes;
        timeRemaining = gameDuration * 60; // Convert minutes to seconds
        StartCoroutine(FadeAndStartGame());
    }

    private IEnumerator FadeAndStartGame()
    {
        yield return StartCoroutine(FadeOut());

        menuScene.SetActive(false);
        gameScene.SetActive(true);
        gameStarted = true;

        if (carGroup != null)
        {
            carGroup.SetActive(true);
            Debug.Log("carGroup SetActive = True");
        }

        // Destroy the VR hands when starting the game
        if (vrLeftHand != null)
        {
            Destroy(vrLeftHand);
        }

        if (vrRightHand != null)
        {
            Destroy(vrRightHand);
        }

        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeAndReturnToMenu()
    {
        yield return StartCoroutine(FadeOut());

        gameScene.SetActive(false);
        menuScene.SetActive(true);

        if (carGroup != null)
        {
            carGroup.SetActive(false);
        }

        yield return StartCoroutine(FadeIn());
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
            float timer = 20f;
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

    public void EndGameAndReturnToMenu()
    {
        var fader = ScreenFader.Instance;
        fader.FadeToBlack();
        fader.WaitUntilFadeComplete();
        ExperienceApp.End();
        StartCoroutine(FadeAndReturnToMenu());
    }

    private void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    public void OnThreeMinuteButtonClick()
    {
        PlayButtonClickSound();
        SetGameDurationAndStart(3);

        if (uiManager != null)
        {
            uiManager.promptDelays = new float[] { 170f };
            uiManager.gameObject.SetActive(true);
        }
    }

    public void OnFiveMinuteButtonClick()
    {
        PlayButtonClickSound();
        SetGameDurationAndStart(5);

        if (uiManager != null)
        {
            uiManager.promptDelays = new float[] { 290f };
            uiManager.gameObject.SetActive(true);
        }
    }

    public void OnTenMinuteButtonClick()
    {
        PlayButtonClickSound();
        SetGameDurationAndStart(10);

        if (uiManager != null)
        {
            uiManager.promptDelays = new float[] { 590f };
            uiManager.gameObject.SetActive(true);
        }
    }
}
