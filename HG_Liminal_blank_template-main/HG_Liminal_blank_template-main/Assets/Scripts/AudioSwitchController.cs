using System.Collections;
using UnityEngine;
using TMPro; // Import TextMesh Pro namespace

public class AudioSwitchController : MonoBehaviour
{
    public Transform controllerTransform; // Reference to the controller's transform
    public LayerMask audioSwitchLayer; // Layer for the audio switch objects
    public GameObject worldSpaceUI; // Reference to the world space UI
    public TextMeshProUGUI uiText; // Reference to the TextMeshProUGUI component in the world space UI
    public AudioSource audioSource; // Reference to the audio source
    public AudioClip[] audioClips; // Array of audio clips to switch between
    public string[] audioClipNames; // Array of names corresponding to each audio clip
    public Outline outlineScript; // Reference to the outline script

    private int currentClipIndex = 0;
    private GameObject currentSwitchObject;

    void Start()
    {
        UpdateUI(); // Initialize the UI with the first clip name
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(controllerTransform.position, controllerTransform.forward, out hit, Mathf.Infinity, audioSwitchLayer))
        {
            // If the controller is pointing at an audio switch object
            GameObject hitObject = hit.collider.gameObject;
            if (currentSwitchObject != hitObject)
            {
                // Set the current switch object and enable the outline
                currentSwitchObject = hitObject;
                outlineScript = currentSwitchObject.GetComponent<Outline>();
                if (outlineScript != null)
                {
                    outlineScript.enabled = true;
                }

                // Activate world space UI
                worldSpaceUI.SetActive(true);
            }

            // Check for trigger input or left mouse click
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetMouseButtonDown(0))
            {
                SwitchAudioClip();
            }
        }
        else
        {
            // If the controller is not pointing at the object
            if (currentSwitchObject != null)
            {
                if (outlineScript != null)
                {
                    outlineScript.enabled = false;
                }

                // Deactivate world space UI
                worldSpaceUI.SetActive(false);
                currentSwitchObject = null;
            }
        }
    }

    void SwitchAudioClip()
    {
        if (audioClips.Length > 0)
        {
            currentClipIndex = (currentClipIndex + 1) % audioClips.Length;
            audioSource.clip = audioClips[currentClipIndex];
            audioSource.Play();
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (audioClipNames.Length > currentClipIndex)
        {
            uiText.text = "Now Playing: " + audioClipNames[currentClipIndex];
        }
        else
        {
            uiText.text = "Now Playing: Unknown Track";
        }
    }
}
