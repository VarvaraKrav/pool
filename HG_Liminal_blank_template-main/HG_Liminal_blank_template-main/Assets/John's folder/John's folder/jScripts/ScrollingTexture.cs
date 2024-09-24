using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTexture : MonoBehaviour
{
    public float scrollSpeedX = 0.5f;
    public float scrollSpeedY = 0.5f;
    public Color startEmissiveColor = Color.blue;  // Starting emissive color
    public Color endEmissiveColor = Color.red;     // Ending emissive color
    public float emissiveColorChangeSpeed = 1f;    // Speed at which the emissive color changes
    private Material material;
    private float emissiveColorTime = 0f;  // A timer to smoothly transition the color

    void Start()
    {
        // Get the material of the object this script is attached to
        material = GetComponent<Renderer>().material;

        // Enable the emission keyword, if not already enabled
        material.EnableKeyword("_EMISSION");

        // Ensure we start with the specified start emissive color
        material.SetColor("_EmissionColor", startEmissiveColor * Mathf.LinearToGammaSpace(1.0f)); // Apply gamma correction
    }

    void Update()
    {
        // Scrolling texture
        float offsetX = Time.time * scrollSpeedX;
        float offsetY = Time.time * scrollSpeedY;
        material.mainTextureOffset = new Vector2(offsetX, offsetY);

        // Smoothly transition the emissive color
        emissiveColorTime += emissiveColorChangeSpeed * Time.deltaTime;
        Color currentEmissiveColor = Color.Lerp(startEmissiveColor, endEmissiveColor, Mathf.PingPong(emissiveColorTime, 1));
        material.SetColor("_EmissionColor", currentEmissiveColor * Mathf.LinearToGammaSpace(1.0f)); // Ensure gamma correction is applied
    }
}
