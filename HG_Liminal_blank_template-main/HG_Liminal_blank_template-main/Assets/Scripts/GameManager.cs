using System.Collections;
using System.Collections.Generic;
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
    private float timeRemaining; // Now serialized so we can view it in the Inspector

    private bool isFading = false;
    private bool gameStarted = false;

    public GameObject menuScene; // Reference to the Menu Scene GameObject
    public GameObject gameScene; // Reference to the Game Scene GameObject

    [Header("Audio")]
    public AudioSource audioSource; // Reference to the AudioSource
    public AudioClip buttonClickSound; // Clip to play when buttons are clicked

    public GameObject carGameObject; // Reference to the Car GameObject
    public UIManager uiManager; // Reference to UIManager


    private void Start()
    {
        // Ensure menuScene is active at the beginning
        menuScene.SetActive(true);
        gameScene.SetActive(false); // Deactivate gameScene at the start

        // Turn off the Car GameObject
        if (carGameObject != null)
        {
            carGameObject.SetActive(false);
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

    // Call this when a UI button is pressed to set the time and start the game
    public void SetGameDurationAndStart(float durationInMinutes)
    {
        gameDuration = durationInMinutes;
        timeRemaining = gameDuration * 60; // Convert minutes to seconds
        StartCoroutine(FadeAndStartGame());
    }

    // Coroutine that waits for fade-out, switches to the game, and then fades in
    private IEnumerator FadeAndStartGame()
    {
        yield return StartCoroutine(FadeOut()); // Wait for fade-out to complete
        menuScene.SetActive(false); // Deactivate the menuScene after fade-out
        gameScene.SetActive(true);  // Activate the gameScene
        gameStarted = true;         // Start the game

        // Turn the Car GameObject back on when leaving the menu
        if (carGameObject != null)
        {
            carGameObject.SetActive(true);
        }

        yield return StartCoroutine(FadeIn());  // Wait for fade-in to complete

    }

    // Coroutine that waits for fade-out, returns to the menu, and then fades in
    private IEnumerator FadeAndReturnToMenu()
    {
        yield return StartCoroutine(FadeOut()); // Wait for fade-out to complete
        gameScene.SetActive(false); // Deactivate the gameScene after fade-out
        menuScene.SetActive(true); // Activate the menuScene after fade in.

        // Turn on the Car GameObject
        if (carGameObject != null)
        {
            carGameObject.SetActive(false);
        }

        yield return StartCoroutine(FadeIn());  // Wait for fade-in to complete
    }

    // Fade out function
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

    // Fade in function
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

    // Call this function to quit the game and return to the menu
    public void EndGameAndReturnToMenu()
    {
        var fader = ScreenFader.Instance;
        fader.FadeToBlack();
        fader.WaitUntilFadeComplete();
        ExperienceApp.End();
        StartCoroutine(FadeAndReturnToMenu());
    }

    // Helper function to play button click sound
    private void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    // Button click handlers for 3 minutes, 5 minutes, and 10 minutes
    public void OnThreeMinuteButtonClick()
    {
        PlayButtonClickSound();
        SetGameDurationAndStart(3);

        // Set prompt delays for 3 minutes
        if (uiManager != null)
        {
            uiManager.promptDelays = new float[] { 170f }; // Example delay times for 3 minutes

            // Activate the UIManager
            uiManager.gameObject.SetActive(true);
        }
    }

    public void OnFiveMinuteButtonClick()
    {
        PlayButtonClickSound();
        SetGameDurationAndStart(5);

        // Set prompt delays for 3 minutes
        if (uiManager != null)
        {
            uiManager.promptDelays = new float[] { 290f }; // Example delay times for 5 minutes

            // Activate the UIManager
            uiManager.gameObject.SetActive(true);
        }
    }

    public void OnTenMinuteButtonClick()
    {
        PlayButtonClickSound();
        SetGameDurationAndStart(10);

        // Set prompt delays for 3 minutes
        if (uiManager != null)
        {
            uiManager.promptDelays = new float[] { 590f }; // Example delay times for 10 minutes

            // Activate the UIManager
            uiManager.gameObject.SetActive(true);
        }
    }
}
