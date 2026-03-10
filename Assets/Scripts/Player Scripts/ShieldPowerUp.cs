using UnityEngine;

/// <summary>
/// Shield power-up collectible.
/// Khi player va chạm (trigger), kích hoạt invincibility và tự hủy.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ShieldPowerUp : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Thời gian bất tử khi nhặt shield này (giây)")]
    private float invincibilityDuration = 5f;

    [SerializeField]
    [Tooltip("Hiệu ứng khi nhặt shield (optional)")]
    private GameObject pickupEffect;

    [SerializeField]
    [Tooltip("Âm thanh khi nhặt shield (optional)")]
    private AudioClip pickupSound;

    private bool isCollected = false;

    void Start()
    {
        // Đảm bảo collider là trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogError("[ShieldPowerUp] No Collider found on shield power-up!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Tránh collect nhiều lần
        if (isCollected)
            return;

        // Kiểm tra xem có phải player không
        if (other.CompareTag("Player"))
        {
            // Tìm PlayerInvincibility component
            PlayerInvincibility invincibility = other.GetComponent<PlayerInvincibility>();
            
            if (invincibility == null)
            {
                invincibility = other.GetComponentInChildren<PlayerInvincibility>();
            }

            if (invincibility != null)
            {
                // Kích hoạt invincibility
                invincibility.ActivateInvincibility(invincibilityDuration);

                // Đánh dấu đã collect
                isCollected = true;

                // Phát hiệu ứng
                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }

                // Phát âm thanh
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                Debug.Log($"[ShieldPowerUp] Shield collected by {other.name}");

                // Hủy shield object
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("[ShieldPowerUp] Player doesn't have PlayerInvincibility component!");
            }
        }
    }
}
