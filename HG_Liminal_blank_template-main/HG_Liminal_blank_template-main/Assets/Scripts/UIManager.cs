using System.Collections;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI[] prompts; // Use TextMeshProUGUI for TextMeshPro components
    public AudioClip[] audioClips;
    public AudioSource audioSource;
    public float[] displayDurations;
    private Animator[] animators;
    private CanvasGroup[] canvasGroups;

    private void Start()
    {
        animators = new Animator[prompts.Length];
        canvasGroups = new CanvasGroup[prompts.Length];
        
        for (int i = 0; i < prompts.Length; i++)
        {
            animators[i] = prompts[i].GetComponent<Animator>();
            canvasGroups[i] = prompts[i].GetComponent<CanvasGroup>();
            // Ensure initial state is hidden
            canvasGroups[i].alpha = 0;
        }
        
        StartCoroutine(DisplayPrompts());
    }

    private IEnumerator DisplayPrompts()
    {
        for (int i = 0; i < prompts.Length; i++)
        {
            // Set active and trigger fade-in
            prompts[i].gameObject.SetActive(true);
            animators[i].SetTrigger("FadeIn");
            audioSource.clip = audioClips[i];
            audioSource.Play();

            // Wait for the fade-in animation to complete (adjust this duration to match the fade-in animation length)
            yield return new WaitForSeconds(1f);

            // Wait for the display duration
            yield return new WaitForSeconds(displayDurations[i]);

            // Trigger fade-out
            animators[i].SetTrigger("FadeOut");

            // Wait for the fade-out animation to complete (adjust this duration to match the fade-out animation length)
            yield return new WaitForSeconds(1f);

            // Set inactive after fade-out
            prompts[i].gameObject.SetActive(false);
        }
    }
}
