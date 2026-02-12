using UnityEngine;

public class RoadMover : MonoBehaviour
{
    [Header("=== REFERENCES ===")]
    public PlayerPhysics playerPhysics;

    private float currentZ = 0f;
    public float speed = 10f;

    void Start()
    {
        if (playerPhysics == null)
            playerPhysics = FindObjectOfType<PlayerPhysics>();
    }

    void Update()
    {
        if (playerPhysics == null) return;

        // Di chuyển RoadContainer ngược lại với tốc độ xe
        speed = playerPhysics.GetCurrentSpeed();
        currentZ -= speed * Time.deltaTime;

        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }
}
