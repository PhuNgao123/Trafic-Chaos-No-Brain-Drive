using UnityEngine;

public class RoadMover : MonoBehaviour
{
    [Header("=== REFERENCES ===")]
    public PlayerPhysics playerPhysics;

    private float currentZ = 0f;

    void Start()
    {
        if (playerPhysics == null)
            playerPhysics = FindObjectOfType<PlayerPhysics>();
    }

    void Update()
    {
        if (playerPhysics == null) return;

        // Di chuyển RoadContainer ngược lại với tốc độ xe
        float speed = playerPhysics.GetCurrentSpeed();
        currentZ -= speed * Time.deltaTime;

        transform.position = new Vector3(0, 0, currentZ);
    }
}
