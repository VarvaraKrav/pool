using System.Collections;
using TMPro;
using UnityEngine;

public class GearShifterController : MonoBehaviour
{
    public Transform leftController;
    public Transform rightController;
    public GameObject gearShifter; // Reference to the gear shifter GameObject itself
    public Outline outlineScript; // Reference to the outline script attached to the gear shifter

    // Positions for each gear (Local positions relative to parent object)
    public Vector3 pPositionLocal; // Park position
    public Vector3 rPositionLocal; // Reverse position
    public Vector3 dPositionLocal; // Drive position
    public Vector3 sPositionLocal; // Sport position

    public float transitionSpeed = 1f; // Speed for transitioning between gears

    public GameObject rpmSlider; // The object you want to move based on gear position
    public Vector3 drivePosition; // Position when in Drive
    public Vector3 sportPosition; // Position when in Sport
    public float moveSpeed = 2.0f; // Speed at which the object moves

    public float raycastDistance = 10f;

    private bool isSelected = false; // To check if the user has clicked on the gear shifter
    private bool isPointing = false; // To check if the user is pointing at the gear shifter
    private string currentGear = "P"; // Track current gear state

    private void Update()
    {
        HandleGearShifterInteraction();

    }

    private void HandleGearShifterInteraction()
    {
        // Check if either controller is pointing at the gear shifter
        bool isLeftControllerPointingAtShifter = IsControllerPointingAtObject(leftController, gearShifter);
        bool isRightControllerPointingAtShifter = IsControllerPointingAtObject(rightController, gearShifter);

        // Enable outline if the controller is pointing at the gear shifter
        outlineScript.enabled = isLeftControllerPointingAtShifter || isRightControllerPointingAtShifter;

        // If the outline is active and the user clicks, shift the gear
        if (outlineScript.enabled && Input.GetMouseButtonDown(0)) // Change Input.GetMouseButtonDown(0) to the appropriate VR interaction
        {
            ShiftGear();
        }

        
    }

    // Function to handle gear shift logic
    private void ShiftGear()
    {
        // Check the current gear and move to the next gear
        switch (currentGear)
        {
            case "P":
                StartCoroutine(MoveGearShifter(pPositionLocal, rPositionLocal));
                currentGear = "R";
                break;
            case "R":
                StartCoroutine(MoveGearShifter(rPositionLocal, dPositionLocal));
                currentGear = "D";
                break;
            case "D":
                StartCoroutine(MoveGearShifter(dPositionLocal, sPositionLocal));
                currentGear = "S";
                RPMSlider();
                break;
            case "S":
                StartCoroutine(MoveGearShifter(sPositionLocal, dPositionLocal));
                currentGear = "D";
                RPMSlider();
                break;
        }
    }

    // Coroutine to smoothly move the gear shifter between local positions
    private IEnumerator MoveGearShifter(Vector3 startPosition, Vector3 targetPosition)
    {
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            gearShifter.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / transitionSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gearShifter.transform.localPosition = targetPosition; // Ensure it reaches the final position
    }

    void RPMSlider()
    {
        if (currentGear == "D")
        {

            StartCoroutine(MoveRPMSlider(sportPosition, drivePosition));

        }
        else if (currentGear == "S")
        {
            
            StartCoroutine(MoveRPMSlider(drivePosition, sportPosition));
           
        }
    }

    // Coroutine to smoothly move the RPM slider between local positions
    private IEnumerator MoveRPMSlider(Vector3 startPosition, Vector3 targetPosition)
    {
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            rpmSlider.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / transitionSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rpmSlider.transform.localPosition = targetPosition; // Ensure it reaches the final position
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
    
    


