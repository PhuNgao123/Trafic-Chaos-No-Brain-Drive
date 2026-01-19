using UnityEngine;

public class ObjectMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float moveSpeed = 50f;   // tốc độ "xe chạy"

    void Update()
    {
        // Di chuyển ground ngược về phía player (trục Z)
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);
    }
}
