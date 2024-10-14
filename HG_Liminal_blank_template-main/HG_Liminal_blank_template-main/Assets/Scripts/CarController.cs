using UnityEngine;
using TMPro;

public class CarController : MonoBehaviour
{
    public Transform leftController;
    public Transform rightController;
    public GameObject steeringWheel;
    public GameObject car;
    public float raycastDistance = 10f;
    public float rotationSpeed = 100f;
    public float carMovementSpeed = 5f;
    public float minX = -5f;
    public float maxX = 5f;
    public float maxZRotation = 45f; // Maximum Z rotation of the steering wheel
    public TextMeshProUGUI autopilotText;
    public float waitTimeMin = 1f;
    public float waitTimeMax = 3f;

    private Outline steeringWheelOutline;
    private Quaternion initialRotation;
    private bool isRotating = false;
    private Transform activeController;

    [SerializeField]
    private bool isAutopilot = false; // Autopilot state exposed in Inspector
    private bool movingRight = true; // Direction of movement in autopilot
    private bool isWaiting = false;

    void Start()
    {
        if (steeringWheel != null)
        {
            steeringWheelOutline = steeringWheel.GetComponent<Outline>();
            if (steeringWheelOutline != null)
            {
                steeringWheelOutline.enabled = false;
            }

            initialRotation = steeringWheel.transform.rotation;
        }

        // Ensure autopilot text is initially off
        if (autopilotText != null)
        {
            autopilotText.gameObject.SetActive(isAutopilot);
        }
    }

    void Update()
    {
        if (isAutopilot)
        {
            MoveCarInAutopilot();
            SyncSteeringWheelWithCar(); // Sync the steering wheel rotation with the car's movement
        }
        else
        {
            if (steeringWheelOutline == null) return;

            bool isLeftControllerPointing = IsControllerPointingAtObject(leftController);
            bool isRightControllerPointing = IsControllerPointingAtObject(rightController);

            steeringWheelOutline.enabled = isLeftControllerPointing || isRightControllerPointing;

            if ((steeringWheelOutline.enabled && isLeftControllerPointing && Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") > 0.1f) ||
                (steeringWheelOutline.enabled && isRightControllerPointing && Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") > 0.1f) ||
                (steeringWheelOutline.enabled && Input.GetMouseButton(0)) ||
                (steeringWheelOutline.enabled && Input.GetMouseButton(1)))
            {
                isRotating = true;
                activeController = isLeftControllerPointing ? leftController : rightController;
            }
            else
            {
                isRotating = false;
                steeringWheel.transform.rotation = Quaternion.Lerp(steeringWheel.transform.rotation, initialRotation, Time.deltaTime * rotationSpeed);
            }

            if (isRotating)
            {
                RotateSteeringWheel();
                MoveCarBasedOnSteeringWheel();
            }
        }
    }

    // Automatically update the autopilot text when toggled in the Inspector
    private void OnValidate()
    {
        if (autopilotText != null)
        {
            autopilotText.gameObject.SetActive(isAutopilot);
        }
    }

    public void ToggleAutopilot()
    {
        isAutopilot = !isAutopilot;

        if (autopilotText != null)
        {
            autopilotText.gameObject.SetActive(isAutopilot);
        }
    }

    private void MoveCarInAutopilot()
    {
        if (isWaiting) return;

        Vector3 newPosition = car.transform.position;

        if (movingRight)
        {
            newPosition.x += carMovementSpeed * Time.deltaTime;
            if (newPosition.x >= maxX)
            {
                newPosition.x = maxX;
                StartCoroutine(WaitAtPosition());
                movingRight = false;
            }
        }
        else
        {
            newPosition.x -= carMovementSpeed * Time.deltaTime;
            if (newPosition.x <= minX)
            {
                newPosition.x = minX;
                StartCoroutine(WaitAtPosition());
                movingRight = true;
            }
        }

        car.transform.position = newPosition;
    }

    private System.Collections.IEnumerator WaitAtPosition()
    {
        isWaiting = true;

        // Smoothly reset the steering wheel rotation to 0 while waiting
        yield return StartCoroutine(ResetSteeringWheelRotation());

        // Random wait time at either end
        float waitTime = Random.Range(waitTimeMin, waitTimeMax);
        yield return new WaitForSeconds(waitTime);

        isWaiting = false;
    }

    private System.Collections.IEnumerator ResetSteeringWheelRotation()
    {
        float resetDuration = 1f; // Time it takes to reset the wheel to 0 rotation
        float elapsedTime = 0f;

        Quaternion currentRotation = steeringWheel.transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(steeringWheel.transform.localEulerAngles.x, steeringWheel.transform.localEulerAngles.y, 0);

        // Smoothly interpolate the rotation back to Z = 0
        while (elapsedTime < resetDuration)
        {
            steeringWheel.transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, elapsedTime / resetDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        steeringWheel.transform.localRotation = targetRotation;
    }

    // Sync steering wheel rotation with car's movement during autopilot
    private void SyncSteeringWheelWithCar()
    {
        if (isWaiting)
        {
            // If the car is waiting, no rotation is applied (it will reset to Z = 0)
            return;
        }

        float targetZRotation;

        // Invert the rotation direction: when car moves right, rotate the wheel to negative Z, and left to positive Z
        if (movingRight)
        {
            targetZRotation = -maxZRotation;
        }
        else
        {
            targetZRotation = maxZRotation;
        }

        // Smoothly rotate the steering wheel's local Z rotation towards the target rotation
        Quaternion targetRotation = Quaternion.Euler(steeringWheel.transform.localEulerAngles.x, steeringWheel.transform.localEulerAngles.y, targetZRotation);
        steeringWheel.transform.localRotation = Quaternion.Lerp(steeringWheel.transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void RotateSteeringWheel()
    {
        float rotationAmount = 0f;

        if (Input.GetMouseButton(0))
        {
            rotationAmount = rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetMouseButton(1))
        {
            rotationAmount = -rotationSpeed * Time.deltaTime;
        }

        if (activeController != null)
        {
            if (activeController == leftController && Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") > 0.1f)
            {
                rotationAmount = -rotationSpeed * Time.deltaTime;
            }
            else if (activeController == rightController && Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") > 0.1f)
            {
                rotationAmount = rotationSpeed * Time.deltaTime;
            }
        }

        steeringWheel.transform.Rotate(Vector3.forward, rotationAmount);
    }

    private void MoveCarBasedOnSteeringWheel()
    {
        float steeringAngle = steeringWheel.transform.localEulerAngles.z;

        if (steeringAngle > 180)
        {
            steeringAngle -= 360;
        }

        float movementDirection = Mathf.Clamp(steeringAngle / 180f, -1f, 1f);
        movementDirection = -movementDirection;

        Vector3 carMovement = new Vector3(movementDirection * carMovementSpeed * Time.deltaTime, 0f, 0f);
        Vector3 newPosition = car.transform.position + carMovement;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        car.transform.position = newPosition;
    }

    private bool IsControllerPointingAtObject(Transform controller)
    {
        Ray ray = new Ray(controller.position, controller.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.collider.gameObject == steeringWheel)
            {
                return true;
            }
        }

        return false;
    }
}
