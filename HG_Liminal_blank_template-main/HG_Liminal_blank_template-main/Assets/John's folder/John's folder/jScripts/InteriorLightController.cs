using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorLightController : MonoBehaviour
{
    public Transform leftController;
    public Transform rightController;
    public GameObject lightToggleButton;
    public Light interiorLight; // The actual light to control
    public Transform glowObject; // The object to scale for glow effect
    public float lightFadeSpeed = 1f;
    public float glowScaleSpeed= 1f;
    public float minIntensity = 0f;
    public float maxIntensity = 5f;
    public Vector3 minGlowScale = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 maxGlowScale = new Vector3(1f, 1f, 1f);
    public float raycastDistance = 10f;

    private bool isLightOn = false;
    private Outline lightToggleButtonOutline;

    private void Start()
    {
        if (lightToggleButton != null)
        {
            lightToggleButtonOutline = lightToggleButton.GetComponent<Outline>();
            if (lightToggleButtonOutline != null) lightToggleButtonOutline.enabled = false;
        }
    }

    private void Update()
    {
        HandleButtonInteraction();
        UpdateLight();
        UpdateGLow();

    }

    private void HandleButtonInteraction()
    {
        bool isLeftControllerPointingAtLight = IsControllerPointingAtObject(leftController, lightToggleButton);
        bool isRightControllerPointingAtLight = IsControllerPointingAtObject(rightController, lightToggleButton);

        lightToggleButtonOutline.enabled = isLeftControllerPointingAtLight || isRightControllerPointingAtLight;

        if (lightToggleButtonOutline.enabled && Input.GetMouseButtonDown(0))
        {
            ToggleLight();
        }
    }

    private void ToggleLight()
    {
        isLightOn = !isLightOn; // Toggle the light state
    }

    private void UpdateLight()
    {
        if (isLightOn)
        {
            // Fade in the light 
            interiorLight.intensity = Mathf.MoveTowards(interiorLight.intensity, maxIntensity, lightFadeSpeed * Time.deltaTime);
           
        }
        else
        {
            // Fade out the light intensity
            interiorLight.intensity = Mathf.MoveTowards(interiorLight.intensity, minIntensity, lightFadeSpeed * Time.deltaTime);
           
        }
    }

    private void UpdateGLow()
    {
        if (isLightOn)
        {
            // Fade in and scale the glow object
            glowObject.localScale = Vector3.MoveTowards(glowObject.localScale, maxGlowScale, glowScaleSpeed * Time.deltaTime);
        }
        else
        {
            // Fade out and scale the glow object down
            glowObject.localScale = Vector3.MoveTowards(glowObject.localScale, minGlowScale, glowScaleSpeed * Time.deltaTime);
        }
    }

    private bool IsControllerPointingAtObject(Transform controller, GameObject targetObject)
    {
        Ray ray = new Ray(controller.position, controller.forward);
        RaycastHit hit;

        // Debug ray in the scene view
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.green);

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.collider.gameObject == targetObject)
            {
                return true;
            }
        }
        return false;
    }
}
