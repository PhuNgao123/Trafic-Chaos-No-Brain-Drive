using UnityEngine;

public class RoadMover : MonoBehaviour
{
    [Header("=== REFERENCES ===")]
    public PlayerPhysics playerPhysics;

    private float _currentZ = 0f;
    public float speed = 10f;

    void Start()
    {
        if (playerPhysics == null)
            playerPhysics = FindFirstObjectByType<PlayerPhysics>();
    }

    void Update()
    {
        if (playerPhysics == null) return;

        // Move road container backward based on player speed
        speed = playerPhysics.GetCurrentSpeed();
        _currentZ -= speed * Time.deltaTime;

        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }
}
