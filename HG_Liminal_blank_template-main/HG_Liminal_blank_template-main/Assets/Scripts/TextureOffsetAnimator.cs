using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureOffsetAnimator : MonoBehaviour
{
    // Variables to control the speed of the offset
    public float xSpeed = 0.1f; // Speed in the X direction
    public float ySpeed = 0.1f; // Speed in the Y direction

    // Reference to the material's main texture offset
    private Material material;
    private Vector2 offset;

    void Start()
    {
        // Get the material of the Renderer attached to the GameObject
        material = GetComponent<Renderer>().material;
        // Initialize the offset to the current value in the material
        offset = material.mainTextureOffset;
    }

    void Update()
    {
        // Update the offset over time based on the speed
        offset.x += xSpeed * Time.deltaTime;
        offset.y += ySpeed * Time.deltaTime;

        // Apply the updated offset to the material
        material.mainTextureOffset = offset;
    }
}

