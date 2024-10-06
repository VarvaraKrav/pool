using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowController : MonoBehaviour
{
    public Transform leftController;
    public Transform rightController;
    public Transform leftWindow; // Left window transform
    public Transform rightWindow; // Right window transform
    public GameObject leftWindowUpButton;
    public GameObject leftWindowDownButton;
    public GameObject rightWindowUpButton;
    public GameObject rightWindowDownButton;
    public float windowMoveSpeed = 0.5f;
    public float raycastDistance = 10f;

    // Window positions
    public Vector3 leftWindowUpPosition = new Vector3(-0.799f, 0.417f, 0.174f); //POSITIONS
    public Vector3 leftWindowDownPosition = new Vector3(-0.997f, 0.032f, 0.174f);
    public Vector3 rightWindowUpPosition = new Vector3(0.654f, 0.499f, -0.579f);
    public Vector3 rightWindowDownPosition = new Vector3(0.855f, 0.114f, -0.579f);


    private Outline leftWindowUpButtonOutline, leftWindowDownButtonOutline;
    private Outline rightWindowUpButtonOutline, rightWindowDownButtonOutline;

    // Movement flags
    private bool isLeftWindowMovingUp = false;
    private bool isLeftWindowMovingDown = false;
    private bool isRightWindowMovingUp = false;
    private bool isRightWindowMovingDown = false;

    

    private void Start()
    {
        InitializeButtonOutlines();

        // Ensure the windows start at their exact "up" positions
        leftWindow.localPosition = leftWindowUpPosition;
        rightWindow.localPosition = rightWindowUpPosition;
    }

    private void Update()
    {
        // Handle button interactions for windows
        HandleButtonInteractions();

        // Move windows while buttons are pressed
        if (isLeftWindowMovingUp) MoveWindow(leftWindow, leftWindowUpPosition);
        if (isLeftWindowMovingDown) MoveWindow(leftWindow, leftWindowDownPosition);
        if (isRightWindowMovingUp) MoveWindow(rightWindow, rightWindowUpPosition);
        if (isRightWindowMovingDown) MoveWindow(rightWindow, rightWindowDownPosition);
    }

    private void InitializeButtonOutlines()
    {
        if (leftWindowUpButton != null)
        {
            leftWindowUpButtonOutline = leftWindowUpButton.GetComponent<Outline>();
            if (leftWindowUpButtonOutline != null) leftWindowUpButtonOutline.enabled = false;
        }

        if (leftWindowDownButton != null)
        {
            leftWindowDownButtonOutline = leftWindowDownButton.GetComponent<Outline>();
            if (leftWindowDownButtonOutline != null) leftWindowDownButtonOutline.enabled = false;
        }

        if (rightWindowUpButton != null)
        {
            rightWindowUpButtonOutline = rightWindowUpButton.GetComponent<Outline>();
            if (rightWindowUpButtonOutline != null) rightWindowUpButtonOutline.enabled = false;
        }

        if (rightWindowDownButton != null)
        {
            rightWindowDownButtonOutline = rightWindowDownButton.GetComponent<Outline>();
            if (rightWindowDownButtonOutline != null) rightWindowDownButtonOutline.enabled = false;
        }
    }

    private void HandleButtonInteractions()
    {
        // Handle Left Window Up Button
        bool isLeftControllerPointingAtUp = IsControllerPointingAtObject(leftController, leftWindowUpButton);
        bool isRightControllerPointingAtUp = IsControllerPointingAtObject(rightController, leftWindowUpButton);
        leftWindowUpButtonOutline.enabled = isLeftControllerPointingAtUp || isRightControllerPointingAtUp;

        if (leftWindowUpButtonOutline.enabled && (Input.GetMouseButton(0)))
        {
            isLeftWindowMovingUp = true;
        }
        else
        {
            isLeftWindowMovingUp = false;
        }

        // Handle Left Window Down Button
        bool isLeftControllerPointingAtDown = IsControllerPointingAtObject(leftController, leftWindowDownButton);
        bool isRightControllerPointingAtDown = IsControllerPointingAtObject(rightController, leftWindowDownButton);
        leftWindowDownButtonOutline.enabled = isLeftControllerPointingAtDown || isRightControllerPointingAtDown;

        if (leftWindowDownButtonOutline.enabled && (Input.GetMouseButton(0)))
        {
            isLeftWindowMovingDown = true;
        }
        else
        {
            isLeftWindowMovingDown = false;
        }

        // Handle Right Window Up Button
        bool isLeftControllerPointingAtRightUp = IsControllerPointingAtObject(leftController, rightWindowUpButton);
        bool isRightControllerPointingAtRightUp = IsControllerPointingAtObject(rightController, rightWindowUpButton);
        rightWindowUpButtonOutline.enabled = isLeftControllerPointingAtRightUp || isRightControllerPointingAtRightUp;

        if (rightWindowUpButtonOutline.enabled && (Input.GetMouseButton(0)))
        {
            isRightWindowMovingUp = true;
        }
        else
        {
            isRightWindowMovingUp = false;
        }

        // Handle Right Window Down Button
        bool isLeftControllerPointingAtRightDown = IsControllerPointingAtObject(leftController, rightWindowDownButton);
        bool isRightControllerPointingAtRightDown = IsControllerPointingAtObject(rightController, rightWindowDownButton);
        rightWindowDownButtonOutline.enabled = isLeftControllerPointingAtRightDown || isRightControllerPointingAtRightDown;

        if (rightWindowDownButtonOutline.enabled && (Input.GetMouseButton(0)))
        {
            isRightWindowMovingDown = true;
        }
        else
        {
            isRightWindowMovingDown = false;
        }
    }

    private void MoveWindow(Transform window, Vector3 targetPosition)
    {
        // Move window smoothly towards target position
        window.localPosition = Vector3.MoveTowards(window.localPosition, targetPosition, windowMoveSpeed * Time.deltaTime);
    }

    private bool IsControllerPointingAtObject(Transform controller, GameObject targetObject)
{
    Ray ray = new Ray(controller.position, controller.forward);
    RaycastHit hit;
    
    // Debug to visualize the ray in the Scene view
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
