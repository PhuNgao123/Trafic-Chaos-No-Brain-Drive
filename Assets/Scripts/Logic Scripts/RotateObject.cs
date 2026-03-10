using UnityEngine;

/// <summary>
/// Script đơn giản để quay object liên tục.
/// Dùng cho shield power-up hoặc collectibles khác.
/// </summary>
public class RotateObject : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Tốc độ quay (độ/giây)")]
    private float rotationSpeed = 50f;

    [SerializeField]
    [Tooltip("Trục quay (X, Y, Z)")]
    private Vector3 rotationAxis = Vector3.up; // Mặc định quay quanh trục Y

    void Update()
    {
        // Quay object theo trục và tốc độ đã chọn
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
