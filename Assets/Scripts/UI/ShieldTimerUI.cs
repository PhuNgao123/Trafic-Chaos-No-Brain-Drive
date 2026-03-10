using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hiển thị thời gian còn lại của shield invincibility trên UI.
/// Optional component - chỉ cần nếu muốn hiển thị shield timer.
/// </summary>
public class ShieldTimerUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference đến PlayerInvincibility component")]
    private PlayerInvincibility playerInvincibility;

    [SerializeField]
    [Tooltip("Text UI để hiển thị thời gian (optional)")]
    private Text timerText;

    [SerializeField]
    [Tooltip("Image UI để hiển thị shield icon (optional)")]
    private Image shieldIcon;

    [SerializeField]
    [Tooltip("Slider UI để hiển thị progress bar (optional)")]
    private Slider timerSlider;

    [SerializeField]
    [Tooltip("GameObject chứa UI elements - sẽ ẩn khi không invincible")]
    private GameObject uiContainer;

    private float maxDuration;

    void Start()
    {
        // Tự động tìm PlayerInvincibility nếu chưa gán
        if (playerInvincibility == null)
        {
            playerInvincibility = FindObjectOfType<PlayerInvincibility>();
        }

        if (playerInvincibility == null)
        {
            Debug.LogError("[ShieldTimerUI] PlayerInvincibility not found!");
            enabled = false;
            return;
        }

        // Subscribe to events
        playerInvincibility.OnInvincibilityStarted += OnShieldActivated;
        playerInvincibility.OnInvincibilityEnded += OnShieldDeactivated;

        // Ẩn UI ban đầu
        if (uiContainer != null)
        {
            uiContainer.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe events
        if (playerInvincibility != null)
        {
            playerInvincibility.OnInvincibilityStarted -= OnShieldActivated;
            playerInvincibility.OnInvincibilityEnded -= OnShieldDeactivated;
        }
    }

    void Update()
    {
        if (playerInvincibility != null && playerInvincibility.IsInvincible)
        {
            UpdateUI();
        }
    }

    private void OnShieldActivated()
    {
        // Hiển thị UI
        if (uiContainer != null)
        {
            uiContainer.SetActive(true);
        }

        // Lưu max duration để tính progress
        maxDuration = playerInvincibility.GetRemainingTime();
    }

    private void OnShieldDeactivated()
    {
        // Ẩn UI
        if (uiContainer != null)
        {
            uiContainer.SetActive(false);
        }
    }

    private void UpdateUI()
    {
        float remainingTime = playerInvincibility.GetRemainingTime();

        // Update text
        if (timerText != null)
        {
            timerText.text = $"Shield: {remainingTime:F1}s";
        }

        // Update slider
        if (timerSlider != null && maxDuration > 0)
        {
            timerSlider.value = remainingTime / maxDuration;
        }

        // Optional: Thay đổi màu icon khi sắp hết
        if (shieldIcon != null && remainingTime < 2f)
        {
            // Nhấp nháy khi còn < 2 giây
            float alpha = Mathf.PingPong(Time.time * 3f, 1f);
            Color color = shieldIcon.color;
            color.a = alpha;
            shieldIcon.color = color;
        }
        else if (shieldIcon != null)
        {
            // Màu bình thường
            Color color = shieldIcon.color;
            color.a = 1f;
            shieldIcon.color = color;
        }
    }
}
