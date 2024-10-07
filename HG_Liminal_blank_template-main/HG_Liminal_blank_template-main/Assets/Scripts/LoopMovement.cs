using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopMovement : MonoBehaviour
{
    public enum MovementDirection { UpDown, FrontBack, LeftRight }
    public MovementDirection direction = MovementDirection.UpDown;

    public float speed = 2.0f;          // Speed of the movement
    public float distance = 5.0f;       // Distance of the movement
    public bool localSpace = true;      // If true, movement is in local space, otherwise in world space

    private Vector3 startPosition;      // Initial position of the object
    private Vector3 movementAxis;       // Axis on which the object will move

    void Start()
    {
        startPosition = localSpace ? transform.localPosition : transform.position;

        // Determine the movement axis based on the selected direction
        switch (direction)
        {
            case MovementDirection.UpDown:
                movementAxis = Vector3.up;
                break;
            case MovementDirection.FrontBack:
                movementAxis = Vector3.forward;
                break;
            case MovementDirection.LeftRight:
                movementAxis = Vector3.right;
                break;
        }
    }

    void Update()
    {
        // Calculate the new position using a sine wave for smooth looping motion
        float movementOffset = Mathf.Sin(Time.time * speed) * distance;
        Vector3 offset = movementAxis * movementOffset;

        if (localSpace)
        {
            transform.localPosition = startPosition + offset;
        }
        else
        {
            transform.position = startPosition + offset;
        }
    }
}

