using UnityEngine;
using UnityEngine.XR;
using TMPro;

public class CarController : MonoBehaviour
{
    public Transform leftController;
    public Transform rightController;
    public GameObject steeringWheel; // Assign the steering wheel GameObject in the Inspector
    public GameObject car; // Assign the car GameObject in the Inspector
    public float raycastDistance = 10f;
    public float rotationSpeed = 100f; // Adjust this to control the rotation speed
    public float carMovementSpeed = 5f; // Adjust this to control the car's movement speed
    public float minX = -5f; // Minimum X position for the car
    public float maxX = 5f;  // Maximum X position for the car

    private Outline steeringWheelOutline;
    private Quaternion initialRotation;
    private bool isRotating = false;
    private Transform activeController;

    void Start()
    {
        if (steeringWheel != null)
        {
            // Get the Outline component from the steering wheel
            steeringWheelOutline = steeringWheel.GetComponent<Outline>();
            if (steeringWheelOutline != null)
            {
                steeringWheelOutline.enabled = false; // Ensure the outline is initially disabled
            }

            // Store the initial rotation of the steering wheel
            initialRotation = steeringWheel.transform.rotation;
        }

    }

    void Update()
    {
        if (steeringWheelOutline == null) return;

        // Check if either controller is pointing at the steering wheel
        bool isLeftControllerPointing = IsControllerPointingAtObject(leftController);
        bool isRightControllerPointing = IsControllerPointingAtObject(rightController);

        // Enable the outline if either controller is pointing at the steering wheel
        steeringWheelOutline.enabled = isLeftControllerPointing || isRightControllerPointing;

        // Check for input and rotate the steering wheel only if it's highlighted
        if ((steeringWheelOutline.enabled && isLeftControllerPointing && Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") > 0.1f) ||
            (steeringWheelOutline.enabled && isRightControllerPointing && Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") > 0.1f) ||
			(steeringWheelOutline.enabled && isRightControllerPointing && Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") > 0.1f) ||
		    (steeringWheelOutline.enabled && isLeftControllerPointing && Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") > 0.1f) ||
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

    private void RotateSteeringWheel()
    {
        float rotationAmount = 0f;


        if (Input.GetMouseButton(0))
        {
            // Left mouse button rotates in one direction (counterclockwise, positive Z)
            rotationAmount = rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetMouseButton(1))
        {
            // Right mouse button rotates in the opposite direction (clockwise, negative Z)
            rotationAmount = -rotationSpeed * Time.deltaTime;
        }
		
        if (activeController != null)
        {
            if (activeController == leftController && Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") > 0.1f || Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") > 0.1f)
            {
                rotationAmount = -rotationSpeed * Time.deltaTime; // Secondary trigger (left) rotates counterwise
            }
            else if (activeController == rightController && Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") > 0.1f || Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") > 0.1f)
            {
                rotationAmount = rotationSpeed * Time.deltaTime; // Primary trigger (right) rotates counterclickwise
            }
        }

        // Apply rotation to the steering wheel on the Z axis
        steeringWheel.transform.Rotate(Vector3.forward, rotationAmount);
    }

    private void MoveCarBasedOnSteeringWheel()
    {
        // Get the current rotation angle of the steering wheel
        float steeringAngle = steeringWheel.transform.localEulerAngles.z;

        // Normalize the steering angle to range [-180, 180]
        if (steeringAngle > 180)
        {
            steeringAngle -= 360;
        }

        // Calculate the movement direction based on the steering angle
        float movementDirection = Mathf.Clamp(steeringAngle / 180f, -1f, 1f);

        // Invert the movement direction to match the correct control scheme
        movementDirection = -movementDirection;

        // Calculate the new position of the car
        Vector3 carMovement = new Vector3(movementDirection * carMovementSpeed * Time.deltaTime, 0f, 0f);
        Vector3 newPosition = car.transform.position + carMovement;

        // Clamp the car's position to the specified min and max X values
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        // Apply the clamped position to the car
        car.transform.position = newPosition;
    }

    private bool IsControllerPointingAtObject(Transform controller)
    {
        Ray ray = new Ray(controller.position, controller.forward);
        RaycastHit hit;

        // Perform the raycast and check if it hits the steering wheel
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
