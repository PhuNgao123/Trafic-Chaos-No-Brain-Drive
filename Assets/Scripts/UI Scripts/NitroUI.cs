using UnityEngine;
using UnityEngine.UI;

// Shows nitro bar and optional corner glow FX.
// Attach this to a Canvas object and wire the images in the inspector.
public class NitroUI : MonoBehaviour
{
    [Header("References")]
    public NitroController nitroController;
    public GameObject nitroBarBackground;  // The parent GameObject to show/hide

    [Header("Bar")]
    public Image nitroFillImage;      // Fill image (type = Filled)
    public Image nitroFrameImage;     // Optional frame image

    [Header("Corner Glow (optional)")]
    public Image[] cornerGlowImages;  // 4 images in screen corners

    [Header("Colors")]
    public Color fillNormalColor = Color.gray;
    public Color fillReadyColor = Color.cyan;
    public Color frameNormalColor = Color.white;
    public Color frameActiveColor = Color.yellow;

    void Start()
    {
        if (nitroController == null)
            nitroController = FindFirstObjectByType<NitroController>();

        SetCornerGlows(false);
        
        // Hide nitro bar until game starts
        if (nitroBarBackground != null)
            nitroBarBackground.SetActive(false);
    }

    void Update()
    {
        // Show nitro bar when game starts
        if (nitroBarBackground != null && !nitroBarBackground.activeSelf)
        {
            if (GameLogicController.Instance != null && GameLogicController.Instance.isGameStarted)
            {
                nitroBarBackground.SetActive(true);
            }
        }

        if (nitroController == null)
            return;

        float percent = nitroController.NitroPercent;

        // Update bar fill and color
        if (nitroFillImage != null)
        {
            nitroFillImage.fillAmount = percent;
            nitroFillImage.color = nitroController.IsNitroReady ? fillReadyColor : fillNormalColor;
        }

        // Frame color: glow when nitro is active
        if (nitroFrameImage != null)
        {
            nitroFrameImage.color = nitroController.IsNitroActive ? frameActiveColor : frameNormalColor;
        }

        // Corner glow on when nitro active
        SetCornerGlows(nitroController.IsNitroActive);
    }

    void SetCornerGlows(bool enabled)
    {
        if (cornerGlowImages == null)
            return;

        foreach (var img in cornerGlowImages)
        {
            if (img == null) continue;
            img.enabled = enabled;
        }
    }
}

