using UnityEngine;

// Controls day/night cycle by rotating directional light (sun)
// Simply rotates X axis over time
public class DayNightCycle : MonoBehaviour
{
    [Header("References")]
    public Light directionalLight;

    [Header("Cycle Settings")]
    public float rotationSpeed = 1f; // Speed of rotation (1 = 360 degrees in 360 seconds = 6 minutes)

    void Start()
    {
        // Auto-find directional light
        if (directionalLight == null)
            directionalLight = GetComponent<Light>();

        if (directionalLight == null)
            directionalLight = FindFirstObjectByType<Light>();
    }

    void Update()
    {
        if (directionalLight == null)
            return;

        // Rotate smoothly around X axis
        directionalLight.transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f, Space.Self);
    }
}
