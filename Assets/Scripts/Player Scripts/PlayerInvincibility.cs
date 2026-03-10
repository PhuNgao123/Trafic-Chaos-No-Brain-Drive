using UnityEngine;

/// <summary>
/// Quản lý trạng thái bất tử (invincibility) của player.
/// Khi active, player sẽ không nhận damage từ va chạm với vehicles.
/// </summary>
public class PlayerInvincibility : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Thời gian bất tử mặc định khi nhặt shield (giây)")]
    private float defaultInvincibilityDuration = 5f;

    [SerializeField]
    [Tooltip("Visual effect khi bất tử (optional)")]
    private GameObject shieldVisualEffect;

    /// <summary>
    /// Kiểm tra xem player có đang bất tử không
    /// </summary>
    public bool IsInvincible { get; private set; }

    private float invincibilityTimer;

    /// <summary>
    /// Event được gọi khi bắt đầu invincibility
    /// </summary>
    public event System.Action OnInvincibilityStarted;

    /// <summary>
    /// Event được gọi khi kết thúc invincibility
    /// </summary>
    public event System.Action OnInvincibilityEnded;

    void Start()
    {
        IsInvincible = false;
        invincibilityTimer = 0f;

        // Ẩn shield visual nếu có
        if (shieldVisualEffect != null)
        {
            shieldVisualEffect.SetActive(false);
        }
    }

    void Update()
    {
        // Đếm ngược thời gian bất tử
        if (IsInvincible)
        {
            invincibilityTimer -= Time.deltaTime;

            if (invincibilityTimer <= 0f)
            {
                DeactivateInvincibility();
            }
        }
    }

    /// <summary>
    /// Kích hoạt trạng thái bất tử
    /// </summary>
    /// <param name="duration">Thời gian bất tử (giây). Nếu <= 0, sử dụng giá trị mặc định</param>
    public void ActivateInvincibility(float duration = 0f)
    {
        if (duration <= 0f)
        {
            duration = defaultInvincibilityDuration;
        }

        IsInvincible = true;
        invincibilityTimer = duration;

        // Hiển thị shield visual
        if (shieldVisualEffect != null)
        {
            shieldVisualEffect.SetActive(true);
        }

        OnInvincibilityStarted?.Invoke();

        Debug.Log($"[PlayerInvincibility] Invincibility activated for {duration} seconds");
    }

    /// <summary>
    /// Tắt trạng thái bất tử
    /// </summary>
    public void DeactivateInvincibility()
    {
        if (!IsInvincible)
            return;

        IsInvincible = false;
        invincibilityTimer = 0f;

        // Ẩn shield visual
        if (shieldVisualEffect != null)
        {
            shieldVisualEffect.SetActive(false);
        }

        OnInvincibilityEnded?.Invoke();

        Debug.Log("[PlayerInvincibility] Invincibility deactivated");
    }

    /// <summary>
    /// Lấy thời gian còn lại của invincibility
    /// </summary>
    public float GetRemainingTime()
    {
        return IsInvincible ? invincibilityTimer : 0f;
    }
}
