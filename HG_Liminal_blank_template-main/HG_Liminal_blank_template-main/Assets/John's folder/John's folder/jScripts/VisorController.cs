using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisorController : MonoBehaviour
{
    public Transform leftController;
    public Transform rightController;
    public GameObject driverVisorToggleButton;
    public GameObject passengerVisorToggleButton;
    public float visorRotateSpeed = 1f;
    public float targetDownAngle = -110f;  // Visor down position angle
    public float targetUpAngle = 0f;  // Visor up position angle
    // Public references for the driver's and passenger's visors
    public GameObject driverVisorModel;
    public GameObject passengerVisorModel;
    public float raycastDistance = 10f;

    // States to track if each visor is down or up
    private bool isDriverVisorDown = false;
    private bool isPassengerVisorDown = false;

    private Outline driverToggleButtonOutline;
    private Outline passengerToggleButtonOutline;

    private void Start()
    {
        if (driverVisorToggleButton != null)
        {
            driverToggleButtonOutline = driverVisorToggleButton.GetComponent<Outline>();
            if (driverToggleButtonOutline != null) driverToggleButtonOutline.enabled = false;
        }

        if (passengerVisorToggleButton != null)
        {
            passengerToggleButtonOutline = passengerVisorToggleButton.GetComponent<Outline>();
            if (passengerToggleButtonOutline != null) passengerToggleButtonOutline.enabled = false;
        }
    }

    private void Update()
    {
        HandleDriverInteraction();
        HandlePassengerInteraction();
        UpdateDriverVisor();
        UpdatePassengerVisor();
        

    }

    private void HandleDriverInteraction()
    {
        bool isLeftControllerPointingAtVisor = IsControllerPointingAtObject(leftController, driverVisorToggleButton);
        bool isRightControllerPointingAtVisor = IsControllerPointingAtObject(rightController, driverVisorToggleButton);

        driverToggleButtonOutline.enabled = isLeftControllerPointingAtVisor || isRightControllerPointingAtVisor;

        if (driverToggleButtonOutline.enabled && Input.GetMouseButtonDown(0))
        {
            ToggleDriverVisor();
        }
    }

    private void HandlePassengerInteraction()
    {
        bool isLeftControllerPointingAtVisor = IsControllerPointingAtObject(leftController, passengerVisorToggleButton);
        bool isRightControllerPointingAtVisor = IsControllerPointingAtObject(rightController, passengerVisorToggleButton);

        passengerToggleButtonOutline.enabled = isLeftControllerPointingAtVisor || isRightControllerPointingAtVisor;

        if (passengerToggleButtonOutline.enabled && Input.GetMouseButtonDown(0))
        {
            TogglePassengerVisor();
        }
    }
    private void ToggleDriverVisor()
    {
        isDriverVisorDown = !isDriverVisorDown; // Toggle the visor state
    }

    private void TogglePassengerVisor()
    {
        isPassengerVisorDown = !isPassengerVisorDown; // Toggle the visor state
    }
    private void UpdateDriverVisor()
    {
        // Get the current local rotation of the visor
        Quaternion currentRotation = driverVisorModel.transform.localRotation;

        // Determine the target rotation based on whether the visor is up or down
        float targetXRotation = isDriverVisorDown ? targetDownAngle : targetUpAngle;

        // Create the target rotation, locking the Y and Z axis to avoid unwanted rotations
        Quaternion targetRotation = Quaternion.Euler(targetXRotation, 0f, 0f);

        // Smoothly rotate the visor only on the X axis
        driverVisorModel.transform.localRotation = Quaternion.RotateTowards(currentRotation, targetRotation, visorRotateSpeed * Time.deltaTime);
    }


    private void UpdatePassengerVisor()
    {
        // Get the current local rotation of the visor
        Quaternion currentRotation = passengerVisorModel.transform.localRotation;

        // Determine the target rotation based on whether the visor is up or down
        float targetXRotation = isPassengerVisorDown ? targetDownAngle : targetUpAngle;

        // Create the target rotation, locking the Y and Z axis to avoid unwanted rotations
        Quaternion targetRotation = Quaternion.Euler(targetXRotation, 0f, 0f);

        // Smoothly rotate the visor only on the X axis
        passengerVisorModel.transform.localRotation = Quaternion.RotateTowards(currentRotation, targetRotation, visorRotateSpeed * Time.deltaTime);
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