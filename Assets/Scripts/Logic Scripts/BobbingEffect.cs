using UnityEngine;

/// <summary>
/// Tạo hiệu ứng lên xuống (bobbing) cho object.
/// Dùng cho shield power-up hoặc collectibles để thu hút sự chú ý.
/// </summary>
public class BobbingEffect : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Tốc độ lên xuống")]
    private float bobbingSpeed = 2f;

    [SerializeField]
    [Tooltip("Độ cao lên xuống (đơn vị Unity)")]
    private float bobbingAmount = 0.3f;

    [SerializeField]
    [Tooltip("Nếu true, dùng vị trí hiện tại làm gốc. Nếu false, dùng vị trí khi Start")]
    private bool useCurrentPosition = false;

    private Vector3 startPosition;

    void Start()
    {
        // Lưu vị trí ban đầu
        startPosition = transform.position;
    }

    void Update()
    {
        if (!useCurrentPosition)
        {
            // Tính toán vị trí Y mới dựa trên sin wave
            float newY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        else
        {
            // Dùng local position để tương thích với parent movement
            float newY = Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
    }
}
