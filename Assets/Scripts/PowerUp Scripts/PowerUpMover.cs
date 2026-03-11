using UnityEngine;

/// <summary>
/// Controls powerup falling speed and movement
/// Attach to powerup prefabs
/// </summary>
public class PowerUpMover : MonoBehaviour
{
    [Header("Fall Settings")]
    [Tooltip("Tốc độ rơi xuống (units/second)")]
    public float fallSpeed = 2f;
    
    [Tooltip("Độ cao tối thiểu (dừng rơi khi đạt)")]
    public float minHeight = 0.5f;
    
    [Header("Rotation (Optional)")]
    [Tooltip("Có quay powerup không")]
    public bool enableRotation = true;
    
    [Tooltip("Tốc độ quay (degrees/second)")]
    public float rotationSpeed = 90f;
    
    [Header("Bobbing (Optional)")]
    [Tooltip("Có nhấp nhô lên xuống không")]
    public bool enableBobbing = true;
    
    [Tooltip("Biên độ nhấp nhô")]
    public float bobbingAmount = 0.3f;
    
    [Tooltip("Tốc độ nhấp nhô")]
    public float bobbingSpeed = 2f;
    
    private float startY;
    private float bobbingOffset;
    private bool hasReachedGround = false;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        // Rơi xuống cho đến khi chạm độ cao tối thiểu
        if (!hasReachedGround)
        {
            Vector3 pos = transform.position;
            pos.y -= fallSpeed * Time.deltaTime;
            
            if (pos.y <= minHeight)
            {
                pos.y = minHeight;
                hasReachedGround = true;
                startY = pos.y; // Reset start Y for bobbing
            }
            
            transform.position = pos;
        }
        
        // Nhấp nhô khi đã chạm đất
        if (hasReachedGround && enableBobbing)
        {
            bobbingOffset = Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
            Vector3 pos = transform.position;
            pos.y = startY + bobbingOffset;
            transform.position = pos;
        }
        
        // Quay
        if (enableRotation)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}
